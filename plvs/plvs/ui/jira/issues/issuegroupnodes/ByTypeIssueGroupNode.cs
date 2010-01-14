using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.ui.jira.issues.issuegroupnodes {
    class ByTypeIssueGroupNode : AbstractByNamedEntityIssueGroupNode {
        public ByTypeIssueGroupNode(JiraNamedEntity type) : base(type) {
        }
    }
}