﻿using System;
using System.Collections.Generic;

namespace Atlassian.plvs.api.jira.facade {
    public class RestJiraServerFacade : AbstractJiraServerFacade {
        public override void login(JiraServer server) {
            restSupported(server);
        }

        public override string getSoapToken(JiraServer server) {
            throw new NotImplementedException();
        }

        public override List<JiraIssue> getSavedFilterIssues(JiraServer server, JiraSavedFilter filter, int start, int count) {
            using (var rest = new RestClient(server)) {
                return rest.getSavedFilterIssues(filter, "priority", "desc", start, count);
            }
        }

        public override List<JiraIssue> getCustomFilterIssues(JiraServer server, JiraFilter filter, int start, int count) {
            using (var rest = new RestClient(server)) {
                return rest.getCustomFilterIssues(filter, "desc", start, count);
            }
        }

        public override JiraIssue getIssue(JiraServer server, string key) {
            using (var rest = new RestClient(server)) {
                return rest.getIssue(key);
            }
        }

        public override string getRenderedContent(JiraIssue issue, string markup) {
            using (var rest = new RestClient(issue.Server)) {
                return setSessionCookieAndWrapExceptions(issue.Server, rest, () => rest.getRenderedContent(issue.Key, -1, -1, markup));
            }
        }

        public override string getRenderedContent(JiraServer server, int issueTypeId, JiraProject project, string markup) {
            using (var rest = new RestClient(server)) {
                return setSessionCookieAndWrapExceptions(server, rest, () => rest.getRenderedContent(null, issueTypeId, project.Id, markup));
            }
        }


        public override List<JiraProject> getProjects(JiraServer server) {
            using (var rest = new RestClient(server)) {
                return rest.getProjects();
            }
        }

        public override List<JiraNamedEntity> getIssueTypes(JiraServer server) {
            using (var rest = new RestClient(server)) {
                return rest.getIssueTypes(false);
            }
        }

        public override List<JiraNamedEntity> getSubtaskIssueTypes(JiraServer server) {
            using (var rest = new RestClient(server)) {
                return rest.getIssueTypes(true);
            }
        }

        public override List<JiraNamedEntity> getSubtaskIssueTypes(JiraServer server, JiraProject project) {
            using (var rest = new RestClient(server)) {
                return rest.getIssueTypes(true, project);
            }
        }

        public override List<JiraNamedEntity> getIssueTypes(JiraServer server, JiraProject project) {
            using (var rest = new RestClient(server)) {
                return rest.getIssueTypes(false, project);
            }
        }

        public override List<JiraSavedFilter> getSavedFilters(JiraServer server) {
            using (var rest = new RestClient(server)) {
                return rest.getSavedFilters();
            }
        }

        public override List<JiraNamedEntity> getPriorities(JiraServer server) {
            using (var rest = new RestClient(server)) {
                return rest.getPriorities();
            }
        }

        public override List<JiraNamedEntity> getStatuses(JiraServer server) {
            using (var rest = new RestClient(server)) {
                return rest.getStatuses();
            }
        }

        public override List<JiraNamedEntity> getResolutions(JiraServer server) {
            using (var rest = new RestClient(server)) {
                return rest.getResolutions();
            }
        }

        public override void addComment(JiraIssue issue, string comment) {
            throw new NotImplementedException();
        }

        public override List<JiraNamedEntity> getActionsForIssue(JiraIssue issue) {
            using (var rest = new RestClient(issue.Server)) {
                return rest.getActionsForIssue(issue);
            }
        }

        public override List<JiraField> getFieldsForAction(JiraIssue issue, int actionId) {
            using (var rest = new RestClient(issue.Server)) {
                return rest.getFieldsForAction(issue, actionId);
            }
        }

        public override void runIssueActionWithoutParams(JiraIssue issue, JiraNamedEntity action) {
            throw new NotImplementedException();
        }

        public override void runIssueActionWithParams(JiraIssue issue, JiraNamedEntity action, ICollection<JiraField> fields, string comment) {
            throw new NotImplementedException();
        }

        public override List<JiraNamedEntity> getComponents(JiraServer server, JiraProject project) {
            using (var rest = new RestClient(server)) {
                return rest.getComponents(project);
            }
        }

        public override List<JiraNamedEntity> getVersions(JiraServer server, JiraProject project) {
            using (var rest = new RestClient(server)) {
                return rest.getVersions(project);
            }
        }

        public override string createIssue(JiraServer server, JiraIssue issue) {
            throw new NotImplementedException();
        }

        public override object getRawIssueObject(JiraIssue issue) {
            using (var rest = new RestClient(issue.Server)) {
                return rest.getRawIssueObject(issue.Key);
            }
        }

        public override JiraNamedEntity getSecurityLevel(JiraIssue issue) {
            using (var rest = new RestClient(issue.Server)) {
                return rest.getSecurityLevel(issue);
            }
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

        public bool restSupported(JiraServer server) {
            using (var rest = new RestClient(server)) {
                return rest.restSupported();
            }
        }
    }
}
