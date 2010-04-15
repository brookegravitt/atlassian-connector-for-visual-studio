using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.util.jira;
using Atlassian.plvs.windows;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

namespace Atlassian.plvs.markers.vs2010 {
    internal abstract class LineTagger<T> : ITagger<T> where T : ITag {
        private readonly ITextBuffer buffer;
        private readonly IClassifier classifier;
        private bool disposed;

        internal LineTagger(ITextBuffer buffer, IClassifier classifier) {
            this.buffer = buffer;
            this.classifier = classifier;

            AtlassianPanel.Instance.Jira.SelectedServerChanged += jiraSelectedServerChanged;
        }

        private void jiraSelectedServerChanged(object sender, EventArgs e) {
            var snapshot = buffer.CurrentSnapshot;

            EventHandler<SnapshotSpanEventArgs> handler = TagsChanged;
            if (handler != null) {
                handler(this, new SnapshotSpanEventArgs(new SnapshotSpan(snapshot, 0, snapshot.Length)));
            }
        }

        IEnumerable<ITagSpan<T>> ITagger<T>.GetTags(NormalizedSnapshotSpanCollection spans) {
            JiraServer selectedServer = AtlassianPanel.Instance.Jira.CurrentlySelectedServerOrDefault;
            if (selectedServer == null) {
                yield break;
            }

            int lastLine = -1;
            foreach (SnapshotSpan requestSpan in spans) {

                var startLine = requestSpan.Start.GetContainingLine();
                var endLine = (startLine.End >= requestSpan.End) ? startLine : requestSpan.End.GetContainingLine();
                SnapshotSpan span = new SnapshotSpan(startLine.Start, endLine.End);

                foreach (ClassificationSpan classification in classifier.GetClassificationSpans(span)) {
                    if (!classification.ClassificationType.Classification.ToLower().Contains("comment")) continue;
                    MatchCollection matches = JiraIssueUtils.ISSUE_REGEX.Matches(classification.Span.GetText());
                    foreach (Match match in matches.Cast<Match>().Where(match => match.Success)) {
                        SortedDictionary<string, JiraProject> projects = JiraServerCache.Instance.getProjects(selectedServer);
                        if (!projects.ContainsKey(match.Groups[2].Value)) continue;

                        SnapshotSpan snapshotSpan = new SnapshotSpan(classification.Span.Start + match.Index, match.Length);

                        TagSpan<T> tagSpan = getTagSpan(match, snapshotSpan, classification, lastLine);
                        lastLine = span.Start.GetContainingLine().LineNumber;
                        if (tagSpan != null) yield return tagSpan;
                    }
                }
            }
        }

        protected abstract TagSpan<T> getTagSpan(Match match, SnapshotSpan span, ClassificationSpan classification, int lastLine);

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {
            if (disposed) return;

            if (disposing) {
                AtlassianPanel.Instance.Jira.SelectedServerChanged -= jiraSelectedServerChanged;
            }

            disposed = true;
        }
    }
}
