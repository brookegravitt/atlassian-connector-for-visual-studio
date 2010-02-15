namespace Atlassian.plvs.dialogs.bamboo {
    partial class AddOrEditBambooServer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddOrEditBambooServer));
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonAddOrEdit = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.name = new System.Windows.Forms.TextBox();
            this.url = new System.Windows.Forms.TextBox();
            this.user = new System.Windows.Forms.TextBox();
            this.password = new System.Windows.Forms.TextBox();
            this.radioUseFavourites = new System.Windows.Forms.RadioButton();
            this.radioSelectManually = new System.Windows.Forms.RadioButton();
            this.checkedListBuilds = new System.Windows.Forms.CheckedListBox();
            this.buttonGetBuilds = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(377, 398);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 0;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonAddOrEdit
            // 
            this.buttonAddOrEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAddOrEdit.Location = new System.Drawing.Point(296, 398);
            this.buttonAddOrEdit.Name = "buttonAddOrEdit";
            this.buttonAddOrEdit.Size = new System.Drawing.Size(75, 23);
            this.buttonAddOrEdit.TabIndex = 1;
            this.buttonAddOrEdit.Text = "AddOrEdit";
            this.buttonAddOrEdit.UseVisualStyleBackColor = true;
            this.buttonAddOrEdit.Click += new System.EventHandler(this.buttonAddOrEdit_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Server Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 98);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Password";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "User Name";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(18, 49);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Server URL";
            // 
            // name
            // 
            this.name.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.name.Location = new System.Drawing.Point(97, 20);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(355, 20);
            this.name.TabIndex = 6;
            this.name.TextChanged += new System.EventHandler(this.name_TextChanged);
            // 
            // url
            // 
            this.url.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.url.Location = new System.Drawing.Point(97, 46);
            this.url.Name = "url";
            this.url.Size = new System.Drawing.Size(355, 20);
            this.url.TabIndex = 7;
            this.url.TextChanged += new System.EventHandler(this.url_TextChanged);
            // 
            // user
            // 
            this.user.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.user.Location = new System.Drawing.Point(97, 72);
            this.user.Name = "user";
            this.user.Size = new System.Drawing.Size(355, 20);
            this.user.TabIndex = 8;
            this.user.TextChanged += new System.EventHandler(this.user_TextChanged);
            // 
            // password
            // 
            this.password.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.password.Location = new System.Drawing.Point(97, 98);
            this.password.Name = "password";
            this.password.PasswordChar = '*';
            this.password.Size = new System.Drawing.Size(355, 20);
            this.password.TabIndex = 9;
            // 
            // radioUseFavourites
            // 
            this.radioUseFavourites.AutoSize = true;
            this.radioUseFavourites.Location = new System.Drawing.Point(97, 125);
            this.radioUseFavourites.Name = "radioUseFavourites";
            this.radioUseFavourites.Size = new System.Drawing.Size(138, 17);
            this.radioUseFavourites.TabIndex = 10;
            this.radioUseFavourites.TabStop = true;
            this.radioUseFavourites.Text = "Monitor Favourite Builds";
            this.radioUseFavourites.UseVisualStyleBackColor = true;
            this.radioUseFavourites.CheckedChanged += new System.EventHandler(this.radioUseFavourites_CheckedChanged);
            // 
            // radioSelectManually
            // 
            this.radioSelectManually.AutoSize = true;
            this.radioSelectManually.Location = new System.Drawing.Point(97, 149);
            this.radioSelectManually.Name = "radioSelectManually";
            this.radioSelectManually.Size = new System.Drawing.Size(136, 17);
            this.radioSelectManually.TabIndex = 11;
            this.radioSelectManually.TabStop = true;
            this.radioSelectManually.Text = "Select Builds to Monitor";
            this.radioSelectManually.UseVisualStyleBackColor = true;
            this.radioSelectManually.CheckedChanged += new System.EventHandler(this.radioSelectManually_CheckedChanged);
            // 
            // checkedListBuilds
            // 
            this.checkedListBuilds.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.checkedListBuilds.FormattingEnabled = true;
            this.checkedListBuilds.Location = new System.Drawing.Point(97, 173);
            this.checkedListBuilds.Name = "checkedListBuilds";
            this.checkedListBuilds.Size = new System.Drawing.Size(355, 214);
            this.checkedListBuilds.TabIndex = 12;
            // 
            // buttonGetBuilds
            // 
            this.buttonGetBuilds.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonGetBuilds.Location = new System.Drawing.Point(334, 144);
            this.buttonGetBuilds.Name = "buttonGetBuilds";
            this.buttonGetBuilds.Size = new System.Drawing.Size(117, 23);
            this.buttonGetBuilds.TabIndex = 13;
            this.buttonGetBuilds.Text = "Get Build List";
            this.buttonGetBuilds.UseVisualStyleBackColor = true;
            this.buttonGetBuilds.Click += new System.EventHandler(this.buttonGetBuilds_Click);
            // 
            // AddOrEditBambooServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 430);
            this.ControlBox = false;
            this.Controls.Add(this.buttonGetBuilds);
            this.Controls.Add(this.checkedListBuilds);
            this.Controls.Add(this.radioSelectManually);
            this.Controls.Add(this.radioUseFavourites);
            this.Controls.Add(this.password);
            this.Controls.Add(this.user);
            this.Controls.Add(this.url);
            this.Controls.Add(this.name);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonAddOrEdit);
            this.Controls.Add(this.buttonCancel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "AddOrEditBambooServer";
            this.Text = "AddOrEditBambooServer";
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.AddOrEditJiraServer_KeyPress);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonAddOrEdit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox name;
        private System.Windows.Forms.TextBox url;
        private System.Windows.Forms.TextBox user;
        private System.Windows.Forms.TextBox password;
        private System.Windows.Forms.RadioButton radioUseFavourites;
        private System.Windows.Forms.RadioButton radioSelectManually;
        private System.Windows.Forms.CheckedListBox checkedListBuilds;
        private System.Windows.Forms.Button buttonGetBuilds;
    }
}