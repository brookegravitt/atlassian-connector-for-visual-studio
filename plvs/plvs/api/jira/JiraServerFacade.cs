using System;
using System.Collections.Generic;
using Atlassian.plvs.api.jira.soap;
using Atlassian.plvs.util;

namespace Atlassian.plvs.api.jira {
    public class JiraServerFacade : ServerFacade {

        private static readonly JiraServerFacade INSTANCE = new JiraServerFacade();

        public static JiraServerFacade Instance {
            get { return INSTANCE; }
        }

        private JiraServerFacade() {
            PlvsUtils.installSslCertificateHandler();
        }

        private static SoapSession createSoapSession(JiraServer server) {
            SoapSession s = new SoapSession(server.Url);
            s.login(server.UserName, server.Password);
            return s;
        }

        public void login(JiraServer server) {
            new SoapSession(server.Url).login(server.UserName, server.Password);
        }

        public void dropAllSessions() {
        }

        public string getSoapToken(JiraServer server) {
            SoapSession session = createSoapSession(server);
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
            SoapSession s = createSoapSession(server);
            return wrapExceptions(s, () => s.getProjects());
        }

        public List<JiraNamedEntity> getIssueTypes(JiraServer server) {
            SoapSession s = createSoapSession(server);
            return wrapExceptions(s, () => s.getIssueTypes());
        }

        public List<JiraNamedEntity> getSubtaskIssueTypes(JiraServer server) {
            SoapSession s = createSoapSession(server);
            return wrapExceptions(s, () => s.getSubtaskIssueTypes());
        }

        public List<JiraNamedEntity> getSubtaskIssueTypes(JiraServer server, JiraProject project) {
            SoapSession s = createSoapSession(server);
            return wrapExceptions(s, () => s.getSubtaskIssueTypes(project));
        }

        public List<JiraNamedEntity> getIssueTypes(JiraServer server, JiraProject project) {
            SoapSession s = createSoapSession(server);
            return wrapExceptions(s, () => s.getIssueTypes(project));
        }

        public List<JiraSavedFilter> getSavedFilters(JiraServer server) {
            SoapSession s = createSoapSession(server);
            return wrapExceptions(s, () => s.getSavedFilters());
        }

        public List<JiraNamedEntity> getPriorities(JiraServer server) {
            SoapSession s = createSoapSession(server);
            return wrapExceptions(s, () => s.getPriorities());
        }

        public List<JiraNamedEntity> getStatuses(JiraServer server) {
            SoapSession s = createSoapSession(server);
            return wrapExceptions(s, () => s.getStatuses());
        }

        public List<JiraNamedEntity> getResolutions(JiraServer server) {
            SoapSession s = createSoapSession(server);
            return wrapExceptions(s, () => s.getResolutions());
        }

        public void addComment(JiraIssue issue, string comment) {
            SoapSession s = createSoapSession(issue.Server);
            wrapExceptionsVoid(s, () => s.addComment(issue, comment));
        }

        public List<JiraNamedEntity> getActionsForIssue(JiraIssue issue) {
            SoapSession s = createSoapSession(issue.Server);
            return wrapExceptions(s, () => s.getActionsForIssue(issue));
        }

        public List<JiraField> getFieldsForAction(JiraIssue issue, int actionId) {
            SoapSession s = createSoapSession(issue.Server);
            return wrapExceptions(s, () => s.getFieldsForAction(issue, actionId));
        }

        public void runIssueActionWithoutParams(JiraIssue issue, JiraNamedEntity action) {
            SoapSession s = createSoapSession(issue.Server);
            wrapExceptionsVoid(s, () => s.runIssueActionWithoutParams(issue, action.Id));
        }

        public void runIssueActionWithParams(JiraIssue issue, JiraNamedEntity action, ICollection<JiraField> fields, string comment) {
            SoapSession s = createSoapSession(issue.Server);
            wrapExceptionsVoid(s, () => s.runIssueActionWithParams(issue, action.Id, fields, comment));
        }

        public List<JiraNamedEntity> getComponents(JiraServer server, JiraProject project) {
            SoapSession s = createSoapSession(server);
            return wrapExceptions(s, () => s.getComponents(project));
        }

        public List<JiraNamedEntity> getVersions(JiraServer server, JiraProject project) {
            SoapSession s = createSoapSession(server);
            return wrapExceptions(s, () => s.getVersions(project));
        }

        public string createIssue(JiraServer server, JiraIssue issue) {
            SoapSession s = createSoapSession(server);
            return wrapExceptions(s, () => s.createIssue(issue));
        }

        public object getIssueSoapObject(JiraIssue issue) {
            SoapSession s = createSoapSession(issue.Server);
            return wrapExceptions(s, () => s.getIssueSoapObject(issue.Key));
        }

        public JiraNamedEntity getSecurityLevel(JiraIssue issue) {
            SoapSession s = createSoapSession(issue.Server);
            return wrapExceptions(s, () => s.getSecurityLevel(issue.Key));
        }

        public void logWorkAndAutoUpdateRemaining(JiraIssue issue, string timeSpent, DateTime startDate) {
            SoapSession s = createSoapSession(issue.Server);
            wrapExceptionsVoid(s, () => s.logWorkAndAutoUpdateRemaining(issue.Key, timeSpent, startDate));
        }

        public void logWorkAndLeaveRemainingUnchanged(JiraIssue issue, string timeSpent, DateTime startDate) {
            SoapSession s = createSoapSession(issue.Server);
            wrapExceptionsVoid(s, () => s.logWorkAndLeaveRemainingUnchanged(issue.Key, timeSpent, startDate));
        }

        public void logWorkAndUpdateRemainingManually(JiraIssue issue, string timeSpent, DateTime startDate, string remainingEstimate) {
            SoapSession s = createSoapSession(issue.Server);
            wrapExceptionsVoid(s, () => s.logWorkAndUpdateRemainingManually(issue.Key, timeSpent, startDate, remainingEstimate));
        }

        public void updateIssue(JiraIssue issue, ICollection<JiraField> fields) {
            SoapSession s = createSoapSession(issue.Server);
            wrapExceptionsVoid(s, () => s.updateIssue(issue.Key, fields));
        }

        public void uploadAttachment(JiraIssue issue, string name, byte[] attachment) {
            SoapSession s = createSoapSession(issue.Server);
            wrapExceptionsVoid(s, () => s.uploadAttachment(issue.Key, name, attachment));    
        }

        private delegate T Wrapped<T>();
        private static T wrapExceptions<T>(SoapSession session, Wrapped<T> wrapped) {
            try {
                T res = wrapped();
                session.cleanup();
                return res;
            } catch (Exception) {
                session.cleanup();
                throw;
            }
        }

        private delegate void WrappedVoid();
        private static void wrapExceptionsVoid(SoapSession session, WrappedVoid wrapped) {
            try {
                wrapped();
                session.cleanup();
            } catch (Exception) {
                session.cleanup();
                throw;
            }
        }
    }
}