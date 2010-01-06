using System;
using System.Collections.Generic;
using System.Globalization;
using Atlassian.plvs.api;
using Atlassian.plvs.Atlassian.plvs.api.soap.service;

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

            // hmm, is it right for all cases? This is the date format JIRA is sending us, will it accept it back?
            List<string> result = new List<string> {((DateTime) dueDate).ToString("dd/MM/yy", new CultureInfo("en-US")) };
            return result;
        }
    }
}