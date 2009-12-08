using System;
using System.Collections.Generic;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Atlassian.plvs.eventsinks {
    internal sealed class TextManagerEventSink : IVsTextManagerEvents {
        private readonly Dictionary<IVsTextLines, int> documentViewCounts = new Dictionary<IVsTextLines, int>();

        #region IVsTextManagerEvents Members

        public void OnRegisterMarkerType(int iMarkerType) {}

        public void OnRegisterView(IVsTextView pView) {

            IVsTextLines buffer;
            ErrorHandler.ThrowOnFailure(pView.GetBuffer(out buffer));
            if (buffer == null)
                return;

            int documentViewCount;
            documentViewCounts.TryGetValue(buffer, out documentViewCount);
            documentViewCounts[buffer] = documentViewCount + 1;

            if (documentViewCount != 0) return;

            IConnectionPoint connectionPointBufferDataEvents;
            IConnectionPoint connectionPointTextLinesEvents;
            uint cookie;

            IConnectionPointContainer container = (IConnectionPointContainer) buffer;

            Guid textBufferDataEventsGuid = typeof (IVsTextBufferDataEvents).GUID;
            container.FindConnectionPoint(ref textBufferDataEventsGuid, out connectionPointBufferDataEvents);
            TextBufferDataEventSink textBufferDataEventSink = new TextBufferDataEventSink();
            connectionPointBufferDataEvents.Advise(textBufferDataEventSink, out cookie);
            textBufferDataEventSink.TextLines = buffer;
            textBufferDataEventSink.ConnectionPoint = connectionPointBufferDataEvents;
            textBufferDataEventSink.Cookie = cookie;

            Guid eventsGuid = typeof(IVsTextLinesEvents).GUID;
            container.FindConnectionPoint(ref eventsGuid, out connectionPointTextLinesEvents); 
            TextLinesEventSink textLinesEventSink = new TextLinesEventSink();
            connectionPointTextLinesEvents.Advise(textLinesEventSink, out cookie);
            textLinesEventSink.TextLines = buffer;
            textLinesEventSink.ConnectionPoint = connectionPointTextLinesEvents;
            textLinesEventSink.Cookie = cookie;
        }

        public void OnUnregisterView(IVsTextView pView) {
            IVsTextLines buffer;
            ErrorHandler.ThrowOnFailure(pView.GetBuffer(out buffer));
            if (buffer == null)
                return;

            int documentViewCount;

            if (!documentViewCounts.TryGetValue(buffer, out documentViewCount)) return;

            if (documentViewCount > 1) {
                documentViewCounts[buffer] = documentViewCount - 1;
            }
            else {
                documentViewCounts.Remove(buffer);
                JiraEditorLinkManager.OnDocumentClosed(buffer);
            }
        }

        public void OnUserPreferencesChanged(VIEWPREFERENCES[] pViewPrefs, FRAMEPREFERENCES[] pFramePrefs,
                                             LANGPREFERENCES[] pLangPrefs, FONTCOLORPREFERENCES[] pColorPrefs) {}

        #endregion
    }
}