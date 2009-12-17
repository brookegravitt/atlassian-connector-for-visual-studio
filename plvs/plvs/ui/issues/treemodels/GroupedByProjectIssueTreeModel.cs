using System;
using System.Collections;
using System.Collections.Generic;
using Aga.Controls.Tree;
using Atlassian.plvs.api;
using Atlassian.plvs.models;
using Atlassian.plvs.ui.issues.issuegroupnodes;

namespace Atlassian.plvs.ui.issues.treemodels {
    internal class GroupedByProjectIssueTreeModel : AbstractIssueTreeModel {

        private readonly SortedDictionary<string, ByProjectIssueGroupNode> groupNodes = 
            new SortedDictionary<string, ByProjectIssueGroupNode>();

        public GroupedByProjectIssueTreeModel(JiraIssueListModel model)
            : base(model) {
        }

        protected override void fillModel(IEnumerable<JiraIssue> issues) {
            groupNodes.Clear();

            foreach (var issue in issues) {
                ByProjectIssueGroupNode group = findProjectGroupNode(issue);
                group.IssueNodes.Add(new IssueNode(issue));
            }

            if (StructureChanged != null) {
                StructureChanged(this, new TreePathEventArgs(TreePath.Empty));
            }
        }

        private ByProjectIssueGroupNode findProjectGroupNode(JiraIssue issue) {
            if (!groupNodes.ContainsKey(issue.ProjectKey)) {
                SortedDictionary<string, JiraProject> projects = JiraServerCache.Instance.getProjects(issue.Server);
                groupNodes[issue.ProjectKey] = new ByProjectIssueGroupNode(projects[issue.ProjectKey]);
            }
            return groupNodes[issue.ProjectKey];
        }

        #region ITreeModel Members

        public override IEnumerable GetChildren(TreePath treePath) {
            if (treePath.IsEmpty()) {
                return groupNodes.Values;
            }
            ByProjectIssueGroupNode groupNode = treePath.LastNode as ByProjectIssueGroupNode;
            return groupNode != null ? groupNode.IssueNodes : null;
        }

        public override bool IsLeaf(TreePath treePath) {
            return treePath.LastNode is IssueNode;
        }

        public override void modelChanged() {
            fillModel(model.Issues);
        }

        public override void issueChanged(JiraIssue issue) {
//            foreach (var node in nodes) {
//                if (node.Issue.Id != issue.Id) continue;

//                node.Issue = issue;
//                if (NodesChanged != null) {
//                    NodesChanged(this, new TreeModelEventArgs(TreePath.Empty, new object[] {node}));
//                }

//                return;
//            }
        }

        #region Overrides of AbstractIssueTreeModel

        public override event EventHandler<TreeModelEventArgs> NodesChanged;
        public override event EventHandler<TreeModelEventArgs> NodesInserted;
        public override event EventHandler<TreeModelEventArgs> NodesRemoved;
        public override event EventHandler<TreePathEventArgs> StructureChanged;

        #endregion

        #endregion
    }
}