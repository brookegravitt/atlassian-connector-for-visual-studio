namespace Atlassian.plvs {
    partial class IssueListWindow
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
            this.mainContainer = new System.Windows.Forms.ToolStripContainer();
            this.productTabs = new System.Windows.Forms.TabControl();
            this.tabJira = new System.Windows.Forms.TabPage();
            this.jiraContainer = new System.Windows.Forms.ToolStripContainer();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.jiraStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.getMoreIssues = new System.Windows.Forms.ToolStripStatusLabel();
            this.jiraSplitter = new System.Windows.Forms.SplitContainer();
            this.filterTreeContainer = new System.Windows.Forms.ToolStripContainer();
            this.filtersTree = new System.Windows.Forms.TreeView();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.buttonRefreshAll = new System.Windows.Forms.ToolStripButton();
            this.buttonAddFilter = new System.Windows.Forms.ToolStripButton();
            this.buttonRemoveFilter = new System.Windows.Forms.ToolStripButton();
            this.buttonEditFilter = new System.Windows.Forms.ToolStripButton();
            this.issueTreeContainer = new System.Windows.Forms.ToolStripContainer();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.labelNarrow = new System.Windows.Forms.ToolStripLabel();
            this.comboFind = new System.Windows.Forms.ToolStripComboBox();
            this.labelGroupBy = new System.Windows.Forms.ToolStripLabel();
            this.comboGroupBy = new System.Windows.Forms.ToolStripComboBox();
            this.buttonExpandAll = new System.Windows.Forms.ToolStripButton();
            this.buttonCollapseAll = new System.Windows.Forms.ToolStripButton();
            this.buttonOpen = new System.Windows.Forms.ToolStripButton();
            this.buttonViewInBrowser = new System.Windows.Forms.ToolStripButton();
            this.buttonEditInBrowser = new System.Windows.Forms.ToolStripButton();
            this.buttonSearch = new System.Windows.Forms.ToolStripButton();
            this.buttonRefresh = new System.Windows.Forms.ToolStripButton();
            this.globalToolBar = new System.Windows.Forms.ToolStrip();
            this.buttonProjectProperties = new System.Windows.Forms.ToolStripButton();
            this.buttonGlobalProperties = new System.Windows.Forms.ToolStripButton();
            this.buttonAbout = new System.Windows.Forms.ToolStripButton();
            this.buttonUpdate = new System.Windows.Forms.ToolStripButton();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.filtersTreeToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.mainContainer.ContentPanel.SuspendLayout();
            this.mainContainer.LeftToolStripPanel.SuspendLayout();
            this.mainContainer.SuspendLayout();
            this.productTabs.SuspendLayout();
            this.tabJira.SuspendLayout();
            this.jiraContainer.BottomToolStripPanel.SuspendLayout();
            this.jiraContainer.ContentPanel.SuspendLayout();
            this.jiraContainer.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.jiraSplitter.Panel1.SuspendLayout();
            this.jiraSplitter.Panel2.SuspendLayout();
            this.jiraSplitter.SuspendLayout();
            this.filterTreeContainer.ContentPanel.SuspendLayout();
            this.filterTreeContainer.TopToolStripPanel.SuspendLayout();
            this.filterTreeContainer.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.issueTreeContainer.TopToolStripPanel.SuspendLayout();
            this.issueTreeContainer.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.globalToolBar.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainContainer
            // 
            // 
            // mainContainer.ContentPanel
            // 
            this.mainContainer.ContentPanel.Controls.Add(this.productTabs);
            this.mainContainer.ContentPanel.Size = new System.Drawing.Size(828, 311);
            this.mainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // mainContainer.LeftToolStripPanel
            // 
            this.mainContainer.LeftToolStripPanel.Controls.Add(this.globalToolBar);
            this.mainContainer.Location = new System.Drawing.Point(0, 0);
            this.mainContainer.Name = "mainContainer";
            this.mainContainer.Size = new System.Drawing.Size(852, 336);
            this.mainContainer.TabIndex = 2;
            this.mainContainer.Text = "toolStripContainer1";
            // 
            // productTabs
            // 
            this.productTabs.Controls.Add(this.tabJira);
            this.productTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.productTabs.Location = new System.Drawing.Point(0, 0);
            this.productTabs.Name = "productTabs";
            this.productTabs.SelectedIndex = 0;
            this.productTabs.Size = new System.Drawing.Size(828, 311);
            this.productTabs.TabIndex = 0;
            // 
            // tabJira
            // 
            this.tabJira.Controls.Add(this.jiraContainer);
            this.tabJira.Location = new System.Drawing.Point(4, 22);
            this.tabJira.Name = "tabJira";
            this.tabJira.Padding = new System.Windows.Forms.Padding(3);
            this.tabJira.Size = new System.Drawing.Size(820, 285);
            this.tabJira.TabIndex = 0;
            this.tabJira.Text = "Issues - JIRA";
            this.tabJira.UseVisualStyleBackColor = true;
            // 
            // jiraContainer
            // 
            // 
            // jiraContainer.BottomToolStripPanel
            // 
            this.jiraContainer.BottomToolStripPanel.Controls.Add(this.statusStrip);
            // 
            // jiraContainer.ContentPanel
            // 
            this.jiraContainer.ContentPanel.Controls.Add(this.jiraSplitter);
            this.jiraContainer.ContentPanel.Size = new System.Drawing.Size(814, 232);
            this.jiraContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.jiraContainer.Location = new System.Drawing.Point(3, 3);
            this.jiraContainer.Name = "jiraContainer";
            this.jiraContainer.Size = new System.Drawing.Size(814, 279);
            this.jiraContainer.TabIndex = 0;
            this.jiraContainer.Text = "toolStripContainer1";
            // 
            // statusStrip
            // 
            this.statusStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.jiraStatus,
            this.getMoreIssues});
            this.statusStrip.Location = new System.Drawing.Point(0, 0);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(814, 22);
            this.statusStrip.TabIndex = 0;
            // 
            // jiraStatus
            // 
            this.jiraStatus.Name = "jiraStatus";
            this.jiraStatus.Size = new System.Drawing.Size(39, 17);
            this.jiraStatus.Text = "Ready";
            // 
            // getMoreIssues
            // 
            this.getMoreIssues.IsLink = true;
            this.getMoreIssues.Name = "getMoreIssues";
            this.getMoreIssues.Size = new System.Drawing.Size(99, 17);
            this.getMoreIssues.Text = "Get More Issues...";
            this.getMoreIssues.Visible = false;
            this.getMoreIssues.Click += new System.EventHandler(this.getMoreIssues_Click);
            // 
            // jiraSplitter
            // 
            this.jiraSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.jiraSplitter.Location = new System.Drawing.Point(0, 0);
            this.jiraSplitter.Name = "jiraSplitter";
            // 
            // jiraSplitter.Panel1
            // 
            this.jiraSplitter.Panel1.Controls.Add(this.filterTreeContainer);
            // 
            // jiraSplitter.Panel2
            // 
            this.jiraSplitter.Panel2.Controls.Add(this.issueTreeContainer);
            this.jiraSplitter.Size = new System.Drawing.Size(814, 232);
            this.jiraSplitter.SplitterDistance = 215;
            this.jiraSplitter.TabIndex = 0;
            // 
            // filterTreeContainer
            // 
            // 
            // filterTreeContainer.ContentPanel
            // 
            this.filterTreeContainer.ContentPanel.Controls.Add(this.filtersTree);
            this.filterTreeContainer.ContentPanel.Size = new System.Drawing.Size(215, 207);
            this.filterTreeContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filterTreeContainer.Location = new System.Drawing.Point(0, 0);
            this.filterTreeContainer.Name = "filterTreeContainer";
            this.filterTreeContainer.Size = new System.Drawing.Size(215, 232);
            this.filterTreeContainer.TabIndex = 0;
            this.filterTreeContainer.Text = "toolStripContainer2";
            // 
            // filterTreeContainer.TopToolStripPanel
            // 
            this.filterTreeContainer.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // filtersTree
            // 
            this.filtersTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filtersTree.HideSelection = false;
            this.filtersTree.Location = new System.Drawing.Point(0, 0);
            this.filtersTree.Name = "filtersTree";
            this.filtersTree.Size = new System.Drawing.Size(215, 207);
            this.filtersTree.TabIndex = 0;
            this.filtersTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.filtersTree_AfterSelect);
            this.filtersTree.MouseMove += new System.Windows.Forms.MouseEventHandler(this.filtersTree_MouseMove);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonRefreshAll,
            this.buttonAddFilter,
            this.buttonRemoveFilter,
            this.buttonEditFilter});
            this.toolStrip1.Location = new System.Drawing.Point(3, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(135, 25);
            this.toolStrip1.TabIndex = 0;
            // 
            // buttonRefreshAll
            // 
            this.buttonRefreshAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonRefreshAll.Image = global::Atlassian.plvs.Resources.refresh;
            this.buttonRefreshAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonRefreshAll.Name = "buttonRefreshAll";
            this.buttonRefreshAll.Size = new System.Drawing.Size(23, 22);
            this.buttonRefreshAll.Text = "Refresh All";
            this.buttonRefreshAll.Click += new System.EventHandler(this.buttonRefreshAll_Click);
            // 
            // buttonAddFilter
            // 
            this.buttonAddFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonAddFilter.Image = global::Atlassian.plvs.Resources.plus;
            this.buttonAddFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonAddFilter.Name = "buttonAddFilter";
            this.buttonAddFilter.Size = new System.Drawing.Size(23, 22);
            this.buttonAddFilter.Text = "Add Custom Filter";
            this.buttonAddFilter.Click += new System.EventHandler(this.buttonAddFilter_Click);
            // 
            // buttonRemoveFilter
            // 
            this.buttonRemoveFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonRemoveFilter.Image = global::Atlassian.plvs.Resources.minus;
            this.buttonRemoveFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonRemoveFilter.Name = "buttonRemoveFilter";
            this.buttonRemoveFilter.Size = new System.Drawing.Size(23, 22);
            this.buttonRemoveFilter.Text = "Remove Custom Filter";
            this.buttonRemoveFilter.Click += new System.EventHandler(this.buttonRemoveFilter_Click);
            // 
            // buttonEditFilter
            // 
            this.buttonEditFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonEditFilter.Image = global::Atlassian.plvs.Resources.edit;
            this.buttonEditFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonEditFilter.Name = "buttonEditFilter";
            this.buttonEditFilter.Size = new System.Drawing.Size(23, 22);
            this.buttonEditFilter.Text = "Edit Custom Filter";
            this.buttonEditFilter.Click += new System.EventHandler(this.buttonEditFilter_Click);
            // 
            // issueTreeContainer
            // 
            // 
            // issueTreeContainer.ContentPanel
            // 
            this.issueTreeContainer.ContentPanel.Size = new System.Drawing.Size(595, 207);
            this.issueTreeContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.issueTreeContainer.Location = new System.Drawing.Point(0, 0);
            this.issueTreeContainer.Name = "issueTreeContainer";
            this.issueTreeContainer.Size = new System.Drawing.Size(595, 232);
            this.issueTreeContainer.TabIndex = 0;
            this.issueTreeContainer.Text = "toolStripContainer2";
            // 
            // issueTreeContainer.TopToolStripPanel
            // 
            this.issueTreeContainer.TopToolStripPanel.Controls.Add(this.toolStrip2);
            // 
            // toolStrip2
            // 
            this.toolStrip2.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.labelNarrow,
            this.comboFind,
            this.labelGroupBy,
            this.comboGroupBy,
            this.buttonExpandAll,
            this.buttonCollapseAll,
            this.buttonOpen,
            this.buttonViewInBrowser,
            this.buttonEditInBrowser,
            this.buttonSearch,
            this.buttonRefresh});
            this.toolStrip2.Location = new System.Drawing.Point(3, 0);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(534, 25);
            this.toolStrip2.TabIndex = 0;
            // 
            // labelNarrow
            // 
            this.labelNarrow.Name = "labelNarrow";
            this.labelNarrow.Size = new System.Drawing.Size(30, 22);
            this.labelNarrow.Text = "Find";
            // 
            // comboFind
            // 
            this.comboFind.Name = "comboFind";
            this.comboFind.Size = new System.Drawing.Size(150, 25);
            this.comboFind.SelectedIndexChanged += new System.EventHandler(this.comboFind_SelectedIndexChanged);
            this.comboFind.TextChanged += new System.EventHandler(this.comboFind_TextChanged);
            this.comboFind.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.comboFind_KeyPress);
            // 
            // labelGroupBy
            // 
            this.labelGroupBy.Name = "labelGroupBy";
            this.labelGroupBy.Size = new System.Drawing.Size(56, 22);
            this.labelGroupBy.Text = "Group By";
            // 
            // comboGroupBy
            // 
            this.comboGroupBy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboGroupBy.Name = "comboGroupBy";
            this.comboGroupBy.Size = new System.Drawing.Size(121, 25);
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
            // buttonOpen
            // 
            this.buttonOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonOpen.Image = global::Atlassian.plvs.Resources.open_in_ide;
            this.buttonOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonOpen.Name = "buttonOpen";
            this.buttonOpen.Size = new System.Drawing.Size(23, 22);
            this.buttonOpen.Text = "Open Issue";
            this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
            // 
            // buttonViewInBrowser
            // 
            this.buttonViewInBrowser.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonViewInBrowser.Image = global::Atlassian.plvs.Resources.view_in_browser;
            this.buttonViewInBrowser.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonViewInBrowser.Name = "buttonViewInBrowser";
            this.buttonViewInBrowser.Size = new System.Drawing.Size(23, 22);
            this.buttonViewInBrowser.Text = "View in Browser";
            this.buttonViewInBrowser.Click += new System.EventHandler(this.buttonViewInBrowser_Click);
            // 
            // buttonEditInBrowser
            // 
            this.buttonEditInBrowser.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonEditInBrowser.Image = global::Atlassian.plvs.Resources.edit_in_browser;
            this.buttonEditInBrowser.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonEditInBrowser.Name = "buttonEditInBrowser";
            this.buttonEditInBrowser.Size = new System.Drawing.Size(23, 22);
            this.buttonEditInBrowser.Text = "Edit in Browser";
            this.buttonEditInBrowser.Click += new System.EventHandler(this.buttonEditInBrowser_Click);
            // 
            // buttonSearch
            // 
            this.buttonSearch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonSearch.Image = global::Atlassian.plvs.Resources.find_jira;
            this.buttonSearch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.Size = new System.Drawing.Size(23, 22);
            this.buttonSearch.Text = "Search Issue";
            this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonRefresh.Image = global::Atlassian.plvs.Resources.refresh;
            this.buttonRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(23, 22);
            this.buttonRefresh.Text = "Refresh Issues";
            this.buttonRefresh.Click += new System.EventHandler(this.buttonRefresh_Click);
            // 
            // globalToolBar
            // 
            this.globalToolBar.Dock = System.Windows.Forms.DockStyle.None;
            this.globalToolBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonProjectProperties,
            this.buttonGlobalProperties,
            this.buttonAbout,
            this.buttonUpdate});
            this.globalToolBar.Location = new System.Drawing.Point(0, 3);
            this.globalToolBar.Name = "globalToolBar";
            this.globalToolBar.Size = new System.Drawing.Size(24, 103);
            this.globalToolBar.TabIndex = 0;
            // 
            // buttonProjectProperties
            // 
            this.buttonProjectProperties.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonProjectProperties.Image = global::Atlassian.plvs.Resources.projectsettings;
            this.buttonProjectProperties.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonProjectProperties.Name = "buttonProjectProperties";
            this.buttonProjectProperties.Size = new System.Drawing.Size(22, 20);
            this.buttonProjectProperties.Text = "Project Configuration";
            this.buttonProjectProperties.Click += new System.EventHandler(this.buttonProjectProperties_Click);
            // 
            // buttonGlobalProperties
            // 
            this.buttonGlobalProperties.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonGlobalProperties.Image = global::Atlassian.plvs.Resources.global_properties;
            this.buttonGlobalProperties.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonGlobalProperties.Name = "buttonGlobalProperties";
            this.buttonGlobalProperties.Size = new System.Drawing.Size(22, 20);
            this.buttonGlobalProperties.Text = "Global Configuration";
            this.buttonGlobalProperties.Click += new System.EventHandler(this.buttonGlobalProperties_Click);
            // 
            // buttonAbout
            // 
            this.buttonAbout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonAbout.Image = global::Atlassian.plvs.Resources.about;
            this.buttonAbout.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonAbout.Name = "buttonAbout";
            this.buttonAbout.Size = new System.Drawing.Size(22, 20);
            this.buttonAbout.Text = "About";
            this.buttonAbout.Click += new System.EventHandler(this.buttonAbout_Click);
            // 
            // buttonUpdate
            // 
            this.buttonUpdate.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonUpdate.Enabled = false;
            this.buttonUpdate.Image = global::Atlassian.plvs.Resources.status_plugin;
            this.buttonUpdate.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonUpdate.Name = "buttonUpdate";
            this.buttonUpdate.Size = new System.Drawing.Size(22, 20);
            this.buttonUpdate.Text = "New Version Available";
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(852, 311);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(852, 336);
            this.toolStripContainer1.TabIndex = 0;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // filtersTreeToolTip
            // 
            this.filtersTreeToolTip.ToolTipTitle = "Custom Filter Summary";
            // 
            // IssueListWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.mainContainer);
            this.Controls.Add(this.toolStripContainer1);
            this.Name = "IssueListWindow";
            this.Size = new System.Drawing.Size(852, 336);
            this.mainContainer.ContentPanel.ResumeLayout(false);
            this.mainContainer.LeftToolStripPanel.ResumeLayout(false);
            this.mainContainer.LeftToolStripPanel.PerformLayout();
            this.mainContainer.ResumeLayout(false);
            this.mainContainer.PerformLayout();
            this.productTabs.ResumeLayout(false);
            this.tabJira.ResumeLayout(false);
            this.jiraContainer.BottomToolStripPanel.ResumeLayout(false);
            this.jiraContainer.BottomToolStripPanel.PerformLayout();
            this.jiraContainer.ContentPanel.ResumeLayout(false);
            this.jiraContainer.ResumeLayout(false);
            this.jiraContainer.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.jiraSplitter.Panel1.ResumeLayout(false);
            this.jiraSplitter.Panel2.ResumeLayout(false);
            this.jiraSplitter.ResumeLayout(false);
            this.filterTreeContainer.ContentPanel.ResumeLayout(false);
            this.filterTreeContainer.TopToolStripPanel.ResumeLayout(false);
            this.filterTreeContainer.TopToolStripPanel.PerformLayout();
            this.filterTreeContainer.ResumeLayout(false);
            this.filterTreeContainer.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.issueTreeContainer.TopToolStripPanel.ResumeLayout(false);
            this.issueTreeContainer.TopToolStripPanel.PerformLayout();
            this.issueTreeContainer.ResumeLayout(false);
            this.issueTreeContainer.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
            this.globalToolBar.ResumeLayout(false);
            this.globalToolBar.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer mainContainer;
        private System.Windows.Forms.ToolStripContainer jiraContainer;
        private System.Windows.Forms.TabControl productTabs;
        private System.Windows.Forms.TabPage tabJira;
        private System.Windows.Forms.ToolStrip globalToolBar;
        private System.Windows.Forms.ToolStripButton buttonProjectProperties;
        private System.Windows.Forms.SplitContainer jiraSplitter;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel jiraStatus;
        private System.Windows.Forms.ToolStripButton buttonAbout;
        private System.Windows.Forms.ToolStripContainer filterTreeContainer;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStripContainer issueTreeContainer;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton buttonRefreshAll;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.TreeView filtersTree;
        private System.Windows.Forms.ToolStripStatusLabel getMoreIssues;
        private System.Windows.Forms.ToolStripButton buttonOpen;
        private System.Windows.Forms.ToolStripButton buttonViewInBrowser;
        private System.Windows.Forms.ToolStripButton buttonEditInBrowser;
        private System.Windows.Forms.ToolStripButton buttonRefresh;
        private System.Windows.Forms.ToolStripButton buttonSearch;
        private System.Windows.Forms.ToolTip filtersTreeToolTip;
        private System.Windows.Forms.ToolStripButton buttonGlobalProperties;
        private System.Windows.Forms.ToolStripButton buttonUpdate;
        private System.Windows.Forms.ToolStripLabel labelGroupBy;
        private System.Windows.Forms.ToolStripComboBox comboGroupBy;
        private System.Windows.Forms.ToolStripButton buttonExpandAll;
        private System.Windows.Forms.ToolStripButton buttonCollapseAll;
        private System.Windows.Forms.ToolStripLabel labelNarrow;
        private System.Windows.Forms.ToolStripComboBox comboFind;
        private System.Windows.Forms.ToolStripButton buttonAddFilter;
        private System.Windows.Forms.ToolStripButton buttonRemoveFilter;
        private System.Windows.Forms.ToolStripButton buttonEditFilter;
    }
}