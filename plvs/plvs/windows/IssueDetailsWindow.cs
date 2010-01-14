using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui;
using EnvDTE;

namespace Atlassian.plvs.windows {
    public partial class IssueDetailsWindow : ToolWindowFrame {
        public static IssueDetailsWindow Instance { get; private set; }

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