using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using Aga.Controls.Tree;
using Atlassian.plvs.api.bamboo;
using Atlassian.plvs.dialogs;
using Atlassian.plvs.models.bamboo;
using Atlassian.plvs.ui.bamboo.treemodels;
using Atlassian.plvs.util.jira;
using Timer=System.Timers.Timer;

namespace Atlassian.plvs.ui.bamboo {
    public partial class TabBamboo : UserControl {

        private readonly Timer pollTimer;

        private BambooBuildTree buildTree;

        private readonly StatusLabel status;

        private bool? summaryStatusOk;

        public TabBamboo() {
            InitializeComponent();

            status = new StatusLabel(statusStrip, statusLabel);

            pollTimer = new Timer();
            pollTimer.Elapsed += pollTimer_Elapsed;

            GlobalSettings.SettingsChanged += globalSettingsChanged;

            notifyBuildStatus.Icon = Resources.bamboo_grey_161;
            notifyBuildStatus.Visible = false;
            notifyBuildStatus.BalloonTipTitle = "Atlassian Bamboo Notification";
            notifyBuildStatus.Text = "No build information retrieved yet";
        }

        public BambooServerFacade Facade { get { return BambooServerFacade.Instance; } }

        private void globalSettingsChanged(object sender, EventArgs e) {
            reinitialize();
        }

        public void init() {
            pollTimer.Interval = 1000 * GlobalSettings.BambooPollingInterval;
            pollTimer.AutoReset = false;

            pollTimer.Enabled = false;
            pollTimer.Start();
            notifyBuildStatus.Visible = BambooServerModel.Instance.getAllServers().Count > 0;

            Invoke(new MethodInvoker(delegate { initBuildTree(); status.setInfo("Idle"); }));
        }

        private void initBuildTree() {
            if (buildTree != null) {
                toolStripContainer.ContentPanel.Controls.Remove(buildTree);
            }

            buildTree = new BambooBuildTree {Model = new FlatBuildTreeModel()};
            buildTree.Model = new FlatBuildTreeModel();
            toolStripContainer.ContentPanel.Controls.Add(buildTree);

            updateBuildListButtons();

            buildTree.SelectionChanged += buildTree_SelectionChanged;
        }

        void buildTree_SelectionChanged(object sender, EventArgs e) {
            updateBuildListButtons();
        }

        private void updateBuildListButtons() {
            TreeNodeAdv node = buildTree.SelectedNode;
            BuildNode n = node == null ? null : node.Tag as BuildNode;
            buttonViewInBrowser.Enabled = n != null;
            buttonRunBuild.Enabled = n != null;
        }

        public void shutdown() {
            notifyBuildStatus.Visible = false;
            pollTimer.Stop();
            pollTimer.Enabled = false;
            showPollResults(null, null);
        }

        public void reinitialize() {
            shutdown();
            init();
        }

        void pollTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {
            Thread t = new Thread(() => pollRunner(true));
            t.Start();
        }

        private void pollRunner(bool rescheduleTimer) {
            List<BambooBuild> allBuilds = new List<BambooBuild>();
            List<Exception> allExceptions = new List<Exception>();

            ICollection<BambooServer> servers = BambooServerModel.Instance.getAllServers();
            if (servers == null || servers.Count == 0) return;

            status.setInfo("Polling all servers...");

            foreach (BambooServer server in servers) {
                try {
                    ICollection<BambooBuild> builds = Facade.getLatestBuildsForFavouritePlans(server);
                    allBuilds.AddRange(builds);
                } catch (Exception e) {
                    allExceptions.Add(new Exception(server.Name + ": " + e.Message));
                }
            }
            
            try {
                Invoke(new MethodInvoker(delegate {
                                                 showPollResults(allBuilds, allExceptions);
                                                 if (rescheduleTimer) {
                                                     pollTimer.Start();
                                                 }
                                             }));
            } catch (Exception e) {
                Debug.WriteLine("Exception while trying to show poll results: " + e.Message);
            }
        }

        private void showPollResults(ICollection<BambooBuild> builds, ICollection<Exception> exceptions) {
            if (buildTree == null) return;

            if (builds == null && exceptions == null) {
                ((FlatBuildTreeModel) buildTree.Model).updateBuilds(null);
                status.setInfo("No builds to poll found");
                return;
            }
            ((FlatBuildTreeModel) buildTree.Model).updateBuilds(builds);

            if (exceptions != null && exceptions.Count > 0) {
                status.setError("Failed to poll some of the servers", exceptions);
            } else {
                status.setInfo("Last poll finished at " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString());
            }
            bool? allpassing = null;
            if (builds != null) {
                allpassing = true;
                foreach (var b in builds) {
                    if (allpassing.Value && b.Result != BambooBuild.BuildResult.SUCCESSFUL) {
                        allpassing = false;
                    }
                } 
            }
            bool allOk = allpassing != null && allpassing.Value;
            if (summaryStatusOk != null && summaryStatusOk == allOk) return;

            notifyBuildStatus.Icon = allpassing != null
                                         ? (allOk ? Resources.bamboo_green_161 : Resources.bamboo_red_161)
                                         : Resources.bamboo_grey_161;
            notifyBuildStatus.BalloonTipText = allOk
                                                   ? "All builds are passing"
                                                   : (exceptions == null || exceptions.Count == 0 
                                                        ? "Some builds failed" 
                                                        : "Failed to retrieve build information");
            notifyBuildStatus.Text = notifyBuildStatus.BalloonTipText;
            notifyBuildStatus.BalloonTipIcon = allOk ? ToolTipIcon.Info : ToolTipIcon.Warning;
            notifyBuildStatus.ShowBalloonTip(30000);
            summaryStatusOk = allOk;
        }

        private void buttonPoll_Click(object sender, EventArgs e) {
            Thread t = new Thread(() => pollRunner(false));
            t.Start();
        }

        private void buttonViewInBrowser_Click(object sender, EventArgs e) {
            runOnSelectedNode(delegate(BambooBuild b) {
                                  try {
                                      Process.Start(b.Server.Url + "/build/viewBuildResults.action?buildKey="
                                                    + BambooBuildUtils.getPlanKey(b) + "&buildNumber=" + b.Number);
                                  }
                                  catch (Exception ex) {
                                      Debug.WriteLine("buttonViewInBrowser_Click - exception: " + ex.Message);
                                  }
                              });
        }

        private void buttonRunBuild_Click(object sender, EventArgs e) {
            runOnSelectedNode(delegate(BambooBuild b) {
                                  string key = BambooBuildUtils.getPlanKey(b);
                                  status.setInfo("Adding build " + key + " to the build queue...");
                                  Thread t = new Thread(() => runBuildWorker(b, key));
                                  t.Start();
                              });
        }

        private void runBuildWorker(BambooBuild b, string key) {
            try {
                Facade.runBuild(b.Server, key);
                status.setInfo("Added build " + key + " to the build queue");
            } catch (Exception ex) {
                status.setError("Failed to add build " + key + " to the build queue", ex);
            }
        }

        private delegate void SelectedBuildAction(BambooBuild b);

        private void runOnSelectedNode(SelectedBuildAction action) {
            TreeNodeAdv node = buildTree.SelectedNode;
            BuildNode n = node == null ? null : node.Tag as BuildNode;
            if (n == null) return;

            action(n.Build);
        }
    }
}
