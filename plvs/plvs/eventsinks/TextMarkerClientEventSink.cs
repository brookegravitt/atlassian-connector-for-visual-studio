using System;
using System.Diagnostics;
using Atlassian.plvs.api;
using Atlassian.plvs.models;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Atlassian.plvs.eventsinks {
    public sealed class TextMarkerClientEventSink : IVsTextMarkerClient {
        public IVsTextLineMarker TextMarker { get; set; }
        public IVsTextLineMarker MarginMarker { get; set; }

        #region IVsTextMarkerClient Members

        public void MarkerInvalidated() {
            JiraEditorLinkManager.OnMarkerInvalidated(TextMarker);
            JiraEditorLinkManager.OnMarkerInvalidated(MarginMarker);
        }

        public int GetTipText(IVsTextMarker pMarker, string[] pbstrText) {
            if (TextMarker != null)
                pbstrText[0] = "Double click to try open PL-1357\non the currently selected server,\nRight click for more options";
            else if (MarginMarker != null)
                pbstrText[0] = "This line contains a link to issue PL-1357";

            return VSConstants.S_OK;
        }

        public void OnBufferSave(string pszFileName) {}

        public void OnBeforeBufferClose() {}

        public int GetMarkerCommandInfo(IVsTextMarker pMarker, int iItem, string[] pbstrText, uint[] pcmdf) {
            // For each command we add we have to specify that we support it.
            // Furthermore it should always be enabled.
            const uint menuItemFlags = (uint) (OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED);

            if (pcmdf == null)
                return VSConstants.S_FALSE;

            switch (iItem) {
                case 0:
                    if (pbstrText != null)
                    {
                        pbstrText[0] = "Open Issue in IDE";
                        pcmdf[0] = menuItemFlags;
                        return VSConstants.S_OK;
                    }
                    return VSConstants.S_FALSE;
                case 1:
                    if (pbstrText != null) {
                        pbstrText[0] = "View Issue in the Browser";
                        pcmdf[0] = menuItemFlags;
                        return VSConstants.S_OK;
                    }
                    return VSConstants.S_FALSE;

                case (int) MarkerCommandValues.mcvBodyDoubleClickCommand:
                    pcmdf[0] = menuItemFlags;
                    return (TextMarker != null) ? VSConstants.S_OK : VSConstants.S_FALSE;

//                case (int) MarkerCommandValues.mcvGlyphSingleClickCommand:
//                    pcmdf[0] = menuItemFlags;
//                    return (MarginMarker != null) ? VSConstants.S_OK : VSConstants.S_FALSE;

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
                    if (TextMarker != null) openInIde();
                    return VSConstants.S_OK;

//                case (int) MarkerCommandValues.mcvGlyphSingleClickCommand:
//                    if (MarginMarker != null) launchBrowser();
//                    return VSConstants.S_FALSE;

                default:
                    return VSConstants.S_FALSE;
            }
        }

        private static void openInIde() {
            bool found = false;
            foreach (JiraIssue issue in JiraIssueListModel.Instance.Issues) {
                if (!issue.Key.Equals("PL-1357") || !issue.Server.Url.Equals("https://studio.atlassian.com")) continue;
                IssueDetailsWindow.Instance.openIssue(issue);
                found = true;
                break;
            }
            if (!found) {
                IssueListWindow.Instance.findAndOpenIssue("PL-1357", null);
            }
        }

        private static void launchBrowser() {
            try {
                Process.Start("https://studio.atlassian.com/browse/PL-1357");
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