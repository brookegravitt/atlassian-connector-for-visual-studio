using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using Atlassian.plvs.api;
using Atlassian.plvs.models;
using Atlassian.plvs.ui;
using Atlassian.plvs.windows;

namespace Atlassian.plvs.dialogs {
    public partial class CreateIssue : Form {
        private readonly JiraServer server;

        public CreateIssue(JiraServer server) {
            this.server = server;
            InitializeComponent();

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
                comboPriorities.SelectedIndex = priorities.Count/2;
            }
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
            List<JiraNamedEntity> issueTypes =
                JiraServerFacade.Instance.getIssueTypes(server, project);
            List<JiraNamedEntity> comps = JiraServerFacade.Instance.getComponents(server, project);
            List<JiraNamedEntity> versions = JiraServerFacade.Instance.getVersions(server, project);
            // newest versions first
            versions.Reverse();
            Invoke(new MethodInvoker(delegate {
                                         fillIssueTypes(issueTypes);
                                         fillComponents(comps);
                                         fillVersions(versions);
                                         setAllEnabled(true);
                                         updateButtons();
                                     }));
        }

        private void fillVersions(IEnumerable<JiraNamedEntity> versions) {
            listAffectsVersions.Items.Clear();
            listFixVersions.Items.Clear();
            foreach (var version in versions) {
                listAffectsVersions.Items.Add(version);
                listFixVersions.Items.Add(version);
            }
        }

        private void fillComponents(IEnumerable<JiraNamedEntity> comps) {
            listComponents.Items.Clear();
            foreach (var comp in comps) {
                listComponents.Items.Add(comp);
            }
        }

        private void fillIssueTypes(IEnumerable<JiraNamedEntity> issueTypes) {
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
            textAssignee.Enabled = enabled;
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
            JiraIssue issue = createIssueTemplate();
            setAllEnabled(false);
            buttonCancel.Enabled = false;
            Thread t = new Thread(() => createIssueWorker(issue));
            t.Start();
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
                                      ((ComboBoxWithImagesItem<JiraNamedEntity>) comboPriorities.SelectedItem).Value.Id
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

            if (textAssignee.Text.Length > 0) {
                issue.Assignee = textAssignee.Text;
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
                                             IssueListWindow.Instance.findAndOpenIssue(key, null);
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
    }
}