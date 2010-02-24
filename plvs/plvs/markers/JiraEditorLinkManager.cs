using System;
using System.Text.RegularExpressions;
using Atlassian.plvs.eventsinks;
using Atlassian.plvs.util.jira;
using Atlassian.plvs.windows;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Atlassian.plvs.markers {
    internal class JiraEditorLinkManager {

        private static readonly Regex BlockInOneLine = new Regex(@"/\*(.*)\*/");
        private static readonly Regex BlockCommentStarted = new Regex(@"/\*(.*)");
        private static readonly Regex BlockCommentEnded = new Regex(@"(.*)\*/");

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
            if (AtlassianPanel.Instance.Jira != null && AtlassianPanel.Instance.Jira.CurrentlySelectedServer != null) {
                addMarkersToDocument(lines);
            }
        }

        public static void OnDocumentSaved(uint cookie) {}

        public static void OnMarkerInvalidated(IVsTextLineMarker marker) {}

        public static void OnDocumentChanged(IVsTextLines textLines) {
            if (!(isCSharp(textLines) || isVb(textLines))) return;

            cleanupMarkers(textLines, JiraLinkTextMarkerType.Id);
            cleanupMarkers(textLines, JiraLinkMarginMarkerType.Id);

            if (AtlassianPanel.Instance.Jira != null && AtlassianPanel.Instance.Jira.CurrentlySelectedServer != null) {
                addMarkersToDocument(textLines);
            }
        }

        private static void addMarkersToDocument(IVsTextLines textLines) {
            int lineCount;
            textLines.GetLineCount(out lineCount);

            CommentStrings commentMarkers = getCommentMarkerStrings(textLines);

            bool isInBlockComment = false;
            for (int lineNumber = 0; lineNumber < lineCount; ++lineNumber) {
                string text;
                int lineLength;

                int issueCount = 0;

                textLines.GetLengthOfLine(lineNumber, out lineLength);
                textLines.GetLineText(lineNumber, 0, lineNumber, lineLength, out text);

                if (text == null) continue;

                if (commentMarkers.BlockOpen != null && commentMarkers.BlockClose != null) {
                    int current = 0;
                    MatchCollection matches;
                    if (isInBlockComment) {
                        matches = BlockCommentEnded.Matches(text);
                        if (matches.Count > 0) {
                            scanCommentedLine(textLines, lineNumber, matches[0].Value, 0, ref issueCount);
                            current = matches[0].Length;
                            isInBlockComment = false;
                        } else {
                            scanCommentedLine(textLines, lineNumber, text, 0, ref issueCount);
                            maybeAddMarginMarker(textLines, lineNumber, lineLength, issueCount);
                            continue;
                        }
                    } else {
                        if (scanForLineComment(textLines, lineNumber, text, commentMarkers, ref issueCount)) {
                            maybeAddMarginMarker(textLines, lineNumber, lineLength, issueCount);
                            continue;
                        }
                    }

                    matches = BlockInOneLine.Matches(text, current);
                    for (int i = 0; i < matches.Count; ++i) {
                        scanCommentedLine(textLines, lineNumber, matches[i].Value, matches[i].Index, ref issueCount);
                        current = matches[i].Index + matches[i].Length;
                    }

                    if (scanForLineComment(textLines, lineNumber, text, commentMarkers, ref issueCount)) {
                        maybeAddMarginMarker(textLines, lineNumber, lineLength, issueCount);
                        continue;
                    }

                    matches = BlockCommentStarted.Matches(text, current);
                    if (matches.Count > 0) {
                        isInBlockComment = true;
                        scanCommentedLine(textLines, lineNumber, matches[0].Value, matches[0].Index, ref issueCount);
                    }
                } else {
                    if (commentMarkers.Line == null) {
                        maybeAddMarginMarker(textLines, lineNumber, lineLength, issueCount);
                        continue;
                    }

                    scanForLineComment(textLines, lineNumber, text, commentMarkers, ref issueCount);
                }
                maybeAddMarginMarker(textLines, lineNumber, lineLength, issueCount);
            }
        }

        private static void maybeAddMarginMarker(IVsTextLines textLines, int lineNumber, int lineLength, int issueCount) {
            if (issueCount > 0)
                addMarker(textLines, lineNumber, 0, lineLength, JiraLinkMarginMarkerType.Id, new MarginMarkerClientEventSink(issueCount));
        }

        private static bool scanForLineComment(IVsTextLines textLines, int lineNumber, string text, CommentStrings commentMarkers, ref int count) {
            int lineCmtIdx = text.IndexOf(commentMarkers.Line);
            return lineCmtIdx != -1 && scanCommentedLine(textLines, lineNumber, text.Substring(lineCmtIdx), lineCmtIdx, ref count);
        }

        private static bool scanCommentedLine(IVsTextLines textLines, int lineNumber, string text, int offset, ref int count) {
            MatchCollection matches = JiraIssueUtils.ISSUE_REGEX.Matches(text);
            if (matches.Count > 0) {
                count += matches.Count;
            }
            for (int j = 0; j < matches.Count; ++j) {
                int index = matches[j].Index + offset;
                addMarker(textLines, lineNumber, index, index + matches[j].Length, JiraLinkTextMarkerType.Id,
                          new TextMarkerClientEventSink(matches[j].Value));
            }
            return matches.Count > 0;
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
                                      AbstractMarkerClientEventSink client) {
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