namespace Atlassian.plvs.models.presetFilters {
    public class JiraPresetFilterRecentlyResolved : JiraPresetFilter {
        public JiraPresetFilterRecentlyResolved() : base("Resolved Recently") { }

        #region Overrides of JiraPresetFilter

        public override string getFilterQueryStringNoProject() {
            return "status=5&status=6&updated:previous=-1w";
        }

        public override string getSortBy() {
            return "updated";
        }

        #endregion
    }
}
