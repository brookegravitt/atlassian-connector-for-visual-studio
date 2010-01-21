using Atlassian.plvs.api.jira;
using Atlassian.plvs.ui;

namespace Atlassian.plvs.explorer.treeNodes {
    class VersionNode : AbstractNavigableTreeNodeWithServer {
        private readonly JiraProject project;
        private readonly JiraNamedEntity version;

        public VersionNode(JiraServer server, JiraProject project, JiraNamedEntity version) : base(server, version.Name, 0) {
            this.project = project;
            this.version = version;
        }

        public override string getUrl(string authString) {
            return Server.Url + "/browse/" + project.Key + "/fixforversion/" + version.Id + "?" + authString; 
        }

        public override void onClick(JiraServerFacade facade, StatusLabel status) { }
    }
}
