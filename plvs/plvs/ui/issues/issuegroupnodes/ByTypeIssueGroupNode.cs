using Atlassian.plvs.api;

namespace Atlassian.plvs.ui.issues.issuegroupnodes {
    class ByTypeIssueGroupNode : AbstractByNamedEntityIssueGroupNode {
        public ByTypeIssueGroupNode(JiraNamedEntity type) : base(type) {
        }
    }
}
