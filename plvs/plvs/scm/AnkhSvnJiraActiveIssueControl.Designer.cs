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
            this.SuspendLayout();
            // 
            // labelJira
            // 
            this.labelJira.AutoSize = true;
            this.labelJira.Location = new System.Drawing.Point(125, 97);
            this.labelJira.Name = "labelJira";
            this.labelJira.Size = new System.Drawing.Size(30, 13);
            this.labelJira.TabIndex = 0;
            this.labelJira.Text = "JIRA";
            // 
            // AnkhSvnJiraActiveIssueControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelJira);
            this.Name = "AnkhSvnJiraActiveIssueControl";
            this.Size = new System.Drawing.Size(341, 207);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelJira;
    }
}
