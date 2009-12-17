using System.Drawing;

namespace Atlassian.plvs.ui.issues {
    public abstract class AbstractIssueTreeNode {
        public abstract Image Icon { get; }
        public abstract string Name { get; }
    }
}
