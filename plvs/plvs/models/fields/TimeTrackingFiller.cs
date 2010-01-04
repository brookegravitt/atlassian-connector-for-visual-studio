using System.Collections.Generic;
using Atlassian.plvs.api;

namespace Atlassian.plvs.models.fields {
    public class TimeTrackingFiller : FieldFiller {
        public List<string> getFieldValues(string field, JiraIssue detailedIssue) {
            return detailedIssue.TimeSpent == null 
                ? new List<string> {translate(detailedIssue.OriginalEstimate)} 
                : new List<string> {translate(detailedIssue.RemainingEstimate)};
        }

        private static string translate(string displayValue) {
            if (displayValue != null) {
                displayValue = displayValue.Replace(" weeks", "w");
                displayValue = displayValue.Replace(" week", "w");
                displayValue = displayValue.Replace(" days", "d");
                displayValue = displayValue.Replace(" day", "d");
                displayValue = displayValue.Replace(" hours", "h");
                displayValue = displayValue.Replace(" hour", "h");
                displayValue = displayValue.Replace(" minutes", "m");
                displayValue = displayValue.Replace(" minute", "m");
                displayValue = displayValue.Replace(",", "");
            }
            return displayValue;
        }
    }
}
