using System;
using System.Drawing;
using System.Windows.Forms;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui.jira.issues;
using Atlassian.plvs.ui.jira.issues.menus;

namespace Atlassian.plvs.ui.jira {
    public sealed class JiraIssueTree : TreeViewAdv {
        private readonly Control parent;
        private readonly StatusLabel status;
        private readonly JiraIssueListModel model;

        private const int MARGIN = 16;
        private const int STATUS_WIDTH = 150;
        private const int UPDATED_WIDTH = 150;
        private const int PRIORITY_WIDTH = 24;

        private readonly TreeColumn colName = new TreeColumn();
        private readonly TreeColumn colStatus = new TreeColumn();
        private readonly TreeColumn colPriority = new TreeColumn();
        private readonly TreeColumn colUpdated = new TreeColumn();
        private readonly NodeIcon controlIcon = new NodeIcon();
        private readonly NodeTextBox controlName = new BoldableNodeTextBox();
        private readonly NodeTextBox controlStatusText = new NodeTextBox();
        private readonly NodeIcon controlStatusIcon = new NodeIcon();
        private readonly NodeIcon controlPriorityIcon = new NodeIcon();
        private readonly NodeTextBox controlUpdated = new NodeTextBox();

        public JiraIssueTree(Control parent, StatusLabel status, JiraIssueListModel model) {
            this.parent = parent;
            this.status = status;
            this.model = model;

            Dock = DockStyle.Fill;
            SelectionMode = TreeSelectionMode.Single;
            FullRowSelect = true;
            GridLineStyle = GridLineStyle.None;
            UseColumns = true;

            colName.Header = "Summary";
            colStatus.Header = "Status";
            colPriority.Header = "P";
            colUpdated.Header = "Updated";

            int i = 0;
            controlIcon.ParentColumn = colName;
            controlIcon.DataPropertyName = "Icon";
            controlIcon.LeftMargin = i++;

            controlName.ParentColumn = colName;
            controlName.DataPropertyName = "Name";
            controlName.Trimming = StringTrimming.EllipsisCharacter;
            controlName.UseCompatibleTextRendering = true;
            controlName.LeftMargin = i++;

            controlPriorityIcon.ParentColumn = colPriority;
            controlPriorityIcon.DataPropertyName = "PriorityIcon";
            controlPriorityIcon.LeftMargin = i++;

            controlStatusIcon.ParentColumn = colStatus;
            controlStatusIcon.DataPropertyName = "StatusIcon";
            controlStatusIcon.LeftMargin = i++;

            controlStatusText.ParentColumn = colStatus;
            controlStatusText.DataPropertyName = "StatusText";
            controlStatusText.Trimming = StringTrimming.EllipsisCharacter;
            controlStatusText.UseCompatibleTextRendering = true;
            controlStatusText.LeftMargin = i++;

            controlUpdated.ParentColumn = colUpdated;
            controlUpdated.DataPropertyName = "Updated";
            controlUpdated.Trimming = StringTrimming.EllipsisCharacter;
            controlUpdated.UseCompatibleTextRendering = true;
            controlUpdated.TextAlign = HorizontalAlignment.Right;
            controlUpdated.LeftMargin = i;

            Columns.Add(colName);
            Columns.Add(colPriority);
            Columns.Add(colStatus);
            Columns.Add(colUpdated);

            NodeControls.Add(controlIcon);
            NodeControls.Add(controlName);
            NodeControls.Add(controlPriorityIcon);
            NodeControls.Add(controlStatusIcon);
            NodeControls.Add(controlStatusText);
            NodeControls.Add(controlUpdated);

            setSummaryColumnWidth();

            parent.SizeChanged += parentSizeChanged;

            colPriority.TextAlign = HorizontalAlignment.Left;
            colPriority.Width = PRIORITY_WIDTH;
            colPriority.MinColumnWidth = PRIORITY_WIDTH;
            colPriority.MaxColumnWidth = PRIORITY_WIDTH;
            colUpdated.Width = UPDATED_WIDTH;
            colUpdated.MinColumnWidth = UPDATED_WIDTH;
            colUpdated.MaxColumnWidth = UPDATED_WIDTH;
            colStatus.Width = STATUS_WIDTH;
            colStatus.MinColumnWidth = STATUS_WIDTH;
            colStatus.MaxColumnWidth = STATUS_WIDTH;
            colName.TextAlign = HorizontalAlignment.Left;
            colPriority.TooltipText = "Priority";
            colStatus.TextAlign = HorizontalAlignment.Left;
            colPriority.TextAlign = HorizontalAlignment.Left;
            colUpdated.TextAlign = HorizontalAlignment.Right;

            MouseDown += jiraIssueTreeMouseDown;
        }

        public void addContextMenu(ToolStripItem[] items) {
            IssueContextMenu strip = new IssueContextMenu(model, status, this, items);
            ContextMenuStrip = strip;
        }

        private void setSummaryColumnWidth() {
            // todo: well, this is lame. figure out how to handle filling first column to occupy all space in a propper manner
            int summaryWidth = parent.Width
                               - PRIORITY_WIDTH - UPDATED_WIDTH - STATUS_WIDTH
                               - SystemInformation.VerticalScrollBarWidth - MARGIN;
            if (summaryWidth < 0) {
                summaryWidth = 4 * PRIORITY_WIDTH;
            }
            colName.Width = summaryWidth;
            //            colName.MinColumnWidth = summaryWidth;
            //            colName.MaxColumnWidth = summaryWidth;
        }

        private void parentSizeChanged(object sender, EventArgs e) {
            setSummaryColumnWidth();
        }

        private class BoldableNodeTextBox : NodeTextBox {
            protected override void OnDrawText(DrawEventArgs args) {
                args.Font = new Font(args.Font, FontStyle.Bold);
                base.OnDrawText(args);
            }

            protected override bool DrawTextMustBeFired(TreeNodeAdv node) {
                return !(node.Tag is IssueNode);
            }
        }

        private void jiraIssueTreeMouseDown(object sender, MouseEventArgs e) {
            if (SelectedNode == null || !(SelectedNode.Tag is IssueNode)) return;

            IssueNode n = (IssueNode) SelectedNode.Tag;
            DataObject d = new DataObject();

            d.SetText("ISSUE:" + n.Issue.Key + ":SERVER:{" + n.Issue.Server.GUID + "}");

            DoDragDrop(d, DragDropEffects.Copy | DragDropEffects.Move);
        }
    }
}