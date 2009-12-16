using System;
using System.Collections;
using System.Collections.Generic;
using Aga.Controls.Tree;
using Atlassian.plvs.api;
using Atlassian.plvs.models;

namespace Atlassian.plvs.ui.issues {
    public abstract class AbstractIssueTreeModel : ITreeModel, JiraIssueListModelListener {
        
        protected JiraIssueListModel model { get; private set; }

        protected AbstractIssueTreeModel(JiraIssueListModel model) {
            this.model = model;
        }

        public void init() {
            fillModel(model.Issues);
            model.addListener(this);
        }

        public void shutdown() {
            model.removeListener(this);
        }

        protected abstract void fillModel(IEnumerable<JiraIssue> issues);

        #region ITreeModel Members

//        public void updateIssue(JiraIssue issue) {}

        public abstract IEnumerable GetChildren(TreePath treePath);
        public abstract bool IsLeaf(TreePath treePath);
        public abstract event EventHandler<TreeModelEventArgs> NodesChanged;
        public abstract event EventHandler<TreeModelEventArgs> NodesInserted;
        public abstract event EventHandler<TreeModelEventArgs> NodesRemoved;
        public abstract event EventHandler<TreePathEventArgs> StructureChanged;

        #endregion

        #region Implementation of JiraIssueListModelListener

        public abstract void modelChanged();
        
        public abstract void issueChanged(JiraIssue issue);

        #endregion
    }
}
