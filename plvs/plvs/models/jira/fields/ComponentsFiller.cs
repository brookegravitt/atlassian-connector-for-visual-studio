using System.Collections.Generic;
using System.Linq;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.Atlassian.plvs.api.soap.service;

namespace Atlassian.plvs.models.jira.fields {
    public class ComponentsFiller : FieldFiller {
        public List<string> getFieldValues(string field, JiraIssue issue, object soapIssueObject) {
            RemoteIssue ri = soapIssueObject as RemoteIssue;
            if (ri == null) {
                return null;
            }
            RemoteComponent[] components = ri.components;
            return components == null ? null : components.Select(component => component.id).ToList();
        }
    }
}