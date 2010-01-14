using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.ui.jira.issues.issuegroupnodes {
    class ByStatusIssueGroupNode : AbstractByNamedEntityIssueGroupNode {
        public ByStatusIssueGroupNode(JiraNamedEntity status) : base(status) {
        }
    }
}