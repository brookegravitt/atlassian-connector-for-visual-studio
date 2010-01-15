using System;
using System.Collections.Generic;
using Atlassian.plvs.api.bamboo.rest;

namespace Atlassian.plvs.api.bamboo {
    public class BambooServerFacade {
        private readonly SortedDictionary<string, RestSession> sessionMap = new SortedDictionary<string, RestSession>();

        private static readonly BambooServerFacade INSTANCE = new BambooServerFacade();

        public static BambooServerFacade Instance {
            get { return INSTANCE; }
        }

        private BambooServerFacade() { }

        private RestSession getSession(Server server) {
            RestSession s;
            if (!sessionMap.TryGetValue(server.Url + server.UserName, out s)) {
                s = new RestSession(server.Url);
                s.login(server.UserName, server.Password);
                sessionMap.Add(getSessionKey(server), s);
            }
            return s;
        }

        private static string getSessionKey(Server server) {
            return server.Url + server.UserName;
        }

        private void removeSession(Server server) {
            getSession(server).logout();
            sessionMap.Remove(getSessionKey(server));
        }

        private delegate T Wrapped<T>();
        private T wrapExceptions<T>(Server server, Wrapped<T> wrapped) {
            try {
                return wrapped();
            } catch (Exception) {
                removeSession(server);
                throw;
            }
        }

        public void login(BambooServer server) {
            lock(this) {
                new RestSession(server.Url).login(server.UserName, server.Password);
            }
        }

        public void logout(BambooServer server) {
            lock (this) {
                getSession(server).logout();
            }
        }

        public ICollection<BambooPlan> getPlanList(BambooServer server) {
            lock (this) {
                return wrapExceptions(server, () => getSession(server).getAllPlans());
            }
        }

        public ICollection<BambooBuild> getLatestBuildsForFavouritePlans(BambooServer server) {
            lock (this) {
                return wrapExceptions(server, () => getSession(server).getLatestBuildsForFavouritePlans());
            }
        }
    }
}
