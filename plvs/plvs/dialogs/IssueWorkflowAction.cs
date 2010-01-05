using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Atlassian.plvs.api;
using Atlassian.plvs.Atlassian.plvs.api.soap.service;
using Atlassian.plvs.models;
using Atlassian.plvs.ui;
using Atlassian.plvs.ui.fields;
using Atlassian.plvs.util;

namespace Atlassian.plvs.dialogs {
    public sealed partial class IssueWorkflowAction : Form {
        private readonly JiraIssue issue;
        private readonly RemoteIssue soapIssueObject;
        private readonly List<JiraField> fields;
        private readonly StatusLabel status;

        private int verticalPosition;
        private int tabIndex;

        private readonly TextBox textComment = new TextBox();
        private TextBox textUnsupported;
        private readonly List<JiraFieldEditor> editors = new List<JiraFieldEditor>();

        private const int LABEL_X_POS = 0;
        private const int FIELD_X_POS = 120;

        private const int LABEL_HEIGHT = 13;

        private const int MARGIN = 16;

        private const int INITIAL_WIDTH = 700;

        private List<JiraNamedEntity> issueTypes = new List<JiraNamedEntity>();
        private List<JiraNamedEntity> versions = new List<JiraNamedEntity>();

        public IssueWorkflowAction(JiraIssue issue, object soapIssueObject, JiraNamedEntity action,
                                   List<JiraField> fields, StatusLabel status) {
            this.issue = issue;
            this.soapIssueObject = soapIssueObject as RemoteIssue;
            this.fields = fields;
            this.status = status;

            InitializeComponent();

            Text = issue.Key + ": " + action.Name;

            ClientSize = new Size(0, 0);

            Visible = false;
            buttonOk.Enabled = false;

            StartPosition = FormStartPosition.CenterParent;
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
                status.setInfo("");

                Invoke(new MethodInvoker(delegate {
                                             verticalPosition = 0;

                                             fillFields();
                                             addCommentField();

                                             if (textUnsupported != null) {
                                                 textUnsupported.Location = new Point(FIELD_X_POS, verticalPosition);
                                                 panelContent.Controls.Add(textUnsupported);
                                                 verticalPosition += textUnsupported.Height + MARGIN;
                                             }

                                             ClientSize = new Size(INITIAL_WIDTH,
                                                                   verticalPosition + buttonOk.Height + 3*MARGIN);

                                             resizeStaticContent();

                                             buttonOk.Enabled = true;
                                             Visible = true;
                                         }));
            } else {
                status.setInfo("");
                Invoke(new MethodInvoker(() => MessageBox.Show("Unable to retrieve issue data", Constants.ERROR_CAPTION,
                                                               MessageBoxButtons.OK, MessageBoxIcon.Error)));
            }
        }

        private void resizeStaticContent() {
            panelContent.Location = new Point(MARGIN, MARGIN);
            panelContent.Size = new Size(ClientSize.Width - 2*MARGIN, ClientSize.Height - 3*MARGIN - buttonOk.Height);

            buttonOk.Location = new Point(ClientSize.Width - 2*buttonOk.Width - 2*MARGIN,
                                          ClientSize.Height - MARGIN - buttonOk.Height);
            buttonCancel.Location = new Point(ClientSize.Width - buttonOk.Width - MARGIN,
                                              ClientSize.Height - MARGIN - buttonCancel.Height);
        }

        private void fillFields() {
            List<JiraField> unsupportedFields = new List<JiraField>();

            foreach (JiraField field in fields) {
                JiraFieldEditor editor = null;
                switch (JiraActionFieldType.getFieldTypeForFieldId(field)) {
                    case JiraActionFieldType.WidgetType.SUMMARY:
                        editor = new TextLineFieldEditor(soapIssueObject != null ? soapIssueObject.summary : null);
                        break;
                    case JiraActionFieldType.WidgetType.DESCRIPTION:
                        editor = new TextAreaFieldEditor(soapIssueObject != null ? soapIssueObject.description : null);
                        break;
                    case JiraActionFieldType.WidgetType.ENVIRONMENT:
                        editor = new TextAreaFieldEditor(soapIssueObject != null ? soapIssueObject.environment : null);
                        break;
                    case JiraActionFieldType.WidgetType.ISSUE_TYPE:
                        editor = new IssueTypeFieldEditor(issue, issueTypes);
                        break;
                    case JiraActionFieldType.WidgetType.VERSIONS:
                        editor = new VersionFieldEditor(issue.Versions, versions);
                        break;
                    case JiraActionFieldType.WidgetType.FIX_VERSIONS:
                        editor = new VersionFieldEditor(issue.FixVersions, versions);
                        break;
                    case JiraActionFieldType.WidgetType.DUE_DATE:
                    case JiraActionFieldType.WidgetType.TIMETRACKING:
                    case JiraActionFieldType.WidgetType.RESOLUTION:
                    case JiraActionFieldType.WidgetType.PRIORITY:
                    case JiraActionFieldType.WidgetType.COMPONENTS:
                    case JiraActionFieldType.WidgetType.SECURITY:
                    case JiraActionFieldType.WidgetType.ASSIGNEE:
                    case JiraActionFieldType.WidgetType.REPORTER:
                    case JiraActionFieldType.WidgetType.UNSUPPORTED:
                    default:
                        unsupportedFields.Add(field);
                        break;
                }

                if (editor == null) continue;

                addLabel(field.Name);

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
            textComment.Size = new Size(calculatedFieldWidth(), JiraFieldEditor.MULTI_LINE_EDITOR_HEIGHT);
            textComment.TabIndex = tabIndex++;

            verticalPosition += JiraFieldEditor.MULTI_LINE_EDITOR_HEIGHT + MARGIN;

            panelContent.Controls.Add(textComment);
        }

        private void addLabel(string text) {
            Label l = new Label
                      {
                          AutoSize = true,
                          Location = new Point(LABEL_X_POS, verticalPosition + 3),
                          Size = new Size(FIELD_X_POS - LABEL_X_POS - MARGIN, LABEL_HEIGHT),
                          TextAlign = ContentAlignment.TopRight,
                          Text = text
                      };

            panelContent.Controls.Add(l);
        }

        private void buttonOk_Click(object sender, EventArgs e) {
//            Invoke(new MethodInvoker(() => model.updateIssue(newIssue)));
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            Close();
        }

        private void IssueWorkflowAction_Resize(object sender, EventArgs e) {
            SuspendLayout();

            resizeStaticContent();

            int width = calculatedFieldWidth();

            textComment.Size = new Size(width, JiraFieldEditor.MULTI_LINE_EDITOR_HEIGHT);
            foreach (JiraFieldEditor editor in editors) {
                editor.resizeToWidth(width);
            }
            if (textUnsupported != null) {
                textUnsupported.Width = width;
            }

            ResumeLayout(true);
        }

        private int calculatedFieldWidth() {
            return ClientSize.Width - FIELD_X_POS - 2*MARGIN;
        }
    }
}