using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.models.bamboo;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui.bamboo.bamboonodes;
using Atlassian.plvs.ui.jira.issuefilternodes;

namespace Atlassian.plvs.dialogs {
    public partial class ProjectConfiguration : Form {
        private readonly TreeNode jiraRoot = new TreeNode("JIRA Servers");
        private readonly TreeNode bambooRoot = new TreeNode("Bamboo Servers");
//        private readonly TreeNode crucibleRoot = new TreeNode("Crucible Servers");
//        private readonly TreeNode fisheyeRoot = new TreeNode("Fisheye Servers");

        private readonly JiraServerModel jiraServerModel;
        private readonly BambooServerModel bambooServerModel;
        private readonly JiraServerFacade facade;

        public bool SomethingChanged { get; private set; }

        public ProjectConfiguration(JiraServerModel jiraServerModel, BambooServerModel bambooServerModel, JiraServerFacade facade) {
            InitializeComponent();

            this.jiraServerModel = jiraServerModel;
            this.bambooServerModel = bambooServerModel;
            this.facade = facade;

            ICollection<JiraServer> jiraServers = jiraServerModel.getAllServers();
            ICollection<BambooServer> bambooServers = bambooServerModel.getAllServers();

            serverTree.Nodes.Add(jiraRoot);
            serverTree.Nodes.Add(bambooRoot);
//            serverTree.Nodes.Add(crucibleRoot);
//            serverTree.Nodes.Add(fisheyeRoot);

            foreach (JiraServer server in jiraServers) {
                jiraRoot.Nodes.Add(new JiraServerTreeNode(server, 0));
            }

            foreach (BambooServer server in bambooServers) {
                bambooRoot.Nodes.Add(new BambooServerTreeNode(server, 0));
            }

            StartPosition = FormStartPosition.CenterParent;

            serverTree.ExpandAll();
        }

        private void buttonClose_Click(object sender, EventArgs e) {
            Close();
        }

        private void serverTree_AfterSelect(object sender, TreeViewEventArgs e) {
            bool jiraRootSelected = serverTree.SelectedNode.Equals(jiraRoot);
            bool jiraServerSelected = serverTree.SelectedNode is JiraServerTreeNode;
            bool bambooRootSelected = serverTree.SelectedNode.Equals(bambooRoot);
            bool bambooServerSelected = serverTree.SelectedNode is BambooServerTreeNode;
            buttonAdd.Enabled = jiraRootSelected || bambooRootSelected;
            buttonEdit.Enabled = jiraServerSelected || bambooServerSelected;
            buttonDelete.Enabled = jiraServerSelected || bambooServerSelected;
            buttonTest.Enabled = jiraServerSelected || bambooServerSelected;

            serverDetails.Text = jiraServerSelected || bambooServerSelected ? createServerSummaryText(serverTree.SelectedNode) : "";
        }

        private static string createServerSummaryText(TreeNode node) {
            JiraServerTreeNode jNode = node as JiraServerTreeNode;
            BambooServerTreeNode bNode = node as BambooServerTreeNode;
            if (jNode != null) {
                StringBuilder sb = new StringBuilder();
                sb.Append("Name: ").Append(jNode.Server.Name).Append("\r\n");
                sb.Append("URL: ").Append(jNode.Server.Url).Append("\r\n");
                sb.Append("User Name: ").Append(jNode.Server.UserName);
                return sb.ToString();
            }
            if (bNode != null) {
                StringBuilder sb = new StringBuilder();
                sb.Append("Name: ").Append(bNode.Server.Name).Append("\r\n");
                sb.Append("URL: ").Append(bNode.Server.Url).Append("\r\n");
                sb.Append("User Name: ").Append(bNode.Server.UserName).Append("\r\n");
                sb.Append("Use Favourite Builds: ").Append(bNode.Server.UseFavourites ? "Yes" : "No");
                return sb.ToString();
            }
            return "";
        }

        private void buttonAdd_Click(object sender, EventArgs e) {
            if (serverTree.SelectedNode.Equals(jiraRoot)) {
                addNewJiraServer();
            } else if (serverTree.SelectedNode.Equals(bambooRoot)) {
                addNewBambooServer();
            }
        }

        private void addNewBambooServer() {
            AddOrEditBambooServer dialog = new AddOrEditBambooServer(null);
            DialogResult result = dialog.ShowDialog();
            if (result != DialogResult.OK) return;
            bambooServerModel.addServer(dialog.Server);
            BambooServerTreeNode newNode = new BambooServerTreeNode(dialog.Server, 0);
            bambooRoot.Nodes.Add(newNode);
            serverTree.ExpandAll();
            serverTree.SelectedNode = newNode;
            SomethingChanged = true;
        }

        private void addNewJiraServer() {
            AddOrEditJiraServer dialog = new AddOrEditJiraServer(null);
            DialogResult result = dialog.ShowDialog();
            if (result != DialogResult.OK) return;
            jiraServerModel.addServer(dialog.Server);
            JiraServerTreeNode newNode = new JiraServerTreeNode(dialog.Server, 0);
            jiraRoot.Nodes.Add(newNode);
            serverTree.ExpandAll();
            serverTree.SelectedNode = newNode;
            SomethingChanged = true;
        }

        private void buttonEdit_Click(object sender, EventArgs e) {
            if (serverTree.SelectedNode is JiraServerTreeNode) {
                JiraServerTreeNode selectedNode = (JiraServerTreeNode)serverTree.SelectedNode;
                AddOrEditJiraServer dialog = new AddOrEditJiraServer(selectedNode.Server);
                DialogResult result = dialog.ShowDialog();
                if (result != DialogResult.OK) return;
                jiraServerModel.removeServer(selectedNode.Server.GUID);
                jiraServerModel.addServer(dialog.Server);
                selectedNode.Server = dialog.Server;
                serverTree.ExpandAll();
                serverDetails.Text = createServerSummaryText(selectedNode);
                serverTree.SelectedNode = selectedNode;
                SomethingChanged = true;
            } else if (serverTree.SelectedNode is BambooServerTreeNode) {
                BambooServerTreeNode selectedNode = (BambooServerTreeNode)serverTree.SelectedNode;
                AddOrEditBambooServer dialog = new AddOrEditBambooServer(selectedNode.Server);
                DialogResult result = dialog.ShowDialog();
                if (result != DialogResult.OK) return;
                bambooServerModel.removeServer(selectedNode.Server.GUID);
                bambooServerModel.addServer(dialog.Server);
                selectedNode.Server = dialog.Server;
                serverTree.ExpandAll();
                serverDetails.Text = createServerSummaryText(selectedNode);
                serverTree.SelectedNode = selectedNode;
                SomethingChanged = true;
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e) {
            if (serverTree.SelectedNode is JiraServerTreeNode) {
                JiraServerTreeNode selectedNode = (JiraServerTreeNode)serverTree.SelectedNode;
                jiraServerModel.removeServer(selectedNode.Server.GUID);
                selectedNode.Remove();
                serverTree.ExpandAll();
                serverDetails.Text = "";
                SomethingChanged = true;
            } else if (serverTree.SelectedNode is BambooServerTreeNode) {
                BambooServerTreeNode selectedNode = (BambooServerTreeNode)serverTree.SelectedNode;
                bambooServerModel.removeServer(selectedNode.Server.GUID);
                selectedNode.Remove();
                serverTree.ExpandAll();
                serverDetails.Text = "";
                SomethingChanged = true;
            }
        }

        private void buttonTest_Click(object sender, EventArgs e) {
            if (serverTree.SelectedNode is JiraServerTreeNode) {
                JiraServerTreeNode selectedNode = (JiraServerTreeNode) serverTree.SelectedNode;
                new TestJiraConnection(facade, selectedNode.Server).ShowDialog();
            } else if (serverTree.SelectedNode is BambooServerTreeNode) {
                BambooServerTreeNode selectedNode = (BambooServerTreeNode)serverTree.SelectedNode;
//                new TestBambooConnection(facade, selectedNode.Server).ShowDialog();
            }
        }

        private void ProjectConfiguration_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == (char) Keys.Escape) {
                Close();
            }
        }
    }
}