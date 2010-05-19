using System;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using Atlassian.plvs.api.bamboo;
using Atlassian.plvs.autoupdate;
using Atlassian.plvs.util;
using Atlassian.plvs.util.jira;
using Process = System.Diagnostics.Process;
using EnvDTE;
using Thread = System.Threading.Thread;

namespace Atlassian.plvs.ui.bamboo {
    public partial class BuildDetailsPanel : UserControl {
        private readonly Solution solution;
        private readonly BambooBuild build;
        private readonly TabPage myTab;
        private readonly Action<TabPage> buttonCloseClicked;

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
        }

        protected override void OnLoad(EventArgs e) {
            init();
        }

        private void toolWindowStateMonitor_ToolWindowHidden(object sender, EventArgs e) {
            uninit();
        }

        private void toolWindowStateMonitor_ToolWindowShown(object sender, EventArgs e) {
            init();
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
            Thread.Sleep(1000);
            status.setInfo("Build log retrieved");    
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
    }
}
