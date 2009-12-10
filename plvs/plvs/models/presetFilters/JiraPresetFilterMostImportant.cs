namespace Atlassian.plvs.models.presetFilters {
    public class JiraPresetFilterMostImportant : JiraPresetFilter {
        public JiraPresetFilterMostImportant() : base("Most Important") { }

        #region Overrides of JiraPresetFilter

        public override string getFilterQueryStringNoProject() {
            return "resolution=-1";
        }

        public override string getSortBy() {
            return "priority";
        }

        #endregion
    }
}
