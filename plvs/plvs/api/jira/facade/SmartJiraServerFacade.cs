using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atlassian.plvs.api.jira.facade {
    public class SmartJiraServerFacade : AbstractJiraServerFacade {
        private static readonly SmartJiraServerFacade INSTANCE = new SmartJiraServerFacade();

        public static SmartJiraServerFacade Instance {
            get { return INSTANCE; }
        }

        private readonly ClassicJiraServerFacade classicFacade = new ClassicJiraServerFacade();
        private readonly RestJiraServerFacade restFacade = new RestJiraServerFacade();

        private readonly List<Guid> restServers = new List<Guid>();

        public override void login(JiraServer server) {
            if (restFacade.restSupported(server)) {
                restServers.Add(server.GUID);   
            }
            delegatedVoid(server, delegate { restFacade.login(server); }, delegate { classicFacade.login(server); });
        }

        public override string getSoapToken(JiraServer server) {
            return delegated(server, delegate { return restFacade.getSoapToken(server); }, delegate { return classicFacade.getSoapToken(server); });
        }

        public override List<JiraIssue> getSavedFilterIssues(JiraServer server, JiraSavedFilter filter, int start, int count) {
            return delegated(server,
                             delegate { return restFacade.getSavedFilterIssues(server, filter, start, count); },
                             delegate { return classicFacade.getSavedFilterIssues(server, filter, start, count); });
        }

        public override List<JiraIssue> getCustomFilterIssues(JiraServer server, JiraFilter filter, int start, int count) {
            throw new NotImplementedException();
        }

        public override JiraIssue getIssue(JiraServer server, string key) {
            throw new NotImplementedException();
        }

        public override string getRenderedContent(JiraIssue issue, string markup) {
            return restFacade.getRenderedContent(issue, markup);
        }

        public override string getRenderedContent(JiraServer server, int issueTypeId, JiraProject project, string markup) {
            return restFacade.getRenderedContent(server, issueTypeId, project, markup);
        }

        public override List<JiraProject> getProjects(JiraServer server) {
            return delegated(server, 
                delegate { return restFacade.getProjects(server); }, 
                delegate { return classicFacade.getProjects(server); });
        }

        public override List<JiraNamedEntity> getIssueTypes(JiraServer server) {
            return delegated(server,
                delegate { return restFacade.getIssueTypes(server); },
                delegate { return classicFacade.getIssueTypes(server); });
        }

        public override List<JiraNamedEntity> getSubtaskIssueTypes(JiraServer server) {
            return delegated(server,
                delegate { return restFacade.getSubtaskIssueTypes(server); },
                delegate { return classicFacade.getSubtaskIssueTypes(server); });
        }

        public override List<JiraNamedEntity> getSubtaskIssueTypes(JiraServer server, JiraProject project) {
            return delegated(server,
                delegate { return restFacade.getSubtaskIssueTypes(server, project); },
                delegate { return classicFacade.getSubtaskIssueTypes(server, project); });
        }

        public override List<JiraNamedEntity> getIssueTypes(JiraServer server, JiraProject project) {
            return delegated(server,
                delegate { return restFacade.getIssueTypes(server, project); },
                delegate { return classicFacade.getIssueTypes(server, project); });
        }

        public override List<JiraSavedFilter> getSavedFilters(JiraServer server) {
            return delegated(server,
                delegate { return restFacade.getSavedFilters(server); },
                delegate { return classicFacade.getSavedFilters(server); });
        }

        public override List<JiraNamedEntity> getPriorities(JiraServer server) {
            return delegated(server,
                delegate { return restFacade.getPriorities(server); },
                delegate { return classicFacade.getPriorities(server); });
        }

        public override List<JiraNamedEntity> getStatuses(JiraServer server) {
            return delegated(server,
                delegate { return restFacade.getStatuses(server); },
                delegate { return classicFacade.getStatuses(server); });
        }

        public override List<JiraNamedEntity> getResolutions(JiraServer server) {
            return delegated(server,
                delegate { return restFacade.getStatuses(server); },
                delegate { return classicFacade.getStatuses(server); });
        }

        public override void addComment(JiraIssue issue, string comment) {
            throw new NotImplementedException();
        }

        public override List<JiraNamedEntity> getActionsForIssue(JiraIssue issue) {
            throw new NotImplementedException();
        }

        public override List<JiraField> getFieldsForAction(JiraIssue issue, int actionId) {
            throw new NotImplementedException();
        }

        public override void runIssueActionWithoutParams(JiraIssue issue, JiraNamedEntity action) {
            throw new NotImplementedException();
        }

        public override void runIssueActionWithParams(JiraIssue issue, JiraNamedEntity action, ICollection<JiraField> fields, string comment) {
            throw new NotImplementedException();
        }

        public override List<JiraNamedEntity> getComponents(JiraServer server, JiraProject project) {
            return delegated(server,
                delegate { return restFacade.getVersions(server, project); },
                delegate { return classicFacade.getVersions(server, project); });
        }

        public override List<JiraNamedEntity> getVersions(JiraServer server, JiraProject project) {
            return delegated(server,
                delegate { return restFacade.getVersions(server, project); },
                delegate { return classicFacade.getVersions(server, project); });
        }

        public override string createIssue(JiraServer server, JiraIssue issue) {
            throw new NotImplementedException();
        }

        public override object getIssueSoapObject(JiraIssue issue) {
            throw new NotImplementedException();
        }

        public override JiraNamedEntity getSecurityLevel(JiraIssue issue) {
            throw new NotImplementedException();
        }

        public override void logWorkAndAutoUpdateRemaining(JiraIssue issue, string timeSpent, DateTime startDate, string comment) {
            throw new NotImplementedException();
        }

        public override void logWorkAndLeaveRemainingUnchanged(JiraIssue issue, string timeSpent, DateTime startDate, string comment) {
            throw new NotImplementedException();
        }

        public override void logWorkAndUpdateRemainingManually(JiraIssue issue, string timeSpent, DateTime startDate, string remainingEstimate, string comment) {
            throw new NotImplementedException();
        }

        public override void updateIssue(JiraIssue issue, ICollection<JiraField> fields) {
            throw new NotImplementedException();
        }

        public override void uploadAttachment(JiraIssue issue, string name, byte[] attachment) {
            throw new NotImplementedException();
        }

        protected delegate T Delegate<T>(JiraServer server);
        protected delegate void DelegateVoid(JiraServer server);

        private T delegated<T>(JiraServer server, Delegate<T> rest, Delegate<T> classic) {
            return restServers.Contains(server.GUID) ? rest(server) : classic(server);
        }

        private void delegatedVoid(JiraServer server, DelegateVoid rest, DelegateVoid classic) {
            if (restServers.Contains(server.GUID)) {
                rest(server);
            } else {
                classic(server);
            }
        }
    }
}
