using Atlassian.plvs.api;

namespace Atlassian.plvs.ui.issuefilternodes {
    class JiraCustomFiltersGroupTreeNode : JiraFilterGroupTreeNode {
        public JiraCustomFiltersGroupTreeNode(JiraServer server, int imageIdx) : base(server, "Custom Filters", imageIdx) {
            Tag = "Right-click to add filter";
        }
    }
}