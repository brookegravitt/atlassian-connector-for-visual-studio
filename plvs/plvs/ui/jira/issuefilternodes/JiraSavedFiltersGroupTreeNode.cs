using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.ui.jira.issuefilternodes {
    public class JiraSavedFiltersGroupTreeNode : JiraFilterGroupTreeNode {
        public JiraSavedFiltersGroupTreeNode(JiraServer server, int imageIdx) : base(server, "Saved Filters", imageIdx) {}
    }
}