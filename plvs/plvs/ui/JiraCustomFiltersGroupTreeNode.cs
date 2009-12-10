using Atlassian.plvs.api;

namespace Atlassian.plvs.ui {
    class JiraCustomFiltersGroupTreeNode : JiraFilterGroupTreeNode {
        public JiraCustomFiltersGroupTreeNode(JiraServer server, int imageIdx) : base(server, "Custom Filters", imageIdx) {}
    }
}
