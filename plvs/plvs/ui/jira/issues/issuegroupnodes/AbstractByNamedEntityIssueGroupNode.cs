using System.Drawing;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.models;

namespace Atlassian.plvs.ui.jira.issues.issuegroupnodes {
    abstract class AbstractByNamedEntityIssueGroupNode: AbstractIssueGroupNode {
        private readonly JiraNamedEntity entity;

        protected AbstractByNamedEntityIssueGroupNode(JiraNamedEntity entity) {
            this.entity = entity;
        }

        #region Overrides of AbstractIssueGroupNode

        public override Image Icon {
            get { return ImageCache.Instance.getImage(entity.IconUrl); }
        }

        public override string getGroupName() {
            return entity.Name;
        }

        #endregion

    }
}