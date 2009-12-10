using System.Windows.Forms;

namespace Atlassian.plvs.dialogs {
    public partial class NewIssueComment : Form {
        public NewIssueComment() {
            InitializeComponent();
            buttonOk.Enabled = false;

            StartPosition = FormStartPosition.CenterParent;
        }

        public string CommentBody {
            get { return commentText.Text; }
        }

        private void commentText_TextChanged(object sender, System.EventArgs e) {
            buttonOk.Enabled = commentText.Text.Trim().Length > 0;
        }

        private void NewIssueComment_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar != (char) Keys.Escape) return;
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}