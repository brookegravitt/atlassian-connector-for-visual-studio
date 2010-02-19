using System.Diagnostics;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui.jira;
using Atlassian.plvs.windows;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using AtlassianConstants = Atlassian.plvs.util.Constants;

namespace Atlassian.plvs.eventsinks {
    public sealed class TextMarkerClientEventSink : AbstractMarkerClientEventSink {
        private readonly string issueKey;

        public TextMarkerClientEventSink(string issueKey) {
            this.issueKey = issueKey;
        }

        public override int GetTipText(IVsTextMarker pMarker, string[] pbstrText) {
            if (issueKey != null) {
#if VS2010
                pbstrText[0] = "Right click for actions available on issue " + issueKey;
#else 
                pbstrText[0] = "Double click to try open " + issueKey +
                               "\non the currently selected server,\nRight click for more options";
#endif
            }

            return VSConstants.S_OK;
        }

        public override int GetMarkerCommandInfo(IVsTextMarker pMarker, int iItem, string[] pbstrText, uint[] pcmdf) {
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
                    return VSConstants.S_OK;

                default:
                    return VSConstants.S_FALSE;
            }
        }

        public override int ExecMarkerCommand(IVsTextMarker pMarker, int iItem) {
            switch (iItem) {
                case 0:
                    openInIde();
                    return VSConstants.S_OK;
                case 1:
                    launchBrowser();
                    return VSConstants.S_OK;

                case (int) MarkerCommandValues.mcvBodyDoubleClickCommand:
                    openInIde();
                    return VSConstants.S_OK;

                default:
                    return VSConstants.S_FALSE;
            }
        }

        private void openInIde() {
            if (issueKey == null) return;

            bool found = false;
            foreach (JiraIssue issue in JiraIssueListModelImpl.Instance.Issues) {
                if (!issue.Key.Equals(issueKey)) continue;
                IssueDetailsWindow.Instance.openIssue(issue);
                found = true;
                break;
            }
            if (!found) {
                AtlassianPanel.Instance.Jira.findAndOpenIssue(issueKey, findFinished);
            }
        }

        private static void findFinished(bool success, string message) {
            if (!success) {
                MessageBox.Show(message, AtlassianConstants.ERROR_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void launchBrowser() {
            if (issueKey == null) {
                return;
            }
            try {
                JiraServer server = AtlassianPanel.Instance.Jira.getCurrentlySelectedServer();
                if (server != null) {
                    Process.Start(server.Url + "/browse/" + issueKey);
                } else {
                    MessageBox.Show("No JIRA server selected", AtlassianConstants.ERROR_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
// ReSharper disable EmptyGeneralCatchClause
            catch {}
// ReSharper restore EmptyGeneralCatchClause
        }
    }
}