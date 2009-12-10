using System;

namespace Atlassian.plvs.models.presetFilters {
    public class JiraPresetFilterRecentlyAdded: JiraPresetFilter {
        public JiraPresetFilterRecentlyAdded() : base("Added Recently") { }

        #region Overrides of JiraPresetFilter

        public override string getFilterQueryStringNoProject() {
            return "created:previous=-1w";
        }

        public override string getSortBy() {
            return "created";
        }

        #endregion
    }
}
