using System;
using System.Windows.Forms;
using System.Threading;
using Atlassian.plvs.api;
using Atlassian.plvs.api.soap;

namespace Atlassian.plvs.dialogs {
    public partial class TestJiraConnection : Form {

        private bool testInProgress = true;
        private readonly Thread worker;

        public TestJiraConnection(JiraServerFacade facade, JiraServer server) {
            InitializeComponent();

            status.Text = "Testing connection to server " + server.Name + ", please wait...";
            buttonClose.Text = "Cancel";

            worker = new Thread(new ThreadStart(delegate {
                                                    string result = "Success!!!";
                                                    try {
                                                        facade.login(server);
                                                    }
                                                    catch (SoapSession.LoginException e) {
                                                        result = e.InnerException.Message;
                                                    }
                                                    Invoke(new MethodInvoker(delegate { stopTest(result); }));
                                                }));

            worker.Start();
        }

        private void buttonClose_Click(object sender, EventArgs e) {
            if (!testInProgress) {
                Close();
            }
            else {
                // too brutal?
                worker.Abort();
                stopTest("Test aborted");
            }
        }

        private void stopTest(string text) {
            testInProgress = false;
            status.Text = text;
            progress.Visible = false;
            buttonClose.Text = "Close";
        }
    }
}