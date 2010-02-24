using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Atlassian.plvs.api;
using Atlassian.plvs.autoupdate;
using Atlassian.plvs.dialogs;
using Atlassian.plvs.models.bamboo;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui;
using Atlassian.plvs.ui.bamboo;
using Atlassian.plvs.ui.jira;
using Atlassian.plvs.util;
using EnvDTE;
using Constants=Atlassian.plvs.util.Constants;

namespace Atlassian.plvs.windows {
    public partial class AtlassianPanel : ToolWindowFrame {

        public static AtlassianPanel Instance { get; private set; }

        private Autoupdate.UpdateAction updateAction;
        private Exception updateException;

        private const string UPDATE_BALOON_TITLE = "Atlassian Connector for Visual Studio";
        private const int UPDATE_BALOON_TIMEOUT = 60000;

        public TabJira Jira { get { return tabJira; }}
        public TabBamboo Bamboo { get { return tabBamboo; }}

        public AtlassianPanel() {
            InitializeComponent();

            productTabs.ImageList = new ImageList();
            productTabs.ImageList.Images.Add(Resources.tab_jira);
            productTabs.ImageList.Images.Add(Resources.tab_bamboo);

            notifyUpdate.Visible = false;

            Instance = this;

            Jira.AddNewServerLinkClicked += jira_AddNewServerLinkClicked;
            Bamboo.AddNewServerLinkClicked += bamboo_AddNewServerLinkClicked;
        }

        private void bamboo_AddNewServerLinkClicked(object sender, EventArgs e) {
            openProjectProperties(Server.BambooServerTypeGuid);
        }

        private void jira_AddNewServerLinkClicked(object sender, EventArgs e) {
            openProjectProperties(Server.JiraServerTypeGuid);
        }

        private void buttonProjectProperties_Click(object sender, EventArgs e) {
            openProjectProperties(null);
        }

        public void openProjectProperties(Guid? serverTypeToCreate) {
            ProjectConfiguration dialog = 
                new ProjectConfiguration(serverTypeToCreate, JiraServerModel.Instance, BambooServerModel.Instance, tabJira.Facade, tabBamboo.Facade);
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

        private void buttonReportBug_Click(object sender, EventArgs e) {
            try {
                System.Diagnostics.Process.Start("https://studio.atlassian.com/secure/CreateIssue.jspa?pid=10500");
// ReSharper disable EmptyGeneralCatchClause
            } catch (Exception) {
// ReSharper restore EmptyGeneralCatchClause
            }
        }

        private void buttonGlobalProperties_Click(object sender, EventArgs e) {
            openGlobalProperties();
        }

        public void openGlobalProperties() {
            GlobalSettings globals = new GlobalSettings();
            globals.ShowDialog();
        }

        private void notifyUpdate_MouseDoubleClick(object sender, MouseEventArgs e) {
            updateIconOrBaloonClicked();
        }

        private void notifyUpdate_BalloonTipClicked(object sender, EventArgs e) {
            updateIconOrBaloonClicked();
        }

        private void updateIconOrBaloonClicked() {
            notifyUpdate.Visible = false;
            if (updateAction != null) {
                updateAction();
            } else if (updateException != null) {
                PlvsUtils.showError("Unable to retrieve autoupdate information", updateException);
            }
        }

        public void setAutoupdateAvailable(Autoupdate.UpdateAction action) {
            Invoke(new MethodInvoker(delegate
                                         {
                                             updateAction = action;
                                             updateException = null;
                                             notifyUpdate.Visible = true;
                                             notifyUpdate.Icon = Resources.status_plugin2;
                                             notifyUpdate.Text = "New version of the connector available, double-click to install";
                                             notifyUpdate.BalloonTipIcon = ToolTipIcon.Info;
                                             notifyUpdate.BalloonTipTitle = UPDATE_BALOON_TITLE;
                                             notifyUpdate.BalloonTipText = "New version of the connector is available, click here to install";
                                             notifyUpdate.ShowBalloonTip(UPDATE_BALOON_TIMEOUT);
                                         }));
        }

        public void setAutoupdateUnavailable(Exception exception) {
            Invoke(new MethodInvoker(delegate
                                         {
                                             updateAction = null;
                                             updateException = exception;
                                             notifyUpdate.Visible = true;
                                             notifyUpdate.Icon = Resources.update_unavailable1;
                                             notifyUpdate.Text = "Unable to retrieve update information, double-click for details";
                                             notifyUpdate.BalloonTipIcon = ToolTipIcon.Error;
                                             notifyUpdate.BalloonTipTitle = UPDATE_BALOON_TITLE;
                                             notifyUpdate.BalloonTipText = "Unable to retrieve connector update information, click here for details";
                                             notifyUpdate.ShowBalloonTip(UPDATE_BALOON_TIMEOUT);
                                         }));
        }

        public void reinitialize(DTE dte) {

            PlvsUtils.updateKeyBindingsInformation(dte, new Dictionary<string, ToolStripItem>
                                            {
                                                { "Tools.AtlassianProjectConfiguration", buttonProjectProperties },
                                                { "Tools.AtlassianGlobalConfiguration", buttonGlobalProperties }
                                            });

            tabJira.reinitialize(dte);
            tabBamboo.reinitialize();
        }

        public void shutdown() {
            tabJira.reinitialize(null);
            tabBamboo.shutdown();
        }
    }
}