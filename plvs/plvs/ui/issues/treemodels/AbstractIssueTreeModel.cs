using System;
using System.Collections;
using System.Collections.Generic;
using Aga.Controls.Tree;
using Atlassian.plvs.api;
using Atlassian.plvs.models;

namespace Atlassian.plvs.ui.issues.treemodels {
    public abstract class AbstractIssueTreeModel : ITreeModel {
        
        protected JiraIssueListModel model { get; private set; }

        protected AbstractIssueTreeModel(JiraIssueListModel model) {
            this.model = model;
        }

        public void init() {
            fillModel(model.Issues);
            model.ModelChanged += model_ModelChanged;
            model.IssueChanged += model_IssueChanged;
        }

        protected abstract void model_IssueChanged(object sender, IssueChangedEventArgs e);

        protected abstract void model_ModelChanged(object sender, EventArgs e);

        public void shutdown() {
            model.ModelChanged -= model_ModelChanged;
            model.IssueChanged -= model_IssueChanged;
        }

        protected abstract void fillModel(IEnumerable<JiraIssue> issues);

        #region ITreeModel Members

        public abstract IEnumerable GetChildren(TreePath treePath);
        public abstract bool IsLeaf(TreePath treePath);
        public abstract event EventHandler<TreeModelEventArgs> NodesChanged;
        public abstract event EventHandler<TreeModelEventArgs> NodesInserted;
        public abstract event EventHandler<TreeModelEventArgs> NodesRemoved;
        public abstract event EventHandler<TreePathEventArgs> StructureChanged;

        #endregion
    }
}