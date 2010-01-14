using System;
using System.Windows.Forms;
using Atlassian.plvs.models.bamboo;

namespace Atlassian.plvs.dialogs {
    public partial class AddOrEditBambooServer : Form {
        private static int invocations;

        private readonly bool editing;

        private readonly BambooServer server;

        public AddOrEditBambooServer(BambooServer server) {
            InitializeComponent();

            editing = server != null;

            this.server = new BambooServer(server);

            Text = editing ? "Edit Bamboo Server" : "Add Bamboo Server";
            buttonAddOrEdit.Text = editing ? "Apply Changes" : "Add Server";

            if (editing) {
                if (server != null) {
                    name.Text = server.Name;
                    url.Text = server.Url;
                    user.Text = server.UserName;
                    password.Text = server.Password;
                }
            }
            else {
                ++invocations;
                name.Text = "Bamboo Server #" + invocations;
                buttonAddOrEdit.Enabled = false;
            }

            StartPosition = FormStartPosition.CenterParent;
        }

        public override sealed string Text {
            get { return base.Text; }
            set { base.Text = value; }
        }

        private void buttonAddOrEdit_Click(object sender, EventArgs e) {
            server.Name = name.Text.Trim();
            string fixedUrl = url.Text.Trim();
            if (!(fixedUrl.StartsWith("http://") || fixedUrl.StartsWith("https://"))) {
                fixedUrl = "http://" + fixedUrl;
            }
            if (fixedUrl.EndsWith("/")) {
                fixedUrl = fixedUrl.Substring(0, fixedUrl.Length - 1);
            }
            server.Url = fixedUrl;
            server.UserName = user.Text.Trim();
            server.Password = password.Text;
            
            // todo!
            server.UseFavourites = true;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void name_TextChanged(object sender, EventArgs e) {
            checkIfValid();
        }

        private void url_TextChanged(object sender, EventArgs e) {
            checkIfValid();
        }

        private void user_TextChanged(object sender, EventArgs e) {
            checkIfValid();
        }

        private void checkIfValid() {
            buttonAddOrEdit.Enabled = name.Text.Trim().Length > 0 && url.Text.Trim().Length > 0 &&
                                      user.Text.Trim().Length > 0;
        }

        public BambooServer Server {
            get { return server; }
        }

        private void AddOrEditJiraServer_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar != (char) Keys.Escape) return;
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}