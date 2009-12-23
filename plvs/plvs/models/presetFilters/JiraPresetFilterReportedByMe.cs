using Atlassian.plvs.api;

namespace Atlassian.plvs.models.presetFilters {
    public class JiraPresetFilterReportedByMe : JiraPresetFilter {
        public JiraPresetFilterReportedByMe(JiraServer server) : base(server, "Reported By Me") { }

        #region Overrides of JiraPresetFilter

        public override string getFilterQueryStringNoProject() {
            return "reporterSelect=issue_current_user";
        }

        public override string getSortBy() {
            return "updated";
        }

        #endregion
    }
}
