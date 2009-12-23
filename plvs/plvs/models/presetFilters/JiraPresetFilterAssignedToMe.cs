using Atlassian.plvs.api;

namespace Atlassian.plvs.models.presetFilters {
    public class JiraPresetFilterAssignedToMe : JiraPresetFilter {
        public JiraPresetFilterAssignedToMe(JiraServer server) : base(server, "Assigned To Me") { }

        #region Overrides of JiraPresetFilter

        public override string getFilterQueryStringNoProject() {
            return "assigneeSelect=issue_current_user&resolution=-1";
        }

        public override string getSortBy() {
            return "priority";
        }

        #endregion
    }
}
