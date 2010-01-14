using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.api.jira.soap;

namespace Atlassian.plvs.dialogs.jira {
    public sealed class TestJiraConnection : AbstractTestConnection {
        private readonly JiraServerFacade facade;
        private readonly JiraServer server;

        public TestJiraConnection(JiraServerFacade facade, JiraServer server) : base(server) {
            this.facade = facade;
            this.server = server;
        }

        public override void testConnection() {
            var result = "Success!!!";
            try {
                facade.login(server);
            } catch (SoapSession.LoginException e) {
                result = e.InnerException.Message;
            }
            Invoke(new MethodInvoker(() => stopTest(result)));
        }
    }
}