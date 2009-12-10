using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Atlassian.plvs.dialogs {
    public partial class AutoUpdateDialog : Form {
        private readonly string updateUrl;
        private bool pageLoaded;

        public AutoUpdateDialog(string stamp, string updateUrl, string blurbText, string releaseNotesUrl) {
            this.updateUrl = updateUrl;

            InitializeComponent();

            browser.DocumentText = string.Format(Resources.autoupdate_html, stamp, blurbText, releaseNotesUrl);
            browser.ScrollBarsEnabled = true;

            StartPosition = FormStartPosition.CenterParent;

            buttonUpdate.Click += buttonUpdate_Click;
        }

        void buttonUpdate_Click(object sender, EventArgs e) {
            try {
                Process.Start(updateUrl);
                // ReSharper disable EmptyGeneralCatchClause
            } catch (Exception) {
                // ReSharper restore EmptyGeneralCatchClause
            }
            Close();
        }

        private void buttonClose_Click(object sender, EventArgs e) {
            Close();
        }

        private void browser_Navigating(object sender, WebBrowserNavigatingEventArgs e) {
            if (!pageLoaded) return;
            string url = e.Url.ToString();
            try {
                Process.Start(url);
// ReSharper disable EmptyGeneralCatchClause
            } catch (Exception) {
// ReSharper restore EmptyGeneralCatchClause
            }
            e.Cancel = true;
        }

        private void browser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e) {
            pageLoaded = true;
        }

        private void About_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == (char) Keys.Escape) {
                Close();
            }
        }

        private void browser_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
            if (e.KeyValue == (char)Keys.Escape) {
                Close();
            }
        }
    }
}