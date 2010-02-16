using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.util;

namespace Atlassian.plvs.dialogs.jira {
    public partial class NewIssueComment : Form {
        private readonly JiraIssue issue;
        private readonly JiraServerFacade facade;

        public NewIssueComment(JiraIssue issue, JiraServerFacade facade) {
            this.issue = issue;
            this.facade = facade;
            InitializeComponent();
            buttonOk.Enabled = false;

            StartPosition = FormStartPosition.CenterParent;
        }

        public string CommentBody {
            get { return commentText.Text; }
        }

        private void commentText_TextChanged(object sender, EventArgs e) {
            buttonOk.Enabled = commentText.Text.Trim().Length > 0;
        }

        private void NewIssueComment_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar != (char) Keys.Escape) return;
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void getPreview() {
            try {
                string content = facade.getRenderedContent(issue, commentText.Text);
                Invoke(new MethodInvoker(delegate {
                                             webPreview.DocumentText = content;
                                         }));
            } catch (Exception ex) {
                Debug.WriteLine("NewIssueComment.getPreview() - exception: " + ex.Message);
            }
        }

        private void tabCommentText_Selected(object sender, TabControlEventArgs e) {
            if (e.TabPage != tabPreview) return;
            Thread t = new Thread(getPreview);
            t.Start();
        }
    }
}