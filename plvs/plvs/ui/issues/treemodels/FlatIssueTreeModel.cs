using System;
using System.Collections;
using System.Collections.Generic;
using Aga.Controls.Tree;
using Atlassian.plvs.api;
using Atlassian.plvs.models;

namespace Atlassian.plvs.ui.issues.treemodels {
    internal class FlatIssueTreeModel : AbstractIssueTreeModel {
        private readonly List<IssueNode> nodes = new List<IssueNode>();

        public FlatIssueTreeModel(JiraIssueListModel model) : base(model) {
        }

        protected override void fillModel(IEnumerable<JiraIssue> issues) {
            nodes.Clear();

            foreach (var issue in issues) {
                nodes.Add(new IssueNode(issue));
            }

            if (StructureChanged != null) {
                StructureChanged(this, new TreePathEventArgs(TreePath.Empty));
            }
        }

        #region ITreeModel Members

        public override IEnumerable GetChildren(TreePath treePath) {
            return treePath.IsEmpty() ? nodes : null;
        }

        public override bool IsLeaf(TreePath treePath) {
            return true;
        }

        protected override void model_ModelChanged(object sender, EventArgs e) {
            fillModel(model.Issues);
        }

        protected override void model_IssueChanged(object sender, IssueChangedEventArgs e) {
            foreach (var node in nodes) {
                if (node.Issue.Id != e.Issue.Id) continue;

                node.Issue = e.Issue;
                if (NodesChanged != null) {
                    NodesChanged(this, new TreeModelEventArgs(TreePath.Empty, new object[] {node}));
                }

                return;
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