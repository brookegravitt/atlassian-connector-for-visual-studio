using System;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.autoupdate;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui.jira.issues;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Atlassian.plvs.ui.jira {
    public partial class IssueDetailsWindow : ToolWindowFrame, IVsWindowFrameNotify, ToolWindowStateMonitor {
        public static IssueDetailsWindow Instance { get; private set; }

        private readonly JiraIssueListModel model = JiraIssueListModelImpl.Instance;

        public Solution Solution { get; set; }


        public event EventHandler<EventArgs> ToolWindowShown;
        public event EventHandler<EventArgs> ToolWindowHidden;

        public IssueDetailsWindow() {
            InitializeComponent();

            Instance = this;
        }

        public void clearAllIssues() {
            // cheating :) - but it is the easiest way to make all 
            // open issue tabs unregister their model listeners
            if (ToolWindowHidden != null) {
                ToolWindowHidden(this, new EventArgs());
            }
            issueTabs.TabPages.Clear();
        }

        public void openIssue(JiraIssue issue) {
            FrameVisible = true;

            string key = getIssueTabKey(issue);
            if (!issueTabs.TabPages.ContainsKey(key)) {
                TabPage issueTab = new TabPage {Name = key, Text = issue.Key};
                IssueDetailsPanel issuePanel = new IssueDetailsPanel(model, Solution, issue, issueTabs, issueTab, this);
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

        public int OnShow(int fShow) {
            switch (fShow) {
                case (int)__FRAMESHOW.FRAMESHOW_WinShown:
                    if (ToolWindowShown != null) {
                        ToolWindowShown(this, new EventArgs());
                    }
                    break;
                case (int)__FRAMESHOW.FRAMESHOW_WinHidden:
                    if (ToolWindowHidden != null) {
                        ToolWindowHidden(this, new EventArgs());
                    }

                    break;
            }

            return VSConstants.S_OK;
        }

        public int OnMove() {
            return VSConstants.S_OK;
        }

        public int OnSize() {
            return VSConstants.S_OK;
        }

        public int OnDockableChange(int fDockable) {
            return VSConstants.S_OK;
        }
    }
}