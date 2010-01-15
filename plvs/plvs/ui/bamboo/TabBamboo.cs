using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Atlassian.plvs.api.bamboo;
using Atlassian.plvs.dialogs;
using Atlassian.plvs.models.bamboo;
using Atlassian.plvs.util;
using Timer=System.Timers.Timer;

namespace Atlassian.plvs.ui.bamboo {
    public partial class TabBamboo : UserControl {

        private readonly Timer pollTimer;

        private bool? summaryStatusOk;

        public TabBamboo() {
            InitializeComponent();

            pollTimer = new Timer();
            pollTimer.Elapsed += pollTimer_Elapsed;

            GlobalSettings.SettingsChanged += globalSettingsChanged;

            notifyBuildStatus.Icon = Resources.bamboo_grey_161;
            notifyBuildStatus.Visible = false;
            notifyBuildStatus.BalloonTipTitle = "Atlassian Bamboo Notification";
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
            foreach (BambooServer server in BambooServerModel.Instance.getAllServers()) {
                try {
                    ICollection<BambooBuild> builds = Facade.getLatestBuildsForFavouritePlans(server);
                    allBuilds.AddRange(builds);
                } catch (Exception e) {
                    allExceptions.Add(e);
                }
            }
            Invoke(new MethodInvoker(delegate
                                         {
                                             showPollResults(allBuilds, allExceptions);
                                             if (rescheduleTimer) {
                                                 pollTimer.Start();
                                             }
                                         }));
        }

        private void showPollResults(IEnumerable<BambooBuild> builds, ICollection<Exception> exceptions) {
            if (builds == null && exceptions == null) {
                webBambooBuildSummary.DocumentText = "Idle";
                return;
            }
            StringBuilder doc = new StringBuilder();
            bool? allpassing = null;
            if (builds != null) {
                allpassing = true;
                foreach (var b in builds) {
                    if (allpassing.Value && b.Result != BambooBuild.BuildResult.SUCCESSFUL) {
                        allpassing = false;
                    }
                    doc.Append("<br>&nbsp;&nbsp;&nbsp;&nbsp;")
                        .Append(b.Key).Append(" - ").Append(b.Result.GetStringValue())
                        .Append(", Tests: ").Append(b.SuccessfulTests).Append("/").Append(b.SuccessfulTests + b.FailedTests);
                    if (b.Reason.Length > 300) {
                        doc.Append(", Reason: [garbage received?]");
                    } else {
                        doc.Append(", Reason: ").Append(b.Reason);
                    }
                    doc.Append(", Completed: ").Append(b.RelativeTime).Append(", Duration: ").Append(b.Duration);
                } 
            }
            bool allOk = allpassing != null && allpassing.Value;
            if (summaryStatusOk == null || summaryStatusOk != allOk) {
                notifyBuildStatus.Icon = allpassing != null
                    ? (allOk ? Resources.bamboo_green_161 : Resources.bamboo_red_161)
                    : Resources.bamboo_grey_161;
                notifyBuildStatus.BalloonTipText = allOk
                    ? "All builds are passing"
                    : "Some builds failed or unable to retrieve build information";
                notifyBuildStatus.Text = notifyBuildStatus.BalloonTipText;
                notifyBuildStatus.BalloonTipIcon = allOk ? ToolTipIcon.Info : ToolTipIcon.Warning;
                notifyBuildStatus.ShowBalloonTip(30000);
                summaryStatusOk = allOk;
            }

            doc.Append("<br><hr><br>");
            if (exceptions != null && exceptions.Count > 0) {
                doc.Append("<br>received " + exceptions.Count + "exceptions:");
                foreach (var e in exceptions) {
                    doc.Append("<br>&nbsp;&nbsp;&nbsp;&nbsp;").Append(e.Message);
                }
            }
            webBambooBuildSummary.DocumentText = doc.ToString();
        }

        private void buttonPoll_Click(object sender, EventArgs e) {
            Thread t = new Thread(() => pollRunner(false));
            t.Start();
        }
    }
}
