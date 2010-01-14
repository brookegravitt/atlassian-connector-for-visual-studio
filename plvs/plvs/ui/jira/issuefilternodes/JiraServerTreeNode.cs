using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.ui.jira.issuefilternodes {
    public class JiraServerTreeNode : TreeNodeWithJiraServer {
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