using System.Collections.Generic;
using Atlassian.plvs.api;
using Atlassian.plvs.Atlassian.plvs.api.soap.service;

namespace Atlassian.plvs.models.fields {
    public class AffectsVersionsFiller : FieldFiller {
        public List<string> getFieldValues(string field, JiraIssue detailedIssue) {
            RemoteIssue ri = detailedIssue.SoapIssueObject as RemoteIssue;
            if (ri == null) {
                return null;
            }
            RemoteVersion[] rv = ri.affectsVersions;
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