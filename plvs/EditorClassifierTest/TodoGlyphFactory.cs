using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Controls;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Language.Intellisense;

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Classification;

namespace EditorClassifierTest {
    class TodoGlyphFactory : IGlyphFactory {
        const double m_glyphSize = 16.0;

        public UIElement GenerateGlyph(IWpfTextViewLine line, IGlyphTag tag) {
            // Ensure we can draw a glyph for this marker.
            if (tag == null || !(tag is TodoTag)) {
                return null;
            }

            System.Windows.Shapes.Ellipse ellipse = new Ellipse();
            ellipse.Fill = Brushes.LightBlue;
            ellipse.StrokeThickness = 2;
            ellipse.Stroke = Brushes.DarkBlue;
            ellipse.Height = m_glyphSize;
            ellipse.Width = m_glyphSize;

            return ellipse;
        }
    }

    [Export(typeof(IGlyphFactoryProvider))]
    [Name("TodoGlyph")]
    [Order(After = "VsTextMarker")]
    [ContentType("code")]
    [TagType(typeof(TodoTag))]
    internal sealed class TodoGlyphFactoryProvider : IGlyphFactoryProvider {
        public IGlyphFactory GetGlyphFactory(IWpfTextView view, IWpfTextViewMargin margin) {
            return new TodoGlyphFactory();
        }
    }

//    internal class TodoTag : IGlyphTag { }

    public class TodoTag : TextMarkerTag, IGlyphTag {
        public TodoTag()
            : base("MarkerFormatDefinition/IssueFormatDefinition") {
        }
    }

    [Export(typeof(EditorFormatDefinition))]
    [Name("MarkerFormatDefinition/IssueFormatDefinition")]
    [UserVisible(true)]
    internal class IssuedFormatDefinition : MarkerFormatDefinition {
        public IssuedFormatDefinition() {
            this.BackgroundColor = Colors.Azure;
            this.ForegroundColor = Colors.DarkRed;
            this.DisplayName = "Highlight Issue";
            this.ZOrder = 5;
        }
    }


    internal class TodoTagger : ITagger<TodoTag> {
        private IClassifier m_classifier;
        private const string m_searchText = "todo";

        internal TodoTagger(IClassifier classifier) {
            m_classifier = classifier;
        }

        private static Regex issueKeyRegex = new Regex(@"([A-Z]+-[0-9]+)");

        IEnumerable<ITagSpan<TodoTag>> ITagger<TodoTag>.GetTags(NormalizedSnapshotSpanCollection spans) {
            foreach (SnapshotSpan span in spans) {
                //look at each classification span \
                foreach (ClassificationSpan classification in m_classifier.GetClassificationSpans(span)) {
                    //if the classification is a comment
                    if (classification.ClassificationType.Classification.ToLower().Contains("comment")) {
                        //if the word "todo" is in the comment,
                        //create a new TodoTag TagSpan
//                        int index = classification.Span.GetText().ToLower().IndexOf(m_searchText);
//                        if (index != -1) {
//                            yield return new TagSpan<TodoTag>(new SnapshotSpan(classification.Span.Start + index, m_searchText.Length), new TodoTag());

                        Match m = issueKeyRegex.Match(classification.Span.GetText());
                        if (m.Success) {
                            yield return new TagSpan<TodoTag>(new SnapshotSpan(classification.Span.Start + m.Index, m.Length), new TodoTag());
                        }
                    }
                }
            }
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
    }

    [Export(typeof(ITaggerProvider))]
    [ContentType("code")]
    [TagType(typeof(TodoTag))]
    class TodoTaggerProvider : ITaggerProvider {
        [Import]
        internal IClassifierAggregatorService AggregatorService;

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag {
            if (buffer == null) {
                throw new ArgumentNullException("buffer");
            }

            return new TodoTagger(AggregatorService.GetClassifier(buffer)) as ITagger<T>;
        }
    }

}
