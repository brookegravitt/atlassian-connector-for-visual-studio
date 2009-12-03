using Atlassian.plvs.eventsinks;
using Atlassian.plvs.markers;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Atlassian.plvs {
    internal class JiraEditorLinkManager {
        public static void OnSolutionOpened() {}

        public static void OnSolutionClosed() {}

        public static void OnDocumentClosed(IVsTextLines lines) {}

        public static void OnDocumentOpened(IVsTextLines lines) {
            addMarkersToDocument(lines);
        }

        public static void OnDocumentSaved(uint cookie) {}

        private static void addMarkersToDocument(IVsTextLines textLines) {
            int lineCount;
            textLines.GetLineCount(out lineCount);
            for (int i = 0; i < lineCount; ++i) {
                string text;
                int len;
                textLines.GetLengthOfLine(i, out len);
                textLines.GetLineText(i, 0, i, len, out text);
                string cmt = "//";
                string issueKey = "PL-1357";
                if (text == null || !text.Contains(cmt) || !text.Contains(issueKey)) continue;

                int cmtIdx = text.IndexOf(cmt);
                int idx = text.IndexOf(issueKey);

                if (idx < cmtIdx) continue;

                addMarker(textLines, i, idx, idx + issueKey.Length);
            }
        }

        private static void addMarker(IVsTextLines textLines, int line, int start, int end) {
            TextMarkerClientEventSink clientEventSinkMargin = new TextMarkerClientEventSink();
            TextMarkerClientEventSink clientEventSinkText = new TextMarkerClientEventSink();

            IVsTextLineMarker[] markers = new IVsTextLineMarker[1];

            int hr = textLines.CreateLineMarker(JiraLinkMarginMarkerType.Id, line, start, line, end,
                                                clientEventSinkMargin, markers);
            if (!ErrorHandler.Succeeded(hr)) return;
            clientEventSinkMargin.MarginMarker = markers[0];

            hr = textLines.CreateLineMarker(JiraLinkTextMarkerType.Id, line, start, line, end, clientEventSinkText,
                                            markers);

            if (!ErrorHandler.Succeeded(hr)) return;
            clientEventSinkText.TextMarker = markers[0];
        }

        public static void OnMarkerInvalidated(IVsTextLineMarker marker) {}
    }
}