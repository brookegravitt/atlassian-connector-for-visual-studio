namespace Atlassian.plvs.models.presetFilters {
    public class JiraPresetFilterReportedByMe : JiraPresetFilter {
        public JiraPresetFilterReportedByMe() : base("Reported By Me") { }

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
