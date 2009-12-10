using System.Windows.Forms;

namespace Atlassian.plvs.dialogs {
    public partial class GlobalSettings : Form {
        static GlobalSettings() {
            JiraIssuesBatch = 25;
        }

        public GlobalSettings() {
            InitializeComponent();

            StartPosition = FormStartPosition.CenterParent;
        }

        public static int JiraIssuesBatch { get; private set; }

        private void GlobalSettings_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == (char) Keys.Escape) {
                Close();
            }
        }
    }
}