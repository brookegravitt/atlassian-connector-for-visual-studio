using System.Collections.Generic;
using Atlassian.plvs.api;
using Atlassian.plvs.Atlassian.plvs.api.soap.service;

namespace Atlassian.plvs.models.fields {
    public class ComponentsFiller : FieldFiller {
        public List<string> getFieldValues(string field, JiraIssue detailedIssue) {
            RemoteIssue ri = detailedIssue.SoapIssueObject as RemoteIssue;
            if (ri == null) {
                return null;
            }
            RemoteComponent[] components = ri.components;
            List<string> result = new List<string>();
            if (components == null) {
                return null;
            }
            foreach (RemoteComponent component in components) {
                result.Add(component.id);
            }
            return result;
        }
    }
}