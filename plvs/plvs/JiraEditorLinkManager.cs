using System;
using System.Text.RegularExpressions;
using Atlassian.plvs.eventsinks;
using Atlassian.plvs.markers;
using Atlassian.plvs.util;
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

        public static void OnMarkerInvalidated(IVsTextLineMarker marker) {}

        public static void OnDocumentChanged(IVsTextLines textLines) {
            cleanupMarkers(textLines, JiraLinkTextMarkerType.Id);
            cleanupMarkers(textLines, JiraLinkMarginMarkerType.Id);
            addMarkersToDocument(textLines);
        }

        private static void addMarkersToDocument(IVsTextLines textLines) {
            int lineCount;
            textLines.GetLineCount(out lineCount);
            for (int i = 0; i < lineCount; ++i) {
                string text;
                int len;
                textLines.GetLengthOfLine(i, out len);
                textLines.GetLineText(i, 0, i, len, out text);
                const string cmt = "//";
                if (text == null || !text.Contains(cmt)) {
                    continue;
                }

                int cmtIdx = text.IndexOf(cmt);

                MatchCollection matches = JiraIssueUtils.ISSUE_REGEX.Matches(text);
                if (matches.Count > 0) {
                    addMarker(textLines, i, 0, len, JiraLinkMarginMarkerType.Id, new TextMarkerClientEventSink(true, null));
                }
                for (int j = 0; j < matches.Count; ++j) {
                    int index = matches[j].Index;
                    if (index < cmtIdx) {
                        continue;
                    }
                    addMarker(textLines, i, index, index + matches[j].Length, JiraLinkTextMarkerType.Id, new TextMarkerClientEventSink(false, matches[j].Value));
                }
            }
        }

        private static void addMarker(IVsTextLines textLines, int line, int start, int end, int markerType, TextMarkerClientEventSink client) {
            IVsTextLineMarker[] markers = new IVsTextLineMarker[1];
            int hr = textLines.CreateLineMarker(markerType, line, start, line, end, client, markers);
            if (!ErrorHandler.Succeeded(hr)) return;
            client.Marker = markers[0];
        }

        private static void cleanupMarkers(IVsTextLines textLines, int markerType) {
            IVsEnumLineMarkers markers;
            textLines.EnumMarkers(0, 0, 0, 0, markerType, (uint) ENUMMARKERFLAGS.EM_ENTIREBUFFER, out markers);

            int count;
            markers.GetCount(out count);

            for (int i = 0; i < count; ++i) {
                IVsTextLineMarker marker;
                markers.Next(out marker);
                if (marker != null) {
                    marker.Invalidate();
                }
            }
        }
    }
}