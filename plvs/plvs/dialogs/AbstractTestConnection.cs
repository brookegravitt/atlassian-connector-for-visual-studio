using System;
using System.Windows.Forms;
using System.Threading;
using Atlassian.plvs.api;

namespace Atlassian.plvs.dialogs {
    public abstract partial class AbstractTestConnection : Form {
        private bool testInProgress = true;
        private readonly Thread worker;

        public abstract void testConnection();

        protected AbstractTestConnection(Server server) {
            InitializeComponent();

            status.Text = "Testing connection to server " + server.Name + ", please wait...";
            buttonClose.Text = "Cancel";

            worker = new Thread(testConnection);

            worker.Start();
        }
                         
        private void buttonClose_Click(object sender, EventArgs e) {
            stopOrClose();
        }

        private void stopOrClose() {
            if (!testInProgress) {
                Close();
            } else {
                // too brutal?
                worker.Abort();
                stopTest("Test aborted");
            }
        }

        protected void stopTest(string text) {
            testInProgress = false;
            status.Text = text;
            progress.Visible = false;
            buttonClose.Text = "Close";
        }

        private void TestJiraConnection_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == (char) Keys.Escape) {
                stopOrClose();
            }
        }
    }
}