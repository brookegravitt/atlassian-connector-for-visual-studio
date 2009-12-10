using System;
using Atlassian.plvs.api;

namespace Atlassian.plvs.ui {
    public class JiraFilterGroupTreeNode: TreeNodeWithServer {
        private readonly JiraServer server;

        public JiraFilterGroupTreeNode(JiraServer server, string name, int imageIdx) : base(name, imageIdx) {
            this.server = server;
        }

        public override JiraServer Server {
            get { return server; }
            set { throw new NotImplementedException(); }
        }
    }
}
