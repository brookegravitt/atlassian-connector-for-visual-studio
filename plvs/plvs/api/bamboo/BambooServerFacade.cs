using System;
using System.Collections.Generic;
using Atlassian.plvs.api.bamboo.rest;
using Atlassian.plvs.models.bamboo;

namespace Atlassian.plvs.api.bamboo {
    public class BambooServerFacade {
        private readonly SortedDictionary<string, RestSession> sessionMap = new SortedDictionary<string, RestSession>();

        private static readonly BambooServerFacade INSTANCE = new BambooServerFacade();

        public static BambooServerFacade Instance {
            get { return INSTANCE; }
        }

        private BambooServerFacade() { }

        private RestSession getSession(BambooServer server) {
            RestSession s;
            if (!sessionMap.TryGetValue(server.Url + server.UserName, out s)) {
                s = new RestSession(server.Url);
                sessionMap.Add(getSessionKey(server), s);
            }
            return s;
        }

        private static string getSessionKey(BambooServer server) {
            return server.Url + server.UserName;
        }

        private void removeSession(BambooServer server) {
            sessionMap.Remove(getSessionKey(server));
        }

        private delegate T Wrapped<T>();
        private T wrapExceptions<T>(BambooServer server, Wrapped<T> wrapped) {
            try {
                return wrapped();
            } catch (Exception) {
                removeSession(server);
                throw;
            }
        }

        public void login(BambooServer server, string userName, string password) {
            getSession(server).login(userName, password);
        }

        public void logout(BambooServer server) {
            getSession(server).logout();
        }

        public ICollection<BambooPlan> getPlanList(BambooServer server) {
            return wrapExceptions(server, () => getSession(server).login(server.UserName, server.Password).getPlanList(true));
        }

        public ICollection<BambooBuild> getLatestBuildsForFavouritePlans(BambooServer server) {
            return wrapExceptions(server, () => getSession(server).login(server.UserName, server.Password).getLatestBuildsForFavouritePlans(true));
        }
    }
}
