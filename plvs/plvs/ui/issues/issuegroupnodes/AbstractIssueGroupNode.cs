using System.Collections.Generic;

namespace Atlassian.plvs.ui.issues.issuegroupnodes {
    public abstract class AbstractIssueGroupNode : AbstractIssueTreeNode {
        protected AbstractIssueGroupNode() {
            IssueNodes = new List<IssueNode>();
        }

        public override string Name { get { return getGroupName() + " (" + IssueNodes.Count + ")"; } }

        public abstract string getGroupName();

        public List<IssueNode> IssueNodes { get; private set; }
    }
}
