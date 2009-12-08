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
            // We have to keep track of the number of views that are currently open per
            // document. That way we can discover when a document is opened and insert
            // our custom text markers.
            IVsTextLines buffer;
            ErrorHandler.ThrowOnFailure(pView.GetBuffer(out buffer));
            if (buffer == null)
                return;

            // Increment the stored view count.
            int documentViewCount;
            documentViewCounts.TryGetValue(buffer, out documentViewCount);
            documentViewCounts[buffer] = documentViewCount + 1;

            // If this view belongs to a document that had no views before the document
            // has been opened. It's time to notify the JiraEditorLinkManager about it.
            // However there is a problem: The text buffer represented by textLines has
            // not been initialized yet, i.e. the file name is not set and the file
            // content is not loaded yet. So we need to subscribe to the text buffer to
            // get notified when the file load procedure completed.
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
            // It's interesting to us when a document is closed because we need this
            // information to keep track when a document is opened. Furthermore we
            // have to free all text markers.
            IVsTextLines buffer;
            ErrorHandler.ThrowOnFailure(pView.GetBuffer(out buffer));
            if (buffer == null)
                return;

            // Decrement the stored view count. This is a little bit special as we use
            // IVsTextLines instances as keys in our dictionary. That means that we
            // have to remove the whole entry from the dictionary when the counter drops
            // to zero to prevent memory leaks.
            int documentViewCount;

            if (!documentViewCounts.TryGetValue(buffer, out documentViewCount)) return;

            if (documentViewCount > 1) {
                // There are several open views for the same document. In this case
                // we only have to decrement the view count.
                documentViewCounts[buffer] = documentViewCount - 1;
            }
            else {
                // When we reach this branch the last view of a document has been
                // closed. That means we have to free the whole IVsTextLines reference
                // by removing it from the dictionary.
                documentViewCounts.Remove(buffer);

                // Notify the CloneDetectiveManager of this event.
                JiraEditorLinkManager.OnDocumentClosed(buffer);
            }
        }

        public void OnUserPreferencesChanged(VIEWPREFERENCES[] pViewPrefs, FRAMEPREFERENCES[] pFramePrefs,
                                             LANGPREFERENCES[] pLangPrefs, FONTCOLORPREFERENCES[] pColorPrefs) {}

        #endregion
    }
}