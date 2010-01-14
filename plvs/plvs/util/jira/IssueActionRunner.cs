using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.dialogs;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui;

namespace Atlassian.plvs.util.jira {
    public sealed class IssueActionRunner {
        public static void runAction(Control owner, JiraNamedEntity action, JiraIssueListModel model, JiraIssue issue, StatusLabel status) {
            Thread runner = new Thread(new ThreadStart(delegate {
                                                           try {
                                                               status.setInfo("Retrieving fields for action \"" + action.Name + "\"...");
                                                               List<JiraField> fields = JiraServerFacade.Instance.getFieldsForAction(issue, action.Id);
                                                               runAction(owner, action, model, issue, fields, status);
                                                           } catch (Exception e) {
                                                               status.setError("Failed to run action " + action.Name + " on issue " + issue.Key, e);
                                                           }
                                                       }));
            runner.Start();
        }

        private static void runAction(Control owner, JiraNamedEntity action, JiraIssueListModel model, 
                                      JiraIssue issue, List<JiraField> fields, StatusLabel status) {

            JiraIssue issueWithTime = JiraServerFacade.Instance.getIssue(issue.Server, issue.Key);
            issueWithTime.SecurityLevel = JiraServerFacade.Instance.getSecurityLevel(issue);
            object soapIssueObject = JiraServerFacade.Instance.getIssueSoapObject(issue);
            List<JiraField> fieldsWithValues = JiraActionFieldType.fillFieldValues(issue, soapIssueObject, fields);
            if (fieldsWithValues == null || fieldsWithValues.Count == 0) {
                runActionWithoutFields(owner, action, model, issue, status);
            } else {
                owner.Invoke(new MethodInvoker(delegate {
                                                   IssueWorkflowAction actionDlg = new IssueWorkflowAction(issue, action, model, fieldsWithValues, status);
                                                   actionDlg.initAndShowDialog();
                                               }));
            }
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