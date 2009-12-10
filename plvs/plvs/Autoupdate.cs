using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml.XPath;
using Atlassian.plvs.dialogs;

namespace Atlassian.plvs {
    public class Autoupdate {
        private static readonly Autoupdate INSTANCE = new Autoupdate();
        private bool initialized;

        public delegate void UpdateAction();

        public static Autoupdate Instance {
            get { return INSTANCE; }
        }

        private const string URL = "http://docs.atlassian.com/atlassian-vs-plugin/latestPossibleVersion.xml";

        public string NewVersionNumber { get; private set; }
        public string UpdateUrl { get; private set; }
        public string BlurbText { get; private set; }
        public string ReleaseNotesUrl { get; private set; }

        private Autoupdate() {}

        public void initialize() {
            if (initialized) return;

            initialized = true;
            startUpdateThread();
        }

        private void startUpdateThread() {
            Thread thread = new Thread(updateWorker);
            thread.Start();
        }

        private void showUpdateDialog() {
            AutoUpdateDialog dialog = new AutoUpdateDialog(NewVersionNumber, UpdateUrl, BlurbText, ReleaseNotesUrl);
            dialog.ShowDialog();
        }

        private void updateWorker() {
            Thread.Sleep(20000);
            IssueListWindow instance = IssueListWindow.Instance;
            try {
                HttpWebRequest req = (HttpWebRequest) WebRequest.Create(URL);
                req.Timeout = 5000;
                req.ReadWriteTimeout = 20000;
                HttpWebResponse resp = (HttpWebResponse) req.GetResponse();
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
                    if (instance != null) {
                        instance.setAutoupdateAvailable(showUpdateDialog);
                    }
                }
            }
            catch (Exception e) {
                if (instance != null) {
                    instance.setAutoupdateUnavailable(e);
                }
            }
        }
    }
}