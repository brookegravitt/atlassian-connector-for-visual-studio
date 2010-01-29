using System;
using System.Windows.Forms;
using Atlassian.plvs.api.bamboo;

namespace Atlassian.plvs.dialogs.bamboo {
    class TestBambooConnection : AbstractTestConnection {
        private readonly BambooServerFacade facade;
        private readonly BambooServer server;

        public TestBambooConnection(BambooServerFacade facade, BambooServer server) : base(server) {
            this.facade = facade;
            this.server = server;
        }

        public override void testConnection() {
            var result = "Connection to server successful";
            bool error = false;
            try {
                facade.login(server);
                facade.logout(server);
            } catch (Exception e) {
                result = e.Message;
                error = true;
            }
            Invoke(new MethodInvoker(() => stopTest(error, result)));
        }
    }
}
