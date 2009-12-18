using System.Collections.Generic;
using Atlassian.plvs.api;
using Atlassian.plvs.models;
using Atlassian.plvs.ui.issues.issuegroupnodes;

namespace Atlassian.plvs.ui.issues.treemodels {
    internal class GroupedByStatusIssueTreeModel : AbstractGroupingIssueTreeModel {

        private readonly SortedDictionary<int, AbstractIssueGroupNode> groupNodes = 
            new SortedDictionary<int, AbstractIssueGroupNode>();

        public GroupedByStatusIssueTreeModel(JiraIssueListModel model)
            : base(model) {
        }

        protected override AbstractIssueGroupNode findGroupNode(JiraIssue issue) {
            if (!groupNodes.ContainsKey(issue.StatusId)) {
                SortedDictionary<int, JiraNamedEntity> statuses = JiraServerCache.Instance.getStatues(issue.Server);
                groupNodes[issue.StatusId] = new ByStatusIssueGroupNode(statuses[issue.StatusId]);
            }
            return groupNodes[issue.StatusId];
        }

        protected override IEnumerable<AbstractIssueGroupNode> getGroupNodes() {
            return groupNodes.Values;
        }

        protected override void clearGroupNodes() {
            groupNodes.Clear();
        }
    }
}