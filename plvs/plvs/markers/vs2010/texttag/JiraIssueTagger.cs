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
        private readonly IClassifier classifier;

        internal JiraIssueTagger(IClassifier classifier) {
            this.classifier = classifier;
        }

        IEnumerable<ITagSpan<JiraIssueTag>> ITagger<JiraIssueTag>.GetTags(NormalizedSnapshotSpanCollection spans) {
            foreach (SnapshotSpan span in spans) {
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


