using System.Collections.Generic;
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
            List<string> result = new List<string>();
            if (rv == null) {
                return null;
            }
            foreach (RemoteVersion version in rv) {
                result.Add(version.id);
            }
            return result;
        }
    }
}