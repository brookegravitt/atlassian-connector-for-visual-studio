using System;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.dialogs {
    public sealed partial class LogWork : Form {
        private readonly JiraIssue issue;

        public LogWork(JiraIssue issue) {
            this.issue = issue;
            InitializeComponent();

            Text = "Log for for issue " + issue.Key;

            StartPosition = FormStartPosition.CenterParent;
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            Close();
        }
    }
}
