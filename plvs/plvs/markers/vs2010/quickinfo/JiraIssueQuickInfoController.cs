using System.Collections.Generic;
using System.Linq;
using Atlassian.plvs.markers.vs2010.texttag;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;

namespace Atlassian.plvs.markers.vs2010.quickinfo {
    internal class JiraIssueQuickInfoController : IIntellisenseController {
        private ITextView textView;
        private readonly IList<ITextBuffer> subjectBuffers;
        private readonly JiraIssueQuickInfoControllerProvider provider;

        internal JiraIssueQuickInfoController(ITextView textView, IList<ITextBuffer> subjectBuffers, JiraIssueQuickInfoControllerProvider provider) {
            this.textView = textView;
            this.subjectBuffers = subjectBuffers;
            this.provider = provider;

            this.textView.MouseHover += OnTextViewMouseHover;
        }

        private void OnTextViewMouseHover(object sender, MouseHoverEventArgs e) {
            SnapshotPoint? point = textView.BufferGraph.MapDownToFirstMatch(
                new SnapshotPoint(textView.TextSnapshot, e.Position),
                PointTrackingMode.Positive,
                snapshot => subjectBuffers.Contains(snapshot.TextBuffer),
                PositionAffinity.Predecessor);

            if (point == null) return;

            ITagAggregator<JiraIssueTextTag> aggregator = provider.TagAggregatorFactoryService.CreateTagAggregator<JiraIssueTextTag>(textView);
            IEnumerable<IMappingTagSpan<JiraIssueTextTag>> spans = aggregator.GetTags(new SnapshotSpan(new SnapshotPoint(textView.TextSnapshot, 0),
                                                                               textView.TextSnapshot.Length - 1));

            JiraIssueTextTag textTag = (from span in spans
                                let t = span.Tag
                                where t.Where.Start.Position <= point.Value.Position && t.Where.End.Position >= point.Value.Position
                                select span.Tag).FirstOrDefault();

            provider.InfoSourceProvider.currentTextTag = textTag;
            
            if (textTag == null) return;

            ITrackingPoint triggerPoint = point.Value.Snapshot.CreateTrackingPoint(point.Value.Position, PointTrackingMode.Positive);
            
            if (!provider.QuickInfoBroker.IsQuickInfoActive(textView)) {
                provider.QuickInfoBroker.TriggerQuickInfo(textView, triggerPoint, true);
            }
        }

        public void Detach(ITextView textview) {
            if (textView != textview) return;
            textView.MouseHover -= OnTextViewMouseHover;
            textView = null;
        }

        public void ConnectSubjectBuffer(ITextBuffer subjectBuffer) {
        }

        public void DisconnectSubjectBuffer(ITextBuffer subjectBuffer) {
        }
    }
}
