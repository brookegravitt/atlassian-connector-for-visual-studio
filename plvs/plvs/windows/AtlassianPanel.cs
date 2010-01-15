using System;
using System.Windows.Forms;
using Atlassian.plvs.autoupdate;
using Atlassian.plvs.dialogs;
using Atlassian.plvs.models.bamboo;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui;
using Atlassian.plvs.ui.bamboo;
using Atlassian.plvs.ui.jira;
using Atlassian.plvs.util;

namespace Atlassian.plvs.windows {
    public partial class AtlassianPanel : ToolWindowFrame {

        public static AtlassianPanel Instance { get; private set; }

        public TabJira Jira { get { return tabJira; }}
        public TabBamboo Bamboo { get { return tabBamboo; }}

        public AtlassianPanel() {
            InitializeComponent();

            productTabs.ImageList = new ImageList();
            productTabs.ImageList.Images.Add(Resources.tab_jira);
            productTabs.ImageList.Images.Add(Resources.tab_bamboo);

            buttonUpdate.Visible = false;

            Instance = this;
        }

        private void buttonProjectProperties_Click(object sender, EventArgs e) {
            ProjectConfiguration dialog = new ProjectConfiguration(
                JiraServerModel.Instance, BambooServerModel.Instance, tabJira.Facade, tabBamboo.Facade);
            dialog.ShowDialog(this);
            if (dialog.SomethingChanged) {
                // todo: only do this for changed servers - add server model listeners
                // currently this code blows :)
                tabJira.reloadKnownJiraServers();
                tabBamboo.reinitialize();
            }
        }

        private void buttonAbout_Click(object sender, EventArgs e) {
            new About().ShowDialog(this);
        }

        private void buttonGlobalProperties_Click(object sender, EventArgs e) {
            GlobalSettings globals = new GlobalSettings();
            globals.ShowDialog();
        }

        private Autoupdate.UpdateAction updateAction;
        private Exception updateException;

        private void buttonUpdate_Click(object sender, EventArgs e) {
            if (updateAction != null) {
                updateAction();
            } else if (updateException != null) {
                MessageBox.Show(
                    "Unable to retrieve autoupdate information:\n\n" + updateException.Message,
                    Constants.ERROR_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void setAutoupdateAvailable(Autoupdate.UpdateAction action) {
            Invoke(new MethodInvoker(delegate
                                         {
                                             updateAction = action;
                                             updateException = null;
                                             buttonUpdate.Enabled = true;
                                             buttonUpdate.Image = Resources.status_plugin;
                                             buttonUpdate.Visible = true;
                                             buttonUpdate.Text = "New version of the connector is available";
                                         }));
        }

        public void setAutoupdateUnavailable(Exception exception) {
            Invoke(new MethodInvoker(delegate
                                         {
                                             updateAction = null;
                                             updateException = exception;
                                             buttonUpdate.Enabled = true;
                                             buttonUpdate.Image = Resources.update_unavailable;
                                             buttonUpdate.Visible = true;
                                             buttonUpdate.Text = "Unable to retrieve connector update information";
                                         }));
        }

        public void reinitialize() {
            tabJira.reinitialize();
            tabBamboo.reinitialize();
        }

        public void shutdown() {
            tabJira.reinitialize();
            tabBamboo.shutdown();
        }
    }
}