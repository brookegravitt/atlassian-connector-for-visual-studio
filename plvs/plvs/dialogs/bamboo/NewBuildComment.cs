using System.Windows.Forms;
using Atlassian.plvs.ui;

namespace Atlassian.plvs.dialogs.bamboo {
    public partial class NewBuildComment : Form {
        private readonly StatusLabel status;

        public NewBuildComment(StatusLabel status) {
            this.status = status;
            InitializeComponent();
        }

        private void newBuildCommentKeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar != (char)Keys.Escape) return;
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void buttonAdd_Click(object sender, System.EventArgs e) {

        }

        private void buttonCancel_Click(object sender, System.EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
