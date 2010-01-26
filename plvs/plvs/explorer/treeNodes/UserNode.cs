using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui;

namespace Atlassian.plvs.explorer.treeNodes {
    sealed class UserNode : AbstractNavigableTreeNodeWithServer, DropZone.DropZoneWorker {
        private readonly JiraUser user;

        private readonly List<ToolStripItem> menuItems = new List<ToolStripItem>();

        public UserNode(JiraIssueListModel model, JiraServerFacade facade, JiraServer server, JiraUser user)
            : base(model, facade, server, user.ToString(), 0) {

            this.user = user;

            ContextMenuStrip = new ContextMenuStrip();

            menuItems.Add(new ToolStripMenuItem("Open \"Assignee\" Drop Zone", null, createDropZone));

            ContextMenuStrip.Items.Add("dummy");

            ContextMenuStrip.Items.AddRange(MenuItems.ToArray());

            ContextMenuStrip.Opening += contextMenuStripOpening;
            ContextMenuStrip.Opened += contextMenuStripOpened;
        }

        public override List<ToolStripItem> MenuItems { get { return menuItems; } }

        public override string getUrl(string authString) {
            return Server.Url + "/secure/ViewProfile.jspa?name=" + user.Id + "&" + authString; 
        }

        public override void onClick(StatusLabel status) { }

        private void contextMenuStripOpened(object sender, EventArgs e) {
            ContextMenuStrip.Items.Clear();
            foreach (ToolStripItem item in MenuItems) {
                ContextMenuStrip.Items.Add(item);
            }
        }

        private static void contextMenuStripOpening(object sender, System.ComponentModel.CancelEventArgs e) {
            e.Cancel = false;
        }

        private void createDropZone(object sender, EventArgs e) {
            DropZone.showDropZoneFor(Model, Server, Facade, this);
        }

        public DropZone.PerformAction Action { get { return dropAction; } }

        private void dropAction(JiraIssue issue, bool add) {
            JiraField field = new JiraField("assignee", null) { Values = new List<string>() };
            if (add) {
                throw new ArgumentException("Unable to have multiple assignees in one issue");
            }

            // skip if issue already has this user
            if (field.Values.Contains(user.Id)) return;

            field.Values.Add(user.Id);
            Facade.updateIssue(issue, new List<JiraField> { field });
        }

        public string ZoneName { get { return "Assignee: " + user; } }

        public string ZoneKey { get { return Server.GUID + "_priority_" + user.Id; } }

        public bool CanAdd { get { return false; } }

        public string IssueWillBeAddedText { get { return "Unavailable"; } }

        public string issueWillBeMovedText { get { return "User " + user + " will be set as assignee for issue"; } }

        public string InitialText { get { return "Drag issues here to set their assignee to \"" + user + "\""; } }
    }
}
