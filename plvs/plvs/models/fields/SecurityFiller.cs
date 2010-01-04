using System.Collections.Generic;
using Atlassian.plvs.api;

namespace Atlassian.plvs.models.fields {
    public class SecurityFiller : FieldFiller {
        public List<string> getFieldValues(string field, JiraIssue detailedIssue) {
            if (detailedIssue.SecurityLevel == null) {
                return null;
            }
            List<string> result = new List<string> {detailedIssue.SecurityLevel.Id.ToString()};
            return result;
        }
    }
}
