using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Atlassian.plvs.util;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

namespace Atlassian.plvs.markers.vs2010.texttag {
    internal class JiraIssueTagger : ITagger<JiraIssueTag> {
//        private readonly ITextView view;
        private readonly IClassifier classifier;

        internal JiraIssueTagger(IClassifier classifier) {
//            this.view = view;
            this.classifier = classifier;

//            AtlassianPanel.Instance.Jira.SelectedServerChanged += jiraSelectedServerChanged;
        }

#if false
        private void jiraSelectedServerChanged(object sender, EventArgs e) {
            if (view == null) {
                return;
            }
            ITextSnapshot snapshot = view.TextSnapshot;
            SnapshotSpan requestSpan = new SnapshotSpan(snapshot, new Span(0, snapshot.Length));

            var startLine = requestSpan.Start.GetContainingLine();
            var endLine = (startLine.End >= requestSpan.End) ? startLine : requestSpan.End.GetContainingLine();
            SnapshotSpan span = new SnapshotSpan(startLine.Start, endLine.End);

            EventHandler<SnapshotSpanEventArgs> handler = TagsChanged;
            if (handler != null) {
                handler(this, new SnapshotSpanEventArgs(span));
            }
        }
#endif 

        IEnumerable<ITagSpan<JiraIssueTag>> ITagger<JiraIssueTag>.GetTags(NormalizedSnapshotSpanCollection spans) {
#if false
            JiraServer selectedServer = AtlassianPanel.Instance.Jira.getCurrentlySelectedServer();
            if (selectedServer == null) {
                yield break;
            }
#endif

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
    }
}


