using Atlassian.plvs.api;

namespace Atlassian.plvs.models.presetFilters {
    public class JiraPresetFilterUnscheduled : JiraPresetFilter {
        public JiraPresetFilterUnscheduled(JiraServer server) : base(server, "Unscheduled") { }

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
