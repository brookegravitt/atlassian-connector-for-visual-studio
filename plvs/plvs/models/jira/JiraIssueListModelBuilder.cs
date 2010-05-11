using System;
using System.Collections.Generic;
using System.Linq;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.dialogs;

namespace Atlassian.plvs.models.jira {
    public class JiraIssueListModelBuilder {
        private readonly JiraServerFacade facade;

        public JiraIssueListModelBuilder(JiraServerFacade facade) {
            this.facade = facade;
        }

        public void rebuildModelWithSavedFilter(JiraIssueListModel model, JiraServer server, JiraSavedFilter filter) {
            List<JiraIssue> issues = facade.getSavedFilterIssues(server, filter, 0, GlobalSettings.JiraIssuesBatch);
            lock (this) {
                model.clear(false);
                model.addIssues(issues);
            }
        }

        public void updateModelWithSavedFilter(JiraIssueListModel model, JiraServer server, JiraSavedFilter filter) {
            List<JiraIssue> issues = facade.getSavedFilterIssues(server, filter, model.Issues.Count, GlobalSettings.JiraIssuesBatch);
            lock (this) {
                model.addIssues(issues);
            }
        }

        public void rebuildModelWithPresetFilter(JiraIssueListModel model, JiraServer server, JiraPresetFilter filter) {
            List<JiraIssue> issues = facade.getCustomFilterIssues(server, filter, 0, GlobalSettings.JiraIssuesBatch);
            lock (this) {
                model.clear(false);
                model.addIssues(issues);
            }
        }

        public void updateModelWithPresetFilter(JiraIssueListModel model, JiraServer server, JiraPresetFilter filter) {
            List<JiraIssue> issues = facade.getCustomFilterIssues(server, filter, model.Issues.Count, GlobalSettings.JiraIssuesBatch);
            lock (this) {
                model.addIssues(issues);
            }
        }

        public void rebuildModelWithCustomFilter(JiraIssueListModel model, JiraServer server, JiraCustomFilter filter) {
            List<JiraIssue> issues = facade.getCustomFilterIssues(server, filter, 0, GlobalSettings.JiraIssuesBatch);
            lock (this) {
                model.clear(false);
                model.addIssues(issues);
            }
        }

        public void updateModelWithCustomFilter(JiraIssueListModel model, JiraServer server, JiraCustomFilter filter) {
            List<JiraIssue> issues = facade.getCustomFilterIssues(server, filter, model.Issues.Count, GlobalSettings.JiraIssuesBatch);
            lock (this) {
                model.addIssues(issues);
            }
        }

        public void rebuildModelWithRecentlyViewedIssues(JiraIssueListModel model) {
            ICollection<RecentlyViewedIssue> issues = RecentlyViewedIssuesModel.Instance.Issues;
            ICollection<JiraServer> servers = JiraServerModel.Instance.getAllEnabledServers();

            List<JiraIssue> list = new List<JiraIssue>(issues.Count);
            list.AddRange(from issue in issues
                          let server = findServer(issue.ServerGuid, servers)
                          where server != null
                          select facade.getIssue(server, issue.IssueKey));

            lock (this) {
                model.clear(false);
                model.addIssues(list);
            }
        }

        private static JiraServer findServer(Guid guid, IEnumerable<JiraServer> servers) {
            return servers.FirstOrDefault(server => server.GUID.Equals(guid));
        }
    }
}