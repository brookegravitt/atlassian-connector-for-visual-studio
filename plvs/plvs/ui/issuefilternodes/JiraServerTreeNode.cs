using Atlassian.plvs.api;

namespace Atlassian.plvs.ui.issuefilternodes {
    public class JiraServerTreeNode : TreeNodeWithServer {
        private JiraServer server;

        public JiraServerTreeNode(JiraServer server, int imageIdx)
            : base(server.Name, imageIdx) {
            this.server = server;
        }

        public override JiraServer Server {
            get { return server; }
            set {
                server = value;
                Text = server.Name;
            }
        }
    }
}