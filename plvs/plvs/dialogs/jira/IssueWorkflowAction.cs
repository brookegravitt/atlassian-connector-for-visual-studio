﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.autoupdate;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui;
using Atlassian.plvs.ui.jira.fields;
using Atlassian.plvs.util;
using Atlassian.plvs.util.jira;

namespace Atlassian.plvs.dialogs.jira {
    public sealed partial class IssueWorkflowAction : Form {
        private readonly JiraIssue issue;
        private readonly JiraNamedEntity action;
        private readonly JiraIssueListModel model;
        private readonly ICollection<JiraField> fields;
        private readonly StatusLabel status;

        private int verticalPosition;
        private int tabIndex;

        private readonly TextBox textComment = new TextBox();
        private TextBox textUnsupported;
        private readonly List<JiraFieldEditorProvider> editors = new List<JiraFieldEditorProvider>();

        private const int LABEL_X_POS = 0;
        private const int FIELD_X_POS = 120;

        private const int LABEL_HEIGHT = 13;

        private const int MARGIN = 16;

        private const int INITIAL_WIDTH = 700;
        private const int INITIAL_HEIGHT = 500;

        private List<JiraNamedEntity> issueTypes = new List<JiraNamedEntity>();
        private List<JiraNamedEntity> versions = new List<JiraNamedEntity>();
        private List<JiraNamedEntity> comps = new List<JiraNamedEntity>();

        public IssueWorkflowAction(
            JiraIssue issue, JiraNamedEntity action, JiraIssueListModel model, List<JiraField> fields, StatusLabel status) {

            this.issue = issue;
            this.action = action;
            this.model = model;
            this.fields = JiraActionFieldType.sortFieldList(fields);

            this.status = status;

            InitializeComponent();

            Text = issue.Key + ": " + action.Name;

            ClientSize = new Size(INITIAL_WIDTH, INITIAL_HEIGHT + buttonOk.Height + 3 * MARGIN);

            buttonOk.Enabled = false;

            StartPosition = FormStartPosition.CenterParent;

            panelThrobber.Visible = true;
            panelContent.Visible = false;
        }

        public void initAndShowDialog() {
            initializeFields();
        }

        private void initializeFields() {
            Thread t = new Thread(initializeThreadWorker);
            t.Start();
            ShowDialog();
        }

        private void initializeThreadWorker() {
            SortedDictionary<string, JiraProject> projects = JiraServerCache.Instance.getProjects(issue.Server);
            if (projects.ContainsKey(issue.ProjectKey)) {
                status.setInfo("Retrieving issue field data...");
                JiraProject project = projects[issue.ProjectKey];
                issueTypes = JiraServerFacade.Instance.getIssueTypes(issue.Server, project);
                versions = JiraServerFacade.Instance.getVersions(issue.Server, project);
                comps = JiraServerFacade.Instance.getComponents(issue.Server, project);

                status.setInfo("");

                if (!Visible) return;

                Invoke(new MethodInvoker(delegate {
                                             panelContent.Visible = true;
                                             panelThrobber.Visible = false;

                                             verticalPosition = 0;

                                             fillFields();
                                             addCommentField();

                                             if (textUnsupported != null) {
                                                 textUnsupported.Location = new Point(LABEL_X_POS, verticalPosition);
                                                 panelContent.Controls.Add(textUnsupported);
                                                 verticalPosition += textUnsupported.Height + MARGIN;
                                             }

                                             ClientSize = new Size(INITIAL_WIDTH,
                                                                   Math.Min(verticalPosition, INITIAL_HEIGHT) + buttonOk.Height + 4*MARGIN);

                                             // resize to perform layout
                                             Size = new Size(Width + 1, Height + 1);

                                             updateOkButton();

                                         }));
            } else {
                status.setInfo("");
                if (Visible) {
                    Invoke(new MethodInvoker(() => MessageBox.Show("Unable to retrieve issue data", Constants.ERROR_CAPTION,
                                                                   MessageBoxButtons.OK, MessageBoxIcon.Error)));
                }
            }
        }


        private void resizeStaticContent() {
            panelContent.Location = new Point(MARGIN, MARGIN);
            panelContent.SuspendLayout();
            panelContent.Size = new Size(ClientSize.Width - 2*MARGIN, ClientSize.Height - 3*MARGIN - buttonOk.Height);
            panelContent.ResumeLayout(true);

            buttonOk.Location = new Point(ClientSize.Width - 2*buttonOk.Width - 3*MARGIN/2,
                                          ClientSize.Height - MARGIN - buttonOk.Height);
            buttonCancel.Location = new Point(ClientSize.Width - buttonOk.Width - MARGIN,
                                              ClientSize.Height - MARGIN - buttonCancel.Height);
        }

        private void fieldValid(JiraFieldEditorProvider sender, bool valid) {
            updateOkButton();    
        }

        private void updateOkButton() {
            foreach (var editor in editors) {
                if (editor.FieldValid) continue;
                buttonOk.Enabled = false;
                return;
            }
            buttonOk.Enabled = true;
        }

        private void fillFields() {
            List<JiraField> unsupportedFields = new List<JiraField>();

            foreach (JiraField field in fields) {
                JiraFieldEditorProvider editor = null;
                switch (JiraActionFieldType.getFieldTypeForField(field)) {
                    case JiraActionFieldType.WidgetType.SUMMARY:
                        editor = new TextLineFieldEditorProvider(field, field.Values.Count > 0 ? field.Values[0] : "", fieldValid);
                        break;
                    case JiraActionFieldType.WidgetType.DESCRIPTION:
                        editor = new TextAreaFieldEditorProvider(field, field.Values.Count > 0 ? field.Values[0] : "", fieldValid);
                        break;
                    case JiraActionFieldType.WidgetType.ENVIRONMENT:
                        editor = new TextAreaFieldEditorProvider(field, field.Values.Count > 0 ? field.Values[0] : "", fieldValid);
                        break;
                    case JiraActionFieldType.WidgetType.ISSUE_TYPE:
                        editor = new NamedEntityComboEditorProvider(field, issue.IssueTypeId, issueTypes, fieldValid);
                        break;
                    case JiraActionFieldType.WidgetType.VERSIONS:
                        editor = new NamedEntityListFieldEditorProvider(field, issue.Versions, versions, fieldValid);
                        break;
                    case JiraActionFieldType.WidgetType.FIX_VERSIONS:
                        editor = new NamedEntityListFieldEditorProvider(field, issue.FixVersions, versions, fieldValid);
                        break;
                    case JiraActionFieldType.WidgetType.ASSIGNEE:
                        editor = new UserFieldEditorProvider(issue.Server, field, field.Values.Count > 0 ? field.Values[0] : "", fieldValid);
                        break;
                    case JiraActionFieldType.WidgetType.REPORTER:
                        editor = new UserFieldEditorProvider(issue.Server, field, field.Values.Count > 0 ? field.Values[0] : "", fieldValid);
                        break;
                    case JiraActionFieldType.WidgetType.DUE_DATE:
                        editor = new DateFieldEditorProvider(field, field.Values.Count > 0 
                                                                ? JiraIssueUtils.getDateTimeFromShortString(field.Values[0]) 
                                                                : (DateTime?) null, fieldValid);
                        break;
                    case JiraActionFieldType.WidgetType.COMPONENTS:
                        editor = new NamedEntityListFieldEditorProvider(field, issue.Components, comps, fieldValid);
                        break;
                    case JiraActionFieldType.WidgetType.RESOLUTION:
                        SortedDictionary<int, JiraNamedEntity> resolutions = JiraServerCache.Instance.getResolutions(issue.Server);
                        editor = new NamedEntityComboEditorProvider(field, issue.ResolutionId, resolutions != null ? resolutions.Values : null, fieldValid, false);
                        break;
                    case JiraActionFieldType.WidgetType.PRIORITY:
                        editor = new NamedEntityComboEditorProvider(field, issue.PriorityId, JiraServerCache.Instance.getPriorities(issue.Server), fieldValid);
                        break;
                    case JiraActionFieldType.WidgetType.TIMETRACKING:
                        editor = new TimeTrackingEditorProvider(field, field.Values.Count > 0 ? field.Values[0] : "", fieldValid);
                        break;
// ReSharper disable RedundantCaseLabel
                    case JiraActionFieldType.WidgetType.SECURITY:
                    case JiraActionFieldType.WidgetType.UNSUPPORTED:
// ReSharper restore RedundantCaseLabel
                    default:
                        if (!JiraActionFieldType.isTimeField(field)) {
                            unsupportedFields.Add(field);
                        }
                        break;
                }

                if (editor == null) continue;

                addLabel(editor.getFieldLabel(issue, field));

                editor.Widget.Location = new Point(FIELD_X_POS, verticalPosition);
                editor.Widget.TabIndex = tabIndex++;

                panelContent.Controls.Add(editor.Widget);
                verticalPosition += editor.VerticalSkip + MARGIN / 2;
                editors.Add(editor);
            }

            if (unsupportedFields.Count == 0) return;

            StringBuilder sb = new StringBuilder();
            sb.Append("Unsupported fields (existing values copied): ");
            foreach (var field in unsupportedFields) {
                sb.Append(field.Name).Append(", ");
            }
            textUnsupported = new TextBox
                              {
                                  Multiline = true,
                                  BorderStyle = BorderStyle.None,
                                  ReadOnly = true,
                                  WordWrap = true,
                                  Text = sb.ToString().Substring(0, sb.Length - 2),
                              };
        }

        private void addCommentField() {
            addLabel("Comment");

            textComment.Location = new Point(FIELD_X_POS, verticalPosition);
            textComment.Multiline = true;
            textComment.Size = new Size(calculatedFieldWidth(), JiraFieldEditorProvider.MULTI_LINE_EDITOR_HEIGHT);
            textComment.TabIndex = tabIndex++;

            verticalPosition += JiraFieldEditorProvider.MULTI_LINE_EDITOR_HEIGHT + MARGIN;

            panelContent.Controls.Add(textComment);
        }

        private void addLabel(string text) {
            Label l = new Label
                      {
                          AutoSize = false,
                          Location = new Point(LABEL_X_POS, verticalPosition + 3),
                          Size = new Size(FIELD_X_POS - LABEL_X_POS - MARGIN / 2, LABEL_HEIGHT),
                          TextAlign = ContentAlignment.TopRight,
                          Text = text
                      };

            panelContent.Controls.Add(l);
        }

        private void buttonOk_Click(object sender, EventArgs e) {
            setAllEnabled(false);
            setThrobberVisible(true);

            ICollection<JiraField> updatedFields = mergeFieldsFromEditors();
            Thread t = new Thread(new ThreadStart(delegate {
                                                      try {
                                                          JiraServerFacade.Instance.runIssueActionWithParams(
                                                              issue, action, updatedFields, textComment.Text.Length > 0 ? textComment.Text : null);
                                                          var newIssue = JiraServerFacade.Instance.getIssue(issue.Server, issue.Key);
                                                          UsageCollector.Instance.bumpJiraIssuesOpen();
                                                          status.setInfo("Action \"" + action.Name + "\" successfully run on issue " + issue.Key);
                                                          Invoke(new MethodInvoker(delegate {
                                                                                       Close();
                                                                                       model.updateIssue(newIssue);
                                                                                   }));
                                                      }
                                                      catch (Exception ex) {
                                                          Invoke(new MethodInvoker(() => showErrorAndResumeEditing(ex)));
                                                      }
                                                  }));
            t.Start();
        }

        private void showErrorAndResumeEditing(Exception ex) {
            MessageBox.Show("Failed to run action " + action.Name + " on issue " + issue.Key + "\n" + ex.Message, 
                            Constants.ERROR_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
            setThrobberVisible(false);
            setAllEnabled(true);
        }

        private void setThrobberVisible(bool visible) {
            SuspendLayout();
            panelContent.SuspendLayout();
            panelThrobber.Visible = visible;
            panelContent.Visible = !visible;
            foreach (JiraFieldEditorProvider editor in editors) {
                editor.Widget.Visible = !visible;
            }
            textComment.Visible = !visible;
            if (textUnsupported != null) {
                textUnsupported.Visible = !visible;
            }
            panelContent.ResumeLayout(true);
            ResumeLayout(true);
        }

        private void setAllEnabled(bool enabled) {
            buttonCancel.Enabled = enabled;
            foreach (JiraFieldEditorProvider editor in editors) {
                editor.Widget.Enabled = enabled;
            }
            textComment.Enabled = enabled;
            if (enabled) {
                updateOkButton();
            } else {
                buttonOk.Enabled = false;
            }
        }

        private ICollection<JiraField> mergeFieldsFromEditors() {
            Dictionary<string, JiraField> map = new Dictionary<string, JiraField>();
            foreach (var field in fields) {
                map[field.Id] = field;
            }
            foreach (JiraFieldEditorProvider editor in editors) {
                editor.Field.Values = editor.getValues();
                map[editor.Field.Id] = editor.Field;
            }
            return map.Values;
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            Close();
        }

        private void IssueWorkflowAction_Resize(object sender, EventArgs e) {
            SuspendLayout();

            resizeStaticContent();

            int width = calculatedFieldWidth();

            textComment.Size = new Size(width, JiraFieldEditorProvider.MULTI_LINE_EDITOR_HEIGHT);
            foreach (JiraFieldEditorProvider editor in editors) {
                editor.resizeToWidth(width);
            }
            if (textUnsupported != null) {
                textUnsupported.Width = ClientSize.Width - 4*MARGIN;
            }

            ResumeLayout(true);
        }

        private int calculatedFieldWidth() {
            return ClientSize.Width - FIELD_X_POS - 4*MARGIN;
        }
    }
}