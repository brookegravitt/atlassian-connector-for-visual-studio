using System;
using System.Collections.Generic;
using Atlassian.plvs.api.bamboo.rest;

namespace Atlassian.plvs.api.bamboo {
    public class BambooServerFacade : ServerFacade {
        private readonly SortedDictionary<string, RestSession> sessionMap = new SortedDictionary<string, RestSession>();

        private static readonly BambooServerFacade INSTANCE = new BambooServerFacade();

        public static BambooServerFacade Instance {
            get { return INSTANCE; }
        }

        private BambooServerFacade() { }

        private RestSession getSession(BambooServer server) {
            RestSession s;
            if (!sessionMap.TryGetValue(server.Url + server.UserName, out s)) {
                s = new RestSession(server);
                s.login(server.UserName, server.Password);
                sessionMap.Add(getSessionKey(server), s);
            }
            return s;
        }

        private static string getSessionKey(Server server) {
            return server.Url + server.UserName;
        }

        private void removeSession(BambooServer server) {
            getSession(server).logout();
            sessionMap.Remove(getSessionKey(server));
        }

        private delegate T Wrapped<T>();
        private T wrapExceptions<T>(BambooServer server, Wrapped<T> wrapped) {
            lock (this) {
                try {
                    return wrapped();
                } catch (Exception) {
                    removeSession(server);
                    throw;
                }
            }
        }

        private delegate void WrappedVoid();
        private void wrapExceptionsVoid(BambooServer server, WrappedVoid wrapped) {
            lock (this) {
                try {
                    wrapped();
                } catch (Exception) {
                    removeSession(server);
                    throw;
                }
            }
        }

        public void login(BambooServer server) {
            lock(this) {
                new RestSession(server).login(server.UserName, server.Password);
            }
        }

        public void logout(BambooServer server) {
            lock (this) {
                getSession(server).logout();
            }
        }

        public ICollection<BambooPlan> getPlanList(BambooServer server) {
            return wrapExceptions(server, () => getSession(server).getAllPlans());
        }

        public ICollection<BambooBuild> getLatestBuildsForFavouritePlans(BambooServer server) {
            return wrapExceptions(server, () => getSession(server).getLatestBuildsForFavouritePlans());
        }

        public void runBuild(BambooServer server, string planKey) {
            wrapExceptionsVoid(server, () => getSession(server).runBuild(planKey));
        }

        public void dropAllSessions() {
            lock(this) {
                sessionMap.Clear();
            }
        }
    }
}
