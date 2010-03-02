using System;
using System.Collections.Generic;
using Atlassian.plvs.api.jira.soap;
using Atlassian.plvs.util;

namespace Atlassian.plvs.api.jira {
    public class JiraServerFacade : ServerFacade {
        private readonly SortedDictionary<string, SoapSession> sessionMap = new SortedDictionary<string, SoapSession>();

        private static readonly JiraServerFacade INSTANCE = new JiraServerFacade();

        public static JiraServerFacade Instance {
            get { return INSTANCE; }
        }

        private JiraServerFacade() {
            PlvsUtils.installSslCertificateHandler();
        }

        private SoapSession getSoapSession(JiraServer server) {
            lock (sessionMap) {
                SoapSession s;
                if (!sessionMap.TryGetValue(server.Url + server.UserName, out s)) {
                    s = new SoapSession(server.Url);
                    s.login(server.UserName, server.Password);
                    sessionMap.Add(getSessionKey(server), s);
                }
                return s;
            }
        }

        private static string getSessionKey(JiraServer server) {
            return server.Url + server.UserName;
        }

        public void removeSession(JiraServer server) {
            lock (sessionMap) {
                sessionMap.Remove(getSessionKey(server));
            }
        }

        public void login(JiraServer server) {
            new SoapSession(server.Url).login(server.UserName, server.Password);
        }

        public void dropAllSessions() {
            lock(sessionMap) {
                sessionMap.Clear();
            }
        }

        public string getSoapToken(JiraServer server) {
            SoapSession session = getSoapSession(server);
            return session != null ? session.Token : null;
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
            return wrapExceptions(server, () => getSoapSession(server).getProjects());
        }

        public List<JiraNamedEntity> getIssueTypes(JiraServer server) {
            return wrapExceptions(server, () => getSoapSession(server).getIssueTypes());
        }

        public List<JiraNamedEntity> getSubtaskIssueTypes(JiraServer server) {
            return wrapExceptions(server, () => getSoapSession(server).getSubtaskIssueTypes());
        }

        public List<JiraNamedEntity> getSubtaskIssueTypes(JiraServer server, JiraProject project) {
            return wrapExceptions(server, () => getSoapSession(server).getSubtaskIssueTypes(project));
        }

        public List<JiraNamedEntity> getIssueTypes(JiraServer server, JiraProject project) {
            return wrapExceptions(server, () => getSoapSession(server).getIssueTypes(project));
        }

        public List<JiraSavedFilter> getSavedFilters(JiraServer server) {
            return wrapExceptions(server, () => getSoapSession(server).getSavedFilters());
        }

        public List<JiraNamedEntity> getPriorities(JiraServer server) {
            return wrapExceptions(server, () => getSoapSession(server).getPriorities());
        }

        public List<JiraNamedEntity> getStatuses(JiraServer server) {
            return wrapExceptions(server, () => getSoapSession(server).getStatuses());
        }

        public List<JiraNamedEntity> getResolutions(JiraServer server) {
            return wrapExceptions(server, () => getSoapSession(server).getResolutions());
        }

        public void addComment(JiraIssue issue, string comment) {
            wrapExceptionsVoid(issue.Server, () => getSoapSession(issue.Server).addComment(issue, comment));
        }

        public List<JiraNamedEntity> getActionsForIssue(JiraIssue issue) {
            return wrapExceptions(issue.Server, () => getSoapSession(issue.Server).getActionsForIssue(issue));
        }

        public List<JiraField> getFieldsForAction(JiraIssue issue, int actionId) {
            return wrapExceptions(issue.Server, () => getSoapSession(issue.Server).getFieldsForAction(issue, actionId));
        }

        public void runIssueActionWithoutParams(JiraIssue issue, JiraNamedEntity action) {
            wrapExceptionsVoid(issue.Server, () => getSoapSession(issue.Server).runIssueActionWithoutParams(issue, action.Id));
        }

        public void runIssueActionWithParams(JiraIssue issue, JiraNamedEntity action, ICollection<JiraField> fields, string comment) {
            wrapExceptionsVoid(issue.Server, () => getSoapSession(issue.Server).runIssueActionWithParams(issue, action.Id, fields, comment));
        }

        public List<JiraNamedEntity> getComponents(JiraServer server, JiraProject project) {
            return wrapExceptions(server, () => getSoapSession(server).getComponents(project));
        }

        public List<JiraNamedEntity> getVersions(JiraServer server, JiraProject project) {
            return wrapExceptions(server, () => getSoapSession(server).getVersions(project));
        }

        public string createIssue(JiraServer server, JiraIssue issue) {
            return wrapExceptions(server, () => getSoapSession(server).createIssue(issue));
        }

        public object getIssueSoapObject(JiraIssue issue) {
            return wrapExceptions(issue.Server, () => getSoapSession(issue.Server).getIssueSoapObject(issue.Key));
        }

        public JiraNamedEntity getSecurityLevel(JiraIssue issue) {
            return wrapExceptions(issue.Server, () => getSoapSession(issue.Server).getSecurityLevel(issue.Key));
        }

        public void logWorkAndAutoUpdateRemaining(JiraIssue issue, string timeSpent, DateTime startDate) {
            wrapExceptionsVoid(issue.Server, 
                () => getSoapSession(issue.Server).logWorkAndAutoUpdateRemaining(issue.Key, timeSpent, startDate));
        }

        public void logWorkAndLeaveRemainingUnchanged(JiraIssue issue, string timeSpent, DateTime startDate) {
            wrapExceptionsVoid(issue.Server, 
                () => getSoapSession(issue.Server).logWorkAndLeaveRemainingUnchanged(issue.Key, timeSpent, startDate));
        }

        public void logWorkAndUpdateRemainingManually(JiraIssue issue, string timeSpent, DateTime startDate, string remainingEstimate) {
            wrapExceptionsVoid(issue.Server, 
                () => getSoapSession(issue.Server).logWorkAndUpdateRemainingManually(issue.Key, timeSpent, startDate, remainingEstimate));
        }

        public void updateIssue(JiraIssue issue, ICollection<JiraField> fields) {
            wrapExceptionsVoid(issue.Server, () => getSoapSession(issue.Server).updateIssue(issue.Key, fields));
        }

        public void uploadAttachment(JiraIssue issue, string name, byte[] attachment) {
            wrapExceptionsVoid(issue.Server, () => getSoapSession(issue.Server).uploadAttachment(issue.Key, name, attachment));    
        }

        private delegate T Wrapped<T>();
        private T wrapExceptions<T>(JiraServer server, Wrapped<T> wrapped) {
            try {
                return wrapped();
            } catch (System.Web.Services.Protocols.SoapException) {
                // let's retry _just once_ - PLVS-27
                removeSession(server);
                return wrapped();
            } catch (Exception) {
                removeSession(server);
                throw;
            }
        }

        private delegate void WrappedVoid();
        private void wrapExceptionsVoid(JiraServer server, WrappedVoid wrapped) {
            try {
                wrapped();
            } catch (System.Web.Services.Protocols.SoapException) {
                // let's retry _just once_ - PLVS-27
                removeSession(server);
                wrapped();
            } catch (Exception) {
                removeSession(server);
                throw;
            }
        }
    }
}