using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.util.jira;

namespace Atlassian.plvs.models.jira.fields {
    public class CustomFieldFiller : FieldFiller {
        public List<string> getFieldValues(string field, JiraIssue issue, object soapIssueObject) {

            object[] customFieldValues = JiraIssueUtils.getIssueSoapObjectPropertyValue<object[]>(soapIssueObject, "customFieldValues");

            if (customFieldValues == null || customFieldValues.Length == 0) {
                return null;
            }

            foreach (object customFieldValue in customFieldValues) {
                PropertyInfo property = customFieldValue.GetType().GetProperty("customfieldId");
                if (property == null) continue;
                string fieldId = property.GetValue(customFieldValue, null) as string;
                if (fieldId == null || !fieldId.Equals(field)) continue;
                property = customFieldValue.GetType().GetProperty("values");
                if (property == null) continue;
                object[] values = property.GetValue(customFieldValue, null) as object[];

                return (from val in values select val.ToString()).ToList();
            }
            return null;
        }
    }
}