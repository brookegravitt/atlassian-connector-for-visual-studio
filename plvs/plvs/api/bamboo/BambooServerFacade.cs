using System.Collections.Generic;
using Atlassian.plvs.api.bamboo.rest;
using Atlassian.plvs.util;

namespace Atlassian.plvs.api.bamboo {
    public class BambooServerFacade : ServerFacade {
        private static readonly BambooServerFacade INSTANCE = new BambooServerFacade();

        public static BambooServerFacade Instance {
            get { return INSTANCE; }
        }

        private BambooServerFacade() {
            PlvsUtils.installSslCertificateHandler();
        }

        private static RestSession createSessionAndLogin(BambooServer server) {
            RestSession s = new RestSession(server);
            s.login(server.UserName, server.Password);
            return s;
        }

        private delegate T Wrapped<T>();
        private static T wrapExceptions<T>(RestSession session, Wrapped<T> wrapped) {
            T result = wrapped();
            session.logout();
            return result;
        }

        private delegate void WrappedVoid();
        private static void wrapExceptionsVoid(RestSession session, WrappedVoid wrapped) {
            wrapped();
            session.logout();
        }

        public void login(BambooServer server) {
            new RestSession(server).login(server.UserName, server.Password);
        }

        public ICollection<BambooPlan> getPlanList(BambooServer server) {
            RestSession session = createSessionAndLogin(server);
            return wrapExceptions(session, () => session.getAllPlans());
        }

        public ICollection<BambooBuild> getLatestBuildsForFavouritePlans(BambooServer server) {
            RestSession session = createSessionAndLogin(server);
            return wrapExceptions(session, () => session.getLatestBuildsForFavouritePlans());
        }

        public ICollection<BambooBuild> getLatestBuildsForPlanKeys(BambooServer server, ICollection<string> keys) {
            RestSession session = createSessionAndLogin(server);
            return wrapExceptions(session, () => session.getLatestBuildsForPlanKeys(keys));
        }

        public void runBuild(BambooServer server, string planKey) {
            RestSession session = createSessionAndLogin(server);
            wrapExceptionsVoid(session, () => session.runBuild(planKey));
        }

        public void dropAllSessions() {}
    }
}
