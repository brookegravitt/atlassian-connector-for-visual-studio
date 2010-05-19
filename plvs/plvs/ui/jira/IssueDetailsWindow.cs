using System;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.autoupdate;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui.jira.issues;
using EnvDTE;

namespace Atlassian.plvs.ui.jira {
    public sealed partial class IssueDetailsWindow : ToolWindowFrame, ToolWindowStateMonitor {
        public static IssueDetailsWindow Instance { get; private set; }

        private readonly JiraIssueListModel model = JiraIssueListModelImpl.Instance;

        public Solution Solution { get; set; }

        public IssueDetailsWindow() {
            InitializeComponent();

            Instance = this;
        }

        public event EventHandler<EventArgs> ToolWindowShown;
        public event EventHandler<EventArgs> ToolWindowHidden;

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

        public void clearAllIssues() {
            // cheating :) - but it is the easiest way to make all 
            // open issue tabs unregister their model listeners
            if (ToolWindowHidden != null) {
                ToolWindowHidden(this, new EventArgs());
            }
            issueTabs.TabPages.Clear();
        }

        public void openIssue(JiraIssue issue, JiraActiveIssueManager activeIssueManager) {
            FrameVisible = true;

            string key = getIssueTabKey(issue);
            if (!issueTabs.TabPages.ContainsKey(key)) {
                TabPage issueTab = new TabPage {Name = key, Text = issue.Key};
                IssueDetailsPanel issuePanel = new IssueDetailsPanel(model, Solution, issue, issueTabs, issueTab, this, activeIssueManager);
                RecentlyViewedIssuesModel.Instance.add(issue);
                issueTab.Controls.Add(issuePanel);
                issuePanel.Dock = DockStyle.Fill;
                issueTabs.TabPages.Add(issueTab);
            }
            issueTabs.SelectTab(key);
            UsageCollector.Instance.bumpJiraIssuesOpen();
        }

        private static string getIssueTabKey(JiraIssue issue) {
            return issue.Server.GUID + issue.Key;
        }
    }
}