using System.Collections.Generic;
using System.Linq;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.Atlassian.plvs.api.soap.service;

namespace Atlassian.plvs.models.jira.fields {
    public class FixVersionsFiller : FieldFiller {
        public List<string> getFieldValues(string field, JiraIssue issue, object soapIssueObject) {
            RemoteIssue ri = soapIssueObject as RemoteIssue;
            if (ri == null) {
                return null;
            }
            RemoteVersion[] rv = ri.fixVersions;
            return rv == null ? null : rv.Select(version => version.id).ToList();
        }
    }
}