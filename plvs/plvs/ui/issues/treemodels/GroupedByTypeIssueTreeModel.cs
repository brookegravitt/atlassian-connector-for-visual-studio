﻿using System.Collections.Generic;
using Atlassian.plvs.api;
using Atlassian.plvs.models;
using Atlassian.plvs.ui.issues.issuegroupnodes;

namespace Atlassian.plvs.ui.issues.treemodels {
    internal class GroupedByTypeIssueTreeModel : AbstractGroupingIssueTreeModel {

        private readonly SortedDictionary<int, AbstractIssueGroupNode> groupNodes = 
            new SortedDictionary<int, AbstractIssueGroupNode>();

        public GroupedByTypeIssueTreeModel(JiraIssueListModel model)
            : base(model) {
        }

        protected override AbstractIssueGroupNode findGroupNode(JiraIssue issue) {
            if (!groupNodes.ContainsKey(issue.IssueTypeId)) {
                SortedDictionary<int, JiraNamedEntity> issueTypes = JiraServerCache.Instance.getIssueTypes(issue.Server);
                groupNodes[issue.IssueTypeId] = new ByTypeIssueGroupNode(issueTypes[issue.IssueTypeId]);
            }
            return groupNodes[issue.IssueTypeId];
        }

        protected override IEnumerable<AbstractIssueGroupNode> getGroupNodes() {
            return groupNodes.Values;
        }

        protected override void clearGroupNodes() {
            groupNodes.Clear();
        }
    }
}