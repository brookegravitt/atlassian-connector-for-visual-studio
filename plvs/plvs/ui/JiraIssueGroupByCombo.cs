using Atlassian.plvs.attributes;
using Atlassian.plvs.util;

namespace Atlassian.plvs.ui {
    class JiraIssueGroupByComboItem {
        public GroupBy By { get; private set; }

        public enum GroupBy {
            [StringValue("None")]
            NONE,
            [StringValue("Project")]
            PROJECT,
            [StringValue("Type")]
            TYPE,
            [StringValue("Status")]
            STATUS,
            [StringValue("Priority")]
            PRIORITY,
            [StringValue("Last Updated")]
            LAST_UPDATED
        }

        public JiraIssueGroupByComboItem(GroupBy groupBy) {
            By = groupBy;
        }

        public override string ToString() {
            return By.GetStringValue();
        }
    }
}
