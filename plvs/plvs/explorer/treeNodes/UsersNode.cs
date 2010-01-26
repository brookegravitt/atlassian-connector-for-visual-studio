using Atlassian.plvs.api.jira;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui;

namespace Atlassian.plvs.explorer.treeNodes {
    class UsersNode : AbstractNavigableTreeNodeWithServer {
        private bool usersLoaded;

        public UsersNode(JiraIssueListModel model, JiraServerFacade facade, JiraServer server)
            : base(model, facade, server, "Users", 0) {
        }

        public override string getUrl(string authString) {
            return Server.Url + "?" + authString;
        }

        public override void onClick(StatusLabel status) {
            if (usersLoaded) return;
            usersLoaded = true;
            loadPriorities();
        }

        private void loadPriorities() {
            JiraUserCache userCache = JiraServerCache.Instance.getUsers(Server);
            foreach (JiraUser user in userCache.getAllUsers()) {
                Nodes.Add(new UserNode(Model, Facade, Server, user));
            }
            ExpandAll();
        }
    }
}
