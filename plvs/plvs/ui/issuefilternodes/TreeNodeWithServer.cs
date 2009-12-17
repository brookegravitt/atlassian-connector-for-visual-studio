using System.Windows.Forms;
using Atlassian.plvs.api;

namespace Atlassian.plvs.ui.issuefilternodes {
    public abstract class TreeNodeWithServer : TreeNode {
        protected TreeNodeWithServer(string name, int imageIdx) : base(name, imageIdx, imageIdx) {}
        public abstract JiraServer Server { get; set; }
    }
}