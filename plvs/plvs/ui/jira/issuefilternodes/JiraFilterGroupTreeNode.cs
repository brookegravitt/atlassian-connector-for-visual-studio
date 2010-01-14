using System;
using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.ui.jira.issuefilternodes {
    public class JiraFilterGroupTreeNode: TreeNodeWithJiraServer {
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