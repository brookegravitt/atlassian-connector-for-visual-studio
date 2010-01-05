using System;
using System.Collections.Generic;
using Atlassian.plvs.api;

namespace Atlassian.plvs.models.fields {
    public interface FieldFiller {
        List<string> getFieldValues(String field, JiraIssue issue, object soapIssueObject);
    }
}
