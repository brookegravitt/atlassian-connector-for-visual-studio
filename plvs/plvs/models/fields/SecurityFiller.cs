using System.Collections.Generic;
using Atlassian.plvs.api;

namespace Atlassian.plvs.models.fields {
    public class SecurityFiller : FieldFiller {
        public List<string> getFieldValues(string field, JiraIssue issue, object soapIssueObject) {
            if (issue.SecurityLevel == null) {
                return null;
            }
            List<string> result = new List<string> {issue.SecurityLevel.Id.ToString()};
            return result;
        }
    }
}
