using System;
using System.Collections.Generic;
using Atlassian.plvs.api;
using Atlassian.plvs.Atlassian.plvs.api.soap.service;
using Atlassian.plvs.util;

namespace Atlassian.plvs.models.fields {
    public class DueDateFiller : FieldFiller {
        public List<string> getFieldValues(string field, JiraIssue issue, object soapIssueObject) {
            RemoteIssue ri = soapIssueObject as RemoteIssue;
            if (ri == null) {
                return null;
            }

            DateTime? dueDate = ri.duedate;
            if (dueDate == null) {
                return new List<string>();
            }

            string dateString = JiraIssueUtils.getShortDateStringFromDateTime((DateTime) dueDate);
            List<string> result = new List<string> {dateString};
            return result;
        }
    }
}