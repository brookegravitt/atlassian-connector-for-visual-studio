using Atlassian.plvs.api;

namespace Atlassian.plvs.ui.issuefilternodes {
    public class JiraPresetFiltersGroupTreeNode : JiraFilterGroupTreeNode {
        public JiraPresetFiltersGroupTreeNode(JiraServer server, int imageIdx) : base(server, "Preset Filters", imageIdx) {
            Tag = "Right-click to set or clear project";
        }

        public JiraProject Project { get; set; }
    }
}