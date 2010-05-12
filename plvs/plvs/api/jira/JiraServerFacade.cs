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

        private readonly Dictionary<string, string> soapTokenMap = new Dictionary<string, string>();
        private readonly Dictionary<string, IDictionary<string, string>> rssSessionCookieMap = new Dictionary<string, IDictionary<string, string>>();

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
            lock(soapTokenMap) {
                soapTokenMap.Clear();
            }
            lock (rssSessionCookieMap) {
                rssSessionCookieMap.Clear();
            }
        }

        public IDictionary<string, string> createOrGetSessionCookie(JiraServer server) {
            IDictionary<string, string> cookie = getExistingSessionCookie(server);
            if (cookie != null) return cookie;
            lock(rssSessionCookieMap) {
                using (RssClient rss = new RssClient(server)) {
                    cookie = rss.login();
                    rssSessionCookieMap[getSessionOrTokenKey(server)] = cookie;
                    return cookie;
                }
            }
        }

        public IDictionary<string, string> getExistingSessionCookie(JiraServer server) {
            lock (rssSessionCookieMap) {
                string key = getSessionOrTokenKey(server);
                return rssSessionCookieMap.ContainsKey(key) ? rssSessionCookieMap[key] : null;
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
            using (RssClient rss = new RssClient(server)) {
                return setSessionCookieAndWrapExceptions(
                    server, rss, () => rss.getSavedFilterIssues(filter.Id, "priority", "DESC", start, count));
            }
        }

        public List<JiraIssue> getCustomFilterIssues(JiraServer server, JiraFilter filter, int start, int count) {
            using (RssClient rss = new RssClient(server)) {
                return setSessionCookieAndWrapExceptions(
                    server, rss, 
                    () => rss.getCustomFilterIssues(filter.getFilterQueryString(), filter.getSortBy(), "DESC", start, count));
            }
        }

        public JiraIssue getIssue(JiraServer server, string key) {
            using (RssClient rss = new RssClient(server)) {
                return setSessionCookieAndWrapExceptions(server, rss, () => rss.getIssue(key));
            }
        }

        public string getRenderedContent(JiraIssue issue, string markup) {
            using (RestClient rest = new RestClient(issue.Server)) {
                return setSessionCookieAndWrapExceptions(issue.Server, rest, () => rest.getRenderedContent(issue.Key, -1, -1, markup));
            }
        }

        public string getRenderedContent(JiraServer server, int issueTypeId, JiraProject project, string markup) {
            using (RestClient rest = new RestClient(server)) {
                return setSessionCookieAndWrapExceptions(server, rest, () => rest.getRenderedContent(null, issueTypeId, project.Id, markup));
            }
        }

        public List<JiraProject> getProjects(JiraServer server) {
            using (SoapSession s = createSoapSession(server)) {
                return setSoapTokenAndWrapExceptions(server, s, () => s.getProjects());
            }
        }

        public List<JiraNamedEntity> getIssueTypes(JiraServer server) {
            using (SoapSession s = createSoapSession(server)) {
                return setSoapTokenAndWrapExceptions(server, s, () => s.getIssueTypes());
            }
        }

        public List<JiraNamedEntity> getSubtaskIssueTypes(JiraServer server) {
            using (SoapSession s = createSoapSession(server)) {
                return setSoapTokenAndWrapExceptions(server, s, () => s.getSubtaskIssueTypes());
            }
        }

        public List<JiraNamedEntity> getSubtaskIssueTypes(JiraServer server, JiraProject project) {
            using (SoapSession s = createSoapSession(server)) {
                return setSoapTokenAndWrapExceptions(server, s, () => s.getSubtaskIssueTypes(project));
            }
        }

        public List<JiraNamedEntity> getIssueTypes(JiraServer server, JiraProject project) {
            using (SoapSession s = createSoapSession(server)) {
                return setSoapTokenAndWrapExceptions(server, s, () => s.getIssueTypes(project));
            }
        }

        public List<JiraSavedFilter> getSavedFilters(JiraServer server) {
            using (SoapSession s = createSoapSession(server)) {
                return setSoapTokenAndWrapExceptions(server, s, () => s.getSavedFilters());
            }
        }

        public List<JiraNamedEntity> getPriorities(JiraServer server) {
            using (SoapSession s = createSoapSession(server)) {
                return setSoapTokenAndWrapExceptions(server, s, () => s.getPriorities());
            }
        }

        public List<JiraNamedEntity> getStatuses(JiraServer server) {
            using (SoapSession s = createSoapSession(server)) {
                return setSoapTokenAndWrapExceptions(server, s, () => s.getStatuses());
            }
        }

        public List<JiraNamedEntity> getResolutions(JiraServer server) {
            using (SoapSession s = createSoapSession(server)) {
                return setSoapTokenAndWrapExceptions(server, s, () => s.getResolutions());
            }
        }

        public void addComment(JiraIssue issue, string comment) {
            using (SoapSession s = createSoapSession(issue.Server)) {
                setSoapTokenAndWrapExceptionsVoid(issue.Server, s, () => s.addComment(issue, comment));
            }
        }

        public List<JiraNamedEntity> getActionsForIssue(JiraIssue issue) {
            using (SoapSession s = createSoapSession(issue.Server)) {
                return setSoapTokenAndWrapExceptions(issue.Server, s, () => s.getActionsForIssue(issue));
            }
        }

        public List<JiraField> getFieldsForAction(JiraIssue issue, int actionId) {
            using (SoapSession s = createSoapSession(issue.Server)) {
                return setSoapTokenAndWrapExceptions(issue.Server, s, () => s.getFieldsForAction(issue, actionId));
            }
        }

        public void runIssueActionWithoutParams(JiraIssue issue, JiraNamedEntity action) {
            using (SoapSession s = createSoapSession(issue.Server)) {
                setSoapTokenAndWrapExceptionsVoid(issue.Server, s, () => s.runIssueActionWithoutParams(issue, action.Id));
            }
        }

        public void runIssueActionWithParams(JiraIssue issue, JiraNamedEntity action, ICollection<JiraField> fields, string comment) {
            using (SoapSession s = createSoapSession(issue.Server)) {
                setSoapTokenAndWrapExceptionsVoid(issue.Server, s, () => s.runIssueActionWithParams(issue, action.Id, fields, comment));
            }
        }

        public List<JiraNamedEntity> getComponents(JiraServer server, JiraProject project) {
            using (SoapSession s = createSoapSession(server)) {
                return setSoapTokenAndWrapExceptions(server, s, () => s.getComponents(project));
            }
        }

        public List<JiraNamedEntity> getVersions(JiraServer server, JiraProject project) {
            using (SoapSession s = createSoapSession(server)) {
                return setSoapTokenAndWrapExceptions(server, s, () => s.getVersions(project));
            }
        }

        public string createIssue(JiraServer server, JiraIssue issue) {
            using (SoapSession s = createSoapSession(server)) {
                return setSoapTokenAndWrapExceptions(server, s, () => s.createIssue(issue));
            }
        }

        public object getIssueSoapObject(JiraIssue issue) {
            using (SoapSession s = createSoapSession(issue.Server)) {
                return setSoapTokenAndWrapExceptions(issue.Server, s, () => s.getIssueSoapObject(issue.Key));
            }
        }

        public JiraNamedEntity getSecurityLevel(JiraIssue issue) {
            using (SoapSession s = createSoapSession(issue.Server)) {
                return setSoapTokenAndWrapExceptions(issue.Server, s, () => s.getSecurityLevel(issue.Key));
            }
        }

        public void logWorkAndAutoUpdateRemaining(JiraIssue issue, string timeSpent, DateTime startDate, string comment) {
            using (SoapSession s = createSoapSession(issue.Server)) {
                setSoapTokenAndWrapExceptionsVoid(issue.Server, s, () => s.logWorkAndAutoUpdateRemaining(issue.Key, timeSpent, startDate, comment));
            }
        }

        public void logWorkAndLeaveRemainingUnchanged(JiraIssue issue, string timeSpent, DateTime startDate, string comment) {
            using (SoapSession s = createSoapSession(issue.Server)) {
                setSoapTokenAndWrapExceptionsVoid(issue.Server, s, () => s.logWorkAndLeaveRemainingUnchanged(issue.Key, timeSpent, startDate, comment));
            }
        }

        public void logWorkAndUpdateRemainingManually(JiraIssue issue, string timeSpent, DateTime startDate, string remainingEstimate, string comment) {
            using (SoapSession s = createSoapSession(issue.Server)) {
                setSoapTokenAndWrapExceptionsVoid(issue.Server, s, () => s.logWorkAndUpdateRemainingManually(issue.Key, timeSpent, startDate, remainingEstimate, comment));
            }
        }

        public void updateIssue(JiraIssue issue, ICollection<JiraField> fields) {
            using (SoapSession s = createSoapSession(issue.Server)) {
                setSoapTokenAndWrapExceptionsVoid(issue.Server, s, () => s.updateIssue(issue.Key, fields));
            }
        }

        public void uploadAttachment(JiraIssue issue, string name, byte[] attachment) {
            using (SoapSession s = createSoapSession(issue.Server)) {
                setSoapTokenAndWrapExceptionsVoid(issue.Server, s, () => s.uploadAttachment(issue.Key, name, attachment));
            }
        }

        private static SoapSession createSoapSession(JiraServer server) {
            SoapSession s = new SoapSession(server.Url, server.NoProxy);
            return s;
        }

        private static string getSessionOrTokenKey(JiraServer server) {
            return server.Url + "_" + server.UserName + "_" + server.Password;
        }

        private delegate T Wrapped<T>();
        private T setSoapTokenAndWrapExceptions<T>(JiraServer server, SoapSession session, Wrapped<T> wrapped) {
            try {
                setSoapSessionToken(server, session);
                return wrapped();
            } catch (System.Web.Services.Protocols.SoapException) {
                // let's retry _just once_ - PLVS-27
                removeSoapSessionToken(server);
                try {
                    setSoapSessionToken(server, session);
                    return wrapped();
                } catch (Exception e) {
                    removeSoapSessionToken(server);
                    maybeHandle503(server, e);
                    throw;
                }
            } catch (Exception e) {
                removeSoapSessionToken(server);
                maybeHandle503(server, e);
                throw;
            }
        }

        private T setSessionCookieAndWrapExceptions<T>(JiraServer server, JiraAuthenticatedClient client, Wrapped<T> wrapped) {
            try {
                setSessionCookie(server, client);
                return wrapped();
            } catch (Exception) {
                removeSessionCookie(server);
                throw;
            }
        }

        private delegate void WrappedVoid();
        private void setSoapTokenAndWrapExceptionsVoid(JiraServer server, SoapSession session, WrappedVoid wrapped) {
            try {
                setSoapSessionToken(server, session);
                wrapped();
            } catch (System.Web.Services.Protocols.SoapException) {
                // let's retry _just once_ - PLVS-27
                removeSoapSessionToken(server);
                try {
                    setSoapSessionToken(server, session);
                    wrapped();
                } catch (Exception e) {
                    removeSoapSessionToken(server);
                    maybeHandle503(server, e);
                    throw;
                }
            } catch (Exception e) {
                removeSoapSessionToken(server);
                maybeHandle503(server, e);
                throw;
            }
        }

        private static void maybeHandle503(JiraServer server, Exception e) {
            if (e is LoginException) {
                Exception inner = e.InnerException;
                if (inner.Message != null && inner.Message.Contains("503")) {
                    throw new FiveOhThreeJiraException(server);
                }
            }
        }

        private void setSessionCookie(JiraServer server, JiraAuthenticatedClient client) {
            lock (rssSessionCookieMap) {
                string key = getSessionOrTokenKey(server);
                if (rssSessionCookieMap.ContainsKey(key)) {
                    client.SessionTokens = rssSessionCookieMap[key];
                } else {
                    rssSessionCookieMap[key] = client.login();
                }
            }
        }

        private void removeSessionCookie(JiraServer server) {
            lock (rssSessionCookieMap) {
                rssSessionCookieMap.Remove(getSessionOrTokenKey(server));
            }
        }

        private void setSoapSessionToken(JiraServer server, SoapSession session) {
            lock (soapTokenMap) {
                string key = getSessionOrTokenKey(server);
                if (soapTokenMap.ContainsKey(key)) {
                    session.Token = soapTokenMap[key];
                } else {
                    soapTokenMap[key] = session.login(server.UserName, server.Password);
                }
            }
        }

        private void removeSoapSessionToken(JiraServer server) {
            lock (soapTokenMap) {
                soapTokenMap.Remove(getSessionOrTokenKey(server));
            }
        }
    }
}