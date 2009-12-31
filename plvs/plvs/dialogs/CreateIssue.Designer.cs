using Atlassian.plvs.api;
using Atlassian.plvs.ui;

namespace Atlassian.plvs.dialogs {
    partial class CreateIssue {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.buttonCreate = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.comboProjects = new System.Windows.Forms.ComboBox();
            this.comboTypes = new JiraNamedEntityComboBox();
            this.listComponents = new System.Windows.Forms.ListBox();
            this.listAffectsVersions = new System.Windows.Forms.ListBox();
            this.listFixVersions = new System.Windows.Forms.ListBox();
            this.comboPriorities = new JiraNamedEntityComboBox();
            this.textSummary = new System.Windows.Forms.TextBox();
            this.textDescription = new System.Windows.Forms.TextBox();
            this.textAssignee = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonCreate
            // 
            this.buttonCreate.Location = new System.Drawing.Point(372, 574);
            this.buttonCreate.Name = "buttonCreate";
            this.buttonCreate.Size = new System.Drawing.Size(75, 23);
            this.buttonCreate.TabIndex = 9;
            this.buttonCreate.Text = "Create";
            this.buttonCreate.UseVisualStyleBackColor = true;
            this.buttonCreate.Click += new System.EventHandler(this.buttonCreate_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(453, 574);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 10;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // comboProjects
            // 
            this.comboProjects.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboProjects.FormattingEnabled = true;
            this.comboProjects.Location = new System.Drawing.Point(102, 12);
            this.comboProjects.Name = "comboProjects";
            this.comboProjects.Size = new System.Drawing.Size(424, 21);
            this.comboProjects.TabIndex = 0;
            this.comboProjects.SelectedIndexChanged += new System.EventHandler(this.comboProjects_SelectedIndexChanged);
            // 
            // comboTypes
            // 
            this.comboTypes.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboTypes.FormattingEnabled = true;
            this.comboTypes.ImageList = null;
            this.comboTypes.ItemHeight = 16;
            this.comboTypes.Location = new System.Drawing.Point(102, 40);
            this.comboTypes.Name = "comboTypes";
            this.comboTypes.Size = new System.Drawing.Size(424, 22);
            this.comboTypes.TabIndex = 1;
            this.comboTypes.SelectedIndexChanged += new System.EventHandler(this.comboTypes_SelectedIndexChanged);
            // 
            // listComponents
            // 
            this.listComponents.FormattingEnabled = true;
            this.listComponents.Location = new System.Drawing.Point(102, 67);
            this.listComponents.Name = "listComponents";
            this.listComponents.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listComponents.Size = new System.Drawing.Size(424, 95);
            this.listComponents.TabIndex = 2;
            // 
            // listAffectsVersions
            // 
            this.listAffectsVersions.FormattingEnabled = true;
            this.listAffectsVersions.Location = new System.Drawing.Point(102, 168);
            this.listAffectsVersions.Name = "listAffectsVersions";
            this.listAffectsVersions.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listAffectsVersions.Size = new System.Drawing.Size(424, 95);
            this.listAffectsVersions.TabIndex = 3;
            // 
            // listFixVersions
            // 
            this.listFixVersions.FormattingEnabled = true;
            this.listFixVersions.Location = new System.Drawing.Point(102, 269);
            this.listFixVersions.Name = "listFixVersions";
            this.listFixVersions.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listFixVersions.Size = new System.Drawing.Size(424, 95);
            this.listFixVersions.TabIndex = 4;
            // 
            // comboPriorities
            // 
            this.comboPriorities.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.comboPriorities.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboPriorities.FormattingEnabled = true;
            this.comboPriorities.ImageList = null;
            this.comboPriorities.ItemHeight = 16;
            this.comboPriorities.Location = new System.Drawing.Point(102, 370);
            this.comboPriorities.Name = "comboPriorities";
            this.comboPriorities.Size = new System.Drawing.Size(424, 22);
            this.comboPriorities.TabIndex = 5;
            // 
            // textSummary
            // 
            this.textSummary.Location = new System.Drawing.Point(102, 397);
            this.textSummary.Name = "textSummary";
            this.textSummary.Size = new System.Drawing.Size(424, 20);
            this.textSummary.TabIndex = 6;
            this.textSummary.TextChanged += new System.EventHandler(this.textSummary_TextChanged);
            // 
            // textDescription
            // 
            this.textDescription.Location = new System.Drawing.Point(102, 423);
            this.textDescription.Multiline = true;
            this.textDescription.Name = "textDescription";
            this.textDescription.Size = new System.Drawing.Size(424, 102);
            this.textDescription.TabIndex = 7;
            // 
            // textAssignee
            // 
            this.textAssignee.Location = new System.Drawing.Point(102, 531);
            this.textAssignee.Name = "textAssignee";
            this.textAssignee.Size = new System.Drawing.Size(136, 20);
            this.textAssignee.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(246, 534);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(282, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Warning! This field is not validated prior to sending to JIRA";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(56, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Project";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(65, 43);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Type";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(25, 67);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Component/s";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 168);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(88, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Affects Version/s";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(28, 269);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(68, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Fix Version/s";
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(58, 373);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(38, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "Priority";
            this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(46, 400);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(50, 13);
            this.label8.TabIndex = 18;
            this.label8.Text = "Summary";
            this.label8.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(36, 426);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(60, 13);
            this.label9.TabIndex = 19;
            this.label9.Text = "Description";
            this.label9.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(46, 534);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(50, 13);
            this.label10.TabIndex = 20;
            this.label10.Text = "Assignee";
            this.label10.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // CreateIssue
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(540, 609);
            this.ControlBox = false;
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textAssignee);
            this.Controls.Add(this.textDescription);
            this.Controls.Add(this.textSummary);
            this.Controls.Add(this.comboPriorities);
            this.Controls.Add(this.listFixVersions);
            this.Controls.Add(this.listAffectsVersions);
            this.Controls.Add(this.listComponents);
            this.Controls.Add(this.comboTypes);
            this.Controls.Add(this.comboProjects);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonCreate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "CreateIssue";
            this.Text = "Create JIRA Issue";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCreate;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ComboBox comboProjects;
        private JiraNamedEntityComboBox comboTypes;
        private System.Windows.Forms.ListBox listComponents;
        private System.Windows.Forms.ListBox listAffectsVersions;
        private System.Windows.Forms.ListBox listFixVersions;
        private JiraNamedEntityComboBox comboPriorities;
        private System.Windows.Forms.TextBox textSummary;
        private System.Windows.Forms.TextBox textDescription;
        private System.Windows.Forms.TextBox textAssignee;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
    }
}