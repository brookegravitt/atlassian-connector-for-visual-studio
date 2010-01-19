using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Atlassian.plvs.util;

namespace Atlassian.plvs.ui {
    public class StatusLabel {
        private readonly StatusStrip statusBar;
        private readonly ToolStripStatusLabel targetLabel;

        public StatusLabel(StatusStrip statusBar, ToolStripStatusLabel targetLabel) {
            this.statusBar = statusBar;
            this.targetLabel = targetLabel;

//            targetLabel.TextChanged += targetLabel_TextChanged;
        }

//        private void targetLabel_TextChanged(object sender, EventArgs e) {
//            Debug.WriteLine(targetLabel.Text);
//        }

        private Exception lastException;

        public void setError(string txt, Exception e) {
            try {
                statusBar.Invoke(new MethodInvoker(delegate
                                                       {
                                                           targetLabel.BackColor = Color.LightPink;
                                                           Exception inner = e.InnerException;
                                                           statusBar.BackColor = Color.LightPink;
                                                           targetLabel.Text = txt;
                                                           lastException = inner ?? e;
                                                           targetLabel.Visible = true;
                                                           targetLabel.IsLink = true;
                                                           targetLabel.Click += targetLabel_Click;
                                                           targetLabel.Image = SystemIcons.Error.ToBitmap();
                                                       }));
            } catch (InvalidOperationException ex) {
                Debug.WriteLine("StatusLabel.setInfo(): " + ex.Message);
            }
        }

        private void targetLabel_Click(object sender, EventArgs e) {
            if (lastException == null) {
                return;
            }
            MessageBox.Show(
                lastException.Message + "\n\nPress Ctrl+C to copy error text to clipboard", 
                Constants.ERROR_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
            lastException = null;
            targetLabel.BackColor = SystemColors.Control;
            statusBar.BackColor = SystemColors.Control;
            targetLabel.Text = "";
            targetLabel.IsLink = false;
            targetLabel.Image = null;
        }

        public void setInfo(string txt) {
            try {
                statusBar.Invoke(new MethodInvoker(delegate
                                                       {
                                                           targetLabel.BackColor = SystemColors.Control;
                                                           statusBar.BackColor = SystemColors.Control;
                                                           targetLabel.Text = txt;
                                                           targetLabel.Visible = true;
                                                           targetLabel.IsLink = false;
                                                           targetLabel.Image = null;
                                                       }));
            } catch (InvalidOperationException e) {
                Debug.WriteLine("StatusLabel.setInfo(): " + e.Message);
            }
        }
    }
}