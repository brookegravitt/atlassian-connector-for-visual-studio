using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Collections.ObjectModel;
using System.Windows.Media;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Text.Classification;

namespace EditorClassifierTest {
    internal class TestSmartTag : SmartTag {
        public TestSmartTag(ReadOnlyCollection<SmartTagActionSet> actionSets) : base(SmartTagType.Factoid, actionSets) { }
    }

    internal class TestSmartTagger : ITagger<TestSmartTag>, IDisposable {
        private ITextBuffer m_buffer;
        private ITextView m_view;
        private TestSmartTaggerProvider m_provider;
        private bool m_disposed;

        private IClassifier m_classifier;

        private static Regex issueKeyRegex = new Regex(@"([A-Z]+-[0-9]+)");

        public TestSmartTagger(ITextView view, IClassifier classifier) {
            m_view = view;
            m_classifier = classifier;
            m_view.LayoutChanged += OnLayoutChanged;
        }

#if false
        public TestSmartTagger(ITextBuffer buffer, ITextView view, TestSmartTaggerProvider provider) {
            m_buffer = buffer;
            m_view = view;
            m_provider = provider;
        }
#endif

        public IEnumerable<ITagSpan<TestSmartTag>> GetTags(NormalizedSnapshotSpanCollection spans) {
            foreach (SnapshotSpan span in spans) {
                foreach (ClassificationSpan classification in m_classifier.GetClassificationSpans(span)) {
                    if (classification.ClassificationType.Classification.ToLower().Contains("comment")) {
                        Match m = issueKeyRegex.Match(classification.Span.GetText());
                        if (m.Success) {
                            SnapshotSpan s = new SnapshotSpan(classification.Span.Start + m.Index, m.Length);
                            yield return new TagSpan<TestSmartTag>(s, new TestSmartTag(GetSmartTagActions(s)));
                        }
                    }
                }
            }



#if false
            ITextSnapshot snapshot = m_buffer.CurrentSnapshot;
            if (snapshot.Length == 0)
                yield break; //don't do anything if the buffer is empty

            //set up the navigator
            ITextStructureNavigator navigator = m_provider.NavigatorService.GetTextStructureNavigator(m_buffer);

            foreach (var span in spans) {
                ITextCaret caret = m_view.Caret;
                SnapshotPoint point;

                if (caret.Position.BufferPosition > 0)
                    point = caret.Position.BufferPosition - 1;
                else
                    yield break;

                TextExtent extent = navigator.GetExtentOfWord(point);
                //don't display the tag if the extent has whitespace
                if (extent.IsSignificant)
                    yield return new TagSpan<TestSmartTag>(extent.Span, new TestSmartTag(GetSmartTagActions(extent.Span)));
                else yield break;
            }
#endif
        }

        private ReadOnlyCollection<SmartTagActionSet> GetSmartTagActions(SnapshotSpan span) {
            List<SmartTagActionSet> actionSetList = new List<SmartTagActionSet>();
            List<ISmartTagAction> actionList = new List<ISmartTagAction>();

            ITrackingSpan trackingSpan = span.Snapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgeInclusive);
            actionList.Add(new LowerCaseSmartTagAction(trackingSpan));
            SmartTagActionSet actionSet = new SmartTagActionSet(actionList.AsReadOnly());
            actionSetList.Add(actionSet);
            return actionSetList.AsReadOnly();
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e) {
            ITextSnapshot snapshot = e.NewSnapshot;
            SnapshotSpan span = new SnapshotSpan(snapshot, new Span(0, snapshot.Length));
            EventHandler<SnapshotSpanEventArgs> handler = this.TagsChanged;
            if (handler != null) {
                handler(this, new SnapshotSpanEventArgs(span));
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {
            if (!this.m_disposed) {
                if (disposing) {
                    m_view.LayoutChanged -= OnLayoutChanged;
                    m_view = null;
                }

                m_disposed = true;
            }
        }
    }

    [Export(typeof(IViewTaggerProvider))]
    [ContentType("text")]
    [Order(Before = "default")]
    [TagType(typeof(SmartTag))]
    internal class TestSmartTaggerProvider : IViewTaggerProvider {
        [Import(typeof(ITextStructureNavigatorSelectorService))]
        internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }

        [Import]
        internal IClassifierAggregatorService AggregatorService;

        public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag {
            if (buffer == null || textView == null) {
                return null;
            }

 
            //make sure we are tagging only the top buffer
            if (buffer == textView.TextBuffer) {
                return new TestSmartTagger(textView, AggregatorService.GetClassifier(buffer)) as ITagger<T>;
                //return new TestSmartTagger(buffer, textView, this) as ITagger<T>;
            } else return null;
        }
    }

    internal class LowerCaseSmartTagAction : ISmartTagAction { 
        private ITrackingSpan m_span;
        private string m_key;
        private string m_display;
        private ITextSnapshot m_snapshot;

        public LowerCaseSmartTagAction(ITrackingSpan span) {
            m_span = span;
            m_snapshot = span.TextBuffer.CurrentSnapshot;
            m_key = span.GetText(m_snapshot);
            m_display = "Open in browser";
        }

        public string DisplayText {
            get { return m_display; }
        }
        public ImageSource Icon {
            get { return null; }
        }
        public bool IsEnabled {
            get { return true; }
        }

        public ISmartTagSource Source {
            get;
            private set;
        }

        public ReadOnlyCollection<SmartTagActionSet> ActionSets {
            get { return null; }
        }

        public void Invoke() {
            try {
                System.Diagnostics.Process.Start("https://studio.atlassian.com/browse/" + m_key);
            } catch (Exception e) {
            }
        }
    }
}
