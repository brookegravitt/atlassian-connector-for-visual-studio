namespace Atlassian.plvs.dialogs.jira {
    partial class NewIssueComment
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewIssueComment));
            this.buttonOk = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.commentText = new System.Windows.Forms.TextBox();
            this.tabCommentText = new System.Windows.Forms.TabControl();
            this.tabMarkup = new System.Windows.Forms.TabPage();
            this.tabPreview = new System.Windows.Forms.TabPage();
            this.webPreview = new System.Windows.Forms.WebBrowser();
            this.tabCommentText.SuspendLayout();
            this.tabMarkup.SuspendLayout();
            this.tabPreview.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Location = new System.Drawing.Point(387, 180);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 1;
            this.buttonOk.Text = "Add";
            this.buttonOk.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(468, 180);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // commentText
            // 
            this.commentText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.commentText.Location = new System.Drawing.Point(3, 3);
            this.commentText.Multiline = true;
            this.commentText.Name = "commentText";
            this.commentText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.commentText.Size = new System.Drawing.Size(517, 130);
            this.commentText.TabIndex = 0;
            this.commentText.TextChanged += new System.EventHandler(this.commentText_TextChanged);
            // 
            // tabCommentText
            // 
            this.tabCommentText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabCommentText.Controls.Add(this.tabMarkup);
            this.tabCommentText.Controls.Add(this.tabPreview);
            this.tabCommentText.Location = new System.Drawing.Point(12, 12);
            this.tabCommentText.Name = "tabCommentText";
            this.tabCommentText.SelectedIndex = 0;
            this.tabCommentText.Size = new System.Drawing.Size(531, 162);
            this.tabCommentText.TabIndex = 4;
            this.tabCommentText.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabCommentText_Selected);
            // 
            // tabMarkup
            // 
            this.tabMarkup.Controls.Add(this.commentText);
            this.tabMarkup.Location = new System.Drawing.Point(4, 22);
            this.tabMarkup.Name = "tabMarkup";
            this.tabMarkup.Padding = new System.Windows.Forms.Padding(3);
            this.tabMarkup.Size = new System.Drawing.Size(523, 136);
            this.tabMarkup.TabIndex = 0;
            this.tabMarkup.Text = "Markup";
            this.tabMarkup.UseVisualStyleBackColor = true;
            // 
            // tabPreview
            // 
            this.tabPreview.Controls.Add(this.webPreview);
            this.tabPreview.Location = new System.Drawing.Point(4, 22);
            this.tabPreview.Name = "tabPreview";
            this.tabPreview.Padding = new System.Windows.Forms.Padding(3);
            this.tabPreview.Size = new System.Drawing.Size(523, 136);
            this.tabPreview.TabIndex = 1;
            this.tabPreview.Text = "Preview";
            this.tabPreview.UseVisualStyleBackColor = true;
            // 
            // webPreview
            // 
            this.webPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webPreview.Location = new System.Drawing.Point(3, 3);
            this.webPreview.MinimumSize = new System.Drawing.Size(20, 20);
            this.webPreview.Name = "webPreview";
            this.webPreview.Size = new System.Drawing.Size(517, 130);
            this.webPreview.TabIndex = 0;
            // 
            // NewIssueComment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(555, 215);
            this.Controls.Add(this.tabCommentText);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NewIssueComment";
            this.Text = "Add Comment";
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NewIssueComment_KeyPress);
            this.tabCommentText.ResumeLayout(false);
            this.tabMarkup.ResumeLayout(false);
            this.tabMarkup.PerformLayout();
            this.tabPreview.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.TextBox commentText;
        private System.Windows.Forms.TabControl tabCommentText;
        private System.Windows.Forms.TabPage tabMarkup;
        private System.Windows.Forms.TabPage tabPreview;
        private System.Windows.Forms.WebBrowser webPreview;
    }
}