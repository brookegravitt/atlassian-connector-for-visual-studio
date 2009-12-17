using System.Windows.Forms;

namespace Atlassian.plvs.ui.issuefilternodes {
    internal class RecentlyOpenIssuesTreeNode : TreeNode {
        public RecentlyOpenIssuesTreeNode(int imageIdx) : base("Recently Open Issues", imageIdx, imageIdx) {}
    }
}