namespace Atlassian.plvs.ui.bamboo {
    public class ProjectNode : TreeNodeCollapseExpandStatusManager.TreeNodeRememberingCollapseState {
        private readonly string guid;
        public string Key { get; private set; }
        public ProjectNode(string guid, string key) {
            this.guid = guid;
            Key = key;
        }

        public string NodeKey { get { return guid + Key; } }
        public bool NodeExpanded { get; set; }
    }
}
