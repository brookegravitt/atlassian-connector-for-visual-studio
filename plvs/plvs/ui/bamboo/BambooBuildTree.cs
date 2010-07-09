using System;
using System.Drawing;
using System.Windows.Forms;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;

namespace Atlassian.plvs.ui.bamboo {
    public sealed class BambooBuildTree : TreeViewAdv {

        private readonly TreeColumn colStatusAndKey = new TreeColumn();
        private readonly TreeColumn colTests = new TreeColumn();
        private readonly TreeColumn colReason = new TreeColumn();
        private readonly TreeColumn colCompleted = new TreeColumn();
        private readonly TreeColumn colDuration = new TreeColumn();
        private readonly TreeColumn colServer = new TreeColumn();
        private readonly NodeIcon controlIcon = new NodeIcon();
        private readonly NodeBuildInProgressIcon inProgressIcon;
        private readonly NodeTextBox controlBuildKey = new NodeTextBox();
        private readonly NodeTextBox controlTests = new NodeTextBox();
        private readonly NodeTextBox controlReason = new NodeTextBox();
        private readonly NodeTextBox controlCompleted = new NodeTextBox();
        private readonly NodeTextBox controlDuration = new NodeTextBox();
        private readonly NodeTextBox controlServer = new NodeTextBox();

        private const int STATUS_AND_KEY_WIDTH = 200;
        private const int TESTS_WIDTH = 200;
        private const int REASON_WIDTH = 300;
        private const int COMPLETED_WIDTH = 150;
        private const int DURATION_WIDTH = 150;
        private const int SERVER_WIDTH = 200;

        private const int MARGIN = 24;

        public BambooBuildTree() {

            Dock = DockStyle.Fill;
            SelectionMode = TreeSelectionMode.Single;
            FullRowSelect = true;
            GridLineStyle = GridLineStyle.None;
            UseColumns = true;

            colStatusAndKey.Header = "Key and Status";
            colTests.Header = "Tests";
            colReason.Header = "Reason";
            colCompleted.Header = "Completed";
            colDuration.Header = "Duration";
            colServer.Header = "Server";

            int i = 0;

            controlIcon.ParentColumn = colStatusAndKey;
            controlIcon.DataPropertyName = "Icon";
            controlIcon.LeftMargin = i++;

            inProgressIcon = new NodeBuildInProgressIcon(this) {
                                 ParentColumn = colStatusAndKey,
                                 DataPropertyName = "IsInProgress",
                                 LeftMargin = i++
                             };

            controlBuildKey.ParentColumn = colStatusAndKey;
            controlBuildKey.DataPropertyName = "Key";
            controlBuildKey.Trimming = StringTrimming.EllipsisCharacter;
            controlBuildKey.UseCompatibleTextRendering = true;
            controlBuildKey.LeftMargin = i++;

            controlTests.ParentColumn = colTests;
            controlTests.DataPropertyName = "Tests";
            controlTests.Trimming = StringTrimming.EllipsisCharacter;
            controlTests.UseCompatibleTextRendering = true;
            controlTests.LeftMargin = i++;

            controlReason.ParentColumn = colReason;
            controlReason.DataPropertyName = "Reason";
            controlReason.Trimming = StringTrimming.EllipsisCharacter;
            controlReason.UseCompatibleTextRendering = true;
            controlReason.LeftMargin = i++;

            controlCompleted.ParentColumn = colCompleted;
            controlCompleted.DataPropertyName = "Completed";
            controlCompleted.Trimming = StringTrimming.EllipsisCharacter;
            controlCompleted.UseCompatibleTextRendering = true;
            controlCompleted.LeftMargin = i++;

            controlDuration.ParentColumn = colDuration;
            controlDuration.DataPropertyName = "Duration";
            controlDuration.Trimming = StringTrimming.EllipsisCharacter;
            controlDuration.UseCompatibleTextRendering = true;
            controlDuration.LeftMargin = i++;

            controlServer.ParentColumn = colServer;
            controlServer.DataPropertyName = "Server";
            controlServer.Trimming = StringTrimming.EllipsisCharacter;
            controlServer.UseCompatibleTextRendering = true;
            controlServer.TextAlign = HorizontalAlignment.Right;
            controlServer.LeftMargin = i;

            Columns.Add(colStatusAndKey);
            Columns.Add(colTests);
            Columns.Add(colReason);
            Columns.Add(colCompleted);
            Columns.Add(colDuration);
            Columns.Add(colServer);

            NodeControls.Add(controlIcon);
            NodeControls.Add(inProgressIcon);
            NodeControls.Add(controlBuildKey);
            NodeControls.Add(controlTests);
            NodeControls.Add(controlReason);
            NodeControls.Add(controlCompleted);
            NodeControls.Add(controlDuration);
            NodeControls.Add(controlServer);

            colServer.TextAlign = HorizontalAlignment.Right;

            Resize += bambooBuildTreeResize;

            resizeColumns();
        }

        private void bambooBuildTreeResize(object sender, EventArgs e) {
            resizeColumns();
        }

        private void resizeColumns() {
            const int total = STATUS_AND_KEY_WIDTH + TESTS_WIDTH + COMPLETED_WIDTH + DURATION_WIDTH + SERVER_WIDTH + MARGIN;
            colStatusAndKey.Width = STATUS_AND_KEY_WIDTH;
            colTests.Width = TESTS_WIDTH;
            colReason.Width = total < Width ? Width - total : REASON_WIDTH;
            colCompleted.Width = COMPLETED_WIDTH;
            colDuration.Width = DURATION_WIDTH;
            colServer.Width = SERVER_WIDTH;
        }
    }
}
