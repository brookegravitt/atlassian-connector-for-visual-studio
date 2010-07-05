using System;
using System.Threading;
using System.Windows.Forms;
using Atlassian.plvs.api.bamboo;
using Atlassian.plvs.ui;
using Atlassian.plvs.util;

namespace Atlassian.plvs.dialogs.bamboo {
    public partial class NewBuildComment : Form {
        private readonly BambooBuild build;
        private readonly string planKey;
        private readonly StatusLabel status;

        public NewBuildComment(BambooBuild build, string planKey, StatusLabel status) {
            this.build = build;
            this.planKey = planKey;
            this.status = status;
            InitializeComponent();
            buttonAdd.Enabled = false;
        }

        protected override void OnLoad(EventArgs e) {
            ActiveControl = textComment;
        }

        private void newBuildCommentKeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar != (char)Keys.Escape) return;
            if (!buttonCancel.Enabled) return;
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void buttonAdd_Click(object sender, EventArgs e) {
            status.setInfo("Adding comment to build " + build.Key);
            setAllEnabled(false);
            Thread t = PlvsUtils.createThread(addCommentWorker);
            t.Start();
        }

        private void addCommentWorker() {
            try {
                BambooServerFacade.Instance.addComment(build.Server, planKey, build.Number, textComment.Text.Trim());
                status.setInfo("Added comment to build " + build.Key);
            } catch (Exception e) {
                status.setError("Adding comment to build failed", e);
            }
            DialogResult = DialogResult.OK;
            this.safeInvoke(new MethodInvoker(Close));
        }

        private void setAllEnabled(bool enabled) {
            foreach (Control ctrl in new Control[] { textComment, buttonAdd, buttonCancel }) {
                ctrl.Enabled = enabled;
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void textComment_TextChanged(object sender, EventArgs e) {
            buttonAdd.Enabled = textComment.Text.Trim().Length > 0;
        }
    }
}
