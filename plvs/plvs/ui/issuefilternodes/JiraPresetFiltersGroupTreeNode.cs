using Atlassian.plvs.api;

namespace Atlassian.plvs.ui.issuefilternodes {
    public class JiraPresetFiltersGroupTreeNode : JiraFilterGroupTreeNode {
        public JiraPresetFiltersGroupTreeNode(JiraServer server, int imageIdx) : base(server, "Preset Filters", imageIdx) {}
    }
}