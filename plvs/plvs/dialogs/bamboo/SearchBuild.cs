using System;
using System.Windows.Forms;
using Atlassian.plvs.ui;

namespace Atlassian.plvs.dialogs.bamboo {
    public sealed partial class SearchBuild : Form {
        public StatusLabel Status { get; set; }

        public SearchBuild(StatusLabel status) {
            Status = status;
            InitializeComponent();

            Text = "Find build";

            buttonOk.Enabled = false;
            StartPosition = FormStartPosition.CenterParent;
        }

        private void textQueryString_TextChanged(object sender, EventArgs e) {
            buttonOk.Enabled = textQueryString.Text.Length > 0;
        }

        private void buttonOk_Click(object sender, EventArgs e) {
            executeSearchAndClose();
        }

        private void textQueryString_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar != (char) Keys.Enter) return;
            executeSearchAndClose();
        }

//        private void fetchAndOpenIssue(string key) {
//            textQueryString.Enabled = false;
//            buttonOk.Enabled = false;
//            buttonCancel.Enabled = false;
//            AtlassianPanel.Instance.Jira.findAndOpenIssue(key, findFinished);
//        }

//        private void findFinished(bool success, string message, Exception e) {
//            if (!success) {
//                PlvsUtils.showError(message, e);
//            }
//            Close();
//        }

        private void executeSearchAndClose() {
            string query = textQueryString.Text.Trim();
            if (query.Length == 0) return;

//            if (JiraIssueUtils.ISSUE_REGEX.IsMatch(query.ToUpper())) {
//                JiraIssue foundIssue = Model.Issues.FirstOrDefault(issue => issue.Key.Equals(query) && issue.Server.Url.Equals(Server.Url));

//                if (foundIssue == null) {
//                    string key = query.ToUpper();
//                    fetchAndOpenIssue(key);
//                    return;
//                }
//                IssueDetailsWindow.Instance.openIssue(foundIssue, AtlassianPanel.Instance.Jira.ActiveIssueManager);
//            }
//            else {
//                string url = Server.Url + "/secure/QuickSearch.jspa?searchString=" + HttpUtility.UrlEncode(query);
//                Process.Start(url);
//            }
            Close();
        }

        private void searchBuildKeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == (char) Keys.Escape && buttonCancel.Enabled) {
                Close();
            }
        }
    }
}