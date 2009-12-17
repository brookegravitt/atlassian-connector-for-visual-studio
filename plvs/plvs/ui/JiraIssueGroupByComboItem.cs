using System.Collections.Generic;
using Atlassian.plvs.attributes;
using Atlassian.plvs.models;
using Atlassian.plvs.ui.issues.treemodels;
using Atlassian.plvs.util;

namespace Atlassian.plvs.ui {
    internal class JiraIssueGroupByComboItem {
        private readonly JiraIssueListModel model;
        public GroupBy By { get; private set; }

        public enum GroupBy {
            [StringValue("None")] NONE,
            [StringValue("Project")] PROJECT,
            [StringValue("Type")] TYPE,
            [StringValue("Status")] STATUS,
            [StringValue("Priority")] PRIORITY,
            [StringValue("Last Updated")] LAST_UPDATED
        }

        private delegate AbstractIssueTreeModel CreateTreeModel(JiraIssueListModel model);

        private static readonly SortedDictionary<GroupBy, CreateTreeModel> TREE_MODEL_CREATORS =
            new SortedDictionary<GroupBy, CreateTreeModel>
                {
                    {GroupBy.NONE, model => new FlatIssueTreeModel(model)},
                    {GroupBy.PROJECT, model => new GroupedByProjectIssueTreeModel(model)},
//                    {GroupBy.TYPE, model => new FlatIssueTreeModel(model)},
//                    {GroupBy.STATUS, model => new FlatIssueTreeModel(model)},
//                    {GroupBy.PRIORITY, model => new FlatIssueTreeModel(model)},
//                    {GroupBy.LAST_UPDATED, model => new FlatIssueTreeModel(model)},
                };

        public JiraIssueGroupByComboItem(GroupBy groupBy, JiraIssueListModel model) {
            this.model = model;
            By = groupBy;
        }

        public override string ToString() {
            return By.GetStringValue();
        }

        public AbstractIssueTreeModel TreeModel {
            get {
                return TREE_MODEL_CREATORS.ContainsKey(By) ? TREE_MODEL_CREATORS[By](model) : new FlatIssueTreeModel(model);
            }
        }
    }
}