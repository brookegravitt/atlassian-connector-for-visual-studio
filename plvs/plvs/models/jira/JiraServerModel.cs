using System;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.store;

namespace Atlassian.plvs.models.jira {
    public class JiraServerModel : AbstractServerModel<JiraServer> {

        private static readonly JiraServer ServerForType = new JiraServer(null, null, null, null);

        private JiraServerModel() { }

        private static readonly JiraServerModel INSTANCE = new JiraServerModel();

        public static JiraServerModel Instance { get { return INSTANCE; } }

        protected override ParameterStoreManager.StoreType StoreType { get { return ParameterStoreManager.StoreType.JIRA_SERVERS; } }
        protected override Guid SupportedServerType { get { return ServerForType.Type; } }

        protected override void loadCustomServerParameters(ParameterStore store, JiraServer server) {}
        protected override void saveCustomServerParameters(ParameterStore store, JiraServer server) {}

        protected override JiraServer createServer(Guid guid, string name, string url, string userName, string password, bool enabled) {
            return new JiraServer(guid, name, url, userName, password, enabled);
        }
    }
}