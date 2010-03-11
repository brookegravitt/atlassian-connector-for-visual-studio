﻿using System.Collections.Generic;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.util.jira;

namespace Atlassian.plvs.models.jira.fields {
    public class IssueTypeFiller : FieldFiller {
        public List<string> getFieldValues(string field, JiraIssue issue, object soapIssueObject) {
            string value = JiraIssueUtils.getIssueSoapObjectPropertyValue<string>(soapIssueObject, "type");
            return value != null ? new List<string> { value } : null; 
        }
    }
}