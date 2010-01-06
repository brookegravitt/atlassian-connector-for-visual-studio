namespace Atlassian.plvs.scm {
    partial class AnkhSvnJiraActiveIssueControl {
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.labelJira = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // labelJira
            // 
            this.labelJira.AutoSize = true;
            this.labelJira.Location = new System.Drawing.Point(62, 41);
            this.labelJira.Name = "labelJira";
            this.labelJira.Size = new System.Drawing.Size(30, 13);
            this.labelJira.TabIndex = 0;
            this.labelJira.Text = "JIRA";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Atlassian.plvs.Resources.atlassian_538x235;
            this.pictureBox1.Location = new System.Drawing.Point(22, 20);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(561, 249);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // AnkhSvnJiraActiveIssueControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.labelJira);
            this.Controls.Add(this.pictureBox1);
            this.Name = "AnkhSvnJiraActiveIssueControl";
            this.Size = new System.Drawing.Size(1072, 294);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelJira;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}
