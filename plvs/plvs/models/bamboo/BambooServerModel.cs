using System;
using Atlassian.plvs.api.bamboo;
using Atlassian.plvs.store;

namespace Atlassian.plvs.models.bamboo {
    public class BambooServerModel : AbstractServerModel<BambooServer> {
        private static readonly BambooServer ServerForType = new BambooServer(null, null, null, null);

        private BambooServerModel() { }

        private static readonly BambooServerModel INSTANCE = new BambooServerModel();
        
        private const string USE_FAVOURITES = "UseFavourites_";

        public static BambooServerModel Instance { get { return INSTANCE; } }

        protected override ParameterStoreManager.StoreType StoreType { get { return ParameterStoreManager.StoreType.BAMBOO_SERVERS; } }
        protected override Guid SupportedServerType { get { return ServerForType.Type; } }

        protected override void loadCustomServerParameters(ParameterStore store, BambooServer server) {
            server.UseFavourites = store.loadParameter(USE_FAVOURITES + server.GUID, 1) > 0;
        }

        protected override void saveCustomServerParameters(ParameterStore store, BambooServer server) {
            store.storeParameter(USE_FAVOURITES + server.GUID, server.UseFavourites ? 1 : 0);
        }

        protected override BambooServer createServer(Guid guid, string name, string url, string userName, string password) {
            return new BambooServer(guid, name, url, userName, password);
        }
    }
}