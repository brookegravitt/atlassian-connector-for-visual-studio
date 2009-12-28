using System.Windows.Forms;
using Atlassian.plvs.api;
using Atlassian.plvs.models;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Atlassian.plvs.windows {
    public partial class IssueDetailsWindow : UserControl {
        public static IssueDetailsWindow Instance { get; private set; }

        public ToolWindowPane DetailsFrame { get; set; }

        public bool FrameVisible {
            get { return DetailsFrame != null && ((IVsWindowFrame) DetailsFrame.Frame).IsVisible() == VSConstants.S_OK; }
            set {
                if (DetailsFrame == null) {
                    return;
                }
                if (value) {
                    ((IVsWindowFrame)DetailsFrame.Frame).Show();
                }
                else {
                    ((IVsWindowFrame)DetailsFrame.Frame).Hide();
                }
            }
        }

        private readonly JiraIssueListModel model = JiraIssueListModelImpl.Instance;

        public Solution Solution { get; set; }

        public IssueDetailsWindow() {
            InitializeComponent();

            Instance = this;
        }

        public void clearAllIssues() {
            issueTabs.TabPages.Clear();
        }

        public void openIssue(JiraIssue issue) {
            FrameVisible = true;

            string key = getIssueTabKey(issue);
            if (!issueTabs.TabPages.ContainsKey(key)) {
                TabPage issueTab = new TabPage {Name = key, Text = issue.Key};
                IssueDetailsPanel issuePanel = new IssueDetailsPanel(model, Solution, issue, issueTabs, issueTab);
                RecentlyViewedIssuesModel.Instance.add(issue);
                issueTab.Controls.Add(issuePanel);
                issuePanel.Dock = DockStyle.Fill;
                issueTabs.TabPages.Add(issueTab);
            }
            issueTabs.SelectTab(key);
        }

        private static string getIssueTabKey(JiraIssue issue) {
            return issue.Server.GUID + issue.Key;
        }
    }
}