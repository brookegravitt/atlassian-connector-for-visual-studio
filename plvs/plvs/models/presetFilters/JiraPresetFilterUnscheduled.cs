namespace Atlassian.plvs.models.presetFilters {
    public class JiraPresetFilterUnscheduled : JiraPresetFilter {
        public JiraPresetFilterUnscheduled() : base("Unscheduled") { }

        #region Overrides of JiraPresetFilter

        public override string getFilterQueryStringNoProject() {
            return "resolution=-1&fixfor=-1";
        }

        public override string getSortBy() {
            return "priority";
        }

        #endregion
    }
}
