using System.Drawing;
using Atlassian.plvs.api;
using Atlassian.plvs.models;

namespace Atlassian.plvs.ui.issues.issuegroupnodes {
    class ByPriorityIssueGroupNode : AbstractIssueGroupNode {
        private readonly JiraNamedEntity priority;

        public ByPriorityIssueGroupNode(JiraNamedEntity priority) {
            this.priority = priority;
        }

        #region Overrides of AbstractIssueGroupNode

        public override Image Icon {
            get { return ImageCache.Instance.getImage(priority.IconUrl); }
        }

        public override string getGroupName() {
            return priority.Name;
        }

        #endregion

    }

}
