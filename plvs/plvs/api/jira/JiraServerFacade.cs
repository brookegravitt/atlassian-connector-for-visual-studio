using System;
using System.Collections.Generic;
using System.Diagnostics;
using Atlassian.plvs.api.jira.soap;
using Atlassian.plvs.util;

namespace Atlassian.plvs.api.jira {
    public class JiraServerFacade : ServerFacade {

        private static readonly JiraServerFacade INSTANCE = new JiraServerFacade();

        public static JiraServerFacade Instance {
            get { return INSTANCE; }
        }

        private readonly Dictionary<string, string> tokenMap = new Dictionary<string, string>();

        private JiraServerFacade() {
            PlvsUtils.installSslCertificateHandler();
        }

        public void login(JiraServer server) {
            try {
                createSoapSession(server).login(server.UserName, server.Password);
            } catch (Exception e) {
                maybeHandle503(server, e);
                throw;
            }
        }

        public void dropAllSessions() {
            lock(tokenMap) {
                tokenMap.Clear();
            }
        }

        public string getSoapToken(JiraServer server) {
            try {
                using (SoapSession session = createSoapSession(server)) {
                    return session.login(server.UserName, server.Password);
                }
            } catch (Exception e) {
                Debug.WriteLine("JiraServerFacade.getSoapToken() - exception: " + e.Message);
            }
            return null;
        }

        public List<JiraIssue> getSavedFilterIssues(JiraServer server, JiraSavedFilter filter, int start, int count) {
            RssClient rss = new RssClient(server);
            return rss.getSavedFilterIssues(filter.Id, "priority", "DESC", start, count);
        }

        public List<JiraIssue> getCustomFilterIssues(JiraServer server, JiraFilter filter, int start, int count) {
            RssClient rss = new RssClient(server);
            return rss.getCustomFilterIssues(filter.getFilterQueryString(), filter.getSortBy(), "DESC", start, count);
        }

        public JiraIssue getIssue(JiraServer server, string key) {
            RssClient rss = new RssClient(server);
            return rss.getIssue(key);
        }

        public string getRenderedContent(JiraIssue issue, string markup) {
            RestClient rest = new RestClient(issue.Server);
            return rest.getRenderedContent(issue.Key, -1, -1, markup);
        }

        public string getRenderedContent(JiraServer server, int issueTypeId, JiraProject project, string markup) {
            RestClient rest = new RestClient(server);
            return rest.getRenderedContent(null, issueTypeId, project.Id, markup);
        }

        public List<JiraProject> getProjects(JiraServer server) {
            using (SoapSession s = createSoapSession(server)) {
                return wrapExceptions(server, s, () => s.getProjects());
            }
        }

        public List<JiraNamedEntity> getIssueTypes(JiraServer server) {
            using (SoapSession s = createSoapSession(server)) {
                return wrapExceptions(server, s, () => s.getIssueTypes());
            }
        }

        public List<JiraNamedEntity> getSubtaskIssueTypes(JiraServer server) {
            using (SoapSession s = createSoapSession(server)) {
                return wrapExceptions(server, s, () => s.getSubtaskIssueTypes());
            }
        }

        public List<JiraNamedEntity> getSubtaskIssueTypes(JiraServer server, JiraProject project) {
            using (SoapSession s = createSoapSession(server)) {
                return wrapExceptions(server, s, () => s.getSubtaskIssueTypes(project));
            }
        }

        public List<JiraNamedEntity> getIssueTypes(JiraServer server, JiraProject project) {
            using (SoapSession s = createSoapSession(server)) {
                return wrapExceptions(server, s, () => s.getIssueTypes(project));
            }
        }

        public List<JiraSavedFilter> getSavedFilters(JiraServer server) {
            using (SoapSession s = createSoapSession(server)) {
                return wrapExceptions(server, s, () => s.getSavedFilters());
            }
        }

        public List<JiraNamedEntity> getPriorities(JiraServer server) {
            using (SoapSession s = createSoapSession(server)) {
                return wrapExceptions(server, s, () => s.getPriorities());
            }
        }

        public List<JiraNamedEntity> getStatuses(JiraServer server) {
            using (SoapSession s = createSoapSession(server)) {
                return wrapExceptions(server, s, () => s.getStatuses());
            }
        }

        public List<JiraNamedEntity> getResolutions(JiraServer server) {
            using (SoapSession s = createSoapSession(server)) {
                return wrapExceptions(server, s, () => s.getResolutions());
            }
        }

        public void addComment(JiraIssue issue, string comment) {
            using (SoapSession s = createSoapSession(issue.Server)) {
                wrapExceptionsVoid(issue.Server, s, () => s.addComment(issue, comment));
            }
        }

        public List<JiraNamedEntity> getActionsForIssue(JiraIssue issue) {
            using (SoapSession s = createSoapSession(issue.Server)) {
                return wrapExceptions(issue.Server, s, () => s.getActionsForIssue(issue));
            }
        }

        public List<JiraField> getFieldsForAction(JiraIssue issue, int actionId) {
            using (SoapSession s = createSoapSession(issue.Server)) {
                return wrapExceptions(issue.Server, s, () => s.getFieldsForAction(issue, actionId));
            }
        }

        public void runIssueActionWithoutParams(JiraIssue issue, JiraNamedEntity action) {
            using (SoapSession s = createSoapSession(issue.Server)) {
                wrapExceptionsVoid(issue.Server, s, () => s.runIssueActionWithoutParams(issue, action.Id));
            }
        }

        public void runIssueActionWithParams(JiraIssue issue, JiraNamedEntity action, ICollection<JiraField> fields, string comment) {
            using (SoapSession s = createSoapSession(issue.Server)) {
                wrapExceptionsVoid(issue.Server, s, () => s.runIssueActionWithParams(issue, action.Id, fields, comment));
            }
        }

        public List<JiraNamedEntity> getComponents(JiraServer server, JiraProject project) {
            using (SoapSession s = createSoapSession(server)) {
                return wrapExceptions(server, s, () => s.getComponents(project));
            }
        }

        public List<JiraNamedEntity> getVersions(JiraServer server, JiraProject project) {
            using (SoapSession s = createSoapSession(server)) {
                return wrapExceptions(server, s, () => s.getVersions(project));
            }
        }

        public string createIssue(JiraServer server, JiraIssue issue) {
            using (SoapSession s = createSoapSession(server)) {
                return wrapExceptions(server, s, () => s.createIssue(issue));
            }
        }

        public object getIssueSoapObject(JiraIssue issue) {
            using (SoapSession s = createSoapSession(issue.Server)) {
                return wrapExceptions(issue.Server, s, () => s.getIssueSoapObject(issue.Key));
            }
        }

        public JiraNamedEntity getSecurityLevel(JiraIssue issue) {
            using (SoapSession s = createSoapSession(issue.Server)) {
                return wrapExceptions(issue.Server, s, () => s.getSecurityLevel(issue.Key));
            }
        }

        public void logWorkAndAutoUpdateRemaining(JiraIssue issue, string timeSpent, DateTime startDate) {
            using (SoapSession s = createSoapSession(issue.Server)) {
                wrapExceptionsVoid(issue.Server, s, () => s.logWorkAndAutoUpdateRemaining(issue.Key, timeSpent, startDate));
            }
        }

        public void logWorkAndLeaveRemainingUnchanged(JiraIssue issue, string timeSpent, DateTime startDate) {
            using (SoapSession s = createSoapSession(issue.Server)) {
                wrapExceptionsVoid(issue.Server, s, () => s.logWorkAndLeaveRemainingUnchanged(issue.Key, timeSpent, startDate));
            }
        }

        public void logWorkAndUpdateRemainingManually(JiraIssue issue, string timeSpent, DateTime startDate, string remainingEstimate) {
            using (SoapSession s = createSoapSession(issue.Server)) {
                wrapExceptionsVoid(issue.Server, s, () => s.logWorkAndUpdateRemainingManually(issue.Key, timeSpent, startDate, remainingEstimate));
            }
        }

        public void updateIssue(JiraIssue issue, ICollection<JiraField> fields) {
            using (SoapSession s = createSoapSession(issue.Server)) {
                wrapExceptionsVoid(issue.Server, s, () => s.updateIssue(issue.Key, fields));
            }
        }

        public void uploadAttachment(JiraIssue issue, string name, byte[] attachment) {
            using (SoapSession s = createSoapSession(issue.Server)) {
                wrapExceptionsVoid(issue.Server, s, () => s.uploadAttachment(issue.Key, name, attachment));
            }
        }

        private static SoapSession createSoapSession(JiraServer server) {
            SoapSession s = new SoapSession(server.Url);
            return s;
        }

        private static string getTokenKey(JiraServer server) {
            return server.Url + "_" + server.UserName + "_" + server.Password;
        }

        private delegate T Wrapped<T>();
        private T wrapExceptions<T>(JiraServer server, SoapSession session, Wrapped<T> wrapped) {
            try {
                setSessionToken(server, session);
                return wrapped();
            } catch (System.Web.Services.Protocols.SoapException) {
                // let's retry _just once_ - PLVS-27
                removeSessionToken(server);
                try {
                    setSessionToken(server, session);
                    return wrapped();
                } catch (Exception e) {
                    removeSessionToken(server);
                    maybeHandle503(server, e);
                    throw;
                }
            } catch (Exception e) {
                removeSessionToken(server);
                maybeHandle503(server, e);
                throw;
            }
        }

        private delegate void WrappedVoid();
        private void wrapExceptionsVoid(JiraServer server, SoapSession session, WrappedVoid wrapped) {
            try {
                setSessionToken(server, session);
                wrapped();
            } catch (System.Web.Services.Protocols.SoapException) {
                // let's retry _just once_ - PLVS-27
                removeSessionToken(server);
                try {
                    setSessionToken(server, session);
                    wrapped();
                } catch (Exception e) {
                    removeSessionToken(server);
                    maybeHandle503(server, e);
                    throw;
                }
            } catch (Exception e) {
                removeSessionToken(server);
                maybeHandle503(server, e);
                throw;
            }
        }

        private static void maybeHandle503(JiraServer server, Exception e) {
            if (e is SoapSession.LoginException) {
                Exception inner = e.InnerException;
                if (inner.Message != null && inner.Message.Contains("503")) {
                    throw new FiveOhThreeJiraException(server);
                }
            }
        }

        private void setSessionToken(JiraServer server, SoapSession session) {
            lock (tokenMap) {
                string tokenKey = getTokenKey(server);
                if (tokenMap.ContainsKey(tokenKey)) {
                    session.Token = tokenMap[tokenKey];
                } else {
                    tokenMap[tokenKey] = session.login(server.UserName, server.Password);
                }
            }
        }

        private void removeSessionToken(JiraServer server) {
            lock (tokenMap) {
                tokenMap.Remove(getTokenKey(server));
            }
        }
    }
}