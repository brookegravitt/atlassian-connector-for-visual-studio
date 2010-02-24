using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.Windows.Media;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui.jira;
using Atlassian.plvs.util;
using Atlassian.plvs.windows;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;

namespace Atlassian.plvs.markers.vs2010.menu {
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
