using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using Atlassian.plvs.api;
using Atlassian.plvs.models;
using Atlassian.plvs.ui;

namespace Atlassian.plvs.util {
    public sealed class IssueActionRunner {
        public static void runAction(Control owner, JiraNamedEntity action, JiraIssueListModel model, JiraIssue issue, StatusLabel status) {
            Thread runner = new Thread(new ThreadStart(delegate {
                                                           try {
                                                               status.setInfo("Retrieving fields for action \"" +
                                                                              action.Name + "\"...");
                                                               List<JiraField> fields = JiraServerFacade.Instance.getFieldsForAction(issue, action.Id);
                                                               if (fields == null || fields.Count == 0) {
                                                                   runActionWithoutFields(owner, action, model, issue, status);
                                                               } else {
                                                                   runActionWithFields(owner, action, model, issue, fields, status);
//                                                                   status.setInfo("Action \"" + action.Name
//                                                                                  + "\" requires input fields, opening action screen in the browser...");
//                                                                   Process.Start(issue.Server.Url
//                                                                                 + "/secure/WorkflowUIDispatcher.jspa?id=" 
//                                                                                 + issue.Id + "&action=" + action.Id);
                                                               }
                                                           } catch (Exception e) {
                                                               status.setError("Failed to run action " + action.Name + " on issue " + issue.Key, e);
                                                           }
                                                       }));
            runner.Start();
        }

        private static void runActionWithFields(Control owner, JiraNamedEntity action, JiraIssueListModel model, JiraIssue issue, List<JiraField> fields, StatusLabel status) {
            JiraIssue issueWithTime = JiraServerFacade.Instance.getIssue(issue.Server, issue.Key);
            issueWithTime.SecurityLevel = JiraServerFacade.Instance.getSecurityLevel(issue);
            issueWithTime.SoapIssueObject = JiraServerFacade.Instance.getIssueSoapObject(issue);
        }

        private static void runActionWithoutFields(Control owner, JiraNamedEntity action, JiraIssueListModel model, JiraIssue issue, StatusLabel status) {
            status.setInfo("Running action \"" + action.Name + "\" on issue " + issue.Key + "...");
            JiraServerFacade.Instance.runIssueActionWithoutParams(issue, action);
            status.setInfo("Action \"" + action.Name + "\" successfully run on issue " + issue.Key);
            var newIssue = JiraServerFacade.Instance.getIssue(issue.Server, issue.Key);
            owner.Invoke(new MethodInvoker(() => model.updateIssue(newIssue)));
        }
    }
}