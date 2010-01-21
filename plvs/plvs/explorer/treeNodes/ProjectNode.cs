using Atlassian.plvs.api.jira;
using Atlassian.plvs.ui;

namespace Atlassian.plvs.explorer.treeNodes {
    class ProjectNode : AbstractNavigableTreeNodeWithServer {
        private readonly JiraProject project;

        public ProjectNode(JiraServer server, JiraProject project) : base(server, project.ToString(), 0) {
            this.project = project;
            Server = server;
        }

        public override string getUrl(string authString) {
            return Server.Url + "/browse/" + project.Key + "?" + authString;
        }

        public override void onClick(JiraServerFacade facade, StatusLabel status) { }

        public override string ToString() {
            return project.ToString();
        }
    }
}
