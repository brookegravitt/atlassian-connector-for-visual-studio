namespace Atlassian.plvs.models.presetFilters {
    public class JiraPresetFilterAssignedToMe : JiraPresetFilter {
        public JiraPresetFilterAssignedToMe() : base("Assigned To Me") { }

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
