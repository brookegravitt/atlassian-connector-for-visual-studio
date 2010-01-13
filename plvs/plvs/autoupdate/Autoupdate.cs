using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.XPath;
using Atlassian.plvs.dialogs;
using Atlassian.plvs.windows;

namespace Atlassian.plvs.autoupdate {
    public class Autoupdate {
        private static readonly Autoupdate INSTANCE = new Autoupdate();
        private bool initialized;
        private volatile bool shouldStop;

        private const int ONE_HOUR = 60 * 60;

        public delegate void UpdateAction();

        public static Autoupdate Instance {
            get { return INSTANCE; }
        }

        private const string STABLE_URL = "http://update.atlassian.com/atlassian-vs-plugin/latestStableVersion.xml";
        private const string SNAPSHOT_URL = "http://docs.atlassian.com/atlassian-vs-plugin/latestPossibleVersion.xml";

        public string NewVersionNumber { get; private set; }
        public string UpdateUrl { get; private set; }
        public string BlurbText { get; private set; }
        public string ReleaseNotesUrl { get; private set; }

        private Thread thread;

        private Autoupdate() {}

        public void initialize() {
            if (initialized) return;

            shouldStop = false;

            initialized = true;
            thread = new Thread(updateWorker);
            thread.Start();
        }

        public void shutdown() {
            if (!initialized) {
                return;
            }
            initialized = false;
            if (thread == null) return;

            shouldStop = true;
            thread.Join(4000);
            thread = null;
        }

        private void showUpdateDialog() {
            AutoUpdateDialog dialog = new AutoUpdateDialog(NewVersionNumber, UpdateUrl, BlurbText, ReleaseNotesUrl);
            dialog.ShowDialog();
        }

        private void updateWorker() {
            Debug.WriteLine("Plvs - Autoupdate: Starting autoupdate thread");
            if (sleepOrExit(20)) {
                return;
            }
            do {
                runSingleUpdateQuery();
            } while (!sleepOrExit(ONE_HOUR));
        }

        private bool sleepOrExit(int seconds) {
            for (int i = 0; i < seconds; ++i) {
                Thread.Sleep(1000);
                if (!shouldStop) continue;
                Debug.WriteLine("Plvs - Autoupdate: Finishing autoupdate thread");
                return true;
            }
            return false;
        }

        private void runSingleUpdateQuery() {
            IssueListWindow issueListWindow = IssueListWindow.Instance;
            try {
                // heh - I certainly do hope boolean properties are atomic and multithreading-safe. 
                // I sure would not like to have to synchronize them
                if (!GlobalSettings.AutoupdateEnabled) {
                    return;
                }
                string url = GlobalSettings.AutoupdateSnapshots ? SNAPSHOT_URL : STABLE_URL;
                if (GlobalSettings.ReportUsage) {
                    url = getUsageReportingUrl(url);
                }
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

                req.Timeout = 5000;
                req.ReadWriteTimeout = 20000;
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                Stream str = resp.GetResponseStream();

                XPathDocument doc = new XPathDocument(str);
                XPathNavigator nav = doc.CreateNavigator();
                XPathExpression expr = nav.Compile("/response/version/number");
                XPathNodeIterator it = nav.Select(expr);
                it.MoveNext();
                NewVersionNumber = it.Current.Value;

                Regex versionPattern = new Regex(@"(\d+\.\d+\.\d+)-(.+)-(\d+-\d+)");
                if (!versionPattern.IsMatch(NewVersionNumber)) {
                    return;
                }
                string stamp = versionPattern.Match(NewVersionNumber).Groups[3].Value;
                if (NewVersionNumber == null) return;


                expr = nav.Compile("/response/version/downloadUrl");
                it = nav.Select(expr);
                it.MoveNext();
                UpdateUrl = it.Current.Value.Trim();
                expr = nav.Compile("/response/version/releaseNotes");
                it = nav.Select(expr);
                it.MoveNext();
                BlurbText = it.Current.Value.Trim();
                expr = nav.Compile("/response/version/releaseNotesUrl");
                it = nav.Select(expr);
                it.MoveNext();
                ReleaseNotesUrl = it.Current.Value.Trim();

                if (PlvsVersionInfo.Stamp.CompareTo(stamp) < 0) {
                    if (issueListWindow != null) {
                        issueListWindow.setAutoupdateAvailable(showUpdateDialog);
                    }
                }
            } catch (Exception e) {
                if (issueListWindow != null) {
                    issueListWindow.setAutoupdateUnavailable(e);
                }
            }
        }

        private string getUsageReportingUrl(string url) {
            return url;
        }
    }
}