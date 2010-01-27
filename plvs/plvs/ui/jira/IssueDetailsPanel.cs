using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.autoupdate;
using Atlassian.plvs.dialogs;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui.jira.issues;
using Atlassian.plvs.util.jira;
using Atlassian.plvs.windows;
using EnvDTE;
using AtlassianConstants=Atlassian.plvs.util.Constants;
using Constants=EnvDTE.Constants;
using Process=System.Diagnostics.Process;
using Thread=System.Threading.Thread;

namespace Atlassian.plvs.ui.jira {
    public partial class IssueDetailsPanel : UserControl {
        private readonly JiraIssueListModel model;
        private readonly Solution solution;

        private readonly JiraServerFacade facade = JiraServerFacade.Instance;

        private readonly StatusLabel status;

        private JiraIssue issue;
        private readonly TabControl tabWindow;
        private readonly TabPage myTab;

        private bool issueCommentsLoaded;
        private bool issueDescriptionLoaded;
        private bool issueSummaryLoaded;

        private bool issueSubtasksLoaded;
        
        private const int A_LOT = 100000;

        private string editImagePath = null;

        public IssueDetailsPanel(JiraIssueListModel model, Solution solution, JiraIssue issue, TabControl tabWindow,
                                 TabPage myTab, ToolWindowStateMonitor toolWindowStateMonitor) {
            this.model = model;
            this.solution = solution;

            InitializeComponent();

            status = new StatusLabel(statusStrip, jiraStatus);

            this.issue = issue;

            this.tabWindow = tabWindow;
            this.myTab = myTab;

            dropDownIssueActions.DropDownItems.Add("dummy");

            toolWindowStateMonitor.ToolWindowShown += toolWindowStateMonitor_ToolWindowShown;
            toolWindowStateMonitor.ToolWindowHidden += toolWindowStateMonitor_ToolWindowHidden;

            listViewAttachments.ContextMenuStrip = new ContextMenuStrip();
            listViewAttachments.ContextMenuStrip.Items.Add(new ToolStripMenuItem("Save as...", null,
                                                                                 new EventHandler(saveAttachment)));
            listViewAttachments.ContextMenuStrip.Opening += attachmentsMenuOpening;

            saveAsToolStripMenuItem.Enabled = false;

            Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            string name = assembly.EscapedCodeBase;
            if (name != null) {
                name = name.Substring(0, name.LastIndexOf("/"));
                editImagePath = name + "/edit.png";
            }
        }

        private void init() {
            addModelListeners();

            rebuildAllPanels(false);
            status.setInfo("No issue details yet");
            runRefreshThread();
        }

        protected override void OnLoad(EventArgs e) {
            init();
        }

        void toolWindowStateMonitor_ToolWindowShown(object sender, EventArgs e) {
            init();
        }

        void toolWindowStateMonitor_ToolWindowHidden(object sender, EventArgs e) {
            removeModelListeners();
        }

        private void addModelListeners() {
            model.IssueChanged += model_IssueChanged;
            model.ModelChanged += model_ModelChanged;
        }

        private void removeModelListeners() {
            model.IssueChanged -= model_IssueChanged;
            model.ModelChanged -= model_ModelChanged;
        }

        private void model_ModelChanged(object sender, EventArgs e) {
            Invoke(new MethodInvoker(delegate {
                                         // this is crude. But issueChanged() will do its job of 
                                         // filtering out the proper issue if it still exists in 
                                         // the model and comparing it what we currently have
                                         foreach (var jiraIssue in model.Issues) {
                                             model_IssueChanged(sender, new IssueChangedEventArgs(jiraIssue));
                                         }
                                     }));
        }

        private void model_IssueChanged(object sender, IssueChangedEventArgs e) {
            if (!e.Issue.Server.GUID.Equals(issue.Server.GUID)) return;
            if (e.Issue.Id.Equals(issue.Id)) {
                if (e.Issue.Equals(issue)) return;
                buttonRefresh.Enabled = false;
                runRefreshThread();
            } else if (e.Issue.IsSubtask && e.Issue.ParentKey.Equals(issue.Key)) {
                rebuildSubtasksPanel();
            }
        }

        private void rebuildAllPanels(bool enableRefresh) {
            Invoke(new MethodInvoker(delegate
                                         {
                                             rebuildSummaryPanel();
                                             rebuildDescriptionPanel();
                                             rebuildCommentsPanel(true);
                                             rebuildSubtasksPanel();
                                             rebuildAttachmentsPanel();
                                             buttonRefresh.Enabled = enableRefresh;
                                         }));
        }

        private void runRefreshThread() {
            Thread worker = new Thread(new ThreadStart(delegate {
                                                           try {
                                                               status.setInfo("Retrieving issue details...");
                                                               issue = facade.getIssue(issue.Server, issue.Key);
                                                               status.setInfo("Issue details retrieved");
                                                               Invoke(new MethodInvoker(() => model.updateIssue(issue)));
                                                           }
                                                           catch (Exception e) {
                                                               status.setError("Failed to retrieve issue details", e);
                                                           }
                                                           rebuildAllPanels(true);
                                                       }));
            worker.Start();
        }

        private string createCommentsHtml(bool expanded) {
            StringBuilder sb = new StringBuilder();

            sb.Append("<html>\n<head>\n")
                .Append(Resources.comments_css)
                .Append(Resources.toggler_javascript)
                .Append("</head>\n<body>\n");

            for (int i = 0; i < issue.Comments.Count; ++i) {
                sb.Append("<div class=\"comment_header\">")
                    .Append("<div class=\"author\">").Append(
                    JiraServerCache.Instance.getUsers(issue.Server).getUser(issue.Comments[i].Author))
                    .Append(" <span class=\"date\">").Append(
                    JiraIssueUtils.getTimeStringFromIssueDateTime(JiraIssueUtils.getDateTimeFromJiraTimeString(issue.Comments[i].Created)))
                    .Append("</span></div>")
                    .Append("<a href=\"javascript:toggle('")
                    .Append(i).Append("', '").Append(i).Append("control');\"><div class=\"toggler\" id=\"")
                    .Append(i).Append("control\">").Append(expanded ? "collapse" : "expand").Append("</div></a></div>\n");

                sb.Append("<div class=\"comment_body\" style=\"display:")
                    .Append(expanded ? "block" : "none").Append(";\" id=\"").Append(i).Append("\">")
                    .Append(createHyperlinedStackTrace(issue.Comments[i].Body)).Append("</div>\n");
            }

            sb.Append("</body></html>");

            return sb.ToString();
        }

        private static readonly Regex STACK_REGEX = new Regex(@"(\s*\w+\S+\(.*\)\s+\w+\s+)(\S+)(:\w+\s+)(\d+)");
        
        private const string ISSUE_EDIT_URL_TYPE = "issueedit:";
        private const string PARENT_ISSUE_URL_TYPE = "parentissue:";
        private const string SUBTASK_ISSUE_URL_TYPE = "subtaskkey:";

        private const string STACKLINE_URL_TYPE = "stackline:";
        private const string STACKLINE_LINE_NUMBER_SEPARATOR = "@";

        private static string createHyperlinedStackTrace(string body) {
            StringReader sr = new StringReader(body);

            StringBuilder result = new StringBuilder();

            string line = sr.ReadLine();
            while (line != null) {
                if (STACK_REGEX.IsMatch(line)) {
                    line = STACK_REGEX.Replace(line,
                                               "$1<a href=\"" + STACKLINE_URL_TYPE + "$2" +
                                               STACKLINE_LINE_NUMBER_SEPARATOR + "$4\">$2$3$4</a>");
                }
                result.Append(line).Append('\n');
                line = sr.ReadLine();
            }

            return result.ToString();
        }

        private string createDescriptionHtml() {
            StringBuilder sb = new StringBuilder();

            sb.Append("<html>\n<head>\n").Append(Resources.summary_and_description_css)
                .Append("\n</head>\n<body class=\"description\">\n")
                .Append(createHyperlinedStackTrace(issue.Description))
                .Append("\n</body>\n</html>\n");

            return sb.ToString();
        }

        private string createSummaryHtml() {
            StringBuilder sb = new StringBuilder();

            sb.Append("<html>\n<head>\n").Append(Resources.summary_and_description_css)
                .Append("\n</head>\n<body>\n<table class=\"summary\">\n")
                .Append("<tr><td class=\"labelcolumn labelsummary\">Summary</td><td class=\"labelsummary\">")
                .Append(issue.Summary).Append("</td></tr>\n")
                .Append("<tr><td class=\"labelcolumn\">Type</td><td>")
                .Append("<img alt=\"\" src=\"").Append(issue.IssueTypeIconUrl).Append("\"/>").Append(issue.IssueType).Append("</td></tr>\n")
                .Append("<tr><td class=\"labelcolumn\">Status</td><td>")
                .Append("<img alt=\"\" src=\"").Append(issue.StatusIconUrl).Append("\"/>").Append(issue.Status).Append("</td></tr>\n");

            if (issue.IsSubtask) {
                sb.Append("<tr><td class=\"labelcolumn\">Parent Issue</td><td>")
                    .Append("<a href=\"").Append(PARENT_ISSUE_URL_TYPE).Append(issue.ParentKey).Append("\">")
                    .Append(issue.ParentKey).Append("</a></td>");
            }
            
            sb.Append("<tr><td class=\"labelcolumn\">Priority</td><td>")
                .Append("<img alt=\"\" src=\"").Append(issue.PriorityIconUrl).Append("\"/>").Append(issue.Priority).Append("</td></tr>\n")
                .Append("<tr><td class=\"labelcolumn\">Assignee</td><td>")
                
                .Append(JiraServerCache.Instance.getUsers(issue.Server).getUser(issue.Assignee))
                .Append(" <a href=\"").Append(ISSUE_EDIT_URL_TYPE).Append("assignee\"><img src=\"")
                .Append(editImagePath).Append("\" alt=\"Edit\"  style=\"border-style: none\"></a>")
                .Append("</td></tr>\n")
                
                .Append("<tr><td class=\"labelcolumn\">Reporter</td><td>")
                .Append(JiraServerCache.Instance.getUsers(issue.Server).getUser(issue.Reporter)).Append("</td></tr>\n")
                .Append("<tr><td class=\"labelcolumn\">Resolution</td><td>")
                .Append(issue.Resolution).Append("</td></tr>\n")
                .Append("<tr><td class=\"labelcolumn\">Created</td><td>")
                .Append(JiraIssueUtils.getTimeStringFromIssueDateTime(issue.CreationDate)).Append("</td></tr>\n")
                .Append("<tr><td class=\"labelcolumn\">Updated</td><td>")
                .Append(JiraIssueUtils.getTimeStringFromIssueDateTime(issue.UpdateDate)).Append("</td></tr>\n");

            if (issue.Versions.Count > 1)
                sb.Append("<tr><td class=\"labelcolumn\">Affects Versions</td><td>");
            else
                sb.Append("<tr><td class=\"labelcolumn\">Affects Version</td><td>");

            if (issue.Versions.Count == 0)
                sb.Append("None").Append("</td></tr>\n");
            else {
                int i = 0;
                foreach (string v in issue.Versions) {
                    sb.Append(v);
                    if (++i < issue.Versions.Count)
                        sb.Append(", ");
                }
                sb.Append("</td></tr>\n");
            }

            if (issue.FixVersions.Count > 1)
                sb.Append("<tr><td class=\"labelcolumn\">Fix Versions</td><td>");
            else
                sb.Append("<tr><td class=\"labelcolumn\">Fix Version</td><td>");

            if (issue.FixVersions.Count == 0)
                sb.Append("None").Append("</td></tr>\n");
            else {
                int i = 0;
                foreach (string v in issue.FixVersions) {
                    sb.Append(v);
                    if (++i < issue.FixVersions.Count)
                        sb.Append(", ");
                }
                sb.Append("</td></tr>\n");
            }

            if (issue.Components.Count > 1)
                sb.Append("<tr><td class=\"labelcolumn\">Components</td><td>");
            else
                sb.Append("<tr><td class=\"labelcolumn\">Component</td><td>");

            if (issue.Components.Count == 0)
                sb.Append("None").Append("</td></tr>\n");
            else {
                int i = 0;
                foreach (string c in issue.Components) {
                    sb.Append(c);
                    if (++i < issue.Components.Count)
                        sb.Append(", ");
                }
                sb.Append("</td></tr>\n");
            }

            sb.Append("<tr><td class=\"labelcolumn\">Original Estimate</td><td>")
                .Append(issue.OriginalEstimate ?? "None").Append("</td></tr>\n")
                .Append("<tr><td class=\"labelcolumn\">Remaining Estimate</td><td>")
                .Append(issue.RemainingEstimate ?? "None").Append("</td></tr>\n")
                .Append("<tr><td class=\"labelcolumn\">Time Spent</td><td>")
                .Append(issue.TimeSpent ?? "None").Append("</td></tr>\n")
                .Append("\n</table>\n</body>\n</html>\n");

            return sb.ToString();
        }

        private void rebuildDescriptionPanel() {
            issueDescriptionLoaded = false;
            issueDescription.DocumentText = createDescriptionHtml();
        }

        private void rebuildSummaryPanel() {
            issueSummaryLoaded = false;
            issueSummary.DocumentText = createSummaryHtml();
        }

        private void rebuildCommentsPanel(bool expanded) {
            issueCommentsLoaded = false;
            issueComments.DocumentText = createCommentsHtml(expanded);
        }

        private void rebuildAttachmentsPanel() {
            listViewAttachments.Items.Clear();
            if (issue.HasAttachments) {
                if (!issueTabs.TabPages.Contains(tabAttachments)) {
                    issueTabs.TabPages.Add(tabAttachments);
                }
                foreach (JiraAttachment att in issue.Attachments) {
                    listViewAttachments.Items.Add(new JiraAttachmentListViewItem(issue, att));
                }
            } else {
                if (issueTabs.TabPages.Contains(tabAttachments)) {
                    issueTabs.TabPages.Remove(tabAttachments);
                }
            }
        }

        private void rebuildSubtasksPanel() {
            if (issue.HasSubtasks) {
                if (!issueTabs.TabPages.Contains(tabSubtasks)) {
                    issueTabs.TabPages.Add(tabSubtasks);
                }

                issueSubtasksLoaded = false;

                List<JiraIssue> subsInModel = new List<JiraIssue>();
                List<string> subsToQuery = new List<string>();
                foreach (string key in issue.SubtaskKeys) {
                    JiraIssue sub = model.getIssue(key, issue.Server);
                    if (sub != null) {
                        subsInModel.Add(sub);
                    } else {
                        subsToQuery.Add(key);
                    }
                }
                Thread t = new Thread(() => querySubtasksAndDisplay(subsInModel, subsToQuery));
                t.Start();
            } else {
                if (issueTabs.TabPages.Contains(tabSubtasks)) {
                    issueTabs.TabPages.Remove(tabSubtasks);
                }
            }
        }

        private void setWebSubtasksText(string txt) {
            webSubtasks.DocumentText =
                "<html>\n<head>\n"
                + Resources.summary_and_description_css
                + "\n</head>\n<body class=\"summary\">\n"
                + txt
                + "\n</body>\n</html>\n";
        }

        private void querySubtasksAndDisplay(ICollection<JiraIssue> subsInModel, IEnumerable<string> subsToQuery) {
            StringBuilder sb = new StringBuilder();

            try {
                foreach (string key in subsToQuery) {
                    JiraIssue sub = facade.getIssue(issue.Server, key);
                    subsInModel.Add(sub);
                }
                Invoke(new MethodInvoker(delegate {
                    sb.Append("<html>\n<head>\n").Append(Resources.summary_and_description_css)
                        .Append("\n</head>\n<body>\n<table class=\"summary\">\n");
                    foreach (JiraIssue sub in subsInModel) {
                        // let's pray all issue icons are 16x16 :)
                        sb.Append("<tr><td width=\"16px\">").Append("<img src=\"" + sub.IssueTypeIconUrl + "\"/>").Append("</td>");
                        sb.Append("<td>").Append("<a href=\"").Append(SUBTASK_ISSUE_URL_TYPE).Append(sub.Key).Append("\">").Append(sub.Key).Append("</a></td>");
                        sb.Append("<td width=\"16px\">").Append("<img src=\"" + sub.PriorityIconUrl + "\" alt=\"" + sub.Priority + "\"/>").Append("</td>");
                        sb.Append("<td width=\"16px\">").Append("<img src=\"" + sub.StatusIconUrl + "\" alt=\"" + sub.Status + "\"/>").Append("</td>");
                        sb.Append("<td>").Append(sub.Summary).Append("</td></tr>\n");
                    }
                    sb.Append("\n</table>\n</body>\n</html>\n");
                    webSubtasks.DocumentText = sb.ToString();
                }));
            } catch (Exception e) {
                try {
                    Invoke(new MethodInvoker(() => setWebSubtasksText("Failed to retrieve subtasks")));
                } catch (InvalidOperationException ex) {
                    Debug.WriteLine("IssueDetailsPanel.querySubtasksAndDisplay(): " + ex.Message);
                }
                status.setError("Failed to retrieve subtasks", e);
            }                
        }

        private void buttonAddComment_Click(object sender, EventArgs e) {
            NewIssueComment dlg = new NewIssueComment();
            dlg.ShowDialog();
            if (dlg.DialogResult != DialogResult.OK) return;

            Thread addCommentThread = new Thread(new ThreadStart(delegate {
                                                                     try {
                                                                         status.setInfo("Adding comment to issue...");
                                                                         facade.addComment(issue, dlg.CommentBody);
                                                                         status.setInfo("Comment added, refreshing view...");
                                                                         UsageCollector.Instance.bumpJiraIssuesOpen();
                                                                         runRefreshThread();
                                                                     }
                                                                     catch (Exception ex) {
                                                                         status.setError("Adding comment failed", ex);
                                                                     }
                                                                 }));
            addCommentThread.Start();
        }

        private void buttonExpandAll_Click(object sender, EventArgs e) {
            rebuildCommentsPanel(true);
        }

        private void buttonCollapseAll_Click(object sender, EventArgs e) {
            rebuildCommentsPanel(false);
        }

        private void buttonRefresh_Click(object sender, EventArgs e) {
            buttonRefresh.Enabled = false;
            runRefreshThread();
        }

        private void buttonClose_Click(object sender, EventArgs e) {
            removeModelListeners();

            tabWindow.TabPages.Remove(myTab);
            if (tabWindow.TabPages.Count == 0) {
                IssueDetailsWindow.Instance.FrameVisible = false;
            }
        }

        private void issueSummary_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e) {
            issueSummaryLoaded = true;
        }

        private void issueDescription_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e) {
            issueDescriptionLoaded = true;
        }

        private void issueComments_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e) {
            issueCommentsLoaded = true;
// ReSharper disable PossibleNullReferenceException
            issueComments.Document.Body.ScrollTop = A_LOT;
// ReSharper restore PossibleNullReferenceException
        }

        private void webSubtasks_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e) {
            issueSubtasksLoaded = true;
        }

        private void issueComments_Navigating(object sender, WebBrowserNavigatingEventArgs e) {
            if (!issueCommentsLoaded) return;
            if (e.Url.ToString().StartsWith("javascript:toggle(")) return;
            if (handleStackUrl(e)) return;
            navigate(e);
        }

        private bool handleStackUrl(WebBrowserNavigatingEventArgs e) {
            string line = e.Url.ToString();

            if (line.StartsWith(STACKLINE_URL_TYPE) && line.LastIndexOf(STACKLINE_LINE_NUMBER_SEPARATOR) != -1) {
                List<ProjectItem> files = new List<ProjectItem>();

                string file = line.Substring(STACKLINE_URL_TYPE.Length,
                                             line.LastIndexOf(STACKLINE_LINE_NUMBER_SEPARATOR) -
                                             STACKLINE_URL_TYPE.Length);

                foreach (Project project in solution.Projects) {
                    matchProjectItemChildren(file, files, project.ProjectItems);
                }
                if (files.Count == 0) {
                    MessageBox.Show("No matching files found for " + file, "Error");
                    Debug.WriteLine("No matching files found for " + file);
                } else if (files.Count > 1) {
                    MessageBox.Show("Multiple matching files found for " + file, "Error");
                    Debug.WriteLine("Multiple matching files found for " + file);
                } else {
                    string lineNoStr = line.Substring(line.LastIndexOf(STACKLINE_LINE_NUMBER_SEPARATOR) + 1);
                    try {
                        int lineNo = int.Parse(lineNoStr);
                        if (lineNo < 0) {
                            throw new ArgumentException();
                        }
                        Debug.WriteLine("opening file " + file + " at line number " + lineNo);

                        Window w = files[0].Open(Constants.vsViewKindCode);
                        w.Visible = true;
                        w.Document.Activate();
                        TextSelection sel = w.DTE.ActiveDocument.Selection as TextSelection;
                        if (sel != null) {
                            sel.SelectAll();
                            sel.MoveToLineAndOffset(lineNo, 1, false);
                            sel.SelectLine();
                        } else {
                            throw new Exception("Cannot get text selection for the document");
                        }
                    } catch (Exception ex) {
                        MessageBox.Show("Unable to open the specified file: " + ex.Message, "Error");
                        Debug.WriteLine(ex);
                    }
                }

                e.Cancel = true;
                return true;
            }
            return false;
        }

        private static void matchProjectItemChildren(string file, ICollection<ProjectItem> files, ProjectItems items) {
            if (items == null) return;

            foreach (ProjectItem item in items) {
//                Debug.WriteLine(item.Name);
                if (file.EndsWith(item.Name)) {
//                    Debug.WriteLine("@@@matched against " + file);
                    files.Add(item);
                }
                matchProjectItemChildren(file, files, item.ProjectItems);
            }
        }

        private void issueDescription_Navigating(object sender, WebBrowserNavigatingEventArgs e) {
            if (!issueDescriptionLoaded) return;
            if (handleStackUrl(e)) return;
            navigate(e);
        }

        private void issueSummary_Navigating(object sender, WebBrowserNavigatingEventArgs e) {
            if (!issueSummaryLoaded) return;
            if (e.Url.ToString().StartsWith(PARENT_ISSUE_URL_TYPE)) {
                AtlassianPanel.Instance.Jira.findAndOpenIssue(e.Url.ToString().Substring(PARENT_ISSUE_URL_TYPE.Length), openParentOrSubtaskFinished);
                e.Cancel = true;
                return;
            }
            if (e.Url.ToString().StartsWith(ISSUE_EDIT_URL_TYPE)) {
                MessageBox.Show("Editing " + e.Url.ToString().Substring(ISSUE_EDIT_URL_TYPE.Length));
                e.Cancel = true;
                return;
            }
            navigate(e);
        }

        private void webSubtasks_Navigating(object sender, WebBrowserNavigatingEventArgs e) {
            if (e.Url.Equals("about:blank")) return;
            if (!issueSubtasksLoaded) return;
            if (e.Url.ToString().StartsWith(SUBTASK_ISSUE_URL_TYPE)) {
                AtlassianPanel.Instance.Jira.findAndOpenIssue(e.Url.ToString().Substring(SUBTASK_ISSUE_URL_TYPE.Length), openParentOrSubtaskFinished);
                e.Cancel = true;
                return;
            }
            navigate(e);
        }

        private static void openParentOrSubtaskFinished(bool success, string message) {
            if (!success) {
                MessageBox.Show(message, AtlassianConstants.ERROR_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void navigate(WebBrowserNavigatingEventArgs e) {
            string url = e.Url.ToString();
            Process.Start(url);
            e.Cancel = true;
        }

        private void buttonViewInBrowser_Click(object sender, EventArgs e) {
            try {
                Process.Start(issue.Server.Url + "/browse/" + issue.Key);
            } catch (Exception ex) {
                Debug.WriteLine("IssueDetailsPanel.buttonViewInBrowser_Click() - exception: " + ex);
            }
            UsageCollector.Instance.bumpJiraIssuesOpen();
        }

        private void dropDownIssueActions_DropDownOpened(object sender, EventArgs e) {
            dropDownIssueActions.DropDownItems.Clear();

            Thread loaderThread = new Thread(addIssueActionItems);
            loaderThread.Start();
        }

        private void addIssueActionItems() {
            List<JiraNamedEntity> actions = null;
            try {
                status.setInfo("Retring issue actions...");
                actions = JiraServerFacade.Instance.getActionsForIssue(issue);
                status.setInfo("Issue actions retrieved");
            } catch (Exception ex) {
                status.setError("Failed to retrieve issue actions", ex);
            }
            if (actions == null || actions.Count == 0) return;

            Invoke(new MethodInvoker(delegate {
                                         foreach (var action in actions) {
                                             var actionCopy = action;
                                             ToolStripMenuItem item = new ToolStripMenuItem(action.Name, null, 
                                                 new EventHandler(delegate
                                                                      {
                                                                          IssueActionRunner.runAction(this, actionCopy, model, issue, status);
                                                                      }));
                                             dropDownIssueActions.DropDownItems.Add(item);
                                         }
                                     }));
        }

        private void buttonLogWork_Click(object sender, EventArgs e) {
            new LogWork(this, model, facade, issue, status).ShowDialog();
        }

        private void listViewAttachments_Click(object sender, EventArgs e) {
            if (listViewAttachments.SelectedItems.Count == 0) return;

            JiraAttachmentListViewItem item = listViewAttachments.SelectedItems[0] as JiraAttachmentListViewItem;
            if (item == null) return;

            if (isInlineNavigable(item.Attachment.Name)) {
                webAttachmentView.Navigate(item.Url + "?" + JiraIssueUtils.getAuthString(issue.Server));
            } else {
                webAttachmentView.DocumentText = string.Format(Resources.attachment_download_html, 
                    Font.FontFamily.Name, item.Url + "?" + JiraIssueUtils.getAuthString(issue.Server));
            }
        }

        private void attachmentsMenuOpening(object sender, System.ComponentModel.CancelEventArgs e) {
            e.Cancel = !(listViewAttachments.SelectedItems.Count > 0);
        }

        private void saveAttachment(object sender, EventArgs e) {
            if (listViewAttachments.SelectedItems.Count == 0) return;

            JiraAttachmentListViewItem item = listViewAttachments.SelectedItems[0] as JiraAttachmentListViewItem;
            if (item == null) return;

            SaveFileDialog dlg = new SaveFileDialog {FileName = item.Attachment.Name};

            if (dlg.ShowDialog() != DialogResult.OK) return;

            saveAttachmentToStream(item, dlg.OpenFile());
        }

        private void saveAttachmentToStream(JiraAttachmentListViewItem item, Stream stream) {
            status.setInfo("Saving attachment \"" + item.Attachment.Name + "\"...");
            WebClient client = new WebClient();
            client.DownloadDataCompleted += ((sender, e) => downloadDataCompleted(item.Attachment.Name, e, stream));
            client.DownloadDataAsync(new Uri(item.Url + "?" + JiraIssueUtils.getAuthString(issue.Server)));
        }

        private void downloadDataCompleted(string name, DownloadDataCompletedEventArgs e, Stream stream) {
            if (e.Error != null) {
                status.setError("Failed to save attachment \"" + name + "\"", e.Error);
                return;
            }
            stream.Write(e.Result, 0, e.Result.Length);
            stream.Flush();
            stream.Close();

            status.setInfo("Attachment \"" + name + "\" saved");
        }

        private static bool isInlineNavigable(string name) {
            // hmm hmm, will these be typical files that are 
            // (1) openable by IE and 
            // (2) usually interesting for users to view?
            return name.EndsWith(".jpg") 
                || name.EndsWith(".png") 
                || name.EndsWith(".gif") 
                || name.EndsWith(".txt") 
                || name.EndsWith(".xml")
                || name.EndsWith(".log");
        }

        private void listViewAttachments_SizeChanged(object sender, EventArgs e) {
            columnName.Width = listViewAttachments.Width / 2;
            columnAuthor.Width = listViewAttachments.Width / 6;
            columnSize.Width = listViewAttachments.Width / 6;
            columnDate.Width = -2;
        }

        private void listViewAttachments_SelectedIndexChanged(object sender, EventArgs e) {
            saveAsToolStripMenuItem.Enabled = listViewAttachments.SelectedItems.Count > 0;
        }
    }
}