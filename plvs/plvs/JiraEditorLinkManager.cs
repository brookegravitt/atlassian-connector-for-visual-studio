using System;
using System.Text.RegularExpressions;
using Atlassian.plvs.eventsinks;
using Atlassian.plvs.markers;
using Atlassian.plvs.util;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Atlassian.plvs {
    internal class JiraEditorLinkManager {
        
        private class CommentStrings {
            public CommentStrings(string line, string blockOpen, string blockClose) {
                Line = line;
                BlockOpen = blockOpen;
                BlockClose = blockClose;
            }

            public CommentStrings() {}

            public readonly string Line;
            public readonly string BlockOpen;
            public readonly string BlockClose;
        }

        public static void OnSolutionOpened() {}

        public static void OnSolutionClosed() {}

        public static void OnDocumentClosed(IVsTextLines lines) {}

        public static void OnDocumentOpened(IVsTextLines lines) {
            if (!(isCSharp(lines) || isVb(lines))) return;
            addMarkersToDocument(lines);
        }

        public static void OnDocumentSaved(uint cookie) {}

        public static void OnMarkerInvalidated(IVsTextLineMarker marker) {}

        public static void OnDocumentChanged(IVsTextLines textLines) {
            if (!(isCSharp(textLines) || isVb(textLines))) return;

            cleanupMarkers(textLines, JiraLinkTextMarkerType.Id);
            cleanupMarkers(textLines, JiraLinkMarginMarkerType.Id);
            addMarkersToDocument(textLines);
        }

        private static readonly Regex blockInOneLine = new Regex(@"/*(.*)*/");
        private static readonly Regex blockCommentStarted = new Regex(@"/*(.*)");
        private static readonly Regex blockCommentEnded = new Regex(@"(.*)*/");

        private static void addMarkersToDocument(IVsTextLines textLines) {
            int lineCount;
            textLines.GetLineCount(out lineCount);

            CommentStrings commentMarkers = getCommentMarkerStrings(textLines);

            bool isInBlockComment = false;
            for (int lineNumber = 0; lineNumber < lineCount; ++lineNumber) {
                string text;
                int lineLength;
                textLines.GetLengthOfLine(lineNumber, out lineLength);
                textLines.GetLineText(lineNumber, 0, lineNumber, lineLength, out text);

                if (text == null) continue;

                //
                // TEST ME!!!!
                //
//                if (commentMarkers.BlockOpen != null && commentMarkers.BlockClose != null) {
//                    int current = 0;
//                    MatchCollection matches;
//                    if (isInBlockComment) {
//                        matches = blockCommentEnded.Matches(text);
//                        if (matches.Count > 0) {
//                            scanCommentedLine(textLines, lineNumber, lineLength, matches[0].Value, 0);
//                            current = matches[0].Length;
//                        } else {
//                            scanCommentedLine(textLines, lineNumber, lineLength, text, 0);
//                            continue;
//                        }
//                    }
//
//                    matches = blockInOneLine.Matches(text, current);
//                    for (int i = 0; i < matches.Count; ++i) {
//                        scanCommentedLine(textLines, lineNumber, lineLength, matches[i].Value, matches[i].Index);
//                        current = matches[i].Index + matches[i].Length;
//                    }
//
//                    matches = blockCommentStarted.Matches(text, current);
//                    if (matches.Count > 0) {
//                        isInBlockComment = true;
//                        scanCommentedLine(textLines, lineNumber, lineLength, matches[0].Value, current);
//                    }
//                }

                if (isInBlockComment || commentMarkers.Line == null) continue;
                int lineCmtIdx = text.IndexOf(commentMarkers.Line);
                if (lineCmtIdx != -1) {
                    scanCommentedLine(textLines, lineNumber, lineLength, text.Substring(lineCmtIdx), lineCmtIdx);
                }
            }
        }

        private static void scanCommentedLine(IVsTextLines textLines, int lineNumber, int lineLength, string text, int offset) {//}, int cmtIdx) {
            MatchCollection matches = JiraIssueUtils.ISSUE_REGEX.Matches(text);
            if (matches.Count > 0) {
                addMarker(textLines, lineNumber, 0, lineLength, JiraLinkMarginMarkerType.Id, new TextMarkerClientEventSink(true, null));
            }
            for (int j = 0; j < matches.Count; ++j) {
                int index = matches[j].Index + offset;
                addMarker(textLines, lineNumber, index, index + matches[j].Length, JiraLinkTextMarkerType.Id,
                          new TextMarkerClientEventSink(false, matches[j].Value));
            }
        }

        private static CommentStrings getCommentMarkerStrings(IVsTextLines lines) {
            if (isCSharp(lines)) return new CommentStrings("//", "/*", "*/");
            if (isVb(lines)) return new CommentStrings("'", null, null);
            return new CommentStrings();
        }

        private static bool isCSharp(IVsTextLines textLines) {
            Guid languageServiceId;
            textLines.GetLanguageServiceID(out languageServiceId);
            return GuidList.CSHARP_LANGUAGE_GUID.Equals(languageServiceId);
        }

        private static bool isVb(IVsTextLines textLines) {
            Guid languageServiceId;
            textLines.GetLanguageServiceID(out languageServiceId);
            return GuidList.VB_LANGUAGE_GUID.Equals(languageServiceId);
        }

        private static void addMarker(IVsTextLines textLines, int line, int start, int end, int markerType,
                                      TextMarkerClientEventSink client) {
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