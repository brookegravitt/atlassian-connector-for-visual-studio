using Atlassian.plvs.api.jira;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui;

namespace Atlassian.plvs.explorer.treeNodes {
    class ComponentNode : AbstractNavigableTreeNodeWithServer {
        private readonly JiraProject project;
        private readonly JiraNamedEntity comp;

        public ComponentNode(JiraIssueListModel model, JiraServerFacade facade, JiraServer server, JiraProject project, JiraNamedEntity comp)
            : base(model, facade, server, comp.Name, 0) {
            this.project = project;
            this.comp = comp;
        }

        public override string getUrl(string authString) {
            return Server.Url + "/browse/" + project.Key + "/component/" + comp.Id + "?" + authString; 
        }

        public override void onClick(StatusLabel status) { }
    }
}
