using Atlassian.plvs.api.jira;
using Atlassian.plvs.ui;
using Atlassian.plvs.ui.jira.issuefilternodes;

namespace Atlassian.plvs.explorer.treeNodes {
    public abstract class AbstractNavigableTreeNodeWithServer : TreeNodeWithJiraServer, NavigableJiraServerEntity {
        protected AbstractNavigableTreeNodeWithServer(JiraServer server, string name, int imageIdx) 
            : base(name, imageIdx) {
            Server = server;
        }

        public override sealed JiraServer Server { get; set; }

        public abstract string getUrl(string authString);
        public abstract void onClick(JiraServerFacade facade, StatusLabel status);
    }
}
