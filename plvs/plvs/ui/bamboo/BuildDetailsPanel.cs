using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;
using Atlassian.plvs.api.bamboo;
using Atlassian.plvs.autoupdate;
using Atlassian.plvs.dialogs.bamboo;
using Atlassian.plvs.store;
using Atlassian.plvs.ui.bamboo.treemodels;
using Atlassian.plvs.util;
using Atlassian.plvs.util.bamboo;
using Atlassian.plvs.windows;
using Microsoft.VisualStudio.Shell.Interop;
using Process = System.Diagnostics.Process;
using EnvDTE;
using Thread = System.Threading.Thread;

namespace Atlassian.plvs.ui.bamboo {
    public partial class BuildDetailsPanel : UserControl {
        private readonly Solution solution;
        private readonly BambooBuild build;
        private readonly TabPage myTab;
        private readonly Action<TabPage> buttonCloseClicked;

        private BambooTestResultTree testResultTree;

        private const int MAX_PARSEABLE_LENGTH = 200;
        private const int A_LOT = 10000000;
        private const string OPENFILE_ON_LINE_URL = "openfileonline:";
        private const string SHOW_FAILED_TESTS_ONLY = "BambooBuildShowFailedTestsOnly";

        private readonly StatusLabel status;

        public BuildDetailsPanel(Solution solution, BambooBuild build, TabPage myTab, BuildDetailsWindow parent, Action<TabPage> buttonCloseClicked) {
            
            this.solution = solution;
            this.build = build;
            this.myTab = myTab;
            this.buttonCloseClicked = buttonCloseClicked;
            
            InitializeComponent();

            parent.setMyTabIconFromBuildResult(build.Result, myTab);

            status = new StatusLabel(statusStrip, labelStatus);
        }

        protected override void OnLoad(EventArgs e) {
            init();
        }

        private void init() {
            displaySummary();
            runGetLogThread();
        }

        private void runGetLogThread() {
            status.setInfo("Retrieving build log...");
            Thread t = PlvsUtils.createThread(getLogRunner);
            t.Start();
        }

        private void getLogRunner() {
            try {
                string buildLog = BambooServerFacade.Instance.getBuildLog(build);
                StringBuilder sb = new StringBuilder();
                sb.Append("<html>\n<head>\n").Append(Resources.summary_and_description_css);
                sb.Append("\n</head>\n<body class=\"description\">\n");
                sb.Append(createAugmentedLog(HttpUtility.HtmlEncode(buildLog)));
                sb.Append("</body></html>");

                this.safeInvoke(new MethodInvoker(delegate { webLog.DocumentText = sb.ToString(); }));
                status.setInfo("Build log retrieved");
            } catch (Exception e) {
                status.setError("Failed to retrieve build log", e);
            }
            runGetTestsThread();
        }

        private void runGetTestsThread() {
            status.setInfo("Retrieving build test results...");
            Thread t = PlvsUtils.createThread(getTestsRunner);
            t.Start();
        }

        private void getTestsRunner() {
            try {
                ICollection<BambooTest> tests = BambooServerFacade.Instance.getTestResults(build);
                if (tests == null || tests.Count == 0) {
                    this.safeInvoke(new MethodInvoker(delegate { toolStripContainerTests.Visible = false; }));
                } else {
                    this.safeInvoke(new MethodInvoker(() => createAndFillTestTree(tests)));
                }
                status.setInfo("Test results retrieved");
            } catch (Exception e) {
                status.setError("Failed to retrieve test results", e);
            }
        }

        private void createAndFillTestTree(ICollection<BambooTest> tests) {
            labelNoTestsFound.Visible = false;
            toolStripContainerTests.Dock = DockStyle.Fill;

            toolStripContainerTests.TopToolStripPanel.Controls.Add(testResultsToolStrip);
            testResultsToolStrip.Dock = DockStyle.None;
            testResultsToolStrip.GripStyle = ToolStripGripStyle.Hidden;
            testResultsToolStrip.Items.AddRange(new ToolStripItem[] { buttonFailedOnly, buttonOpenTest, buttonRunTestInVs, buttonDebugTestInVs});
            testResultsToolStrip.Location = new System.Drawing.Point(3, 0);
            testResultsToolStrip.Size = new System.Drawing.Size(126, 25);
            testResultsToolStrip.TabIndex = 0;
            testResultsToolStrip.Visible = true;

            ParameterStore store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.SETTINGS);
            bool failedOnly = store.loadParameter(SHOW_FAILED_TESTS_ONLY, 0) > 0;

            testResultTree = new BambooTestResultTree {Model = new TestResultTreeModel(tests, failedOnly)};
            toolStripContainerTests.ContentPanel.Controls.Add(testResultTree);
            testResultTree.SelectionChanged += testResultTree_SelectionChanged;
            testResultTree.MouseDoubleClick += testResultTree_MouseDoubleClick;
            testResultTree.KeyPress += testResultTree_KeyPress;

            buttonFailedOnly.Checked = failedOnly;
            buttonFailedOnly.CheckedChanged += buttonFailedOnly_CheckedChanged;

            buttonOpenTest.Click += buttonOpenTest_Click;
            buttonRunTestInVs.Click += buttonRunTestInVs_Click;
            buttonDebugTestInVs.Click += buttonDebugTestInVs_Click;
            updateTestButtons();
            testResultTree.ExpandAll();
        }

        private void buttonDebugTestInVs_Click(object sender, EventArgs e) {
            TestMethodNode method = getSelectedTestMethod();
            if (method == null) return;
            if (navigateToTestClassAndMethod(method.Test)) {
                solution.DTE.ExecuteCommand("Test.DebugTestsInCurrentContext", "");
            }
        }

        private void buttonRunTestInVs_Click(object sender, EventArgs e) {
            TestMethodNode method = getSelectedTestMethod();
            if (method == null) return;
            if (navigateToTestClassAndMethod(method.Test)) {
                solution.DTE.ExecuteCommand("Test.RunTestsInCurrentContext", "");
            }
        }

        private void testResultTree_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == (int) Keys.Enter) {
                openTestMethod(true);
            }
        }

        private void testResultTree_MouseDoubleClick(object sender, MouseEventArgs e) {
            openTestMethod(false);
        }

        private void buttonOpenTest_Click(object sender, EventArgs e) {
            openTestMethod(false);
        }

        private void openTestMethod(bool refocusOnTestList) {
            TestMethodNode method = getSelectedTestMethod();
            if (method == null) return;
            navigateToTestClassAndMethod(method.Test);
            if (!refocusOnTestList) return;

            // this seems to be the only way to refocus the bamboo toolwindow
            IVsWindowFrame windowFrame = (IVsWindowFrame) ToolWindowManager.Instance.BuildDetailsWindow.Frame;
            windowFrame.Show();
        }

        private TestMethodNode getSelectedTestMethod() {
            if (testResultTree == null || testResultTree.SelectedNode == null) return null;
            return testResultTree.SelectedNode.Tag as TestMethodNode;
        }

        private void buttonFailedOnly_CheckedChanged(object sender, EventArgs e) {
            ParameterStore store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.SETTINGS);
            store.storeParameter(SHOW_FAILED_TESTS_ONLY, buttonFailedOnly.Checked ? 1 : 0);
            TestResultTreeModel model = testResultTree.Model as TestResultTreeModel;
            if (model == null) return;
            model.FailedOnly = buttonFailedOnly.Checked;
            testResultTree.ExpandAll();
        }

        private void testResultTree_SelectionChanged(object sender, EventArgs e) {
            updateTestButtons();
        }

        private void updateTestButtons() {
            bool enabled = 
                testResultTree != null 
                && testResultTree.SelectedNode != null 
                && testResultTree.SelectedNode.Tag is TestMethodNode;
            buttonOpenTest.Enabled = enabled;
            buttonRunTestInVs.Enabled = enabled;
            buttonDebugTestInVs.Enabled = enabled;
        }

        private static readonly Regex FILE_AND_LINE = new Regex("(.*?)\\s(&quot;)?((([a-zA-Z]:)|(\\\\))?\\S+?)(&quot;)?\\s*\\(((\\d+)(,\\d+)?)\\)(.*?)"); 

        private string createAugmentedLog(string log) {

            SolutionUtils.refillAllSolutionProjectItems(solution);

            string[] strings = log.Split(new[] {'\n'});
            StringBuilder logSb = new StringBuilder();
            foreach (var s in strings) {
                StringBuilder lineSb = new StringBuilder();
                bool tooLong = s.Length > MAX_PARSEABLE_LENGTH;
                string parseablePart = tooLong ? s.Substring(0, MAX_PARSEABLE_LENGTH) : s;
                string restPart = tooLong ? s.Substring(MAX_PARSEABLE_LENGTH) : "";

                MatchCollection matches = FILE_AND_LINE.Matches(parseablePart);
                if (matches.Count > 0) {
                    foreach (Match match in matches) {
                        string fileName = match.Groups[3].Value;
                        if (!fileName.EndsWith(".sln") && !fileName.EndsWith("proj") && SolutionUtils.solutionContainsFile(fileName, solution)) {
                            string lineNumber = match.Groups[8].Value.Trim();
                            lineSb.Append(match.Groups[1].Value)
                                .Append(" <a href=\"").Append(OPENFILE_ON_LINE_URL)
                                .Append(fileName).Append('@').Append(lineNumber).Append("\">")
                                .Append(fileName).Append('(').Append(lineNumber).Append(")</a>")
                                .Append(match.Groups[11]);
                        } else {
                            lineSb.Append(match.Groups[0]);
                        }
                    }
                    Match lastMatch = matches[matches.Count - 1];
                    lineSb.Append(parseablePart.Substring(lastMatch.Index + lastMatch.Length));
                } else {
                    lineSb.Append(parseablePart);
                }
                lineSb.Append(restPart);
                logSb.Append(colorLine(lineSb.ToString())).Append("<br>\r\n");
            }
            return logSb.ToString();
        }

        private static readonly Regex SUCCESS_REGEX = new Regex(@"(BUILD SUCCESSFUL)|(BUILD SUCCEEDED)|(\[INFO\])|(Build succeeded)");
        private static readonly Regex FAILURE_REGEX = new Regex(@"(Build FAILED)|(BUILD FAILED)|(\[ERROR\])|([Ee]rror \w+\d+)");
        private static readonly Regex WARNING_REGEX = new Regex(@"(\[WARNING\])|([Ww]arning \w+\d+)");

        private static string colorLine(string line) {
            string color = null;
            if (FAILURE_REGEX.IsMatch(line)) {
                color = BambooBuild.BuildResult.FAILED.GetColorValue();
            } else if (WARNING_REGEX.IsMatch(line)) {
                color = "#87721e";
            } else if (SUCCESS_REGEX.IsMatch(line)) {
                color = BambooBuild.BuildResult.SUCCESSFUL.GetColorValue();
            }
            return color != null ? "<span style=\"color:" + color + ";\">" + line + "</span>" : line;
        }

        private void displaySummary() {
            StringBuilder sb = new StringBuilder();

            BuildNode bn = new BuildNode(build);
            sb.Append("<html>\n<head>\n").Append(Resources.summary_and_description_css)
                .Append("\n</head>\n<body class=\"description\">\n")
                .Append(
                    string.Format(Resources.build_summary_html, 
                        build.Number, 
                        build.Key, 
                        bn.Reason, 
                        bn.Tests, 
                        build.RelativeTime, 
                        build.Duration, 
                        getServerHtml(), 
                        build.Result, 
                        build.Result.GetColorValue()))
                .Append("\n</body>\n</html>\n");

            webSummary.DocumentText = sb.ToString();
        }

        private object getServerHtml() {
            return build.Server.Name + " (<a href=" + build.Server.Url + ">" + build.Server.Url + "</a>)"; 
        }

        private void buttonClose_Click(object sender, EventArgs e) {
            if (buttonCloseClicked != null) {
                buttonCloseClicked(myTab);
            }
        }

        private void buttonRun_Click(object sender, EventArgs e) {
            string key = BambooBuildUtils.getPlanKey(build);
            status.setInfo("Adding build " + key + " to the build queue...");
            Thread t = PlvsUtils.createThread(() => runBuildWorker(key));
            t.Start();
        }

        private void runBuildWorker(string key) {
            try {
                BambooServerFacade.Instance.runBuild(build.Server, key);
                status.setInfo("Added build " + key + " to the build queue");
                UsageCollector.Instance.bumpBambooBuildsOpen();
            } catch (Exception ex) {
                status.setError("Failed to add build " + key + " to the build queue", ex);
            }
        }

        private void buttonViewInBrowser_Click(object sender, EventArgs e) {
            try {
                Process.Start(
                    build.Server.Url + "/build/viewBuildResults.action?buildKey="
                    + BambooBuildUtils.getPlanKey(build) + "&buildNumber=" + build.Number);
            } catch (Exception ex) {
                Debug.WriteLine("buttonViewInBrowser_Click - exception: " + ex.Message);
            }
        }

        private bool summaryLoaded;

        private void webSummary_Navigating(object sender, WebBrowserNavigatingEventArgs e) {
            if (!summaryLoaded) return;

            e.Cancel = true;

            if (e.Url.Equals("about:blank")) return;

            try {
                Process.Start(e.Url.ToString());
            } catch (Exception ex) {
                Debug.WriteLine("webSummary_Navigating - exception: " + ex.Message);
            }
        }

        private void webSummary_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e) {
            summaryLoaded = true;
        }

        private bool logCompleted;

        private void webLog_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e) {
            if (throbberLoaded) {
                logCompleted = true;
                if (webLog.Document != null && webLog.Document.Body != null) webLog.Document.Body.ScrollTop = A_LOT;
            } else {
                throbberLoaded = true;
            }
        }

        private bool throbberLoaded = true;
        private bool scrolledDown;

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e) {
            if (!tabControl.SelectedTab.Equals(tabLog)) return;

            if (!logCompleted) {
                throbberLoaded = false;
                webLog.DocumentText = PlvsUtils.getThrobberHtml(PlvsUtils.getThroberPath(), "Fetching build log...");
            } else {
                throbberLoaded = true;
                if (scrolledDown) return;
                BeginInvoke(new MethodInvoker(delegate {
                    if (webLog.Document != null && webLog.Document.Body != null) {
                        webLog.Document.Body.ScrollTop = A_LOT;
                        scrolledDown = true;
                    }
                }));
            }
        }

        private void webLog_Navigating(object sender, WebBrowserNavigatingEventArgs e) {
            if (!logCompleted) return;

            e.Cancel = true;
            
            if (e.Url.Equals("about:blank")) return;

            string url = e.Url.ToString();
            if (url.StartsWith(OPENFILE_ON_LINE_URL)) {
                string file = url.Substring(OPENFILE_ON_LINE_URL.Length, url.LastIndexOf('@') - OPENFILE_ON_LINE_URL.Length);

                string lineNoStr = url.Substring(url.LastIndexOf('@') + 1);

                SolutionUtils.openSolutionFile(file, lineNoStr, solution);
            }
        }

        private bool navigateToTestClassAndMethod(BambooTest test) {
            string fileName = null, lineNo = null;
            foreach (Project project in solution.Projects) {
                if (examineProjectItems(project.ProjectItems, test.ClassName, test.MethodName, ref fileName, ref lineNo)) {
                    return SolutionUtils.openSolutionFile(fileName, lineNo, solution);
                }
            }
            return false;
        }

        private static bool examineProjectItems(ProjectItems projectItems, string classFqdn, string methodName, ref string fileName, ref string lineNo) {
            if (projectItems == null || projectItems.Count == 0) return false;
            foreach (ProjectItem item in projectItems) {
                if (examineOneItem(item, classFqdn, methodName, ref fileName, ref lineNo)) return true;
                if (examineProjectItems(item.ProjectItems, classFqdn, methodName, ref fileName, ref lineNo)) return true;
            }
            return false;
        }

        private static bool examineOneItem(ProjectItem item, string classFqdn, string methodName, ref string fileName, ref string lineNo) {
            FileCodeModel codeModel = item.FileCodeModel;
            if (codeModel == null || codeModel.CodeElements == null) return false;

            foreach (CodeElement element in codeModel.CodeElements) {
                if (examineClass(item, element, classFqdn, methodName, ref fileName, ref lineNo)) return true;
                if (examineNamespace(item, element, classFqdn, methodName, ref fileName, ref lineNo)) return true;
            }
            return false;
        }

        private static bool examineNamespace(ProjectItem item, CodeElement element, string classFqdn, string methodName, ref string fileName, ref string lineNo) {
            if (element.Kind != vsCMElement.vsCMElementNamespace) return false;

            foreach (CodeElement childElement in element.Children) {
                if (examineClass(item, childElement, classFqdn, methodName, ref fileName, ref lineNo)) return true;
                if (examineNamespace(item, childElement, classFqdn, methodName, ref fileName, ref lineNo)) return true;
            }
            return false;
        }

        private static bool examineClass(ProjectItem item, CodeElement element, string classFqdn, string methodName, ref string fileName, ref string lineNo) {
            if (element.Kind != vsCMElement.vsCMElementClass || !element.FullName.Equals(classFqdn)) return false;
            fileName = item.Name;
            foreach (CodeElement grandChildElement in element.Children) {
                if (grandChildElement.Kind != vsCMElement.vsCMElementFunction || !grandChildElement.Name.Equals(methodName)) continue;

                lineNo = "" + grandChildElement.StartPoint.Line + "," + grandChildElement.StartPoint.LineCharOffset;
                return true;
            }
            return false;
        }

        private void buttonLabel_Click(object sender, EventArgs e) {
            LabelBuild dlg = new LabelBuild(build, BambooBuildUtils.getPlanKey(build), status);
            dlg.ShowDialog();
        }

        private void buttonComment_Click(object sender, EventArgs e) {
            NewBuildComment dlg = new NewBuildComment(build, BambooBuildUtils.getPlanKey(build), status);
            dlg.ShowDialog();
        }
    }
}
