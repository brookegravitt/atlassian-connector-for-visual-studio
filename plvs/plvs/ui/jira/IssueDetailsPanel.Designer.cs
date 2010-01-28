using System.Text.RegularExpressions;
using Atlassian.plvs;

namespace Atlassian.plvs.ui.jira {
    partial class IssueDetailsPanel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IssueDetailsPanel));
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.jiraStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.issueTabs = new System.Windows.Forms.TabControl();
            this.tabSummary = new System.Windows.Forms.TabPage();
            this.issueSummary = new System.Windows.Forms.WebBrowser();
            this.tabDescriptionAndComments = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.issueDescription = new System.Windows.Forms.WebBrowser();
            this.label1 = new System.Windows.Forms.Label();
            this.toolStripContainer2 = new System.Windows.Forms.ToolStripContainer();
            this.issueComments = new System.Windows.Forms.WebBrowser();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.buttonAddComment = new System.Windows.Forms.ToolStripButton();
            this.buttonExpandAll = new System.Windows.Forms.ToolStripButton();
            this.buttonCollapseAll = new System.Windows.Forms.ToolStripButton();
            this.label2 = new System.Windows.Forms.Label();
            this.tabSubtasks = new System.Windows.Forms.TabPage();
            this.webSubtasks = new System.Windows.Forms.WebBrowser();
            this.tabAttachments = new System.Windows.Forms.TabPage();
            this.splitContainerAttachments = new System.Windows.Forms.SplitContainer();
            this.listViewAttachments = new ui.AutosizeListView();
            this.columnName = new System.Windows.Forms.ColumnHeader();
            this.columnAuthor = new System.Windows.Forms.ColumnHeader();
            this.columnSize = new System.Windows.Forms.ColumnHeader();
            this.columnDate = new System.Windows.Forms.ColumnHeader();
            this.webAttachmentView = new System.Windows.Forms.WebBrowser();
            this.label3 = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.buttonViewInBrowser = new System.Windows.Forms.ToolStripButton();
            this.buttonRefresh = new System.Windows.Forms.ToolStripButton();
            this.buttonClose = new System.Windows.Forms.ToolStripButton();
            this.buttonLogWork = new System.Windows.Forms.ToolStripButton();
            this.dropDownIssueActions = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolTipAttachments = new System.Windows.Forms.ToolTip(this.components);
            this.toolStripContainer3 = new System.Windows.Forms.ToolStripContainer();
            this.toolStrip3 = new System.Windows.Forms.ToolStrip();
            this.buttonSaveAttachmentAs = new System.Windows.Forms.ToolStripButton();
            this.buttonUploadNew = new System.Windows.Forms.ToolStripButton();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.issueTabs.SuspendLayout();
            this.tabSummary.SuspendLayout();
            this.tabDescriptionAndComments.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStripContainer2.ContentPanel.SuspendLayout();
            this.toolStripContainer2.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer2.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.tabSubtasks.SuspendLayout();
            this.tabAttachments.SuspendLayout();
            this.splitContainerAttachments.Panel1.SuspendLayout();
            this.splitContainerAttachments.Panel2.SuspendLayout();
            this.splitContainerAttachments.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.toolStripContainer3.ContentPanel.SuspendLayout();
            this.toolStripContainer3.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer3.SuspendLayout();
            this.toolStrip3.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.statusStrip);
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.issueTabs);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(785, 428);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(785, 475);
            this.toolStripContainer1.TabIndex = 0;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // statusStrip
            // 
            this.statusStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.jiraStatus});
            this.statusStrip.Location = new System.Drawing.Point(0, 0);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(785, 22);
            this.statusStrip.TabIndex = 0;
            // 
            // jiraStatus
            // 
            this.jiraStatus.Name = "jiraStatus";
            this.jiraStatus.Size = new System.Drawing.Size(38, 17);
            this.jiraStatus.Text = "status";
            // 
            // issueTabs
            // 
            this.issueTabs.Controls.Add(this.tabSummary);
            this.issueTabs.Controls.Add(this.tabDescriptionAndComments);
            this.issueTabs.Controls.Add(this.tabSubtasks);
            this.issueTabs.Controls.Add(this.tabAttachments);
            this.issueTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.issueTabs.Location = new System.Drawing.Point(0, 0);
            this.issueTabs.Name = "issueTabs";
            this.issueTabs.SelectedIndex = 0;
            this.issueTabs.Size = new System.Drawing.Size(785, 428);
            this.issueTabs.TabIndex = 0;
            // 
            // tabSummary
            // 
            this.tabSummary.Controls.Add(this.issueSummary);
            this.tabSummary.Location = new System.Drawing.Point(4, 22);
            this.tabSummary.Name = "tabSummary";
            this.tabSummary.Padding = new System.Windows.Forms.Padding(3);
            this.tabSummary.Size = new System.Drawing.Size(777, 402);
            this.tabSummary.TabIndex = 0;
            this.tabSummary.Text = "Summary";
            this.tabSummary.UseVisualStyleBackColor = true;
            // 
            // issueSummary
            // 
            this.issueSummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.issueSummary.IsWebBrowserContextMenuEnabled = false;
            this.issueSummary.Location = new System.Drawing.Point(3, 3);
            this.issueSummary.MinimumSize = new System.Drawing.Size(20, 20);
            this.issueSummary.Name = "issueSummary";
            this.issueSummary.Size = new System.Drawing.Size(771, 396);
            this.issueSummary.TabIndex = 0;
            this.issueSummary.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.issueSummary_Navigating);
            this.issueSummary.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.issueSummary_DocumentCompleted);
            // 
            // tabDescriptionAndComments
            // 
            this.tabDescriptionAndComments.Controls.Add(this.splitContainer1);
            this.tabDescriptionAndComments.Location = new System.Drawing.Point(4, 22);
            this.tabDescriptionAndComments.Name = "tabDescriptionAndComments";
            this.tabDescriptionAndComments.Padding = new System.Windows.Forms.Padding(3);
            this.tabDescriptionAndComments.Size = new System.Drawing.Size(777, 402);
            this.tabDescriptionAndComments.TabIndex = 1;
            this.tabDescriptionAndComments.Text = "Description and Comments";
            this.tabDescriptionAndComments.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.issueDescription);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.toolStripContainer2);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Size = new System.Drawing.Size(771, 396);
            this.splitContainer1.SplitterDistance = 273;
            this.splitContainer1.TabIndex = 0;
            // 
            // issueDescription
            // 
            this.issueDescription.Dock = System.Windows.Forms.DockStyle.Fill;
            this.issueDescription.IsWebBrowserContextMenuEnabled = false;
            this.issueDescription.Location = new System.Drawing.Point(0, 13);
            this.issueDescription.MinimumSize = new System.Drawing.Size(20, 20);
            this.issueDescription.Name = "issueDescription";
            this.issueDescription.Size = new System.Drawing.Size(273, 383);
            this.issueDescription.TabIndex = 1;
            this.issueDescription.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.issueDescription_Navigating);
            this.issueDescription.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.issueDescription_DocumentCompleted);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Description";
            // 
            // toolStripContainer2
            // 
            // 
            // toolStripContainer2.ContentPanel
            // 
            this.toolStripContainer2.ContentPanel.Controls.Add(this.issueComments);
            this.toolStripContainer2.ContentPanel.Size = new System.Drawing.Size(494, 358);
            this.toolStripContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer2.Location = new System.Drawing.Point(0, 13);
            this.toolStripContainer2.Name = "toolStripContainer2";
            this.toolStripContainer2.Size = new System.Drawing.Size(494, 383);
            this.toolStripContainer2.TabIndex = 1;
            this.toolStripContainer2.Text = "toolStripContainer2";
            // 
            // toolStripContainer2.TopToolStripPanel
            // 
            this.toolStripContainer2.TopToolStripPanel.Controls.Add(this.toolStrip2);
            // 
            // issueComments
            // 
            this.issueComments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.issueComments.IsWebBrowserContextMenuEnabled = false;
            this.issueComments.Location = new System.Drawing.Point(0, 0);
            this.issueComments.MinimumSize = new System.Drawing.Size(20, 20);
            this.issueComments.Name = "issueComments";
            this.issueComments.Size = new System.Drawing.Size(494, 358);
            this.issueComments.TabIndex = 0;
            this.issueComments.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.issueComments_Navigating);
            this.issueComments.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.issueComments_DocumentCompleted);
            // 
            // toolStrip2
            // 
            this.toolStrip2.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonAddComment,
            this.buttonExpandAll,
            this.buttonCollapseAll});
            this.toolStrip2.Location = new System.Drawing.Point(3, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(72, 25);
            this.toolStrip2.TabIndex = 0;
            // 
            // buttonAddComment
            // 
            this.buttonAddComment.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonAddComment.Image = global::Atlassian.plvs.Resources.new_comment;
            this.buttonAddComment.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonAddComment.Name = "buttonAddComment";
            this.buttonAddComment.Size = new System.Drawing.Size(23, 22);
            this.buttonAddComment.Text = "Add Comment";
            this.buttonAddComment.Click += new System.EventHandler(this.buttonAddComment_Click);
            // 
            // buttonExpandAll
            // 
            this.buttonExpandAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonExpandAll.Image = global::Atlassian.plvs.Resources.expand_all;
            this.buttonExpandAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonExpandAll.Name = "buttonExpandAll";
            this.buttonExpandAll.Size = new System.Drawing.Size(23, 22);
            this.buttonExpandAll.Text = "Expand All";
            this.buttonExpandAll.Click += new System.EventHandler(this.buttonExpandAll_Click);
            // 
            // buttonCollapseAll
            // 
            this.buttonCollapseAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonCollapseAll.Image = global::Atlassian.plvs.Resources.collapse_all;
            this.buttonCollapseAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonCollapseAll.Name = "buttonCollapseAll";
            this.buttonCollapseAll.Size = new System.Drawing.Size(23, 22);
            this.buttonCollapseAll.Text = "Collapse All";
            this.buttonCollapseAll.Click += new System.EventHandler(this.buttonCollapseAll_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Comments";
            // 
            // tabSubtasks
            // 
            this.tabSubtasks.Controls.Add(this.webSubtasks);
            this.tabSubtasks.Location = new System.Drawing.Point(4, 22);
            this.tabSubtasks.Name = "tabSubtasks";
            this.tabSubtasks.Padding = new System.Windows.Forms.Padding(3);
            this.tabSubtasks.Size = new System.Drawing.Size(777, 402);
            this.tabSubtasks.TabIndex = 2;
            this.tabSubtasks.Text = "Subtasks";
            this.tabSubtasks.UseVisualStyleBackColor = true;
            // 
            // webSubtasks
            // 
            this.webSubtasks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webSubtasks.Location = new System.Drawing.Point(3, 3);
            this.webSubtasks.MinimumSize = new System.Drawing.Size(20, 20);
            this.webSubtasks.Name = "webSubtasks";
            this.webSubtasks.Size = new System.Drawing.Size(771, 396);
            this.webSubtasks.TabIndex = 0;
            this.webSubtasks.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.webSubtasks_Navigating);
            this.webSubtasks.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.webSubtasks_DocumentCompleted);
            // 
            // tabAttachments
            // 
            this.tabAttachments.Controls.Add(this.splitContainerAttachments);
            this.tabAttachments.Location = new System.Drawing.Point(4, 22);
            this.tabAttachments.Name = "tabAttachments";
            this.tabAttachments.Size = new System.Drawing.Size(777, 402);
            this.tabAttachments.TabIndex = 3;
            this.tabAttachments.Text = "Attachments";
            this.tabAttachments.UseVisualStyleBackColor = true;
            // 
            // splitContainerAttachments
            // 
            this.splitContainerAttachments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerAttachments.Location = new System.Drawing.Point(0, 0);
            this.splitContainerAttachments.Name = "splitContainerAttachments";
            // 
            // splitContainerAttachments.Panel1
            // 
            this.splitContainerAttachments.Panel1.Controls.Add(this.toolStripContainer3);
            // 
            // splitContainerAttachments.Panel2
            // 
            this.splitContainerAttachments.Panel2.Controls.Add(this.webAttachmentView);
            this.splitContainerAttachments.Panel2.Controls.Add(this.label3);
            this.splitContainerAttachments.Size = new System.Drawing.Size(777, 402);
            this.splitContainerAttachments.SplitterDistance = 340;
            this.splitContainerAttachments.TabIndex = 0;
            // 
            // listViewAttachments
            // 
            this.listViewAttachments.AllowDrop = true;
            this.listViewAttachments.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnName,
            this.columnAuthor,
            this.columnSize,
            this.columnDate});
            this.listViewAttachments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewAttachments.FullRowSelect = true;
            this.listViewAttachments.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewAttachments.HideSelection = false;
            this.listViewAttachments.Location = new System.Drawing.Point(0, 0);
            this.listViewAttachments.MultiSelect = false;
            this.listViewAttachments.Name = "listViewAttachments";
            this.listViewAttachments.Size = new System.Drawing.Size(340, 377);
            this.listViewAttachments.TabIndex = 0;
            this.listViewAttachments.UseCompatibleStateImageBehavior = false;
            this.listViewAttachments.View = System.Windows.Forms.View.Details;
            this.listViewAttachments.SelectedIndexChanged += new System.EventHandler(this.listViewAttachments_SelectedIndexChanged);
            this.listViewAttachments.SizeChanged += new System.EventHandler(this.listViewAttachments_SizeChanged);
            this.listViewAttachments.DragDrop += new System.Windows.Forms.DragEventHandler(this.listViewAttachments_DragDrop);
            this.listViewAttachments.DragEnter += new System.Windows.Forms.DragEventHandler(this.listViewAttachments_DragEnter);
            this.listViewAttachments.Click += new System.EventHandler(this.listViewAttachments_Click);
            // 
            // columnName
            // 
            this.columnName.Text = "Name";
            this.columnName.Width = 94;
            // 
            // columnAuthor
            // 
            this.columnAuthor.Text = "Author";
            this.columnAuthor.Width = 76;
            // 
            // columnSize
            // 
            this.columnSize.Text = "Size [bytes]";
            this.columnSize.Width = 100;
            // 
            // columnDate
            // 
            this.columnDate.Text = "Date";
            this.columnDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnDate.Width = 66;
            // 
            // webAttachmentView
            // 
            this.webAttachmentView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webAttachmentView.Location = new System.Drawing.Point(0, 24);
            this.webAttachmentView.MinimumSize = new System.Drawing.Size(20, 20);
            this.webAttachmentView.Name = "webAttachmentView";
            this.webAttachmentView.ScriptErrorsSuppressed = true;
            this.webAttachmentView.Size = new System.Drawing.Size(433, 378);
            this.webAttachmentView.TabIndex = 0;
            this.webAttachmentView.Url = new System.Uri("", System.UriKind.Relative);
            // 
            // label3
            // 
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Location = new System.Drawing.Point(0, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(433, 24);
            this.label3.TabIndex = 0;
            this.label3.Text = "Preview";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonViewInBrowser,
            this.buttonRefresh,
            this.buttonClose,
            this.buttonLogWork,
            this.dropDownIssueActions});
            this.toolStrip1.Location = new System.Drawing.Point(3, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(124, 25);
            this.toolStrip1.TabIndex = 0;
            // 
            // buttonViewInBrowser
            // 
            this.buttonViewInBrowser.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonViewInBrowser.Image = global::Atlassian.plvs.Resources.view_in_browser;
            this.buttonViewInBrowser.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonViewInBrowser.Name = "buttonViewInBrowser";
            this.buttonViewInBrowser.Size = new System.Drawing.Size(23, 22);
            this.buttonViewInBrowser.Text = "View In Browser";
            this.buttonViewInBrowser.Click += new System.EventHandler(this.buttonViewInBrowser_Click);
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonRefresh.Image = global::Atlassian.plvs.Resources.refresh;
            this.buttonRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(23, 22);
            this.buttonRefresh.Text = "Refresh";
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonClose.Image = global::Atlassian.plvs.Resources.close;
            this.buttonClose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(23, 22);
            this.buttonClose.Text = "Close";
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // buttonLogWork
            // 
            this.buttonLogWork.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonLogWork.Image = global::Atlassian.plvs.Resources.log_work;
            this.buttonLogWork.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonLogWork.Name = "buttonLogWork";
            this.buttonLogWork.Size = new System.Drawing.Size(23, 22);
            this.buttonLogWork.Text = "Log Work";
            this.buttonLogWork.Click += new System.EventHandler(this.buttonLogWork_Click);
            // 
            // dropDownIssueActions
            // 
            this.dropDownIssueActions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.dropDownIssueActions.Image = global::Atlassian.plvs.Resources.pin_other;
            this.dropDownIssueActions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.dropDownIssueActions.Name = "dropDownIssueActions";
            this.dropDownIssueActions.Size = new System.Drawing.Size(29, 22);
            this.dropDownIssueActions.Text = "Issue Actions";
            this.dropDownIssueActions.DropDownOpened += new System.EventHandler(this.dropDownIssueActions_DropDownOpened);
            // 
            // toolStripContainer3
            // 
            // 
            // toolStripContainer3.ContentPanel
            // 
            this.toolStripContainer3.ContentPanel.Controls.Add(this.listViewAttachments);
            this.toolStripContainer3.ContentPanel.Size = new System.Drawing.Size(340, 377);
            this.toolStripContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer3.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer3.Name = "toolStripContainer3";
            this.toolStripContainer3.Size = new System.Drawing.Size(340, 402);
            this.toolStripContainer3.TabIndex = 3;
            this.toolStripContainer3.Text = "toolStripContainer3";
            // 
            // toolStripContainer3.TopToolStripPanel
            // 
            this.toolStripContainer3.TopToolStripPanel.Controls.Add(this.toolStrip3);
            // 
            // toolStrip3
            // 
            this.toolStrip3.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip3.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonSaveAttachmentAs,
            this.buttonUploadNew});
            this.toolStrip3.Location = new System.Drawing.Point(3, 0);
            this.toolStrip3.Name = "toolStrip3";
            this.toolStrip3.Size = new System.Drawing.Size(148, 25);
            this.toolStrip3.TabIndex = 0;
            // 
            // buttonSaveAttachmentAs
            // 
            this.buttonSaveAttachmentAs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonSaveAttachmentAs.Image = ((System.Drawing.Image)(resources.GetObject("buttonSaveAttachmentAs.Image")));
            this.buttonSaveAttachmentAs.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonSaveAttachmentAs.Name = "buttonSaveAttachmentAs";
            this.buttonSaveAttachmentAs.Size = new System.Drawing.Size(60, 22);
            this.buttonSaveAttachmentAs.Text = "Save As...";
            this.buttonSaveAttachmentAs.Click += new System.EventHandler(this.saveAttachment);
            // 
            // buttonUploadNew
            // 
            this.buttonUploadNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonUploadNew.Image = ((System.Drawing.Image)(resources.GetObject("buttonUploadNew.Image")));
            this.buttonUploadNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonUploadNew.Name = "buttonUploadNew";
            this.buttonUploadNew.Size = new System.Drawing.Size(85, 22);
            this.buttonUploadNew.Text = "Upload New...";
            this.buttonUploadNew.Click += new System.EventHandler(this.uploadAttachment);
            // 
            // IssueDetailsPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStripContainer1);
            this.Name = "IssueDetailsPanel";
            this.Size = new System.Drawing.Size(785, 475);
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.issueTabs.ResumeLayout(false);
            this.tabSummary.ResumeLayout(false);
            this.tabDescriptionAndComments.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.toolStripContainer2.ContentPanel.ResumeLayout(false);
            this.toolStripContainer2.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer2.TopToolStripPanel.PerformLayout();
            this.toolStripContainer2.ResumeLayout(false);
            this.toolStripContainer2.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.tabSubtasks.ResumeLayout(false);
            this.tabAttachments.ResumeLayout(false);
            this.splitContainerAttachments.Panel1.ResumeLayout(false);
            this.splitContainerAttachments.Panel2.ResumeLayout(false);
            this.splitContainerAttachments.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.toolStripContainer3.ContentPanel.ResumeLayout(false);
            this.toolStripContainer3.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer3.TopToolStripPanel.PerformLayout();
            this.toolStripContainer3.ResumeLayout(false);
            this.toolStripContainer3.PerformLayout();
            this.toolStrip3.ResumeLayout(false);
            this.toolStrip3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.TabControl issueTabs;
        private System.Windows.Forms.TabPage tabSummary;
        private System.Windows.Forms.TabPage tabDescriptionAndComments;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.WebBrowser issueDescription;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripContainer toolStripContainer2;
        private System.Windows.Forms.WebBrowser issueComments;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.WebBrowser issueSummary;
        private System.Windows.Forms.ToolStripButton buttonAddComment;
        private System.Windows.Forms.ToolStripButton buttonExpandAll;
        private System.Windows.Forms.ToolStripButton buttonCollapseAll;
        private System.Windows.Forms.ToolStripButton buttonRefresh;
        private System.Windows.Forms.ToolStripButton buttonClose;
        private System.Windows.Forms.ToolStripStatusLabel jiraStatus;
        private System.Windows.Forms.ToolStripButton buttonViewInBrowser;
        private System.Windows.Forms.ToolStripDropDownButton dropDownIssueActions;
        private System.Windows.Forms.ToolStripButton buttonLogWork;
        private System.Windows.Forms.TabPage tabSubtasks;
        private System.Windows.Forms.WebBrowser webSubtasks;
        private System.Windows.Forms.TabPage tabAttachments;
        private System.Windows.Forms.SplitContainer splitContainerAttachments;
        private ui.AutosizeListView listViewAttachments;
        private System.Windows.Forms.ColumnHeader columnName;
        private System.Windows.Forms.WebBrowser webAttachmentView;
        private System.Windows.Forms.ColumnHeader columnAuthor;
        private System.Windows.Forms.ColumnHeader columnDate;
        private System.Windows.Forms.ColumnHeader columnSize;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolTip toolTipAttachments;
        private System.Windows.Forms.ToolStripContainer toolStripContainer3;
        private System.Windows.Forms.ToolStrip toolStrip3;
        private System.Windows.Forms.ToolStripButton buttonSaveAttachmentAs;
        private System.Windows.Forms.ToolStripButton buttonUploadNew;
    }
}