using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.models;
using Atlassian.plvs.store;
using Atlassian.plvs.ui;
using Atlassian.plvs.windows;

namespace Atlassian.plvs.dialogs.jira {
    public partial class CreateIssue : Form {
        private readonly JiraServer server;

        private const string PROJECT = "createIssueDialog_selectedProject_";
        private const string ISSUE_TYPE = "createIssueDialog_selectedIssueType_";
        private const string PRIORITY = "createIssueDialog_selectedPriority_";
        private const string COMPS_SIZE = "createIssueDialog_selectedComponentsSize_";
        private const string COMPS_SEL = "createIssueDialog_selectedComponent_";
        private const string AFFECTS_SIZE = "createIssueDialog_selectedAffectsVersionsSize_";
        private const string AFFECTS_SEL = "createIssueDialog_selectedAffectsVersion_";
        private const string FIXES_SIZE = "createIssueDialog_selectedFixVersionsSize_";
        private const string FIXES_SEL = "createIssueDialog_selectedFixVersion_";

        private bool initialUpdate;

        private static CreateIssue instance;

        public static void createDialogOrBringToFront(JiraServer server) {
            if (instance == null) {
                instance = new CreateIssue(server);
                instance.Show();
            } else {
                instance.BringToFront();
            }
        }

        private CreateIssue(JiraServer server) {
            this.server = server;
            InitializeComponent();

            ParameterStore store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.SETTINGS);

            buttonCreate.Enabled = false;

            SortedDictionary<string, JiraProject> projects = JiraServerCache.Instance.getProjects(server);
            foreach (var project in projects.Values) {
                comboProjects.Items.Add(project);
            }

            List<JiraNamedEntity> priorities = JiraServerCache.Instance.getPriorities(server);
            ImageList imageList = new ImageList();
            int i = 0;
            foreach (var priority in priorities) {
                imageList.Images.Add(ImageCache.Instance.getImage(priority.IconUrl));
                comboPriorities.Items.Add(new ComboBoxWithImagesItem<JiraNamedEntity>(priority, i));
                ++i;
            }
            comboPriorities.ImageList = imageList;

            if (priorities.Count > 0) {
                int idx = store.loadParameter(PRIORITY + server.GUID, -1);
                if (idx != -1 && comboPriorities.Items.Count > idx) {
                    comboPriorities.SelectedIndex = idx;
                } else {
                    comboPriorities.SelectedIndex = priorities.Count/2;
                }
            }

            if (projects.Count > 0) {
                int idx = store.loadParameter(PROJECT + server.GUID, -1);
                if (idx != -1 && comboProjects.Items.Count > idx) {
                    initialUpdate = true;
                    comboProjects.SelectedIndex = idx;
                }
            }

            StartPosition = FormStartPosition.CenterParent;

            jiraAssigneePicker.init(server, null, true);
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            Close();
        }

        private void comboProjects_SelectedIndexChanged(object sender, EventArgs e) {
            setAllEnabled(false);
            startProjectUpdateThread();
        }

        private void startProjectUpdateThread() {
            JiraProject project = (JiraProject) comboProjects.SelectedItem;
            Thread t = new Thread(() => projectUpdateWorker(project));
            t.Start();
        }

        private void projectUpdateWorker(JiraProject project) {
            ParameterStore store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.SETTINGS);

            List<JiraNamedEntity> issueTypes =
                JiraServerFacade.Instance.getIssueTypes(server, project);
            List<JiraNamedEntity> comps = JiraServerFacade.Instance.getComponents(server, project);
            List<JiraNamedEntity> versions = JiraServerFacade.Instance.getVersions(server, project);
            // newest versions first
            versions.Reverse();
            try {
                Invoke(new MethodInvoker(delegate {
                                             fillIssueTypes(issueTypes, store);
                                             fillComponents(comps, store);
                                             fillVersions(versions, store);
                                             setAllEnabled(true);
                                             updateButtons();
                                             initialUpdate = false;
                                         }));
            } catch (InvalidOperationException e) {
                Debug.WriteLine("CreateIssue.projectUpdateWorker() - InvalidOperationException: " + e.Message);
            }
        }

        private void fillVersions(IEnumerable<JiraNamedEntity> versions, ParameterStore store) {
            listAffectsVersions.Items.Clear();
            listFixVersions.Items.Clear();
            foreach (var version in versions) {
                listAffectsVersions.Items.Add(version);
                listFixVersions.Items.Add(version);
            }
            if (!initialUpdate) {
                return;
            }
            int cnt = store.loadParameter(AFFECTS_SIZE + server.GUID, 0);
            if (cnt > 0) {
                for (int i = 0; i < cnt; ++i) {
                    int sel = store.loadParameter(AFFECTS_SEL + i + "_" + server.GUID, -1);
                    if (sel == -1) {
                        continue;
                    }
                    if (listAffectsVersions.Items.Count > sel) {
                        listAffectsVersions.SelectedIndices.Add(sel);
                    }
                }
            }
            cnt = store.loadParameter(FIXES_SIZE + server.GUID, 0);
            if (cnt > 0) {
                for (int i = 0; i < cnt; ++i) {
                    int sel = store.loadParameter(FIXES_SEL + i + "_" + server.GUID, -1);
                    if (sel == -1) {
                        continue;
                    }
                    if (listFixVersions.Items.Count > sel) {
                        listFixVersions.SelectedIndices.Add(sel);
                    }
                }
            }
        }

        private void fillComponents(IEnumerable<JiraNamedEntity> comps, ParameterStore store) {
            listComponents.Items.Clear();
            foreach (var comp in comps) {
                listComponents.Items.Add(comp);
            }
            if (!initialUpdate) {
                return;
            }
            int cnt = store.loadParameter(COMPS_SIZE + server.GUID, 0);
            if (cnt <= 0) {
                return;
            }
            for (int i = 0; i < cnt; ++i) {
                int sel = store.loadParameter(COMPS_SEL + i + "_" + server.GUID, -1);
                if (sel == -1) {
                    continue;
                }
                if (listComponents.Items.Count > sel) {
                    listComponents.SelectedIndices.Add(sel);
                }
            }
        }

        private void fillIssueTypes(ICollection<JiraNamedEntity> issueTypes, ParameterStore store) {
            ImageList imageList = new ImageList();
            comboTypes.Items.Clear();
            int i = 0;
            foreach (var type in issueTypes) {
                ComboBoxWithImagesItem<JiraNamedEntity> item = new ComboBoxWithImagesItem<JiraNamedEntity>(type, i);
                imageList.Images.Add(ImageCache.Instance.getImage(type.IconUrl));
                comboTypes.Items.Add(item);
                ++i;
            }
            comboTypes.ImageList = imageList;

            if (initialUpdate) {
                if (issueTypes.Count > 0) {
                    int idx = store.loadParameter(ISSUE_TYPE + server.GUID, -1);
                    if (idx != -1 && comboTypes.Items.Count > idx) {
                        comboTypes.SelectedIndex = idx;
                    }
                }
            }
        }

        private void setAllEnabled(bool enabled) {
            comboProjects.Enabled = enabled;
            comboTypes.Enabled = enabled;
            listAffectsVersions.Enabled = enabled;
            listFixVersions.Enabled = enabled;
            listComponents.Enabled = enabled;
            comboPriorities.Enabled = enabled;
            textSummary.Enabled = enabled;
            textDescription.Enabled = enabled;
            jiraAssigneePicker.Enabled = enabled;
            buttonCreate.Enabled = enabled;
        }

        private void updateButtons() {
            buttonCreate.Enabled =
                textSummary.Text.Length > 0
                && comboProjects.SelectedItem != null
                && comboTypes.SelectedItem != null
                && comboPriorities.SelectedItem != null;
        }

        private void textSummary_TextChanged(object sender, EventArgs e) {
            updateButtons();
        }

        private void comboTypes_SelectedIndexChanged(object sender, EventArgs e) {
            updateButtons();
        }

        private void buttonCreate_Click(object sender, EventArgs e) {
            saveSelectedValues();
            JiraIssue issue = createIssueTemplate();
            setAllEnabled(false);
            buttonCancel.Enabled = false;
            Thread t = new Thread(() => createIssueWorker(issue));
            t.Start();
        }

        private void saveSelectedValues() {
            ParameterStore store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.SETTINGS);
            store.storeParameter(PROJECT + server.GUID, comboProjects.SelectedIndex);
            store.storeParameter(ISSUE_TYPE + server.GUID, comboTypes.SelectedIndex);
            store.storeParameter(PRIORITY + server.GUID, comboPriorities.SelectedIndex);
            store.storeParameter(COMPS_SIZE + server.GUID, listComponents.SelectedIndices.Count);
            int i = 0;
            foreach (int index in listComponents.SelectedIndices) {
                store.storeParameter(COMPS_SEL + (i++) + "_" + server.GUID, index);
            }

            store.storeParameter(AFFECTS_SIZE + server.GUID, listAffectsVersions.SelectedIndices.Count);
            i = 0;
            foreach (int index in listAffectsVersions.SelectedIndices) {
                store.storeParameter(AFFECTS_SEL + (i++) + "_" + server.GUID, index);
            }
            store.storeParameter(FIXES_SIZE + server.GUID, listFixVersions.SelectedIndices.Count);
            i = 0;
            foreach (int index in listFixVersions.SelectedIndices) {
                store.storeParameter(FIXES_SEL + (i++) + "_" + server.GUID, index);
            }
        }

        private JiraIssue createIssueTemplate() {
            JiraIssue issue = new JiraIssue
                              {
                                  Summary = textSummary.Text,
                                  Description = textDescription.Text,
                                  ProjectKey = ((JiraProject) comboProjects.SelectedItem).Key,
                                  IssueTypeId =
                                      ((ComboBoxWithImagesItem<JiraNamedEntity>) comboTypes.SelectedItem).Value.Id,
                                  PriorityId =
                                      ((ComboBoxWithImagesItem<JiraNamedEntity>) comboPriorities.SelectedItem).Value
                                      .Id
                              };

            if (listComponents.SelectedItems.Count > 0) {
                List<string> comps = new List<string>();
                foreach (var comp in listComponents.SelectedItems) {
                    comps.Add(comp.ToString());
                }
                issue.Components = comps;
            }

            if (listAffectsVersions.SelectedItems.Count > 0) {
                List<string> affects = new List<string>();
                foreach (var ver in listAffectsVersions.SelectedItems) {
                    affects.Add(ver.ToString());
                }
                issue.Versions = affects;
            }

            if (listFixVersions.SelectedItems.Count > 0) {
                List<string> fixes = new List<string>();
                foreach (var fix in listFixVersions.SelectedItems) {
                    fixes.Add(fix.ToString());
                }
                issue.FixVersions = fixes;
            }

            string assignee = jiraAssigneePicker.Value;
            if (assignee.Length > 0) {
                issue.Assignee = assignee;
            }

            return issue;
        }

        private void createIssueWorker(JiraIssue issue) {
            try {
                string key = JiraServerFacade.Instance.createIssue(server, issue);
                Invoke(new MethodInvoker(delegate {
                                             setAllEnabled(true);
                                             buttonCancel.Enabled = true;
                                             Close();
                                             AtlassianPanel.Instance.Jira.findAndOpenIssue(key, null);
                                         }));
            }
            catch (Exception e) {
                Invoke(new MethodInvoker(delegate {
                                             MessageBox.Show("Unable to create issue: " + e.Message, "Error",
                                                             MessageBoxButtons.OK, MessageBoxIcon.Error);
                                             setAllEnabled(true);
                                             buttonCancel.Enabled = true;
                                         }));
            }
        }

        private void CreateIssue_KeyPress(object sender, KeyPressEventArgs e) {
            if (buttonCancel.Enabled && e.KeyChar == (char)Keys.Escape) {
                Close();
            }
        }

        protected override void OnClosed(EventArgs e) {
            instance = null;
        }
    }
}