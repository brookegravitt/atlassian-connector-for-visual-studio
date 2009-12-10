namespace Atlassian.plvs.models.presetFilters {
    public class JiraPresetFilterOutstanding : JiraPresetFilter {
        public JiraPresetFilterOutstanding() : base("Outstanding") { }

        #region Overrides of JiraPresetFilter

        public override string getFilterQueryStringNoProject() {
            return "resolution=-1";
        }

        public override string getSortBy() {
            return "updated";
        }

        #endregion
    }
}
