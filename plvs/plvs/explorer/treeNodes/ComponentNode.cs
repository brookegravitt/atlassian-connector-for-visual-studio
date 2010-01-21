using Atlassian.plvs.api.jira;
using Atlassian.plvs.ui;

namespace Atlassian.plvs.explorer.treeNodes {
    class ComponentNode : AbstractNavigableTreeNodeWithServer {
        private readonly JiraProject project;
        private readonly JiraNamedEntity comp;

        public ComponentNode(JiraServer server, JiraProject project, JiraNamedEntity comp)
            : base(server, comp.Name, 0) {
            this.project = project;
            this.comp = comp;
        }

        public override string getUrl(string authString) {
            return Server.Url + "/browse/" + project.Key + "/component/" + comp.Id + "?" + authString; 
        }

        public override void onClick(JiraServerFacade facade, StatusLabel status) { }
    }
}
