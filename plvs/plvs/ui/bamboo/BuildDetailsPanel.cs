using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;
using Atlassian.plvs.api.bamboo;
using Atlassian.plvs.autoupdate;
using Atlassian.plvs.util;
using Atlassian.plvs.util.bamboo;
using Process = System.Diagnostics.Process;
using EnvDTE;
using Thread = System.Threading.Thread;

namespace Atlassian.plvs.ui.bamboo {
    public partial class BuildDetailsPanel : UserControl {
        private readonly Solution solution;
        private readonly BambooBuild build;
        private readonly TabPage myTab;
        private readonly Action<TabPage> buttonCloseClicked;

        private const int MAX_PARSEABLE_LENGTH = 200;
        private const int A_LOT = 10000000;
        private const string OPENFILE_ON_LINE_URL = "openfileonline:";

        private readonly StatusLabel status;

        public BuildDetailsPanel(Solution solution, BambooBuild build, TabPage myTab,
            BuildDetailsWindow parent, Action<TabPage> buttonCloseClicked) {
            
            this.solution = solution;
            this.build = build;
            this.myTab = myTab;
            this.buttonCloseClicked = buttonCloseClicked;
            
            InitializeComponent();

            parent.ToolWindowShown += toolWindowStateMonitor_ToolWindowShown;
            parent.ToolWindowHidden += toolWindowStateMonitor_ToolWindowHidden;

            parent.setMyTabIconFromBuildResult(build.Result, myTab);

            status = new StatusLabel(statusStrip, labelStatus);

            webLog.WebBrowserShortcutsEnabled = true;
            webLog.IsWebBrowserContextMenuEnabled = true;
        }

        protected override void OnLoad(EventArgs e) {
            init();
        }

        private void toolWindowStateMonitor_ToolWindowHidden(object sender, EventArgs e) {
            uninit();
        }

        private void toolWindowStateMonitor_ToolWindowShown(object sender, EventArgs e) {
//            init();
        }

        private void init() {
            displaySummary();
            runGetLogThread();
        }

        private void uninit() {
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
            logCompleted = true;
            if (webLog.Document != null && webLog.Document.Body != null) webLog.Document.Body.ScrollTop = A_LOT;
        }

        private bool scrolledDown;

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e) {
            if (!tabControl.SelectedTab.Equals(tabLog)) return;
            if (scrolledDown) return;
            BeginInvoke(new MethodInvoker(delegate {
                                              if (webLog.Document != null && webLog.Document.Body != null) {
                                                  webLog.Document.Body.ScrollTop = A_LOT;
                                                  scrolledDown = true;
                                              }
                                          }));
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
    }
}
