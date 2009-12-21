namespace Atlassian.plvs.dialogs {
    partial class EditCustomFilter
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonClose = new System.Windows.Forms.Button();
            this.listBoxProjects = new System.Windows.Forms.ListBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.listViewIssueTypes = new System.Windows.Forms.ListView();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.listBoxFixForVersions = new System.Windows.Forms.ListBox();
            this.listBoxAffectsVersions = new System.Windows.Forms.ListBox();
            this.listBoxComponents = new System.Windows.Forms.ListBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.listViewPriorities = new System.Windows.Forms.ListView();
            this.listViewStatuses = new System.Windows.Forms.ListView();
            this.listBoxResolutions = new System.Windows.Forms.ListBox();
            this.comboBoxAssignee = new System.Windows.Forms.ComboBox();
            this.comboBoxReporter = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonClear = new System.Windows.Forms.Button();
            this.textBoxFilterName = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.Location = new System.Drawing.Point(802, 495);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 2;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // listBoxProjects
            // 
            this.listBoxProjects.FormattingEnabled = true;
            this.listBoxProjects.Location = new System.Drawing.Point(12, 34);
            this.listBoxProjects.Name = "listBoxProjects";
            this.listBoxProjects.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxProjects.Size = new System.Drawing.Size(208, 173);
            this.listBoxProjects.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listViewIssueTypes);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.listBoxProjects);
            this.groupBox1.Location = new System.Drawing.Point(7, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(236, 444);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Project / Issue";
            // 
            // listViewIssueTypes
            // 
            this.listViewIssueTypes.FullRowSelect = true;
            this.listViewIssueTypes.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewIssueTypes.HideSelection = false;
            this.listViewIssueTypes.Location = new System.Drawing.Point(12, 242);
            this.listViewIssueTypes.Name = "listViewIssueTypes";
            this.listViewIssueTypes.Size = new System.Drawing.Size(208, 178);
            this.listViewIssueTypes.TabIndex = 1;
            this.listViewIssueTypes.UseCompatibleStateImageBehavior = false;
            this.listViewIssueTypes.View = System.Windows.Forms.View.Details;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 224);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Issue Type";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Project";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.listBoxFixForVersions);
            this.groupBox2.Controls.Add(this.listBoxAffectsVersions);
            this.groupBox2.Controls.Add(this.listBoxComponents);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Location = new System.Drawing.Point(249, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(351, 444);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Components / Versions";
            // 
            // listBoxFixForVersions
            // 
            this.listBoxFixForVersions.FormattingEnabled = true;
            this.listBoxFixForVersions.Location = new System.Drawing.Point(99, 34);
            this.listBoxFixForVersions.Name = "listBoxFixForVersions";
            this.listBoxFixForVersions.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxFixForVersions.Size = new System.Drawing.Size(236, 121);
            this.listBoxFixForVersions.TabIndex = 0;
            // 
            // listBoxAffectsVersions
            // 
            this.listBoxAffectsVersions.FormattingEnabled = true;
            this.listBoxAffectsVersions.Location = new System.Drawing.Point(99, 299);
            this.listBoxAffectsVersions.Name = "listBoxAffectsVersions";
            this.listBoxAffectsVersions.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxAffectsVersions.Size = new System.Drawing.Size(236, 121);
            this.listBoxAffectsVersions.TabIndex = 2;
            // 
            // listBoxComponents
            // 
            this.listBoxComponents.FormattingEnabled = true;
            this.listBoxComponents.Location = new System.Drawing.Point(99, 166);
            this.listBoxComponents.Name = "listBoxComponents";
            this.listBoxComponents.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxComponents.Size = new System.Drawing.Size(236, 121);
            this.listBoxComponents.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 303);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(83, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Affects Versions";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 170);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Components";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 37);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Fix For";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.listViewPriorities);
            this.groupBox3.Controls.Add(this.listViewStatuses);
            this.groupBox3.Controls.Add(this.listBoxResolutions);
            this.groupBox3.Controls.Add(this.comboBoxAssignee);
            this.groupBox3.Controls.Add(this.comboBoxReporter);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Location = new System.Drawing.Point(606, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(271, 444);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Issue Attributes";
            // 
            // listViewPriorities
            // 
            this.listViewPriorities.FullRowSelect = true;
            this.listViewPriorities.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewPriorities.HideSelection = false;
            this.listViewPriorities.Location = new System.Drawing.Point(84, 325);
            this.listViewPriorities.Name = "listViewPriorities";
            this.listViewPriorities.Size = new System.Drawing.Size(173, 91);
            this.listViewPriorities.TabIndex = 4;
            this.listViewPriorities.UseCompatibleStateImageBehavior = false;
            this.listViewPriorities.View = System.Windows.Forms.View.Details;
            // 
            // listViewStatuses
            // 
            this.listViewStatuses.FullRowSelect = true;
            this.listViewStatuses.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewStatuses.HideSelection = false;
            this.listViewStatuses.Location = new System.Drawing.Point(84, 116);
            this.listViewStatuses.Name = "listViewStatuses";
            this.listViewStatuses.Size = new System.Drawing.Size(173, 91);
            this.listViewStatuses.TabIndex = 2;
            this.listViewStatuses.UseCompatibleStateImageBehavior = false;
            this.listViewStatuses.View = System.Windows.Forms.View.Details;
            // 
            // listBoxResolutions
            // 
            this.listBoxResolutions.Enabled = false;
            this.listBoxResolutions.FormattingEnabled = true;
            this.listBoxResolutions.Location = new System.Drawing.Point(84, 219);
            this.listBoxResolutions.Name = "listBoxResolutions";
            this.listBoxResolutions.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBoxResolutions.Size = new System.Drawing.Size(173, 95);
            this.listBoxResolutions.TabIndex = 3;
            // 
            // comboBoxAssignee
            // 
            this.comboBoxAssignee.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxAssignee.Enabled = false;
            this.comboBoxAssignee.FormattingEnabled = true;
            this.comboBoxAssignee.Location = new System.Drawing.Point(84, 61);
            this.comboBoxAssignee.Name = "comboBoxAssignee";
            this.comboBoxAssignee.Size = new System.Drawing.Size(173, 21);
            this.comboBoxAssignee.TabIndex = 1;
            // 
            // comboBoxReporter
            // 
            this.comboBoxReporter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxReporter.Enabled = false;
            this.comboBoxReporter.FormattingEnabled = true;
            this.comboBoxReporter.Location = new System.Drawing.Point(84, 34);
            this.comboBoxReporter.Name = "comboBoxReporter";
            this.comboBoxReporter.Size = new System.Drawing.Size(173, 21);
            this.comboBoxReporter.TabIndex = 0;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(13, 325);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(46, 13);
            this.label10.TabIndex = 4;
            this.label10.Text = "Priorities";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(13, 221);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(62, 13);
            this.label9.TabIndex = 3;
            this.label9.Text = "Resolutions";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(13, 116);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(37, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "Status";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 64);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(50, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "Assignee";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 37);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(48, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Reporter";
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.Location = new System.Drawing.Point(640, 495);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 0;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // buttonClear
            // 
            this.buttonClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClear.Location = new System.Drawing.Point(721, 495);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(75, 23);
            this.buttonClear.TabIndex = 1;
            this.buttonClear.Text = "Clear Filter";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // textBoxFilterName
            // 
            this.textBoxFilterName.Location = new System.Drawing.Point(70, 462);
            this.textBoxFilterName.Name = "textBoxFilterName";
            this.textBoxFilterName.Size = new System.Drawing.Size(807, 20);
            this.textBoxFilterName.TabIndex = 3;
            this.textBoxFilterName.TextChanged += new System.EventHandler(this.textBoxFilterName_TextChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(4, 465);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(60, 13);
            this.label11.TabIndex = 4;
            this.label11.Text = "Filter Name";
            // 
            // EditCustomFilter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(889, 536);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.textBoxFilterName);
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditCustomFilter";
            this.ShowIcon = false;
            this.Text = "Edit Custom Filter";
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.EditCustomFilter_KeyPress);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.ListBox listBoxProjects;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox listBoxAffectsVersions;
        private System.Windows.Forms.ListBox listBoxComponents;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ListBox listBoxResolutions;
        private System.Windows.Forms.ComboBox comboBoxAssignee;
        private System.Windows.Forms.ComboBox comboBoxReporter;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.ListView listViewIssueTypes;
        private System.Windows.Forms.ListBox listBoxFixForVersions;
        private System.Windows.Forms.ListView listViewPriorities;
        private System.Windows.Forms.ListView listViewStatuses;
        private System.Windows.Forms.TextBox textBoxFilterName;
        private System.Windows.Forms.Label label11;
    }
}