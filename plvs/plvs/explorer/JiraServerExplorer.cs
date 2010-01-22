using System;
using System.Collections.Generic;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.explorer.treeNodes;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui;

namespace Atlassian.plvs.explorer {
    public sealed partial class JiraServerExplorer : Form {
        private readonly JiraIssueListModel model;
        private readonly JiraServer server;
        private readonly JiraServerFacade facade;

        private readonly StatusLabel status;

        private static readonly Dictionary<string, JiraServerExplorer> activeExplorers = new Dictionary<string, JiraServerExplorer>();

        public static void showJiraServerExplorerFor(JiraIssueListModel model, JiraServer server, JiraServerFacade facade) {
            if (activeExplorers.ContainsKey(server.GUID.ToString())) {
                activeExplorers[server.GUID.ToString()].BringToFront();
            } else {
                new JiraServerExplorer(model, server, facade).Show();
            }
        }

        private JiraServerExplorer(JiraIssueListModel model, JiraServer server, JiraServerFacade facade) {
            this.model = model;
            this.server = server;
            this.facade = facade;

            InitializeComponent();

            status = new StatusLabel(statusStrip, labelPath);

            Text = "JIRA Server Explorer: " + server.Name + " (" + server.Url + ")";

            StartPosition = FormStartPosition.CenterParent;

            dropDownCreateDropZone.Enabled = false;
        }

        public static void closeAll() {
            List<JiraServerExplorer> l = new List<JiraServerExplorer>(activeExplorers.Values);
            foreach (JiraServerExplorer explorer in l) {
                explorer.Close();
            }
        }

        protected override void OnLoad(EventArgs e) {

            activeExplorers[server.GUID.ToString()] = this;

            webJira.Navigate(server.Url + "?" + getAuthString());

            Thread t = new Thread(loadProjects);
            t.Start();
        }

        private void loadProjects() {
            try {
                List<JiraProject> projects = facade.getProjects(server);
                Invoke(new MethodInvoker(() => populateProjects(projects)));
            } catch (Exception e) {
                status.setError("Failed to load projects", e);
            }
        }

        private void populateProjects(IEnumerable<JiraProject> projects) {
            SortedDictionary<string, JiraProject> sorted = new SortedDictionary<string, JiraProject>();
            foreach (JiraProject project in projects) {
                sorted[project.Key] = project;
            }
            foreach (JiraProject project in sorted.Values) {
                ProjectNode projectNode = new ProjectNode(model, facade, server, project);
                treeJira.Nodes.Add(projectNode);
                projectNode.Nodes.Add(new ComponentsNode(this, model, facade, server, project));
                projectNode.Nodes.Add(new VersionsNode(this, model, facade, server, project));
            }
        }

        private string getAuthString() {
            return "os_username=" + HttpUtility.UrlEncode(server.UserName) + "&os_password=" + HttpUtility.UrlEncode(server.Password);
        }

        private void treeJira_AfterSelect(object sender, TreeViewEventArgs e) {
            AbstractNavigableTreeNodeWithServer node = treeJira.SelectedNode as AbstractNavigableTreeNodeWithServer;
            if (node != null) {
                node.onClick(status);
                string url = node.getUrl(getAuthString());
                webJira.Navigate(url);

                ICollection<ToolStripItem> menuItems = node.MenuItems;
                dropDownCreateDropZone.Enabled = menuItems != null && menuItems.Count > 0;
                if (menuItems != null) {
                    dropDownCreateDropZone.DropDownItems.Clear();
                    foreach (ToolStripItem item in menuItems) {
                        dropDownCreateDropZone.DropDownItems.Add(item);
                    }
                }
            } else {
                dropDownCreateDropZone.Enabled = false;
            }
        }

        private void JiraServerExplorer_FormClosed(object sender, FormClosedEventArgs e) {
            activeExplorers.Remove(server.GUID.ToString());
        }
    }
}
