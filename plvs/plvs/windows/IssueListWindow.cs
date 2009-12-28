using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using Atlassian.plvs.api;
using Atlassian.plvs.autoupdate;
using Atlassian.plvs.dialogs;
using Atlassian.plvs.models;
using Atlassian.plvs.ui;
using Atlassian.plvs.ui.issuefilternodes;
using Atlassian.plvs.ui.issues;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using Atlassian.plvs.ui.issues.menus;
using Atlassian.plvs.ui.issues.treemodels;

namespace Atlassian.plvs.windows {
    public partial class IssueListWindow : UserControl {
        private readonly JiraServerFacade facade = JiraServerFacade.Instance;

        private TreeViewAdv issuesTree;

        private readonly JiraIssueListModelBuilder builder;

        private static readonly JiraIssueListModel MODEL = JiraIssueListModelImpl.Instance;

        private readonly JiraIssueListSearchingModel searchingModel = new JiraIssueListSearchingModel(MODEL);

        private readonly StatusLabel status;

        public static IssueListWindow Instance { get; private set; }

        public IssueListWindow() {
            InitializeComponent();
            setupGroupByCombo();

            status = new StatusLabel(statusStrip, jiraStatus);

            registerIssueModelListener();
            builder = new JiraIssueListModelBuilder(facade);

            filtersTree.setReloadIssuesCallback(reloadIssues);
            filtersTree.addToolTip(filtersTreeToolTip);

            buttonUpdate.Visible = false;

            Instance = this;
        }

        private void setupGroupByCombo() {
            foreach (JiraIssueGroupByComboItem.GroupBy groupBy in Enum.GetValues(typeof (JiraIssueGroupByComboItem.GroupBy))) {
                comboGroupBy.Items.Add(new JiraIssueGroupByComboItem(groupBy, searchingModel));
            }
            comboGroupBy.SelectedIndexChanged += comboGroupBy_SelectedIndexChanged;
        }

        private class BoldableNodeTextBox : NodeTextBox {
            protected override void OnDrawText(DrawEventArgs args) {
                args.Font = new Font(args.Font, FontStyle.Bold);
                base.OnDrawText(args);
            }

            protected override bool DrawTextMustBeFired(TreeNodeAdv node) {
                return !(node.Tag is IssueNode);
            }
        }

        private void registerIssueModelListener() {
            searchingModel.ModelChanged += searchingModel_ModelChanged;
        }

        private readonly TreeColumn colName = new TreeColumn();
        private readonly TreeColumn colStatus = new TreeColumn();
        private readonly TreeColumn colPriority = new TreeColumn();
        private readonly TreeColumn colUpdated = new TreeColumn();
        private readonly NodeIcon controlIcon = new NodeIcon();
        private readonly NodeTextBox controlName = new BoldableNodeTextBox();
        private readonly NodeTextBox controlStatusText = new NodeTextBox();
        private readonly NodeIcon controlStatusIcon = new NodeIcon();
        private readonly NodeIcon controlPriorityIcon = new NodeIcon();
        private readonly NodeTextBox controlUpdated = new NodeTextBox();
        private const string RETRIEVING_ISSUES_FAILED = "Retrieving issues failed";
        private const int MARGIN = 16;
        private const int STATUS_WIDTH = 150;
        private const int UPDATED_WIDTH = 150;
        private const int PRIORITY_WIDTH = 24;

        private void initIssuesTree() {
            if (issuesTree != null) {
                issueTreeContainer.ContentPanel.Controls.Remove(issuesTree);
            }

            issuesTree = new TreeViewAdv();

            issueTreeContainer.ContentPanel.Controls.Add(issuesTree);
            issuesTree.Dock = DockStyle.Fill;
            issuesTree.SelectionMode = TreeSelectionMode.Single;
            issuesTree.FullRowSelect = true;
            issuesTree.GridLineStyle = GridLineStyle.None;
            issuesTree.UseColumns = true;
            issuesTree.NodeMouseDoubleClick += issuesTree_NodeMouseDoubleClick;
            issuesTree.KeyPress += issuesTree_KeyPress;
            issuesTree.SelectionChanged += issuesTree_SelectionChanged;

            ToolStripMenuItem[] items = new[]
                                        {
                                            new ToolStripMenuItem("Open in IDE", Resources.open_in_ide,
                                                                  new EventHandler(openIssue)),
                                            new ToolStripMenuItem("View in Browser", Resources.view_in_browser,
                                                                  new EventHandler(browseIssue)),
                                            new ToolStripMenuItem("Edit in Browser", Resources.edit_in_browser,
                                                                  new EventHandler(browseEditIssue)),
                                        };
            IssueContextMenu strip = new IssueContextMenu(searchingModel, status, issuesTree, items);
            issuesTree.ContextMenuStrip = strip;
            colName.Header = "Summary";
            colStatus.Header = "Status";
            colPriority.Header = "P";
            colUpdated.Header = "Updated";

            int i = 0;
            controlIcon.ParentColumn = colName;
            controlIcon.DataPropertyName = "Icon";
            controlIcon.LeftMargin = i++;

            controlName.ParentColumn = colName;
            controlName.DataPropertyName = "Name";
            controlName.Trimming = StringTrimming.EllipsisCharacter;
            controlName.UseCompatibleTextRendering = true;
            controlName.LeftMargin = i++;

            controlPriorityIcon.ParentColumn = colPriority;
            controlPriorityIcon.DataPropertyName = "PriorityIcon";
            controlPriorityIcon.LeftMargin = i++;

            controlStatusIcon.ParentColumn = colStatus;
            controlStatusIcon.DataPropertyName = "StatusIcon";
            controlStatusIcon.LeftMargin = i++;

            controlStatusText.ParentColumn = colStatus;
            controlStatusText.DataPropertyName = "StatusText";
            controlStatusText.Trimming = StringTrimming.EllipsisCharacter;
            controlStatusText.UseCompatibleTextRendering = true;
            controlStatusText.LeftMargin = i++;

            controlUpdated.ParentColumn = colUpdated;
            controlUpdated.DataPropertyName = "Updated";
            controlUpdated.Trimming = StringTrimming.EllipsisCharacter;
            controlUpdated.UseCompatibleTextRendering = true;
            controlUpdated.TextAlign = HorizontalAlignment.Right;
            controlUpdated.LeftMargin = i;

            issuesTree.Columns.Add(colName);
            issuesTree.Columns.Add(colPriority);
            issuesTree.Columns.Add(colStatus);
            issuesTree.Columns.Add(colUpdated);

            issuesTree.NodeControls.Add(controlIcon);
            issuesTree.NodeControls.Add(controlName);
            issuesTree.NodeControls.Add(controlPriorityIcon);
            issuesTree.NodeControls.Add(controlStatusIcon);
            issuesTree.NodeControls.Add(controlStatusText);
            issuesTree.NodeControls.Add(controlUpdated);

            setSummaryColumnWidth();

            colPriority.TextAlign = HorizontalAlignment.Left;
            colPriority.Width = PRIORITY_WIDTH;
            colPriority.MinColumnWidth = PRIORITY_WIDTH;
            colPriority.MaxColumnWidth = PRIORITY_WIDTH;
            colUpdated.Width = UPDATED_WIDTH;
            colUpdated.MinColumnWidth = UPDATED_WIDTH;
            colUpdated.MaxColumnWidth = UPDATED_WIDTH;
            colStatus.Width = STATUS_WIDTH;
            colStatus.MinColumnWidth = STATUS_WIDTH;
            colStatus.MaxColumnWidth = STATUS_WIDTH;
            colName.TextAlign = HorizontalAlignment.Left;
            colPriority.TooltipText = "Priority";
            colStatus.TextAlign = HorizontalAlignment.Left;
            colPriority.TextAlign = HorizontalAlignment.Left;
            colUpdated.TextAlign = HorizontalAlignment.Right;

            jiraSplitter.Panel2.SizeChanged += issuesTree_SizeChanged;

            updateIssueListButtons();
        }

        private void comboGroupBy_SelectedIndexChanged(object sender, EventArgs e) {
            updateIssuesTreeModel();
            updateIssueListButtons();
            expandIssuesTree();
        }

        private void updateIssuesTreeModel() {
            AbstractIssueTreeModel oldIssueTreeModel = issuesTree.Model as AbstractIssueTreeModel;
            if (oldIssueTreeModel != null) {
                oldIssueTreeModel.shutdown();
            }

            AbstractIssueTreeModel issueTreeModel;

            if (filtersTree.RecentlyViewedSelected) {
                issueTreeModel = new FlatIssueTreeModel(searchingModel);
            } else {
                JiraIssueGroupByComboItem item = comboGroupBy.SelectedItem as JiraIssueGroupByComboItem;
                if (item == null) {
                    return;
                }
                issueTreeModel = item.TreeModel;
            }

            // just in case somebody reuses the old model object :)
            issueTreeModel.shutdown();
            issuesTree.Model = issueTreeModel;
            issueTreeModel.StructureChanged += issuesTree_StructureChanged;
            issueTreeModel.init();
        }

        private void issuesTree_SelectionChanged(object sender, EventArgs e) {
            Invoke(new MethodInvoker(updateIssueListButtons));
        }

        private void updateIssueListButtons() {
            bool issueSelected = (issuesTree.SelectedNode != null && issuesTree.SelectedNode.Tag is IssueNode);
            buttonViewInBrowser.Enabled = issueSelected;
            buttonEditInBrowser.Enabled = issueSelected;
            buttonOpen.Enabled = issueSelected;
            buttonRefresh.Enabled = filtersTree.FilterOrRecentlyViewedSelected;
            buttonSearch.Enabled = filtersTree.NodeWithServerSelected;

            bool groupingControlsEnabled = !(filtersTree.RecentlyViewedSelected);
            comboGroupBy.Enabled = groupingControlsEnabled;
            comboGroupBy.Visible = groupingControlsEnabled;
            labelGroupBy.Visible = groupingControlsEnabled;

            JiraIssueGroupByComboItem selected = comboGroupBy.SelectedItem as JiraIssueGroupByComboItem;
            Boolean notNone = selected != null && selected.By != JiraIssueGroupByComboItem.GroupBy.NONE;
            buttonExpandAll.Visible = groupingControlsEnabled && notNone;
            buttonExpandAll.Enabled = groupingControlsEnabled && notNone;
            buttonCollapseAll.Visible = groupingControlsEnabled && notNone;
            buttonCollapseAll.Enabled = groupingControlsEnabled && notNone;
            buttonEditFilter.Enabled = filtersTree.SelectedNode is JiraCustomFilterTreeNode;
            buttonRemoveFilter.Enabled = filtersTree.SelectedNode is JiraCustomFilterTreeNode;
            buttonAddFilter.Enabled = filtersTree.SelectedNode is JiraCustomFiltersGroupTreeNode;
        }

        private delegate void IssueAction(JiraIssue issue);

        private void runSelectedIssueAction(IssueAction action) {
            TreeNodeAdv node = issuesTree.SelectedNode;
            if (node == null || !(node.Tag is IssueNode)) return;
            action((node.Tag as IssueNode).Issue);
        }

        private void browseIssue(object sender, EventArgs e) {
            runSelectedIssueAction(browseSelectedIssue);
        }

        private static void browseSelectedIssue(JiraIssue issue) {
            Process.Start(issue.Server.Url + "/browse/" + issue.Key);
        }

        private void browseEditIssue(object sender, EventArgs e) {
            runSelectedIssueAction(browseEditSelectedIssue);
        }

        private static void browseEditSelectedIssue(JiraIssue issue) {
            Process.Start(issue.Server.Url + "/secure/EditIssue!default.jspa?id=" + issue.Id);
        }

        private void openIssue(object sender, EventArgs e) {
            runSelectedIssueAction(openSelectedIssue);
        }

        private void issuesTree_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar != (char) Keys.Enter) return;
            runSelectedIssueAction(openSelectedIssue);
        }

        private void issuesTree_NodeMouseDoubleClick(object sender, TreeNodeAdvMouseEventArgs e) {
            runSelectedIssueAction(openSelectedIssue);
        }

        private static void openSelectedIssue(JiraIssue issue) {
            IssueDetailsWindow.Instance.openIssue(issue);
        }

        private void setSummaryColumnWidth() {
            // todo: well, this is lame. figure out how to handle filling first column to occupy all space in a propper manner
            int summaryWidth = jiraSplitter.Panel2.Width
                               - PRIORITY_WIDTH - UPDATED_WIDTH - STATUS_WIDTH
                               - SystemInformation.VerticalScrollBarWidth - MARGIN;
            if (summaryWidth < 0) {
                summaryWidth = 4*PRIORITY_WIDTH;
            }
            colName.Width = summaryWidth;
//            colName.MinColumnWidth = summaryWidth;
//            colName.MaxColumnWidth = summaryWidth;
        }

        private void issuesTree_SizeChanged(object sender, EventArgs e) {
            setSummaryColumnWidth();
        }

        private void issuesTree_StructureChanged(object sender, TreePathEventArgs e) {
            expandIssuesTree();
        }

        private void reloadKnownJiraServers() {
            filtersTree.clear();
            searchingModel.clear(true);

            getMoreIssues.Visible = false;

            // copy to local list so that we can reuse in our threads
            List<JiraServer> servers = new List<JiraServer>(JiraServerModel.Instance.getAllServers());
            if (servers.Count == 0) {
                status.setInfo("No JIRA servers defined");
                return;
            }

            filtersTree.addServerNodes(servers);

            Thread metadataThread = new Thread(() => reloadKnownServersWorker(servers));
            metadataThread.Start();
        }

        private void reloadKnownServersWorker(IEnumerable<JiraServer> servers) {
            try {
                JiraServerCache.Instance.clearProjects();
                JiraServerCache.Instance.clearIssueTypes();
                JiraServerCache.Instance.clearStatuses();
                JiraServerCache.Instance.clearPriorities();
                JiraServerCache.Instance.clearResolutions();

                foreach (JiraServer server in servers) {
                    status.setInfo("[" + server.Name + "] Loading project definitions...");
                    List<JiraProject> projects = facade.getProjects(server);
                    foreach (JiraProject proj in projects) {
                        JiraServerCache.Instance.addProject(server, proj);
                    }

                    status.setInfo("[" + server.Name + "] Loading issue types...");
                    List<JiraNamedEntity> issueTypes = facade.getIssueTypes(server);
                    foreach (JiraNamedEntity type in issueTypes) {
                        JiraServerCache.Instance.addIssueType(server, type);
                        ImageCache.Instance.getImage(type.IconUrl);
                    }
                    List<JiraNamedEntity> subtaskIssueTypes = facade.getSubtaskIssueTypes(server);
                    foreach (JiraNamedEntity type in subtaskIssueTypes) {
                        JiraServerCache.Instance.addIssueType(server, type);
                        ImageCache.Instance.getImage(type.IconUrl);
                    }

                    status.setInfo("[" + server.Name + "] Loading issue priorities...");
                    List<JiraNamedEntity> priorities = facade.getPriorities(server);
                    foreach (JiraNamedEntity prio in priorities) {
                        JiraServerCache.Instance.addPriority(server, prio);
                        ImageCache.Instance.getImage(prio.IconUrl);
                    }

                    status.setInfo("[" + server.Name + "] Loading issue resolutions...");
                    List<JiraNamedEntity> resolutions = facade.getResolutions(server);
                    foreach (JiraNamedEntity res in resolutions) {
                        JiraServerCache.Instance.addResolution(server, res);
                    }

                    status.setInfo("[" + server.Name + "] Loading issue statuses...");
                    List<JiraNamedEntity> statuses = facade.getStatuses(server);
                    foreach (JiraNamedEntity s in statuses) {
                        JiraServerCache.Instance.addStatus(server, s);
                        ImageCache.Instance.getImage(s.IconUrl);
                    }

                    status.setInfo("[" + server.Name + "] Loading saved filters...");
                    List<JiraSavedFilter> filters = facade.getSavedFilters(server);
                    JiraServer jiraServer = server;
                    Invoke(new MethodInvoker(delegate {
                                                 filtersTree.addFilterGroupNodes(jiraServer);
                                                 filtersTree.addPresetFilterNodes(jiraServer);
                                                 filtersTree.addSavedFilterNodes(jiraServer, filters);
                                                 status.setInfo("Loaded saved filters for server " + jiraServer.Name);
                                                 filtersTree.addCustomFilterNodes(jiraServer);
                                             }));
                }
                Invoke(new MethodInvoker(delegate {
                                             filtersTree.addRecentlyViewedNode();
                                             filtersTree.ExpandAll();
                                             filtersTree.restoreLastSelectedFilterItem();
                                         }));
            }
            catch (Exception e) {
                status.setError("Failed to load server metadata", e);
            }
        }

        private void searchingModel_ModelChanged(object sender, EventArgs e) {
            Invoke(new MethodInvoker(delegate {
                                         if (!(filtersTree.FilterOrRecentlyViewedSelected)) return;
                                         status.setInfo("Loaded " + MODEL.Issues.Count + " issues");
                                         getMoreIssues.Visible = !(filtersTree.RecentlyViewedSelected) && MODEL.Issues.Count > 0 &&
                                                                 probablyHaveMoreIssues();
                                         updateIssueListButtons();
                                     }));
        }

        private static bool probablyHaveMoreIssues() {
            return MODEL.Issues.Count%GlobalSettings.JiraIssuesBatch == 0;
        }

        private void buttonProjectProperties_Click(object sender, EventArgs e) {
            ProjectConfiguration dialog = new ProjectConfiguration(JiraServerModel.Instance, facade);
            dialog.ShowDialog(this);
            if (dialog.SomethingChanged) {
                // todo: only do this for changed servers - add server model listeners
                reloadKnownJiraServers();
            }
        }

        private void buttonAbout_Click(object sender, EventArgs e) {
            new About().ShowDialog(this);
        }

        private void buttonRefreshAll_Click(object sender, EventArgs e) {
            comboFind.Text = "";
            reloadKnownJiraServers();
        }

        public void reinitialize() {
            searchingModel.reinit(MODEL);
            registerIssueModelListener();
            Invoke(new MethodInvoker(initIssuesTree));
            reloadKnownJiraServers();
            comboGroupBy.restoreSelectedIndex();
        }

        private void filtersTree_AfterSelect(object sender, TreeViewEventArgs e) {
            comboFind.Text = "";
            updateIssueListButtons();
            updateIssuesTreeModel();
            if (filtersTree.FilterOrRecentlyViewedSelected) {
                reloadIssues();
            } else {
                searchingModel.clear(true);
            }

            filtersTree.rememberLastSelectedFilterItem();
        }

        private void reloadIssues() {
            JiraSavedFilterTreeNode savedFilterNode;
            RecentlyOpenIssuesTreeNode recentIssuesNode;
            JiraCustomFilterTreeNode customFilterNode;
            JiraPresetFilterTreeNode presetFilterNode;

            filtersTree.getAndCastSelectedNode(out savedFilterNode, out recentIssuesNode, out customFilterNode, out presetFilterNode);

            Thread issueLoadThread = null;

            if (savedFilterNode != null)
                issueLoadThread = reloadIssuesWithSavedFilter(savedFilterNode);
            else if (customFilterNode != null && !customFilterNode.Filter.Empty)
                issueLoadThread = reloadIssuesWithCustomFilter(customFilterNode);
            else if (presetFilterNode != null)
                issueLoadThread = reloadIssuesWithPresetFilter(presetFilterNode);
            else if (recentIssuesNode != null)
                issueLoadThread = reloadIssuesWithRecentlyViewedIssues();

            loadIssuesInThread(issueLoadThread);
        }

        private void loadIssuesInThread(Thread issueLoadThread) {
            if (issueLoadThread == null) {
                searchingModel.clear(true);
                return;
            }

            status.setInfo("Loading issues...");
            getMoreIssues.Visible = false;
            issueLoadThread.Start();
        }

        private Thread reloadIssuesWithRecentlyViewedIssues() {
            return new Thread(new ThreadStart(delegate {
                                                  try {
                                                      builder.rebuildModelWithRecentlyViewedIssues(MODEL);
                                                  }
                                                  catch (Exception ex) {
                                                      status.setError(RETRIEVING_ISSUES_FAILED, ex);
                                                  }
                                              }));
        }

        private Thread reloadIssuesWithSavedFilter(JiraSavedFilterTreeNode node) {
            return new Thread(new ThreadStart(delegate {
                                                  try {
                                                      builder.rebuildModelWithSavedFilter(MODEL, node.Server, node.Filter);
                                                  }
                                                  catch (Exception ex) {
                                                      status.setError(RETRIEVING_ISSUES_FAILED, ex);
                                                  }
                                              }));
        }

        private Thread updateIssuesWithSavedFilter(JiraSavedFilterTreeNode node) {
            return new Thread(new ThreadStart(delegate {
                                                  try {
                                                      builder.updateModelWithSavedFilter(MODEL, node.Server, node.Filter);
                                                  }
                                                  catch (Exception ex) {
                                                      status.setError(RETRIEVING_ISSUES_FAILED, ex);
                                                  }
                                              }));
        }

        private Thread reloadIssuesWithPresetFilter(JiraPresetFilterTreeNode node) {
            return new Thread(new ThreadStart(delegate {
                                                  try {
                                                      builder.rebuildModelWithPresetFilter(MODEL, node.Server, node.Filter);
                                                  }
                                                  catch (Exception ex) {
                                                      status.setError(RETRIEVING_ISSUES_FAILED, ex);
                                                  }
                                              }));
        }

        private Thread updateIssuesWithPresetFilter(JiraPresetFilterTreeNode node) {
            return new Thread(new ThreadStart(delegate {
                                                  try {
                                                      builder.updateModelWithPresetFilter(MODEL, node.Server, node.Filter);
                                                  }
                                                  catch (Exception ex) {
                                                      status.setError(RETRIEVING_ISSUES_FAILED, ex);
                                                  }
                                              }));
        }

        private Thread reloadIssuesWithCustomFilter(JiraCustomFilterTreeNode node) {
            return new Thread(new ThreadStart(delegate {
                                                  try {
                                                      builder.rebuildModelWithCustomFilter(MODEL, node.Server, node.Filter);
                                                  }
                                                  catch (Exception ex) {
                                                      status.setError(RETRIEVING_ISSUES_FAILED, ex);
                                                  }
                                              }));
        }

        private Thread updateIssuesWithCustomFilter(JiraCustomFilterTreeNode node) {
            return new Thread(new ThreadStart(delegate {
                                                  try {
                                                      builder.updateModelWithCustomFilter(MODEL, node.Server, node.Filter);
                                                  }
                                                  catch (Exception ex) {
                                                      status.setError(RETRIEVING_ISSUES_FAILED, ex);
                                                  }
                                              }));
        }

        private void getMoreIssues_Click(object sender, EventArgs e) {

            JiraSavedFilterTreeNode savedFilterNode;
            RecentlyOpenIssuesTreeNode recentIssuesNode;
            JiraCustomFilterTreeNode customFilterNode;
            JiraPresetFilterTreeNode presetFilterNode;

            filtersTree.getAndCastSelectedNode(out savedFilterNode, out recentIssuesNode, out customFilterNode, out presetFilterNode);

            Thread issueLoadThread = null;

            if (savedFilterNode != null)
                issueLoadThread = updateIssuesWithSavedFilter(savedFilterNode);
            else if (customFilterNode != null && !customFilterNode.Filter.Empty)
                issueLoadThread = updateIssuesWithCustomFilter(customFilterNode);
            else if (presetFilterNode != null)
                issueLoadThread = updateIssuesWithPresetFilter(presetFilterNode);

            loadIssuesInThread(issueLoadThread);
        }

        private void buttonOpen_Click(object sender, EventArgs e) {
            runSelectedIssueAction(openSelectedIssue);
        }

        private void buttonViewInBrowser_Click(object sender, EventArgs e) {
            runSelectedIssueAction(browseSelectedIssue);
        }

        private void buttonEditInBrowser_Click(object sender, EventArgs e) {
            runSelectedIssueAction(browseEditSelectedIssue);
        }

        private void buttonRefresh_Click(object sender, EventArgs e) {
            comboFind.Text = "";
            reloadIssues();
        }

        private void buttonSearch_Click(object sender, EventArgs e) {
            TreeNodeWithServer node = filtersTree.SelectedNode as TreeNodeWithServer;
            if (node == null) return;
            SearchIssue dlg = new SearchIssue(node.Server, MODEL, status);
            dlg.ShowDialog(this);
        }

        private void buttonGlobalProperties_Click(object sender, EventArgs e) {
            GlobalSettings globals = new GlobalSettings();
            globals.ShowDialog();
        }

        public delegate void FindFinished(bool success, string message);

        public void findAndOpenIssue(string key, FindFinished onFinish) {
            JiraServer server = filtersTree.getCurrentlySelectedServer();
            if (server == null) {
                if (onFinish != null) {
                    onFinish(false, "No JIRA server selected");
                }
                return;
            }

            Thread runner = new Thread(() => finishAndOpenIssueWorker(key, server, onFinish));
            runner.Start();
        }

        private void finishAndOpenIssueWorker(string key, JiraServer server, FindFinished onFinish) {
            try {
                status.setInfo("Fetching issue " + key + "...");
                JiraIssue issue =
                    JiraServerFacade.Instance.getIssue(server, key);
                if (issue != null) {
                    status.setInfo("Issue " + key + " found");
                    Invoke(new MethodInvoker(delegate {
                                                 if (onFinish != null) {
                                                     onFinish(true, null);
                                                 }
                                                 IssueDetailsWindow.Instance.openIssue(issue);
                                             }));
                }
            }
            catch (Exception ex) {
                status.setError("Failed to find issue " + key, ex);
                Invoke(new MethodInvoker(delegate {
                                             string message = "Unable to find issue " +
                                                              key + " on server \"" +
                                                              server.Name + "\"\n\n" + ex.Message;
                                             if (onFinish != null) {
                                                 onFinish(false, message);
                                             }
                                         }));
            }
        }

        public void setAutoupdateAvailable(Autoupdate.UpdateAction action) {
            buttonUpdate.Click += delegate { action(); };
            Invoke(new MethodInvoker(delegate {
                                         buttonUpdate.Enabled = true;
                                         buttonUpdate.Visible = true;
                                     }));
        }

        public void setAutoupdateUnavailable(Exception exception) {
            buttonUpdate.Click += delegate { MessageBox.Show("Unable to retrieve autoupdate information:\n\n" + exception.Message); };
            Invoke(new MethodInvoker(delegate {
                                         buttonUpdate.Enabled = true;
                                         buttonUpdate.Image = Resources.update_unavailable;
                                         buttonUpdate.Visible = true;
                                     }));
        }

        private void buttonExpandAll_Click(object sender, EventArgs e) {
            expandIssuesTree();
        }

        private void buttonCollapseAll_Click(object sender, EventArgs e) {
            collapseIssuesTree();
        }

        private void expandIssuesTree() {
            if (issuesTree != null) {
                issuesTree.ExpandAll();
            }
        }

        private void collapseIssuesTree() {
            if (issuesTree != null) {
                issuesTree.CollapseAll();
            }
        }

        private void comboFind_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == (char) Keys.Enter) {
                addFindComboText(comboFind.Text);
            }
        }

        private void updateSearchingModel(string text) {
            searchingModel.Query = text;   
        }

        private void addFindComboText(string text) {
            foreach (var item in comboFind.Items) {
                if (item.ToString().Equals(text)) {
                    return;
                }
            }
            if (text.Length > 0) {
                comboFind.Items.Add(text);
            }
        }

        private void comboFind_SelectedIndexChanged(object sender, EventArgs e) {
            updateSearchingModel(comboFind.Text);
        }

        private void comboFind_TextChanged(object sender, EventArgs e) {
            updateSearchingModel(comboFind.Text);
        }

        private void buttonAddFilter_Click(object sender, EventArgs e) {
            filtersTree.addCustomFilter();
        }

        private void buttonRemoveFilter_Click(object sender, EventArgs e) {
            JiraCustomFilterTreeNode node = filtersTree.SelectedNode as JiraCustomFilterTreeNode;
            filtersTree.removeCustomFilter(node);
        }

        private void buttonEditFilter_Click(object sender, EventArgs e) {
            JiraCustomFilterTreeNode node = filtersTree.SelectedNode as JiraCustomFilterTreeNode;
            filtersTree.editCustomFilter(node);
        }

        public JiraServer getCurrentlySelectedServer() {
            return filtersTree.getCurrentlySelectedServer();
        }
    }
}