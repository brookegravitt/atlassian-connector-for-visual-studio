using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

namespace Atlassian.plvs.markers.vs2010.marginglyph {
    internal class JiraIssueLineGlyphTagger : LineTagger<JiraIssueLineGlyphTag> {
        public JiraIssueLineGlyphTagger(ITextBuffer buffer, IClassifier classifier) : base(buffer, classifier) {}

        private JiraIssueLineGlyphTag lastTag;

        protected override TagSpan<JiraIssueLineGlyphTag> getTagSpan(Match match, SnapshotSpan span, ClassificationSpan classification, int lastLine) {
            int currentLine = span.Start.GetContainingLine().LineNumber;
            string issueKey = classification.Span.GetText().Substring(match.Index, match.Length);
            // ugly, stateful code, which Darth would hate. Do I care? Take a guess
            if (lastLine == currentLine) {
                if (lastTag != null) {
                    lastTag.IssueKeys.Add(issueKey);
                }
                return null;
            }
            lastTag = new JiraIssueLineGlyphTag(span, new List<string> { issueKey });
            return new TagSpan<JiraIssueLineGlyphTag>(span, lastTag);
        }
    }
}


