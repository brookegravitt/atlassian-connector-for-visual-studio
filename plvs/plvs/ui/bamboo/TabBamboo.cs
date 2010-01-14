using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Atlassian.plvs.api.bamboo;
using Atlassian.plvs.dialogs;
using Atlassian.plvs.models.bamboo;
using Timer=System.Timers.Timer;

namespace Atlassian.plvs.ui.bamboo {
    public partial class TabBamboo : UserControl {

        private readonly Timer pollTimer;

        public TabBamboo() {
            InitializeComponent();

            pollTimer = new Timer();
            pollTimer.Elapsed += pollTimer_Elapsed;

            GlobalSettings.SettingsChanged += globalSettingsChanged;
        }

        private void globalSettingsChanged(object sender, EventArgs e) {
            shutdown();
            init();
        }

        public void init() {
            pollTimer.Interval = 1000 * GlobalSettings.BambooPollingInterval;
            pollTimer.AutoReset = false;

            // todo: enable timer when bugs fixed
            pollTimer.Enabled = false;
//            pollTimer.Start();
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
                    ICollection<BambooBuild> builds = BambooServerFacade.Instance.getLatestBuildsForFavouritePlans(server);
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

        private void showPollResults(ICollection builds, ICollection<Exception> exceptions) {
            StringBuilder doc = new StringBuilder();
            if (builds != null) {
                doc.Append("<br>received " + builds.Count + " build results");
            } 

            if (exceptions != null && exceptions.Count > 0) {
                doc.Append("<br>received " + exceptions.Count + "exceptions");
            }
            webBambooBuildSummary.DocumentText = doc.ToString();
        }

        public void shutdown() {
            pollTimer.Stop();
            pollTimer.Enabled = false;
            showPollResults(null, null);
        }

        private void buttonPoll_Click(object sender, EventArgs e) {
            Thread t = new Thread(() => pollRunner(false));
            t.Start();
        }
    }
}
