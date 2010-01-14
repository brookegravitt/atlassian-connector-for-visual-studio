using System.Collections.Generic;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui.jira.issues.issuegroupnodes;

namespace Atlassian.plvs.ui.jira.issues.treemodels {
    internal class GroupedByProjectIssueTreeModel : AbstractGroupingIssueTreeModel {

        private readonly SortedDictionary<string, AbstractIssueGroupNode> groupNodes = 
            new SortedDictionary<string, AbstractIssueGroupNode>();

        public GroupedByProjectIssueTreeModel(JiraIssueListModel model)
            : base(model) {
        }

        protected override AbstractIssueGroupNode findGroupNode(JiraIssue issue) {
            if (!groupNodes.ContainsKey(issue.ProjectKey)) {
                SortedDictionary<string, JiraProject> projects = JiraServerCache.Instance.getProjects(issue.Server);
                groupNodes[issue.ProjectKey] = new ByProjectIssueGroupNode(projects[issue.ProjectKey]);
            }
            return groupNodes[issue.ProjectKey];
        }

        protected override IEnumerable<AbstractIssueGroupNode> getGroupNodes() {
            return groupNodes.Values;
        }

        protected override void clearGroupNodes() {
            groupNodes.Clear();
        }
    }
}