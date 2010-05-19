using System;
using System.Windows.Forms;
using Atlassian.plvs.api.bamboo;
using Atlassian.plvs.autoupdate;
using Atlassian.plvs.ui.jira.issues;
using EnvDTE;

namespace Atlassian.plvs.ui.bamboo {
    public sealed partial class BuildDetailsWindow : ToolWindowFrame, ToolWindowStateMonitor {
        public static BuildDetailsWindow Instance { get; private set; }

        public Solution Solution { get; set; }

        public event EventHandler<EventArgs> ToolWindowShown;
        public event EventHandler<EventArgs> ToolWindowHidden;

        public BuildDetailsWindow() {
            InitializeComponent();

            Instance = this;
        }

        public void clearAllBuilds() {
            if (ToolWindowHidden != null) {
                ToolWindowHidden(this, new EventArgs());
            }
            tabBuilds.TabPages.Clear();
        }

        public void openBuild(BambooBuild build) {
            FrameVisible = true;

            string key = getBuildTabKey(build);
            if (!tabBuilds.TabPages.ContainsKey(key)) {
                TabPage buildTab = new TabPage { Name = key, Text = build.Key };
//                IssueDetailsPanel issuePanel = new IssueDetailsPanel(model, Solution, issue, issueTabs, issueTab, this, activeIssueManager);
//                tabBuilds.Controls.Add(issuePanel);
//                issuePanel.Dock = DockStyle.Fill;
                tabBuilds.TabPages.Add(buildTab);
            }
            tabBuilds.SelectTab(key);
            UsageCollector.Instance.bumpBambooBuildsOpen();
        }

        private static string getBuildTabKey(BambooBuild build) {
            return build.Server.GUID + build.Key;
        }

        protected override void notifyWindowVisibility(bool visible) {
            if (visible) {
                if (ToolWindowShown != null) {
                    ToolWindowShown(this, new EventArgs());
                }
            } else {
                if (ToolWindowHidden != null) {
                    ToolWindowHidden(this, new EventArgs());
                }
            }
        }
    }
}
