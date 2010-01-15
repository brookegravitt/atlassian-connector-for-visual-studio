using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.ui.jira;

namespace Atlassian.plvs.scm {
    public partial class AnkhSvnJiraActiveIssueControl : UserControl {
        private readonly bool enabled;

        private const string NO_ISSUE_SELECTED = "No issue is selected in the Atlassian Connector window";
        private const string NO_INTEGRATION = "Atlassian Connector Integration Disabled";

        public AnkhSvnJiraActiveIssueControl(bool enabled) {
            this.enabled = enabled;
            InitializeComponent();

//            IssueListWindow.Instance.SelectedIssueChanged += selectedIssueChanged;
//            labelJira.Text = enabled ? getCommentText() : NO_INTEGRATION;
        }

        private void selectedIssueChanged(object sender, TabJira.SelectedIssueEventArgs e) {
            labelJira.Text = enabled ? getCommentText() : NO_INTEGRATION;
        }

        private static string getCommentText() {
//            JiraIssue issue = IssueListWindow.Instance.SelectedIssue;
//            if (issue == null) {
//                return NO_ISSUE_SELECTED;
//            }
//            return "Commit message is set to: \"" + issue.Key + " - " + issue.Summary + "\"";
            return null;
        }
    }
}
