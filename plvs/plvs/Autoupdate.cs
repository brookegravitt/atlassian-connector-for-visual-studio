using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell.Interop;

namespace Atlassian.plvs {
    public class Autoupdate {
        private static readonly Autoupdate INSTANCE = new Autoupdate();
        private bool initialized;

        public static Autoupdate Instance {
            get { return INSTANCE; }
        }

        private const string URL = "http://docs.atlassian.com/atlassian-vs-plugin/latestPossibleVersion.xml";

        private Autoupdate() {}

        public void initialize() {
            if (initialized) return;

            initialized = true;
            startUpdateThread();
        }

        private static void startUpdateThread() {
            Thread thread = new Thread(updateWorker);
            thread.Start();
        }

        private static void updateWorker() {
            Thread.Sleep(20000);
            IssueListWindow instance = IssueListWindow.Instance;
            try {
                HttpWebRequest req = (HttpWebRequest) WebRequest.Create(URL);
                req.Timeout = 5000;
                req.ReadWriteTimeout = 20000;
                HttpWebResponse resp = (HttpWebResponse) req.GetResponse();
                Stream str = resp.GetResponseStream();
                if (instance != null) {
                    instance.setAutoupdateAvailable();
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