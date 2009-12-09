using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Atlassian.plvs.eventsinks {
    public sealed class MarginMarkerClientEventSink : AbstractMarkerClientEventSink {
        private readonly int numberOfIssues;

        public MarginMarkerClientEventSink(int numberOfIssues) {
            this.numberOfIssues = numberOfIssues;
        }

        public override int GetTipText(IVsTextMarker pMarker, string[] pbstrText) {
            pbstrText[0] = numberOfIssues > 1 ? "This line contains links to issues" : "This line contains a link to an issue";

            return VSConstants.S_OK;
        }

        public override int GetMarkerCommandInfo(IVsTextMarker pMarker, int iItem, string[] pbstrText, uint[] pcmdf) {
            return VSConstants.S_FALSE;
        }

        public override int ExecMarkerCommand(IVsTextMarker pMarker, int iItem) {
            return VSConstants.S_FALSE;
        }
    }
}