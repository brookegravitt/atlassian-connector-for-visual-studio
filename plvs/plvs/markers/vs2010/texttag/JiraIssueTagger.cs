using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.util;
using Atlassian.plvs.windows;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

namespace Atlassian.plvs.markers.vs2010.texttag {
    internal class JiraIssueTagger : ITagger<JiraIssueTag> {
        private readonly ITextBuffer buffer;
        private readonly IClassifier classifier;
        private bool disposed;

        internal JiraIssueTagger(ITextBuffer buffer, IClassifier classifier) {
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

        IEnumerable<ITagSpan<JiraIssueTag>> ITagger<JiraIssueTag>.GetTags(NormalizedSnapshotSpanCollection spans) {
            JiraServer selectedServer = AtlassianPanel.Instance.Jira.CurrentlySelectedServer;
            if (selectedServer == null) {
                yield break;
            }

            foreach (SnapshotSpan requestSpan in spans) {

                var startLine = requestSpan.Start.GetContainingLine();
                var endLine = (startLine.End >= requestSpan.End) ? startLine : requestSpan.End.GetContainingLine();
                SnapshotSpan span = new SnapshotSpan(startLine.Start, endLine.End);

                foreach (ClassificationSpan classification in classifier.GetClassificationSpans(span)) {
                    if (!classification.ClassificationType.Classification.ToLower().Contains("comment")) continue;
                    MatchCollection matches = Constants.ISSUE_KEY_REGEX.Matches(classification.Span.GetText());
                    foreach (Match match in matches.Cast<Match>().Where(match => match.Success)) {
                        string issueKey = classification.Span.GetText().Substring(match.Index, match.Length);
                        SnapshotSpan snapshotSpan = new SnapshotSpan(classification.Span.Start + match.Index, match.Length);
                        yield return new TagSpan<JiraIssueTag>(snapshotSpan, new JiraIssueTag(snapshotSpan, issueKey));
                    }
                }
            }
        }

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


