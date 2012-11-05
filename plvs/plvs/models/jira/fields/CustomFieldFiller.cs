using System;
using System.Collections.Generic;
using System.Linq;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.util.jira;
using Newtonsoft.Json.Linq;

namespace Atlassian.plvs.models.jira.fields {
    public class CustomFieldFiller : FieldFiller {
        public List<string> getFieldValues(string field, JiraIssue issue, object rawIssueObject) {

            var customFieldValues = JiraIssueUtils.getRawIssueObjectPropertyValue<object[]>(rawIssueObject, "customFieldValues");

            if (customFieldValues == null || customFieldValues.Length == 0) {
                return null;
            }

            foreach (var customFieldValue in customFieldValues) {
                var property = customFieldValue.GetType().GetProperty("customfieldId");
                if (property == null) continue;
                var fieldId = property.GetValue(customFieldValue, null) as string;
                if (fieldId == null || !fieldId.Equals(field)) continue;
                property = customFieldValue.GetType().GetProperty("values");
                if (property == null) continue;
                var values = property.GetValue(customFieldValue, null) as object[];

                return (from val in values select val.ToString()).ToList();
            }
            return null;
        }
    }
}