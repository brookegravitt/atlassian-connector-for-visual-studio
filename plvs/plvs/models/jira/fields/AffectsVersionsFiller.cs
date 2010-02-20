using System.Collections.Generic;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.Atlassian.plvs.api.soap.service;
using System.Linq;

namespace Atlassian.plvs.models.jira.fields {
    public class AffectsVersionsFiller : FieldFiller {
        public List<string> getFieldValues(string field, JiraIssue issue, object soapIssueObject) {
            RemoteIssue ri = soapIssueObject as RemoteIssue;
            if (ri == null) {
                return null;
            }
            RemoteVersion[] rv = ri.affectsVersions;
            return rv == null ? null : rv.Select(version => version.id).ToList();
        }
    }
}