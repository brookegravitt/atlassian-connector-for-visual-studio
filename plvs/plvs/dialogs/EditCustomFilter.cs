using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using Atlassian.plvs.api;
using Atlassian.plvs.models;
using Atlassian.plvs.ui;

namespace Atlassian.plvs.dialogs {
    public partial class EditCustomFilter : Form {
        private readonly JiraServer server;
        private readonly JiraCustomFilter filter;
        
        private const string NAME_COLUMN = "Name";

        public bool Changed { get; private set; }

        public EditCustomFilter(JiraServer server, JiraCustomFilter filter) {

            this.server = server;
            this.filter = filter;

            InitializeComponent();

            listViewIssueTypes.Columns.Add(NAME_COLUMN, listViewIssueTypes.Width - 10, HorizontalAlignment.Left);
            listViewPriorities.Columns.Add(NAME_COLUMN, listViewPriorities.Width - 10, HorizontalAlignment.Left);
            listViewStatuses.Columns.Add(NAME_COLUMN, listViewStatuses.Width - 10, HorizontalAlignment.Left);

            StartPosition = FormStartPosition.CenterParent;

            SortedDictionary<string, JiraProject> projects = JiraServerCache.Instance.getProjects(server);
            SortedDictionary<int, JiraNamedEntity> statuses = JiraServerCache.Instance.getStatues(server);
            SortedDictionary<int, JiraNamedEntity> resolutions = JiraServerCache.Instance.getResolutions(server);
            SortedDictionary<int, JiraNamedEntity> priorities = JiraServerCache.Instance.getPriorities(server);

            refillProjects(projects);
            refillStatuses(statuses);
            refillResolutions(resolutions);
            refillPriorities(priorities);
            refillReporter();
            refillAssignee();

            refillIssueTypes(null);
            refillFixFor(null);
            refillComponents(null);
            refillAffectsVersions(null);

            manageSelections();

            listBoxProjects.SelectedValueChanged += listBoxProjects_SelectedValueChanged;
        }

        private void manageSelections() {
            foreach (JiraProject project in filter.Projects) {
                foreach (var item in listBoxProjects.Items) {
                    if (!project.Key.Equals(((JiraProject) item).Key)) continue;
                    listBoxProjects.SelectedItems.Add(item);
                    break;
                }
            }
            foreach (JiraNamedEntity priority in filter.Priorities) {
                foreach (ListViewItem item in listViewPriorities.Items) {
                    if (priority.Id != (((JiraNamedEntityListViewItem)item).Entity.Id)) continue;
                    item.Selected = true;
                    break;
                }
            }
            foreach (JiraNamedEntity status in filter.Statuses) {
                foreach (ListViewItem item in listViewStatuses.Items) {
                    if (status.Id != (((JiraNamedEntityListViewItem)item).Entity.Id)) continue;
                    item.Selected = true;
                    break;
                }
            }
            foreach (JiraNamedEntity resolution in filter.Resolutions) {
                foreach (var item in listBoxResolutions.Items) {
                    if (resolution.Id != (((JiraNamedEntity)item).Id)) continue;
                    listBoxResolutions.SelectedItems.Add(item);
                    break;
                }
            }

            comboBoxReporter.SelectedItem = comboBoxReporter.Items[0];
            foreach (var item in comboBoxReporter.Items) {
                if (!(item is UserTypeComboBoxItem)) continue;
                if ((item as UserTypeComboBoxItem).Type != filter.Reporter) continue;
                comboBoxReporter.SelectedItem = item;
                break;
            }

            comboBoxAssignee.SelectedItem = comboBoxAssignee.Items[0];
            foreach (var item in comboBoxAssignee.Items) {
                if (!(item is UserTypeComboBoxItem)) continue;
                if ((item as UserTypeComboBoxItem).Type != filter.Assignee) continue;
                comboBoxAssignee.SelectedItem = item;
                break;
            }

            setProjectRelatedValues(true);
        }

        private void refillProjects(SortedDictionary<string, JiraProject> projects) {
            listBoxProjects.Items.Clear();
            if (projects == null) return;
            foreach (string projectKey in projects.Keys) {
                listBoxProjects.Items.Add(projects[projectKey]);
            }
        }

        private void refillAssignee() {
            comboBoxAssignee.Items.Add(new UserTypeComboBoxItem(JiraCustomFilter.UserType.UNDEFINED));
            comboBoxAssignee.Items.Add(new UserTypeComboBoxItem(JiraCustomFilter.UserType.ANY));
            comboBoxAssignee.Items.Add(new UserTypeComboBoxItem(JiraCustomFilter.UserType.CURRENT));
            comboBoxAssignee.Items.Add(new UserTypeComboBoxItem(JiraCustomFilter.UserType.UNASSIGNED));
        }

        private void refillReporter() {
            comboBoxReporter.Items.Add(new UserTypeComboBoxItem(JiraCustomFilter.UserType.UNDEFINED));
            comboBoxReporter.Items.Add(new UserTypeComboBoxItem(JiraCustomFilter.UserType.ANY));
            comboBoxReporter.Items.Add(new UserTypeComboBoxItem(JiraCustomFilter.UserType.CURRENT));
        }

        private void refillPriorities(SortedDictionary<int, JiraNamedEntity> priorities) {
            listViewPriorities.Items.Clear();

            if (priorities == null) return;

            ImageList imageList = new ImageList();

            int i = 0;

            foreach (int key in priorities.Keys) {
                imageList.Images.Add(ImageCache.Instance.getImage(priorities[key].IconUrl));
                ListViewItem lvi = new JiraNamedEntityListViewItem(priorities[key], i);
                listViewPriorities.Items.Add(lvi);
                ++i;
            }
            listViewPriorities.SmallImageList = imageList;
        }

        private void refillStatuses(SortedDictionary<int, JiraNamedEntity> statuses) {
            listViewStatuses.Items.Clear();

            if (statuses == null) return;

            ImageList imageList = new ImageList();

            int i = 0;

            foreach (int key in statuses.Keys) {
                imageList.Images.Add(ImageCache.Instance.getImage(statuses[key].IconUrl));
                ListViewItem lvi = new JiraNamedEntityListViewItem(statuses[key], i);
                listViewStatuses.Items.Add(lvi);
                ++i;
            }
            listViewStatuses.SmallImageList = imageList;
        }

        private void refillResolutions(SortedDictionary<int, JiraNamedEntity> resolutions) {
            listBoxResolutions.Items.Clear();
            if (resolutions == null) return;
            foreach (int key in resolutions.Keys) {
                listBoxResolutions.Items.Add(resolutions[key]);
            }
        }

        private void refillIssueTypes(ICollection<JiraNamedEntity> issueTypes) {
            listViewIssueTypes.Items.Clear();

            ImageList imageList = new ImageList();

            int i = 0;

            if (issueTypes == null) {
                issueTypes = JiraServerCache.Instance.getIssueTypes(server).Values;
            }
            foreach (JiraNamedEntity issueType in issueTypes) {
                imageList.Images.Add(ImageCache.Instance.getImage(issueType.IconUrl));
                ListViewItem lvi = new JiraNamedEntityListViewItem(issueType, i);
                listViewIssueTypes.Items.Add(lvi);
                ++i;
            }
            listViewIssueTypes.SmallImageList = imageList;
        }

        private void refillFixFor(IEnumerable<JiraNamedEntity> fixFors) {
            listBoxFixForVersions.Items.Clear();

            if (fixFors == null)
                return;
            foreach (JiraNamedEntity fixFor in fixFors)
                listBoxFixForVersions.Items.Add(fixFor);
        }

        private void refillComponents(IEnumerable<JiraNamedEntity> comps) {
            listBoxComponents.Items.Clear();

            if (comps == null)
                return;
            foreach (JiraNamedEntity component in comps)
                listBoxComponents.Items.Add(component);
        }

        private void refillAffectsVersions(IEnumerable<JiraNamedEntity> versions) {
            listBoxAffectsVersions.Items.Clear();

            if (versions == null)
                return;
            foreach (JiraNamedEntity version in versions)
                listBoxAffectsVersions.Items.Add(version);
        }

        private void buttonClose_Click(object sender, EventArgs e) {
            Close();
        }

        private void listBoxProjects_SelectedValueChanged(object sender, EventArgs e) {
            setProjectRelatedValues(false);
        }

        private void setProjectRelatedValues(bool initial) {
            if (listBoxProjects.SelectedItems.Count == 1) {
                setAllEnabled(false);
                JiraProject project = listBoxProjects.SelectedItems[0] as JiraProject;

                Thread runner = new Thread(() => setProjectRelatedValuesRunner(project, initial));
                runner.Start();
            }
            else {
                refillIssueTypes(null);
                refillComponents(null);
                refillFixFor(null);
                refillAffectsVersions(null);

                if (initial) {
                    setProjectRelatedSelections();
                }
            }
        }

        private void setProjectRelatedValuesRunner(JiraProject project, bool initial) {
            try {
                List<JiraNamedEntity> issueTypes = JiraServerFacade.Instance.getIssueTypes(server, project);
                List<JiraNamedEntity> comps = JiraServerFacade.Instance.getComponents(server, project);
                List<JiraNamedEntity> versions = JiraServerFacade.Instance.getVersions(server, project);

                versions.Reverse();
                Invoke(new MethodInvoker(delegate {
                                             refillIssueTypes(issueTypes);
                                             refillComponents(comps);
                                             refillFixFor(versions);
                                             refillAffectsVersions(versions);

                                             if (initial)
                                                 setProjectRelatedSelections();

                                             setAllEnabled(true);
                                         }));
            }
            catch (Exception ex) {
                if (ex is InvalidOperationException) {
                    // probably the window got closed while we were fetching data
                    Debug.WriteLine(ex.Message);
                }
                else {
                    MessageBox.Show("Unable to retrieve project-related data: " + ex.Message, "Error");
                }
            }
        }

        private void setProjectRelatedSelections() {
            foreach (JiraNamedEntity issueType in filter.IssueTypes) {
                foreach (ListViewItem item in listViewIssueTypes.Items) {
                    if (issueType.Id != (((JiraNamedEntityListViewItem) item).Entity.Id)) continue;
                    item.Selected = true;
                    break;
                }
            }

            foreach (JiraNamedEntity fixFor in filter.FixForVersions) {
                foreach (var item in listBoxFixForVersions.Items) {
                    if (fixFor.Id != (((JiraNamedEntity) item).Id)) continue;
                    listBoxFixForVersions.SelectedItems.Add(item);
                    break;
                }
            }

            foreach (JiraNamedEntity comp in filter.Components) {
                foreach (var item in listBoxComponents.Items) {
                    if (comp.Id != (((JiraNamedEntity) item).Id)) continue;
                    listBoxComponents.SelectedItems.Add(item);
                    break;
                }
            }

            foreach (JiraNamedEntity affectVersion in filter.AffectsVersions) {
                foreach (var item in listBoxAffectsVersions.Items) {
                    if (affectVersion.Id != (((JiraNamedEntity) item).Id)) continue;
                    listBoxAffectsVersions.SelectedItems.Add(item);
                    break;
                }
            }
        }

        private void setAllEnabled(bool enabled) {
            listBoxProjects.Enabled = enabled;
            listViewIssueTypes.Enabled = enabled;
            listBoxFixForVersions.Enabled = enabled;
            listBoxComponents.Enabled = enabled;
            listBoxAffectsVersions.Enabled = enabled;
            comboBoxReporter.Enabled = enabled;
            comboBoxAssignee.Enabled = enabled;
            listViewStatuses.Enabled = enabled;
            listBoxResolutions.Enabled = enabled;
            listViewPriorities.Enabled = enabled;

            buttonOk.Enabled = enabled;
            buttonClear.Enabled = enabled;
        }

        private void buttonClear_Click(object sender, EventArgs e) {
//            clearFilterValues();
            clearSelections();
//            Changed = true;
        }

        private void buttonOk_Click(object sender, EventArgs e) {
            clearFilterValues();
            foreach (var item in listBoxProjects.SelectedItems) {
                JiraProject proj = item as JiraProject;
                if (proj != null)
                    filter.Projects.Add(proj);
            }
            foreach (var item in listViewIssueTypes.SelectedItems) {
                JiraNamedEntityListViewItem itlvi = item as JiraNamedEntityListViewItem;
                if (itlvi != null)
                    filter.IssueTypes.Add(itlvi.Entity);
            }
            foreach (var item in listBoxFixForVersions.SelectedItems) {
                JiraNamedEntity version = item as JiraNamedEntity;
                if (version != null)
                    filter.FixForVersions.Add(version);
            }
            foreach (var item in listBoxAffectsVersions.SelectedItems) {
                JiraNamedEntity version = item as JiraNamedEntity;
                if (version != null)
                    filter.AffectsVersions.Add(version);
            }
            foreach (var item in listBoxComponents.SelectedItems) {
                JiraNamedEntity comp = item as JiraNamedEntity;
                if (comp != null)
                    filter.Components.Add(comp);
            }
            foreach (var item in listViewPriorities.SelectedItems) {
                JiraNamedEntityListViewItem itlvi = item as JiraNamedEntityListViewItem;
                if (itlvi != null)
                    filter.Priorities.Add(itlvi.Entity);
            }
            foreach (var item in listViewStatuses.SelectedItems) {
                JiraNamedEntityListViewItem itlvi = item as JiraNamedEntityListViewItem;
                if (itlvi != null)
                    filter.Statuses.Add(itlvi.Entity);
            }
            foreach (var item in listBoxResolutions.SelectedItems) {
                JiraNamedEntity resolution = item as JiraNamedEntity;
                if (resolution != null)
                    filter.Resolutions.Add(resolution);
            }
            var reporter = comboBoxReporter.SelectedItem;
            if (reporter != null) {
                UserTypeComboBoxItem item = reporter as UserTypeComboBoxItem;
                if (item != null) {
                    filter.Reporter = item.Type;
                }
            }
            else {
                filter.Reporter = JiraCustomFilter.UserType.UNDEFINED;
            }
            var assignee = comboBoxAssignee.SelectedItem;
            if (assignee != null) {
                UserTypeComboBoxItem item = assignee as UserTypeComboBoxItem;
                if (item != null) {
                    filter.Assignee = item.Type;
                }
            } else {
                filter.Assignee = JiraCustomFilter.UserType.UNDEFINED;
            }

            Changed = true;
            JiraCustomFilter.save();
            Close();
        }

        private void clearSelections() {
            listViewIssueTypes.SelectedItems.Clear();
            listBoxFixForVersions.SelectedItems.Clear();
            listBoxComponents.SelectedItems.Clear();
            listBoxAffectsVersions.SelectedItems.Clear();
            listBoxResolutions.SelectedItems.Clear();
            listViewStatuses.SelectedItems.Clear();
            listViewPriorities.SelectedItems.Clear();
            comboBoxReporter.SelectedItem = comboBoxReporter.Items[0];
            comboBoxAssignee.SelectedItem = comboBoxAssignee.Items[0];
            // make it last, so that project-related updates are not triggered too early
            listBoxProjects.SelectedItems.Clear();
        }

        private void clearFilterValues() {
            filter.Projects.Clear();
            filter.IssueTypes.Clear();
            filter.AffectsVersions.Clear();
            filter.FixForVersions.Clear();
            filter.Components.Clear();
            filter.Reporter = JiraCustomFilter.UserType.UNDEFINED;
            filter.Assignee = JiraCustomFilter.UserType.UNDEFINED;
            filter.Statuses.Clear();
            filter.Priorities.Clear();
            filter.Resolutions.Clear();
        }

        private void EditCustomFilter_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == (char) Keys.Escape) {
                Close();
            }
        }
    }
}