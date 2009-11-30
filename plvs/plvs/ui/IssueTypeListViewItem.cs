using System.Windows.Forms;
using Atlassian.plvs.api;

namespace Atlassian.plvs.ui {
    internal class IssueTypeListViewItem : ListViewItem {
        public JiraNamedEntity IssueType { get; private set; }

        public IssueTypeListViewItem(JiraNamedEntity issueType, int imageIdx) : base(issueType.Name, imageIdx) {
            IssueType = issueType;
        }
    }
}