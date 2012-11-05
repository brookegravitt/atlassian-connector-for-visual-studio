using System.Collections.Generic;
using Atlassian.plvs.api.jira;
using System.Linq;
using Atlassian.plvs.util.jira;

namespace Atlassian.plvs.models.jira.fields {
    public class AffectsVersionsFiller : FieldFiller {
        public List<string> getFieldValues(string field, JiraIssue issue, object rawIssueObject) {

            object[] value = JiraIssueUtils.getRawIssueObjectPropertyValue<object[]>(rawIssueObject, "affectsVersions");

            if (value == null || value.Length == 0) {
                return null;
            }

            return (from v in value.ToList()
                    let prop = v.GetType().GetProperty("id")
                    where prop != null
                    select (string)prop.GetValue(v, null)).ToList();
        }
    }
}