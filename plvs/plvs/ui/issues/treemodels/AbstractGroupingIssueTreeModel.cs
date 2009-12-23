using System;
using System.Collections;
using System.Collections.Generic;
using Aga.Controls.Tree;
using Atlassian.plvs.api;
using Atlassian.plvs.models;
using Atlassian.plvs.ui.issues.issuegroupnodes;

namespace Atlassian.plvs.ui.issues.treemodels {
    internal abstract class AbstractGroupingIssueTreeModel : AbstractIssueTreeModel {

        protected AbstractGroupingIssueTreeModel(JiraIssueListModel model)
            : base(model) {
        }

        protected override void fillModel(IEnumerable<JiraIssue> issues) {
            clearGroupNodes();

            foreach (var issue in issues) {
                AbstractIssueGroupNode group = findGroupNode(issue);
                group.IssueNodes.Add(new IssueNode(issue));
            }

            if (StructureChanged != null) {
                StructureChanged(this, new TreePathEventArgs(TreePath.Empty));
            }
        }

        protected abstract void clearGroupNodes();

        protected abstract AbstractIssueGroupNode findGroupNode(JiraIssue issue);

        #region ITreeModel Members

        public override IEnumerable GetChildren(TreePath treePath) {
            if (treePath.IsEmpty()) {
                return getGroupNodes();
            }
            AbstractIssueGroupNode groupNode = treePath.LastNode as AbstractIssueGroupNode;
            return groupNode != null ? groupNode.IssueNodes : null;
        }

        protected abstract IEnumerable<AbstractIssueGroupNode> getGroupNodes();

        public override bool IsLeaf(TreePath treePath) {
            return treePath.LastNode is IssueNode;
        }

        public override void modelChanged() {
            fillModel(model.Issues);
        }

        public override void issueChanged(JiraIssue issue) {
            foreach (var groupNode in getGroupNodes()) {
                foreach (var issueNode in groupNode.IssueNodes) {
                    if (issueNode.Issue.Id != issue.Id) continue;
                    if (findGroupNode(issue) != groupNode) {
                        fillModel(model.Issues);
                    } else if (NodesChanged != null) {
                        issueNode.Issue = issue;
                        NodesChanged(this, new TreeModelEventArgs(new TreePath(groupNode), new object[] { issueNode }));
                    }
                    return;
                }
            }
        }

        #region Overrides of AbstractIssueTreeModel

        public override event EventHandler<TreeModelEventArgs> NodesChanged;
        public override event EventHandler<TreePathEventArgs> StructureChanged;

        #pragma warning disable 67
        public override event EventHandler<TreeModelEventArgs> NodesInserted;
        public override event EventHandler<TreeModelEventArgs> NodesRemoved;

        #endregion

        #endregion
    }

}
