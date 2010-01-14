using Atlassian.plvs.api.bamboo;

namespace Atlassian.plvs.ui.bamboo.bamboonodes {
    public class BambooServerTreeNode : TreeNodeWithBambooServer {
        private BambooServer server;

        public BambooServerTreeNode(BambooServer server, int imageIdx) : base(server.Name, imageIdx) {
            this.server = server;
        }

        public override BambooServer Server {
            get { return server; }
            set {
                server = value;
                Text = server.Name;
            }
        }
    }
}