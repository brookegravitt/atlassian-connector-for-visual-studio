using System;
using System.Diagnostics;
using System.Windows.Forms;
using Atlassian.plvs.api;
using Atlassian.plvs.models;

namespace Atlassian.plvs.ui.issues {
    public sealed class FilterContextMenu : ContextMenuStrip {
        private readonly JiraServer server;
        private readonly JiraCustomFilter filter;
        private readonly MenuSelectionAction editAction;
        private readonly MenuSelectionAction removeAction;

        private readonly ToolStripMenuItem[] items;

        public delegate void MenuSelectionAction();

        public FilterContextMenu(JiraServer server, JiraCustomFilter filter, MenuSelectionAction editAction, MenuSelectionAction removeAction) {
            this.server = server;
            this.filter = filter;
            this.editAction = editAction;
            this.removeAction = removeAction;

            items = new[]
                    {
                        new ToolStripMenuItem("Edit Filter", Resources.edit, new EventHandler(editFilter)),
                        new ToolStripMenuItem("Remove Filter", Resources.minus, new EventHandler(removeFilter)), 
                        new ToolStripMenuItem("View Filter in Browser", Resources.view_in_browser,
                                              new EventHandler(browseFilter)),
                    };

            Items.Add("dummy");

            Opened += filterContextMenuOpened;
        }

        private void filterContextMenuOpened(object sender, EventArgs e) {
            Items.Clear();

            Items.Add(items[0]);
            Items.Add(items[1]);
            if (!filter.Empty) {
                Items.Add(items[2]);
            }
        }

        private void browseFilter(object sender, EventArgs e) {
            string url = server.Url;
            try {
                Process.Start(url + filter.getBrowserQueryString());
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.Message);
            }
        }

        private void editFilter(object sender, EventArgs e) {
            editAction();
        }

        private void removeFilter(object sender, EventArgs e) {
            removeAction();
        }
    }
}