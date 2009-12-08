using System;
using System.Diagnostics;
using System.Windows.Forms;
using Atlassian.plvs.api;
using Atlassian.plvs.models;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Atlassian.plvs.eventsinks {
    public sealed class TextMarkerClientEventSink : IVsTextMarkerClient {
        private readonly bool marginMarker;
        private readonly string issueKey;

        public TextMarkerClientEventSink(bool marginMarker, string issueKey) {
            this.marginMarker = marginMarker;
            this.issueKey = issueKey;
        }

        public IVsTextLineMarker Marker { get; set; }

        #region IVsTextMarkerClient Members

        public void MarkerInvalidated() {
            JiraEditorLinkManager.OnMarkerInvalidated(Marker);
        }

        public int GetTipText(IVsTextMarker pMarker, string[] pbstrText) {
            if (issueKey != null && !marginMarker)
                pbstrText[0] = "Double click to try open " + issueKey +
                               "\non the currently selected server,\nRight click for more options";
            else if (marginMarker)
                pbstrText[0] = "This line contains a link(s) to issues";

            return VSConstants.S_OK;
        }

        public void OnBufferSave(string pszFileName) {}

        public void OnBeforeBufferClose() {}

        public int GetMarkerCommandInfo(IVsTextMarker pMarker, int iItem, string[] pbstrText, uint[] pcmdf) {
            // For each command we add we have to specify that we support it.
            // Furthermore it should always be enabled.
            const uint menuItemFlags = (uint) (OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED);

            if (pcmdf == null) {
                return VSConstants.S_FALSE;
            }
                
            switch (iItem) {
                case 0:
                    if (pbstrText != null && issueKey != null) {
                        pbstrText[0] = "Open Issue " + issueKey + " in IDE";
                        pcmdf[0] = menuItemFlags;
                        return VSConstants.S_OK;
                    }
                    return VSConstants.S_FALSE;
                case 1:
                    if (pbstrText != null && issueKey != null) {
                        pbstrText[0] = "View Issue " + issueKey + " in the Browser";
                        pcmdf[0] = menuItemFlags;
                        return VSConstants.S_OK;
                    }
                    return VSConstants.S_FALSE;

                case (int) MarkerCommandValues.mcvBodyDoubleClickCommand:
                    pcmdf[0] = menuItemFlags;
                    return (marginMarker) ? VSConstants.S_FALSE : VSConstants.S_OK;

                default:
                    return VSConstants.S_FALSE;
            }
        }

        public int ExecMarkerCommand(IVsTextMarker pMarker, int iItem) {
            switch (iItem) {
                case 0:
                    openInIde();
                    return VSConstants.S_OK;
                case 1:
                    launchBrowser();
                    return VSConstants.S_OK;

                case (int) MarkerCommandValues.mcvBodyDoubleClickCommand:
                    if (!marginMarker) openInIde();
                    return VSConstants.S_OK;

                default:
                    return VSConstants.S_FALSE;
            }
        }

        private void openInIde() {
            if (issueKey == null) return;

            bool found = false;
            foreach (JiraIssue issue in JiraIssueListModel.Instance.Issues) {
                if (!issue.Key.Equals(issueKey)) continue;
                IssueDetailsWindow.Instance.openIssue(issue);
                found = true;
                break;
            }
            if (!found) {
                IssueListWindow.Instance.findAndOpenIssue(issueKey, findFinished);
            }
        }

        private static void findFinished(bool success, string message) {
            if (!success) {
                MessageBox.Show(message, "Error");
            }
        }

        private void launchBrowser() {
            if (issueKey == null) {
                return;
            }
            try {
                JiraServer server = IssueListWindow.Instance.getCurrentlySelectedServer();
                if (server != null) {
                    Process.Start(server.Url + "/browse/" + issueKey);
                } else {
                    MessageBox.Show("No JIRA server selected", "Error");
                }
            }
// ReSharper disable EmptyGeneralCatchClause
            catch {}
// ReSharper restore EmptyGeneralCatchClause
        }

        public void OnAfterSpanReload() {}

        public int OnAfterMarkerChange(IVsTextMarker pMarker) {
            return VSConstants.S_OK;
        }

        #endregion
    }
}