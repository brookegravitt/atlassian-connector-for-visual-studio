using System.Collections.Generic;
using System.Linq;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.util.jira;

namespace Atlassian.plvs.models.jira.fields {
    public class ComponentsFiller : FieldFiller {
        public List<string> getFieldValues(string field, JiraIssue issue, object rawIssueObject) {

            object[] value = JiraIssueUtils.getRawIssueObjectPropertyValue<object[]>(rawIssueObject, "components");

            if (value == null || value.Length == 0) {
                return null;
            }

            return (from v in value.ToList()
                    let prop = v.GetType().GetProperty("id")
                    where prop != null
                    select (string) prop.GetValue(v, null)).ToList();
        }
    }
}