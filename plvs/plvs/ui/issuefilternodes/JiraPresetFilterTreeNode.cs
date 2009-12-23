using System;
using Atlassian.plvs.models;
using Atlassian.plvs.api;
using Atlassian.plvs.ui.issues.menus;

namespace Atlassian.plvs.ui.issuefilternodes {
    public sealed class JiraPresetFilterTreeNode : TreeNodeWithServer {
        private readonly JiraServer server;

        public JiraPresetFilterTreeNode(JiraServer server, JiraPresetFilter filter, 
            PresetFilterContextMenu.MenuSelectionAction setProjectAction, 
            PresetFilterContextMenu.MenuSelectionAction clearProjectAction, int imageIdx)
            : base(filter.Name, imageIdx) {

            this.server = server;
            Filter = filter;

            Tag = "Right-click to set or clear project";

            ContextMenuStrip = new PresetFilterContextMenu(this, setProjectAction, clearProjectAction);
        }

        public void setProject(JiraProject project) {
            Filter.Project = project;
            Text = Filter.Name;
        }

        public JiraPresetFilter Filter { get; private set; }

        public override JiraServer Server {
            get { return server; }
            set { throw new NotImplementedException(); }
        }
    }
}