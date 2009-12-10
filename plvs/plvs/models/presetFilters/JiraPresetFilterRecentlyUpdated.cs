namespace Atlassian.plvs.models.presetFilters {
    public class JiraPresetFilterRecentlyUpdated : JiraPresetFilter {
        public JiraPresetFilterRecentlyUpdated() : base("Updated Recently") { }

        #region Overrides of JiraPresetFilter

        public override string getFilterQueryStringNoProject() {
            return "updated:previous=-1w";
        }

        public override string getSortBy() {
            return "updated";
        }

        #endregion
    }
}
