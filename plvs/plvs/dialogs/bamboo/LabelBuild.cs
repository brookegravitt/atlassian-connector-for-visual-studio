using System;
using System.Threading;
using System.Windows.Forms;
using Atlassian.plvs.api.bamboo;
using Atlassian.plvs.ui;
using Atlassian.plvs.util;

namespace Atlassian.plvs.dialogs.bamboo {
    public sealed partial class LabelBuild : Form {
        private readonly BambooBuild build;
        private readonly string planKey;
        private readonly StatusLabel status;

        public LabelBuild(BambooBuild build, string planKey, StatusLabel status) {
            this.build = build;
            this.planKey = planKey;
            this.status = status;
            InitializeComponent();

            Text = "Label Build";

            buttonOk.Enabled = false;
            StartPosition = FormStartPosition.CenterParent;
        }

        private void textLabel_TextChanged(object sender, EventArgs e) {
            buttonOk.Enabled = textLabel.Text.Length > 0;
        }

        private void buttonOk_Click(object sender, EventArgs e) {
            addLabelAndClose();
        }

        private void textLabel_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar != (char) Keys.Enter) return;
            addLabelAndClose();
        }

        private void addLabelAndClose() {
            status.setInfo("Adding label to build " + build.Key);
            setAllEnabled(false);
            Thread t = PlvsUtils.createThread(addLabelWorker);
            t.Start();
        }

        private void addLabelWorker() {
            try {
                BambooServerFacade.Instance.addLabel(build.Server, planKey, build.Number, textLabel.Text.Trim());
                status.setInfo("Added label to build " + build.Key);
            } catch (Exception e) {
                status.setError("Adding label to build failed", e);
            }
            DialogResult = DialogResult.OK;
            this.safeInvoke(new MethodInvoker(Close));
        }

        private void setAllEnabled(bool enabled) {
            foreach (Control ctrl in new Control[] { textLabel, buttonOk, buttonCancel }) {
                ctrl.Enabled = enabled;
            }
        }

        private void searchBuildKeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == (char) Keys.Escape && buttonCancel.Enabled) {
                Close();
            }
        }
    }
}