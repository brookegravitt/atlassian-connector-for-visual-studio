using System;
using System.Collections.Generic;
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

            treeJira.Nodes.Add(new PrioritiesNode(this, model, facade, server));
            treeJira.Nodes.Add(new ProjectsNode(this, model, facade, server));

            treeJira.SelectedNode = null;
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

        //
        // this is a stupid trick that prevents the first node to be selected 
        // (and hence - expanded) on first loading of the form.
        // Beats me why TreeView behaves like that - go complain to Microsoft
        //
        private bool firstSelect = true;
        private void treeJira_BeforeSelect(object sender, TreeViewCancelEventArgs e) {
            if (!firstSelect) return;
            firstSelect = false;
            e.Cancel = true;
        }
    }
}
