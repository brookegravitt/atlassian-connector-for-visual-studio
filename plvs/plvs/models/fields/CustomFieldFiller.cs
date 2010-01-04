using System;
using System.Collections.Generic;
using System.Linq;
using Atlassian.plvs.api;
using Atlassian.plvs.Atlassian.plvs.api.soap.service;

namespace Atlassian.plvs.models.fields {
    public class CustomFieldFiller : FieldFiller {
        public List<string> getFieldValues(String field, JiraIssue detailedIssue) {
            RemoteIssue ri = detailedIssue.SoapIssueObject as RemoteIssue;
            if (ri == null) {
                return null;
            }
            RemoteCustomFieldValue[] customFields = ri.customFieldValues;
            foreach (RemoteCustomFieldValue customField in customFields) {
                if (customField.customfieldId.Equals(field)) {
                    return customField.values.ToList();
                }
            }
            return null;
        }
    }
}