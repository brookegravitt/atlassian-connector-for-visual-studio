using Atlassian.plvs.markers;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Atlassian.plvs.eventsinks {
    public sealed class TextBufferDataEventSink : IVsTextBufferDataEvents {
        public IVsTextLines TextLines { get; set; }

        public IConnectionPoint ConnectionPoint { get; set; }

        public uint Cookie { get; set; }

        public void OnFileChanged(uint grfChange, uint dwFileAttrs) {}

        public int OnLoadCompleted(int fReload) {
            ConnectionPoint.Unadvise(Cookie);
            JiraEditorLinkManager.OnDocumentOpened(TextLines);

            return VSConstants.S_OK;
        }
    }
}