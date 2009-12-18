using Atlassian.plvs.api;

namespace Atlassian.plvs.ui.issues.issuegroupnodes {
    class ByStatusIssueGroupNode : AbstractByNamedEntityIssueGroupNode {
        public ByStatusIssueGroupNode(JiraNamedEntity status) : base(status) {
        }
    }
}
