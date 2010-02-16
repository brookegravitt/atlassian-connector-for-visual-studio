using System;
using System.Collections.Generic;
using System.Text;
using Atlassian.plvs.api.bamboo;
using Atlassian.plvs.store;

namespace Atlassian.plvs.models.bamboo {
    public class BambooServerModel : AbstractServerModel<BambooServer> {
        private static readonly BambooServer ServerForType = new BambooServer(null, null, null, null);

        private BambooServerModel() { }

        private static readonly BambooServerModel INSTANCE = new BambooServerModel();
        
        private const string USE_FAVOURITES = "UseFavourites_";
        private const string PLAN_KEYS = "PlanKeys_";

        public static BambooServerModel Instance { get { return INSTANCE; } }

        protected override ParameterStoreManager.StoreType StoreType { get { return ParameterStoreManager.StoreType.BAMBOO_SERVERS; } }
        protected override Guid SupportedServerType { get { return ServerForType.Type; } }

        protected override void loadCustomServerParameters(ParameterStore store, BambooServer server) {
            server.UseFavourites = store.loadParameter(USE_FAVOURITES + server.GUID, 1) > 0;
            string keyString = store.loadParameter(PLAN_KEYS + server.GUID, "");
            if (keyString.Trim().Length <= 0) return;
            string[] keys = keyString.Split(new[] { ' ' });
            server.PlanKeys = new List<string>(keys.Length);
            server.PlanKeys.AddRange(keys);
        }

        protected override void saveCustomServerParameters(ParameterStore store, BambooServer server) {
            store.storeParameter(USE_FAVOURITES + server.GUID, server.UseFavourites ? 1 : 0);

            StringBuilder sb = new StringBuilder();
            if (server.PlanKeys != null) {
                foreach (var key in server.PlanKeys) {
                    sb.Append(key).Append(" ");
                }
            }
            store.storeParameter(PLAN_KEYS + server.GUID, sb.ToString().Trim());
        }

        protected override BambooServer createServer(Guid guid, string name, string url, string userName, string password, bool enabled) {
            return new BambooServer(guid, name, url, userName, password, enabled);
        }
    }
}