using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.autoupdate;
using Atlassian.plvs.dialogs;
using Atlassian.plvs.dialogs.jira;
using Atlassian.plvs.explorer;
using Atlassian.plvs.models;
using Atlassian.plvs.models.jira;
using Aga.Controls.Tree;
using Atlassian.plvs.store;
using Atlassian.plvs.ui.jira.issuefilternodes;
using Atlassian.plvs.ui.jira.issues;
using Atlassian.plvs.ui.jira.issues.treemodels;
using Atlassian.plvs.util;
using EnvDTE;
using Process=System.Diagnostics.Process;
using Thread=System.Threading.Thread;

namespace Atlassian.plvs.ui.jira {
    public partial class TabJira : UserControl, AddNewServerLink {

        private const string GROUP_SUBTASKS_UNDER_PARENT = "JiraIssueListGroupSubtasksUnderParent";

        private JiraIssueTree issuesTree;

        private readonly JiraIssueListModelBuilder builder;

        private static readonly JiraIssueListModel MODEL = JiraIssueListModelImpl.Instance;

        private readonly JiraIssueListSearchingModel searchingModel = new JiraIssueListSearchingModel(MODEL);

        private readonly StatusLabel status;

        private LinkLabel linkAddJiraServer;

        private JiraIssue lastSelectedIssue;

        private int currentGeneration;
        private bool metadataFetched;
        private JiraActiveIssueManager activeIssueManager;

        public TabJira() {
            InitializeComponent();
            setupGroupByCombo();

            status = new StatusLabel(statusStrip, jiraStatus);

            registerIssueModelListener();
            builder = new JiraIssueListModelBuilder(Facade);

            filtersTree.setReloadIssuesCallback(reloadIssues);
            filtersTree.addToolTip(filtersTreeToolTip);
            filtersTree.setModel(JiraServerModel.Instance);

            ParameterStore store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.SETTINGS);
            bool groupSubtasks = store.loadParameter(GROUP_SUBTASKS_UNDER_PARENT, 1) != 0;
            buttonGroupSubtasks.Checked = groupSubtasks;

            buttonServerExplorer.Visible = GlobalSettings.JiraServerExplorerEnabled;
            buttonServerExplorer.Enabled = GlobalSettings.JiraServerExplorerEnabled;

            GlobalSettings.SettingsChanged += globalSettingsSettingsChanged;

            initializeActiveIssueToolStrip();
        }

        private void initializeActiveIssueToolStrip() {
            statusStrip.Items.Clear();
            activeIssueManager = new JiraActiveIssueManager(statusStrip);
            statusStrip.Items.Add(jiraStatus);
            statusStrip.Items.Add(getMoreIssues);
        }

        public event EventHandler<EventArgs> AddNewServerLinkClicked;

        public JiraServerFacade Facade {
            get { return JiraServerFacade.Instance; }
        }

        private void setupGroupByCombo() {
            foreach (JiraIssueGroupByComboItem.GroupBy groupBy in Enum.GetValues(typeof (JiraIssueGroupByComboItem.GroupBy))) {
                comboGroupBy.Items.Add(new JiraIssueGroupByComboItem(groupBy, searchingModel, buttonGroupSubtasks));
            }
            comboGroupBy.SelectedIndexChanged += comboGroupBy_SelectedIndexChanged;
        }

        private void registerIssueModelListener() {
            searchingModel.ModelChanged += searchingModel_ModelChanged;
        }

        private const string RETRIEVING_ISSUES_FAILED = "Retrieving issues failed";

        private void initIssuesTree() {
            if (issuesTree != null) {
                issueTreeContainer.ContentPanel.Controls.Remove(issuesTree);
            }

            issuesTree = new JiraIssueTree(jiraSplitter.Panel2, status, searchingModel, filtersTree.ItemHeight, filtersTree.Font);

            ToolStripMenuItem copyToClipboardItem = new ToolStripMenuItem("Copy to Clipboard", Resources.ico_copytoclipboard)
                                                    {
                                                        DropDown = createCopyIssueMenuItems()
                                                    };

            ((ContextMenuStrip) copyToClipboardItem.DropDown).ShowImageMargin = false;
            ((ContextMenuStrip) copyToClipboardItem.DropDown).ShowCheckMargin = false;

            issuesTree.addContextMenu(new ToolStripItem[]
                                  {
                                      new ToolStripMenuItem("Open in IDE", Resources.ico_editinide,
                                                            new EventHandler(openIssue)),
                                      new ToolStripMenuItem("View in Browser", Resources.view_in_browser,
                                                            new EventHandler(browseIssue)),
                                      new ToolStripMenuItem("Edit in Browser", Resources.ico_editinbrowser,
                                                            new EventHandler(browseEditIssue)),
                                      copyToClipboardItem,
                                      new ToolStripSeparator(),
                                      new ToolStripMenuItem("Log Work", Resources.ico_logworkonissue,
                                                            new EventHandler(logWork))
                                  });

            issuesTree.NodeMouseDoubleClick += issuesTree_NodeMouseDoubleClick;
            issuesTree.KeyPress += issuesTree_KeyPress;
            issuesTree.SelectionChanged += issuesTree_SelectionChanged;

            issueTreeContainer.ContentPanel.Controls.Add(issuesTree);

            updateIssueListButtons();
        }

        private ContextMenuStrip createCopyIssueMenuItems() {
            ContextMenuStrip strip = new ContextMenuStrip();
            strip.Items.Add("phony");

            strip.Opened += (s, e) => addCopyMenuActions(strip);

            return strip;
        }

        private void addCopyMenuActions(ToolStrip strip) {
            strip.Items.Clear();
            ToolStripMenuItem[] items = new ToolStripMenuItem[4];
            items[0] = new CopyToClipboardMenuItem(issuesTree, CopyToClipboardMenuItem.CopyType.KEY);
            items[1] = new CopyToClipboardMenuItem(issuesTree, CopyToClipboardMenuItem.CopyType.SUMMARY);
            items[2] = new CopyToClipboardMenuItem(issuesTree, CopyToClipboardMenuItem.CopyType.URL);
            items[3] = new CopyToClipboardMenuItem(issuesTree, CopyToClipboardMenuItem.CopyType.KEY_AND_SUMMARY);
            strip.Items.AddRange(items);
        }

        private class CopyToClipboardMenuItem : ToolStripMenuItem {
            private readonly JiraIssueTree issuesTree;
            private readonly CopyType type;

            internal enum CopyType {
                KEY,
                SUMMARY,
                URL,
                KEY_AND_SUMMARY
            }

            public CopyToClipboardMenuItem(JiraIssueTree issuesTree, CopyType type) {
                this.issuesTree = issuesTree;
                this.type = type;

                Click += (s, e) => Clipboard.SetText(Text); 
            }

            public override string Text {
                get {
                    if (issuesTree == null) {
                        return base.Text;
                    }
                    TreeNodeAdv node = issuesTree.SelectedNode;
                    if (node != null && node.Tag is IssueNode) {
                        JiraIssue issue = (node.Tag as IssueNode).Issue;

                        switch (type) {
                            case CopyType.KEY:
                                return issue.Key;
                            case CopyType.SUMMARY:
                                return issue.Summary;
                            case CopyType.URL:
                                return issue.Server.Url + "/browse/" + issue.Key;
                            case CopyType.KEY_AND_SUMMARY:
                                return issue.Key + ": " + issue.Summary;
                        }
                    }
                    return "No issue selected";
                }
            }
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

            JiraIssueGroupByComboItem item = comboGroupBy.SelectedItem as JiraIssueGroupByComboItem;
            if (item == null) {
                return;
            }
            AbstractIssueTreeModel issueTreeModel = item.TreeModel;

            // just in case somebody reuses the old model object :)
            issueTreeModel.shutdown();
            issuesTree.Model = issueTreeModel;
            issueTreeModel.StructureChanged += issuesTree_StructureChanged;
            issueTreeModel.TreeAboutToChange += issueTreeModel_TreeAboutToChange;
            issueTreeModel.NodesChanged += issueTreeModel_NodesChanged;
            issueTreeModel.NodesInserted += issueTreeModel_NodesInserted;
            issueTreeModel.NodesRemoved += issueTreeModel_NodesRemoved;
            issueTreeModel.init();
        }

        private void updateIssueListButtons() {
            bool issueSelected = (issuesTree.SelectedNode != null && issuesTree.SelectedNode.Tag is IssueNode);
            buttonViewInBrowser.Enabled = issueSelected;
            buttonEditInBrowser.Enabled = issueSelected;
            buttonOpen.Enabled = issueSelected;
            buttonRefresh.Enabled = filtersTree.FilterOrRecentlyViewedSelected;
            buttonSearch.Enabled = CurrentlySelectedServerOrDefault != null;
            buttonCreate.Enabled = CurrentlySelectedServerOrDefault != null && metadataFetched;

            JiraIssueGroupByComboItem selected = comboGroupBy.SelectedItem as JiraIssueGroupByComboItem;
            Boolean notNone = selected != null && selected.By != JiraIssueGroupByComboItem.GroupBy.NONE;
            buttonExpandAll.Visible = notNone;
            buttonExpandAll.Enabled = notNone;
            buttonCollapseAll.Visible = notNone;
            buttonCollapseAll.Enabled = notNone;
            buttonEditFilter.Enabled = filtersTree.SelectedNode is JiraCustomFilterTreeNode;
            buttonRemoveFilter.Enabled = filtersTree.SelectedNode is JiraCustomFilterTreeNode;
            buttonAddFilter.Enabled = filtersTree.NodeWithServerSelected && metadataFetched;
            buttonServerExplorer.Enabled = filtersTree.NodeWithServerSelected;
        }

        private delegate void IssueAction(JiraIssue issue);

        private void runSelectedIssueAction(IssueAction action) {
            TreeNodeAdv node = issuesTree.SelectedNode;
            if (node == null || !(node.Tag is IssueNode)) {
                return;
            }
            action((node.Tag as IssueNode).Issue);
        }

        private void browseIssue(object sender, EventArgs e) {
            runSelectedIssueAction(browseSelectedIssue);
        }

        private static void browseSelectedIssue(JiraIssue issue) {
            try {
                Process.Start(issue.Server.Url + "/browse/" + issue.Key);
            } catch (Exception e) {
                Debug.WriteLine("TabJira.browseSelectedIssue() - exception: " + e.Message);
            }
            UsageCollector.Instance.bumpJiraIssuesOpen();
        }

        private void browseEditIssue(object sender, EventArgs e) {
            runSelectedIssueAction(browseEditSelectedIssue);
        }

        private void logWork(object sender, EventArgs e) {
            runSelectedIssueAction(logWorkOnSelectedIssue);
        }

        private static void browseEditSelectedIssue(JiraIssue issue) {
            try {
                Process.Start(issue.Server.Url + "/secure/EditIssue!default.jspa?id=" + issue.Id);
            } catch (Exception e) {
                Debug.WriteLine("TabJira.browseEditSelectedIssue() - exception: " + e.Message);
            }
            UsageCollector.Instance.bumpJiraIssuesOpen();
        }

        private void logWorkOnSelectedIssue(JiraIssue issue) {
            new LogWork(this, MODEL, Facade, issue, status).ShowDialog();
        }

        private void openIssue(object sender, EventArgs e) {
            runSelectedIssueAction(openSelectedIssue);
        }

        private void issuesTree_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar != (char) Keys.Enter) {
                return;
            }
            runSelectedIssueAction(openSelectedIssue);
        }

        private void issuesTree_NodeMouseDoubleClick(object sender, TreeNodeAdvMouseEventArgs e) {
            runSelectedIssueAction(openSelectedIssue);
        }

        private void issuesTree_SelectionChanged(object sender, EventArgs e) {
            Invoke(new MethodInvoker(updateIssueListButtons));
            invokeSelectedIssueChanged();
        }

        private static void openSelectedIssue(JiraIssue issue) {
            IssueDetailsWindow.Instance.openIssue(issue);
        }

        private void issuesTree_StructureChanged(object sender, TreePathEventArgs e) {
            expandIssuesTree();
            restoreSelectedIssue(lastSelectedIssue);
            invokeSelectedIssueChanged();
        }

        private void issueTreeModel_NodesInserted(object sender, TreeModelEventArgs e) {
            restoreSelectedIssue(lastSelectedIssue);
            invokeSelectedIssueChanged();
        }

        private void issueTreeModel_NodesChanged(object sender, TreeModelEventArgs e) {
            restoreSelectedIssue(lastSelectedIssue);
            invokeSelectedIssueChanged();
        }

        private void issueTreeModel_NodesRemoved(object sender, TreeModelEventArgs e) {
            restoreSelectedIssue(lastSelectedIssue);
            invokeSelectedIssueChanged();
        }

        void issueTreeModel_TreeAboutToChange(object sender, EventArgs e) {
            lastSelectedIssue = SelectedIssue;
        }

        public event EventHandler<SelectedIssueEventArgs> SelectedIssueChanged;

        private void invokeSelectedIssueChanged() {
            EventHandler<SelectedIssueEventArgs> handler = SelectedIssueChanged;
            if (handler == null) {
                return;
            }

            handler(this, new SelectedIssueEventArgs(SelectedIssue));
        }

        public JiraIssue SelectedIssue {
            get {
                bool issueSelected = (issuesTree.SelectedNode != null && issuesTree.SelectedNode.Tag is IssueNode);
                JiraIssue issue = null;
                if (issueSelected) {
                    issue = ((IssueNode) issuesTree.SelectedNode.Tag).Issue;
                }
                return issue;
            }
        }

        public void reloadKnownJiraServers() {

            if (linkAddJiraServer != null) {
                Controls.Remove(linkAddJiraServer);
            }

            if (JiraServerModel.Instance.getAllServers().Count == 0) {
                linkAddJiraServer = new LinkLabel
                                    {
                                        Dock = DockStyle.Fill,
                                        Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 238),
                                        Image = Resources.jira_blue_16_with_padding,
                                        Location = new Point(0, 0),
                                        Name = "linkAddJiraServers",
                                        Size = new Size(1120, 510),
                                        TabIndex = 0,
                                        TabStop = true,
                                        Text = "Add JIRA Servers",
                                        BackColor = Color.White,
                                        TextAlign = ContentAlignment.MiddleCenter
                                    };

                linkAddJiraServer.LinkClicked += linkAddJiraServers_LinkClicked;
                jiraContainer.Visible = false;
                Controls.Add(linkAddJiraServer);
            } else {

                jiraContainer.Visible = true;

                filtersTree.clear();
                searchingModel.clear(true);

                getMoreIssues.Visible = false;

                // copy to local list so that we can reuse in our threads
                List<JiraServer> servers = new List<JiraServer>(JiraServerModel.Instance.getAllEnabledServers());
                if (servers.Count == 0) {
                    status.setInfo("No JIRA servers enabled");
                    return;
                }

                filtersTree.addServerNodes(servers);

                ++currentGeneration;

                metadataFetched = false;
                Thread metadataThread = PlvsUtils.createThread(() => reloadKnownServersWorker(servers, currentGeneration));
                metadataThread.Start();
            }
        }

        private void linkAddJiraServers_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            if (AddNewServerLinkClicked != null) {
                AddNewServerLinkClicked(this, new EventArgs());
            }
        }

        private void reloadKnownServersWorker(IEnumerable<JiraServer> servers, int myGeneration) {

            try {
                JiraServerCache.Instance.clearProjects();
                JiraServerCache.Instance.clearIssueTypes();
                JiraServerCache.Instance.clearStatuses();
                JiraServerCache.Instance.clearPriorities();
                JiraServerCache.Instance.clearResolutions();

                foreach (JiraServer server in servers) {

                    status.setInfo("[" + server.Name + "] Loading project definitions...");
                    List<JiraProject> projects = Facade.getProjects(server);
                    foreach (JiraProject proj in projects) {
                        JiraServerCache.Instance.addProject(server, proj);
                    }

                    status.setInfo("[" + server.Name + "] Loading issue types...");
                    List<JiraNamedEntity> issueTypes = Facade.getIssueTypes(server);
                    foreach (JiraNamedEntity type in issueTypes) {
                        JiraServerCache.Instance.addIssueType(server, type);
                        ImageCache.Instance.getImage(server, type.IconUrl);
                    }
                    List<JiraNamedEntity> subtaskIssueTypes = Facade.getSubtaskIssueTypes(server);
                    foreach (JiraNamedEntity type in subtaskIssueTypes) {
                        JiraServerCache.Instance.addIssueType(server, type);
                        ImageCache.Instance.getImage(server, type.IconUrl);
                    }

                    status.setInfo("[" + server.Name + "] Loading issue priorities...");
                    List<JiraNamedEntity> priorities = Facade.getPriorities(server);
                    foreach (JiraNamedEntity prio in priorities) {
                        JiraServerCache.Instance.addPriority(server, prio);
                        ImageCache.Instance.getImage(server, prio.IconUrl);
                    }

                    status.setInfo("[" + server.Name + "] Loading issue resolutions...");
                    List<JiraNamedEntity> resolutions = Facade.getResolutions(server);
                    foreach (JiraNamedEntity res in resolutions) {
                        JiraServerCache.Instance.addResolution(server, res);
                    }

                    status.setInfo("[" + server.Name + "] Loading issue statuses...");
                    List<JiraNamedEntity> statuses = Facade.getStatuses(server);
                    foreach (JiraNamedEntity s in statuses) {
                        JiraServerCache.Instance.addStatus(server, s);
                        ImageCache.Instance.getImage(server, s.IconUrl);
                    }

                    status.setInfo("[" + server.Name + "] Loading saved filters...");
                    List<JiraSavedFilter> filters = Facade.getSavedFilters(server);
                    JiraServer jiraServer = server;
                    Invoke(new MethodInvoker(delegate
                                                 {
                                                     // PLVS-59
                                                     if (myGeneration != currentGeneration) {
                                                         return;
                                                     }
                                                     filtersTree.addFilterGroupNodes(jiraServer);
                                                     filtersTree.addPresetFilterNodes(jiraServer);
                                                     filtersTree.addSavedFilterNodes(jiraServer, filters);
                                                     status.setInfo("Loaded saved filters for server " + jiraServer.Name);
                                                     filtersTree.addCustomFilterNodes(jiraServer);
                                                 }));
                }
                Invoke(new MethodInvoker(delegate
                                             {
                                                 // PLVS-59
                                                 if (myGeneration != currentGeneration) {
                                                     return;
                                                 }
                                                 metadataFetched = true;
                                                 filtersTree.addRecentlyViewedNode();
                                                 filtersTree.ExpandAll();
                                                 filtersTree.restoreLastSelectedFilterItem();
                                                 updateIssueListButtons();

                                                 activeIssueManager.init();
                                             }));
            } catch (Exception e) {
                status.setError("Failed to load JIRA server information", e);
            }
        }

        private void searchingModel_ModelChanged(object sender, EventArgs e) {
            Invoke(new MethodInvoker(delegate
                                         {
                                             if (!(filtersTree.FilterOrRecentlyViewedSelected)) {
                                                 return;
                                             }
                                             status.setInfo("Loaded " + MODEL.Issues.Count + " issues");
                                             getMoreIssues.Visible = !(filtersTree.RecentlyViewedSelected) && MODEL.Issues.Count > 0 &&
                                                                     probablyHaveMoreIssues();
                                             updateIssueListButtons();
                                         }));
        }

        private void globalSettingsSettingsChanged(object sender, EventArgs e) {
            buttonServerExplorer.Visible = GlobalSettings.JiraServerExplorerEnabled;
            buttonServerExplorer.Enabled = GlobalSettings.JiraServerExplorerEnabled;
        }

        private static bool probablyHaveMoreIssues() {
            return MODEL.Issues.Count%GlobalSettings.JiraIssuesBatch == 0;
        }

        private void buttonRefreshAll_Click(object sender, EventArgs e) {
            comboFind.Text = "";
            reloadKnownJiraServers();
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

            if (SelectedServerChanged != null) {
                SelectedServerChanged(this, new EventArgs());
            }
        }

        public event EventHandler<EventArgs> SelectedServerChanged;

        private void restoreSelectedIssue(JiraIssue issue) {
            if (issue == null) {
                return;
            }

            IEnumerable<TreeNodeAdv> nodes = issuesTree.AllNodes;
            foreach (TreeNodeAdv node in nodes) {
                IssueNode tag = node.Tag as IssueNode;
                if (tag == null || !tag.Issue.Key.Equals(issue.Key) || !tag.Issue.Server.GUID.Equals(issue.Server.GUID)) {
                    continue;
                }

                issuesTree.SelectedNode = node;
                break;
            }
        }

        private void reloadIssues() {
            JiraSavedFilterTreeNode savedFilterNode;
            RecentlyOpenIssuesTreeNode recentIssuesNode;
            JiraCustomFilterTreeNode customFilterNode;
            JiraPresetFilterTreeNode presetFilterNode;

            filtersTree.getAndCastSelectedNode(out savedFilterNode, out recentIssuesNode, out customFilterNode, out presetFilterNode);

            Thread issueLoadThread = null;

            JiraIssue selectedIssue = SelectedIssue;

            if (savedFilterNode != null) {
                issueLoadThread = reloadIssuesWithSavedFilter(savedFilterNode, () => restoreSelectedIssue(selectedIssue));
            } else if (customFilterNode != null && !customFilterNode.Filter.Empty) {
                issueLoadThread = reloadIssuesWithCustomFilter(customFilterNode, () => restoreSelectedIssue(selectedIssue));
            } else if (presetFilterNode != null) {
                issueLoadThread = reloadIssuesWithPresetFilter(presetFilterNode, () => restoreSelectedIssue(selectedIssue));
            } else if (recentIssuesNode != null) {
                issueLoadThread = reloadIssuesWithRecentlyViewedIssues(() => restoreSelectedIssue(selectedIssue));
            }

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

        private Thread reloadIssuesWithRecentlyViewedIssues(Action reloadCompleted) {
            return PlvsUtils.createThread(delegate {
                                              try {
                                                  builder.rebuildModelWithRecentlyViewedIssues(MODEL);
                                              } catch (Exception ex) {
                                                  status.setError(RETRIEVING_ISSUES_FAILED, ex);
                                              } finally {
                                                  reloadCompleted();
                                              }
                                          });
        }

        private Thread reloadIssuesWithSavedFilter(JiraSavedFilterTreeNode node, Action reloadCompleted) {
            return PlvsUtils.createThread(delegate {
                                              try {
                                                  builder.rebuildModelWithSavedFilter(MODEL, node.Server, node.Filter);
                                              } catch (Exception ex) {
                                                  status.setError(RETRIEVING_ISSUES_FAILED, ex);
                                              } finally {
                                                  reloadCompleted();
                                              }
                                          });
        }

        private Thread updateIssuesWithSavedFilter(JiraSavedFilterTreeNode node, Action reloadCompleted) {
            return PlvsUtils.createThread(delegate {
                                              try {
                                                  builder.updateModelWithSavedFilter(MODEL, node.Server, node.Filter);
                                              } catch (Exception ex) {
                                                  status.setError(RETRIEVING_ISSUES_FAILED, ex);
                                              } finally {
                                                  reloadCompleted();
                                              }
                                          });
        }

        private Thread reloadIssuesWithPresetFilter(JiraPresetFilterTreeNode node, Action reloadCompleted) {
            return PlvsUtils.createThread(delegate {
                                              try {
                                                  builder.rebuildModelWithPresetFilter(MODEL, node.Server, node.Filter);
                                              } catch (Exception ex) {
                                                  status.setError(RETRIEVING_ISSUES_FAILED, ex);
                                              } finally {
                                                  reloadCompleted();
                                              }
                                          });
        }

        private Thread updateIssuesWithPresetFilter(JiraPresetFilterTreeNode node, Action reloadCompleted) {
            return PlvsUtils.createThread(delegate {
                                              try {
                                                  builder.updateModelWithPresetFilter(MODEL, node.Server, node.Filter);
                                              } catch (Exception ex) {
                                                  status.setError(RETRIEVING_ISSUES_FAILED, ex);
                                              } finally {
                                                  reloadCompleted();
                                              }
                                          });
        }

        private Thread reloadIssuesWithCustomFilter(JiraCustomFilterTreeNode node, Action reloadCompleted) {
            return PlvsUtils.createThread(delegate {
                                              try {
                                                  builder.rebuildModelWithCustomFilter(MODEL, node.Server, node.Filter);
                                              } catch (Exception ex) {
                                                  status.setError(RETRIEVING_ISSUES_FAILED, ex);
                                              } finally {
                                                  reloadCompleted();
                                              }
                                          });
        }

        private Thread updateIssuesWithCustomFilter(JiraCustomFilterTreeNode node, Action reloadCompleted) {
            return PlvsUtils.createThread(delegate {
                                              try {
                                                  builder.updateModelWithCustomFilter(MODEL, node.Server, node.Filter);
                                              } catch (Exception ex) {
                                                  status.setError(RETRIEVING_ISSUES_FAILED, ex);
                                              } finally {
                                                  reloadCompleted();
                                              }
                                          });
        }

        private void getMoreIssues_Click(object sender, EventArgs e) {
            JiraSavedFilterTreeNode savedFilterNode;
            RecentlyOpenIssuesTreeNode recentIssuesNode;
            JiraCustomFilterTreeNode customFilterNode;
            JiraPresetFilterTreeNode presetFilterNode;

            filtersTree.getAndCastSelectedNode(out savedFilterNode, out recentIssuesNode, out customFilterNode, out presetFilterNode);

            Thread issueLoadThread = null;

            JiraIssue selectedIssue = SelectedIssue;

            if (savedFilterNode != null) {
                issueLoadThread = updateIssuesWithSavedFilter(savedFilterNode, () => restoreSelectedIssue(selectedIssue));
            } else if (customFilterNode != null && !customFilterNode.Filter.Empty) {
                issueLoadThread = updateIssuesWithCustomFilter(customFilterNode, () => restoreSelectedIssue(selectedIssue));
            } else if (presetFilterNode != null) {
                issueLoadThread = updateIssuesWithPresetFilter(presetFilterNode, () => restoreSelectedIssue(selectedIssue));
            }

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

        private void buttonCreate_Click(object sender, EventArgs e) {
            createIssue();
        }

        public void createIssue() {
            if (!metadataFetched) return;
            JiraServer server = CurrentlySelectedServerOrDefault;
            CreateIssue.createDialogOrBringToFront(server);
        }

        private void buttonSearch_Click(object sender, EventArgs e) {
            searchIssue();
        }

        public void searchIssue() {
            TreeNodeWithJiraServer node = filtersTree.SelectedNode as TreeNodeWithJiraServer;
            if (node == null) {
                return;
            }
            SearchIssue dlg = new SearchIssue(node.Server, MODEL, status);
            dlg.ShowDialog(this);
        }

        public delegate void FindFinished(bool success, string message, Exception e);

        public void findAndOpenIssue(string key, FindFinished onFinish) {
            JiraServer server = CurrentlySelectedServerOrDefault;
            if (server == null) {
                if (onFinish != null) {
                    onFinish(false, "No JIRA server selected", null);
                }
                return;
            }
            Thread runner = PlvsUtils.createThread(() => finishAndOpenIssueWorker(key, server, onFinish));
            runner.Start();
        }

        private void finishAndOpenIssueWorker(string key, JiraServer server, FindFinished onFinish) {
            try {
                status.setInfo("Fetching issue " + key + "...");
                JiraIssue issue =
                    JiraServerFacade.Instance.getIssue(server, key);
                if (issue != null) {
                    status.setInfo("Issue " + key + " found");
                    Invoke(new MethodInvoker(delegate
                                                 {
                                                     if (onFinish != null) {
                                                         onFinish(true, null, null);
                                                     }
                                                     IssueDetailsWindow.Instance.openIssue(issue);
                                                 }));
                }
            } catch (Exception ex) {
                status.setError("Failed to find issue " + key, ex);
                Invoke(new MethodInvoker(delegate
                                             {
                                                 string message = "Unable to find issue " + key + " on server \"" + server.Name;
                                                 if (onFinish != null) {
                                                     onFinish(false, message, ex);
                                                 }
                                             }));
            }
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

        public JiraServer CurrentlySelectedServerOrDefault {
            get {
                JiraServer server = filtersTree.CurrentlySelectedServer ?? JiraServerModel.Instance.DefaultServer;
                if (server == null && JiraServerModel.Instance.getAllEnabledServers().Count == 1) {
                    server = JiraServerModel.Instance.getAllEnabledServers().First();
                }
                return server;
            }
        }

        public class SelectedIssueEventArgs : EventArgs {
            public SelectedIssueEventArgs(JiraIssue issue) {
                Issue = issue;
            }

            public JiraIssue Issue { get; private set; }
        }

        public void reinitialize(DTE dte) {
            PlvsUtils.updateKeyBindingsInformation(dte, new Dictionary<string, ToolStripItem>
                                                        {
                                                            { "Tools.FindIssue", buttonSearch },
                                                            { "Tools.CreateIssue", buttonCreate }
                                                        });

            searchingModel.reinit(MODEL);
            registerIssueModelListener();
            Invoke(new MethodInvoker(initIssuesTree));
            reloadKnownJiraServers();
            comboGroupBy.restoreSelectedIndex();
        }

        public void shutdown() {}

        private void buttonGroupSubtasks_Click(object sender, EventArgs e) {
            ParameterStore store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.SETTINGS);
            store.storeParameter(GROUP_SUBTASKS_UNDER_PARENT, buttonGroupSubtasks.Checked ? 1 : 0);
        }

        private void buttonServerExplorer_Click(object sender, EventArgs e) {
            TreeNodeWithJiraServer node = filtersTree.SelectedNode as TreeNodeWithJiraServer;
            if (node != null) {
                JiraServerExplorer.showJiraServerExplorerFor(MODEL, node.Server, Facade);
            }
        }

        private void buttonHelp_Click(object sender, EventArgs e) {
            try {
                Process.Start("http://confluence.atlassian.com/display/IDEPLUGIN/Using+JIRA+in+the+Visual+Studio+Connector");
            } catch (Exception ex) {
                Debug.WriteLine("TabJira.buttonHelp_Click() - exception: " + ex.Message);
            }
        }
    }
}