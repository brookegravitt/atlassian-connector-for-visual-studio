using System;
using System.Collections.Generic;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.explorer.treeNodes;
using Atlassian.plvs.ui;

namespace Atlassian.plvs.explorer {
    public partial class JiraServerExplorer : Form {
        private readonly JiraServer server;
        private readonly JiraServerFacade facade;

        private readonly StatusLabel status;

        public JiraServerExplorer(JiraServer server, JiraServerFacade facade) {
            this.server = server;
            this.facade = facade;

            InitializeComponent();

            status = new StatusLabel(statusStrip, labelPath);

            StartPosition = FormStartPosition.CenterParent;
        }

        protected override void OnLoad(EventArgs e) {

            webJira.DocumentText = "Loading projects...";

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
                ProjectNode projectNode = new ProjectNode(server, project);
                treeJira.Nodes.Add(projectNode);
                projectNode.Nodes.Add(new ComponentsNode(this, server, project));
                projectNode.Nodes.Add(new VersionsNode(this, server, project));
            }
            webJira.DocumentText = "Projects loaded";
        }

        private string getAuthString() {
            return "os_username=" + HttpUtility.UrlEncode(server.UserName) + "&os_password=" + HttpUtility.UrlEncode(server.Password);
        }

        private void treeJira_AfterSelect(object sender, TreeViewEventArgs e) {
            NavigableJiraServerEntity node = treeJira.SelectedNode as AbstractNavigableTreeNodeWithServer;
            if (node != null) {
                node.onClick(facade, status);
                string url = node.getUrl(getAuthString());
                webJira.Navigate(url);
            }
        }
    }
}
