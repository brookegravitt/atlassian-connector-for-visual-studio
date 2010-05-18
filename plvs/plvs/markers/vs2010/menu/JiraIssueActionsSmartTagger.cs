﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.dialogs;
using Atlassian.plvs.util.jira;
using Atlassian.plvs.windows;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;

namespace Atlassian.plvs.markers.vs2010.menu {
    class JiraIssueActionsSmartTagger : ITagger<JiraIssueActionsSmartTag>, IDisposable {
        private ITextView view;
        private bool disposed;

        private readonly IClassifier classifier;

        private readonly Dictionary<string, IEnumerable<ITagSpan<JiraIssueActionsSmartTag>>> tagCache =
            new Dictionary<string, IEnumerable<ITagSpan<JiraIssueActionsSmartTag>>>();

        public JiraIssueActionsSmartTagger(ITextView view, IClassifier classifier) {
            this.view = view;
            this.classifier = classifier;
            this.view.LayoutChanged += layoutChanged;
            view.Caret.PositionChanged += caretPositionChanged;
            AtlassianPanel.Instance.Jira.SelectedServerChanged += jiraSelectedServerChanged;
            GlobalSettings.SettingsChanged += globalSettingsChanged;
            view.TextBuffer.Changed += textBufferChanged;
        }

        private void textBufferChanged(object sender, TextContentChangedEventArgs e) {
//            DebugMon.Instance().addText(GetType().Name + " buffer_Changed(): " + view.TextBuffer + " changed, clearing tag cache");
            tagCache.Clear();
        }

        private void globalSettingsChanged(object sender, EventArgs e) {
            tagCache.Clear();
            updateTags();
        }

        private void jiraSelectedServerChanged(object sender, EventArgs e) {
            tagCache.Clear();
            updateTags();
        }

        private void updateTags() {
            if (view == null) {
                return;
            }
            ITextSnapshot snapshot = view.TextSnapshot;
            SnapshotSpan span = new SnapshotSpan(snapshot, new Span(0, snapshot.Length));
            EventHandler<SnapshotSpanEventArgs> handler = TagsChanged;
            if (handler != null) {
                handler(this, new SnapshotSpanEventArgs(span));
            }
        }

        public IEnumerable<ITagSpan<JiraIssueActionsSmartTag>> GetTags(NormalizedSnapshotSpanCollection spans) {
            string key = spans.ToString();
            if (!tagCache.ContainsKey(key)) {
//                DebugMon.Instance().addText(GetType().Name + ".GetTags(): spans: " + spans + " returning from tag cache");
                tagCache[key] = getTagsInternal(spans);
            }
            return tagCache[key];
        }

        public IEnumerable<ITagSpan<JiraIssueActionsSmartTag>> getTagsInternal(NormalizedSnapshotSpanCollection spans) {

//            DebugMon.Instance().addText(GetType().Name + ".GetTags(): spans: " + spans);

            if (!GlobalSettings.shouldShowIssueLinks(view.TextSnapshot.LineCount)) {
                yield break;
            }

            JiraServer selectedServer = AtlassianPanel.Instance.Jira.CurrentlySelectedServerOrDefault;
            if (selectedServer == null) {
                yield break;
            }

            ITextCaret caret = view.Caret;
            SnapshotPoint point;

            if (caret.Position.BufferPosition > 0)
                point = caret.Position.BufferPosition;
            else
                yield break;

            SortedDictionary<string, JiraProject> projects = JiraServerCache.Instance.getProjects(selectedServer);

            foreach (SnapshotSpan span in spans) {
                foreach (SnapshotSpan s in from classification in classifier.GetClassificationSpans(span)
                                           where classification.ClassificationType.Classification.ToLower().Contains("comment")
                                           let matches = JiraIssueUtils.ISSUE_REGEX.Matches(classification.Span.GetText())
                                           let c = classification
                                           from s in matches.Cast<Match>().Where(match => match.Success 
                                               && projects.ContainsKey(match.Groups[2].Value))
                                                    .Select(match => new SnapshotSpan(c.Span.Start + match.Index, match.Length))
                                           select s) {
                    if (s.Start.Position <= point.Position && s.End.Position >= point.Position) {
                        yield return new TagSpan<JiraIssueActionsSmartTag>(s, new JiraIssueActionsSmartTag(getSmartTagActions(s)));
                    }
                }
            }
        }

        private static ReadOnlyCollection<SmartTagActionSet> getSmartTagActions(SnapshotSpan span) {
            List<SmartTagActionSet> actionSetList = new List<SmartTagActionSet>();
            List<ISmartTagAction> actionList = new List<ISmartTagAction>();

            ITrackingSpan trackingSpan = span.Snapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgeInclusive);
            actionList.Add(new OpenIssueInIdeSmartTagAction(trackingSpan));
            actionList.Add(new OpenIssueInBrowserSmartTagAction(trackingSpan));
            SmartTagActionSet actionSet = new SmartTagActionSet(actionList.AsReadOnly());
            actionSetList.Add(actionSet);
            return actionSetList.AsReadOnly();
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        private void layoutChanged(object sender, TextViewLayoutChangedEventArgs e) {
            ITextSnapshot snapshot = e.NewSnapshot;
            invokeTagsChanged(snapshot);
        }

        private void caretPositionChanged(object sender, CaretPositionChangedEventArgs e) {
            ITextSnapshot snapshot = view.TextSnapshot;
            invokeTagsChanged(snapshot);
        }

        private void invokeTagsChanged(ITextSnapshot snapshot) {
            SnapshotSpan span = new SnapshotSpan(snapshot, new Span(0, snapshot.Length));
            EventHandler<SnapshotSpanEventArgs> handler = TagsChanged;
            if (handler != null) {
                handler(this, new SnapshotSpanEventArgs(span));
            }
        }


        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {
            if (disposed) return;
            if (disposing) {
                view.LayoutChanged -= layoutChanged;
                view.Caret.PositionChanged -= caretPositionChanged;
                AtlassianPanel.Instance.Jira.SelectedServerChanged -= jiraSelectedServerChanged;
                GlobalSettings.SettingsChanged -= globalSettingsChanged;
                view.TextBuffer.Changed -= textBufferChanged;
                view = null;
            }

            disposed = true;
        }
    }
}
