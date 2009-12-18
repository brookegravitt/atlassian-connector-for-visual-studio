using System.Drawing;
using Atlassian.plvs.api;
using Atlassian.plvs.models;

namespace Atlassian.plvs.ui.issues.issuegroupnodes {
    class ByTypeIssueGroupNode: AbstractIssueGroupNode {
        private readonly JiraNamedEntity type;

        public ByTypeIssueGroupNode(JiraNamedEntity type) {
            this.type = type;
        }

        #region Overrides of AbstractIssueGroupNode

        public override Image Icon {
            get { return ImageCache.Instance.getImage(type.IconUrl); }
        }

        public override string getGroupName() {
            return type.Name;
        }

        #endregion

    }
}
