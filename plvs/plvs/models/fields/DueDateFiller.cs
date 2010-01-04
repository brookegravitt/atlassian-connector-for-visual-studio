using System;
using System.Collections.Generic;
using Atlassian.plvs.api;
using Atlassian.plvs.Atlassian.plvs.api.soap.service;

namespace Atlassian.plvs.models.fields {
    public class DueDateFiller : FieldFiller {
        public List<string> getFieldValues(string field, JiraIssue detailedIssue) {
            RemoteIssue ri = detailedIssue.SoapIssueObject as RemoteIssue;
            if (ri == null) {
                return null;
            }

            DateTime dueDate = (DateTime)ri.duedate;

            // hmm, is it right for all cases? This is the date format JIRA is sending us, will it accept it back?
            List<string> result = new List<string> { String.Format("{0:dd/MM/yy}", dueDate) };
            return result;
        }
    }
}