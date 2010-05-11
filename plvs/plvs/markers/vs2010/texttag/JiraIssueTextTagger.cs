using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

namespace Atlassian.plvs.markers.vs2010.texttag {
    internal class JiraIssueTextTagger : LineTagger<JiraIssueTextTag> {
        public JiraIssueTextTagger(ITextBuffer buffer, IClassifier classifier) : base(buffer, classifier) {}

        protected override TagSpan<JiraIssueTextTag> getTagSpan(Match match, SnapshotSpan span, ClassificationSpan classification, int lastLine) {
            string issueKey = classification.Span.GetText().Substring(match.Index, match.Length);
            return new TagSpan<JiraIssueTextTag>(span, new JiraIssueTextTag(span, issueKey));
        }
    }
}


