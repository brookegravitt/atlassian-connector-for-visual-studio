using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Media;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui.jira;
using Atlassian.plvs.util;
using Atlassian.plvs.windows;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.Text.Classification;

namespace Atlassian.plvs.markers.vs2010.menu {
    internal class JiraIssueActionsSmartTag : SmartTag {
        public JiraIssueActionsSmartTag(ReadOnlyCollection<SmartTagActionSet> actionSets) : base(SmartTagType.Factoid, actionSets) { }
    }

    class JiraIssueActionsSmartTagger : ITagger<JiraIssueActionsSmartTag>, IDisposable {
        private ITextView view;
        private bool disposed;

        private readonly IClassifier classifier;

        public JiraIssueActionsSmartTagger(ITextView view, IClassifier classifier) {
            this.view = view;
            this.classifier = classifier;
            this.view.LayoutChanged += layoutChanged;
            view.Caret.PositionChanged += caretPositionChanged;
//            AtlassianPanel.Instance.Jira.SelectedServerChanged += jiraSelectedServerChanged;
        }

#if false
        private void jiraSelectedServerChanged(object sender, EventArgs e) {
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
#endif 

        public IEnumerable<ITagSpan<JiraIssueActionsSmartTag>> GetTags(NormalizedSnapshotSpanCollection spans) {
#if false
            JiraServer selectedServer = AtlassianPanel.Instance.Jira.getCurrentlySelectedServer();
            if (selectedServer == null) {
                yield break;
            }
#endif
            ITextCaret caret = view.Caret;
            SnapshotPoint point;

            if (caret.Position.BufferPosition > 0)
                point = caret.Position.BufferPosition;
            else
                yield break;

            foreach (SnapshotSpan span in spans) {
                foreach (SnapshotSpan s in from classification in classifier.GetClassificationSpans(span)
                                           where classification.ClassificationType.Classification.ToLower().Contains("comment")
                                           let matches = Constants.ISSUE_KEY_REGEX.Matches(classification.Span.GetText())
                                           let c = classification
                                           from s in matches.Cast<Match>().Where(match => match.Success).Select(match => new SnapshotSpan(c.Span.Start + match.Index, match.Length))
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
                view = null;
            }

            disposed = true;
        }
    }

    [Export(typeof(IViewTaggerProvider))]
    [ContentType("text")]
    [Order(Before = "default")]
    [TagType(typeof(SmartTag))]
    internal class JiraIssueActionsSmartTaggerProvider : IViewTaggerProvider {
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
                return new JiraIssueActionsSmartTagger(textView, AggregatorService.GetClassifier(buffer)) as ITagger<T>;
            }
            return null;
        }
    }

    internal class OpenIssueInBrowserSmartTagAction : ISmartTagAction {
        private readonly string issueKey;
        private readonly string menuText;
        private readonly ITextSnapshot snapshot;

        public OpenIssueInBrowserSmartTagAction(ITrackingSpan span) {
            snapshot = span.TextBuffer.CurrentSnapshot;
            issueKey = span.GetText(snapshot);
            menuText = "View JIRA Issue in the Browser";
        }

        public string DisplayText {
            get { return menuText; }
        }
        public ImageSource Icon {
            get { return PlvsUtils.bitmapSourceFromPngImage(Resources.view_in_browser); }
        }
        public bool IsEnabled {
            get { return true; }
        }

        public ReadOnlyCollection<SmartTagActionSet> ActionSets {
            get { return null; }
        }

        public void Invoke() {
            try {
                JiraServer server = AtlassianPanel.Instance.Jira.getCurrentlySelectedServer();
                if (server != null) {
                    Process.Start(server.Url + "/browse/" + issueKey);
                } else {
                    MessageBox.Show("No JIRA server selected", Constants.ERROR_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                // ReSharper disable EmptyGeneralCatchClause
            } catch (Exception) {
                // ReSharper restore EmptyGeneralCatchClause
            }
        }
    }

    internal class OpenIssueInIdeSmartTagAction : ISmartTagAction { 
        private readonly string issueKey;
        private readonly string menuText;
        private readonly ITextSnapshot snapshot;

        public OpenIssueInIdeSmartTagAction(ITrackingSpan span) {
            snapshot = span.TextBuffer.CurrentSnapshot;
            issueKey = span.GetText(snapshot);
            menuText = "Open JIRA Issue in IDE";
        }

        public string DisplayText {
            get { return menuText; }
        }
        public ImageSource Icon {
            get { return PlvsUtils.bitmapSourceFromPngImage(Resources.open_in_ide); }
        }
        public bool IsEnabled {
            get { return true; }
        }

        public ReadOnlyCollection<SmartTagActionSet> ActionSets {
            get { return null; }
        }

        public void Invoke() {
            bool found = false;
            foreach (JiraIssue issue in JiraIssueListModelImpl.Instance.Issues) {
                if (!issue.Key.Equals(issueKey)) continue;
                IssueDetailsWindow.Instance.openIssue(issue);
                found = true;
                break;
            }
            if (!found) {
                AtlassianPanel.Instance.Jira.findAndOpenIssue(issueKey, findFinished);
            }
        }

        private static void findFinished(bool success, string message) {
            if (!success) {
                MessageBox.Show(message, Constants.ERROR_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}


