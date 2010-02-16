using System;
using System.Windows.Forms;
using Atlassian.plvs.api;
using Atlassian.plvs.api.bamboo;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.dialogs.bamboo;
using Atlassian.plvs.dialogs.jira;
using Atlassian.plvs.models.bamboo;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui.bamboo.bamboonodes;
using Atlassian.plvs.ui.jira.issuefilternodes;

namespace Atlassian.plvs.dialogs {
    public partial class ProjectConfiguration : Form {
        private readonly TreeNode jiraRoot = new TreeNode("JIRA Servers", 0, 0);
        private readonly TreeNode bambooRoot = new TreeNode("Bamboo Servers", 1, 1);
//        private readonly TreeNode crucibleRoot = new TreeNode("Crucible Servers");
//        private readonly TreeNode fisheyeRoot = new TreeNode("Fisheye Servers");

        private const int JIRA_ENABLED = 2;
        private const int JIRA_DISABLED = 3;
        private const int BAMBOO_ENABLED = 4;
        private const int BAMBOO_DISABLED = 5;

        private readonly JiraServerModel jiraServerModel;
        private readonly BambooServerModel bambooServerModel;
        private readonly JiraServerFacade jiraFacade;
        private readonly BambooServerFacade bambooFacade;

        private readonly ImageList imageList;

        public bool SomethingChanged { get; private set; }

        public ProjectConfiguration(
            Guid? serverTypeToCreate, JiraServerModel jiraServerModel, BambooServerModel bambooServerModel, 
            JiraServerFacade jiraFacade, BambooServerFacade bambooFacade) {

            InitializeComponent();

            this.jiraServerModel = jiraServerModel;
            this.bambooServerModel = bambooServerModel;
            this.jiraFacade = jiraFacade;
            this.bambooFacade = bambooFacade;

            var jiraServers = jiraServerModel.getAllServers();
            var bambooServers = bambooServerModel.getAllServers();

            imageList = new ImageList();
            imageList.Images.Add(Resources.folder_jira);
            imageList.Images.Add(Resources.folder_bamboo);
            imageList.Images.Add(Resources.tab_jira);
            imageList.Images.Add(Resources.tab_jira_grey);
            imageList.Images.Add(Resources.tab_bamboo);
            imageList.Images.Add(Resources.tab_bamboo_grey);

            serverTree.ImageList = imageList;

            serverTree.Nodes.Add(jiraRoot);
            serverTree.Nodes.Add(bambooRoot);
//            serverTree.Nodes.Add(crucibleRoot);
//            serverTree.Nodes.Add(fisheyeRoot);

            foreach (var server in jiraServers) {
                jiraRoot.Nodes.Add(new JiraServerTreeNode(server, server.Enabled ? JIRA_ENABLED : JIRA_DISABLED));
            }

            foreach (var server in bambooServers) {
                bambooRoot.Nodes.Add(new BambooServerTreeNode(server, server.Enabled ? BAMBOO_ENABLED : BAMBOO_DISABLED));
            }

            StartPosition = FormStartPosition.CenterParent;

            serverTree.ExpandAll();

            if (serverTypeToCreate != null) {
                if (serverTypeToCreate.Value.Equals(Server.JiraServerTypeGuid)) {
                    serverTree.SelectedNode = jiraRoot;
                } else if (serverTypeToCreate.Value.Equals(Server.BambooServerTypeGuid)) {
                    serverTree.SelectedNode = bambooRoot;
                }
            }
        }

        private void buttonClose_Click(object sender, EventArgs e) {
            Close();
            if (SomethingChanged) {
                jiraFacade.dropAllSessions();
                bambooFacade.dropAllSessions();
            }
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

            serverDetails.Text = createServerSummaryText(serverTree.SelectedNode);
        }

        private static string createServerSummaryText(TreeNode node) {
            var jiraServerNode = node as TreeNodeWithJiraServer;
            var bambooServerNode = node as TreeNodeWithBambooServer;
            if (jiraServerNode != null) return jiraServerNode.Server.displayDetails();
            if (bambooServerNode != null) return bambooServerNode.Server.displayDetails();
            return "No server selected";
        }

        private void buttonAdd_Click(object sender, EventArgs e) {
            if (serverTree.SelectedNode.Equals(jiraRoot)) {
                addNewJiraServer();
            } else if (serverTree.SelectedNode.Equals(bambooRoot)) {
                addNewBambooServer();
            }
        }

        private void addNewBambooServer() {
            var dialog = new AddOrEditBambooServer(null, bambooFacade);
            var result = dialog.ShowDialog();
            if (result != DialogResult.OK) return;
            bambooServerModel.addServer(dialog.Server);
            var newNode = new BambooServerTreeNode(dialog.Server, dialog.Server.Enabled ? BAMBOO_ENABLED : BAMBOO_DISABLED);
            bambooRoot.Nodes.Add(newNode);
            serverTree.ExpandAll();
            serverTree.SelectedNode = newNode;
            SomethingChanged = true;
        }

        private void addNewJiraServer() {
            var dialog = new AddOrEditJiraServer(null);
            var result = dialog.ShowDialog();
            if (result != DialogResult.OK) return;
            jiraServerModel.addServer(dialog.Server);
            var newNode = new JiraServerTreeNode(dialog.Server, dialog.Server.Enabled ? JIRA_ENABLED : JIRA_DISABLED);
            jiraRoot.Nodes.Add(newNode);
            serverTree.ExpandAll();
            serverTree.SelectedNode = newNode;
            SomethingChanged = true;
        }

        private void buttonEdit_Click(object sender, EventArgs e) {
            if (serverTree.SelectedNode is JiraServerTreeNode) {
                var selectedNode = (JiraServerTreeNode)serverTree.SelectedNode;
                var dialog = new AddOrEditJiraServer(selectedNode.Server);
                var result = dialog.ShowDialog();
                if (result != DialogResult.OK) return;
                jiraServerModel.removeServer(selectedNode.Server.GUID);
                jiraServerModel.addServer(dialog.Server);
                selectedNode.Server = dialog.Server;
                selectedNode.ImageIndex = dialog.Server.Enabled ? JIRA_ENABLED : JIRA_DISABLED;
                selectedNode.SelectedImageIndex = dialog.Server.Enabled ? JIRA_ENABLED : JIRA_DISABLED;
                serverTree.ExpandAll();
                serverDetails.Text = createServerSummaryText(selectedNode);
                serverTree.SelectedNode = selectedNode;
                SomethingChanged = true;
            } else if (serverTree.SelectedNode is BambooServerTreeNode) {
                var selectedNode = (BambooServerTreeNode)serverTree.SelectedNode;
                var dialog = new AddOrEditBambooServer(selectedNode.Server, bambooFacade);
                var result = dialog.ShowDialog();
                if (result != DialogResult.OK) return;
                bambooServerModel.removeServer(selectedNode.Server.GUID);
                bambooServerModel.addServer(dialog.Server);
                selectedNode.Server = dialog.Server;
                selectedNode.ImageIndex = dialog.Server.Enabled ? BAMBOO_ENABLED : BAMBOO_DISABLED;
                selectedNode.SelectedImageIndex = dialog.Server.Enabled ? BAMBOO_ENABLED : BAMBOO_DISABLED;
                serverTree.ExpandAll();
                serverDetails.Text = createServerSummaryText(selectedNode);
                serverTree.SelectedNode = selectedNode;
                SomethingChanged = true;
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e) {
            if (serverTree.SelectedNode is JiraServerTreeNode) {
                var selectedNode = (JiraServerTreeNode)serverTree.SelectedNode;
                jiraServerModel.removeServer(selectedNode.Server.GUID);
                selectedNode.Remove();
                serverTree.ExpandAll();
                serverDetails.Text = "";
                SomethingChanged = true;
            } else if (serverTree.SelectedNode is BambooServerTreeNode) {
                var selectedNode = (BambooServerTreeNode)serverTree.SelectedNode;
                bambooServerModel.removeServer(selectedNode.Server.GUID);
                selectedNode.Remove();
                serverTree.ExpandAll();
                serverDetails.Text = "";
                SomethingChanged = true;
            }
        }

        private void buttonTest_Click(object sender, EventArgs e) {
            if (serverTree.SelectedNode is JiraServerTreeNode) {
                var selectedNode = (JiraServerTreeNode) serverTree.SelectedNode;
                new TestJiraConnection(jiraFacade, selectedNode.Server).ShowDialog();
            } else if (serverTree.SelectedNode is BambooServerTreeNode) {
                var selectedNode = (BambooServerTreeNode)serverTree.SelectedNode;
                new TestBambooConnection(bambooFacade, selectedNode.Server).ShowDialog();
            }
        }

        private void ProjectConfiguration_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == (char) Keys.Escape) {
                Close();
            }
        }
    }
}