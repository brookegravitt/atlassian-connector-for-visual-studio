using System;
using System.Collections.Generic;
using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.models.jira.fields {
    public interface FieldFiller {
        List<string> getFieldValues(String field, JiraIssue issue, object soapIssueObject);
    }
}