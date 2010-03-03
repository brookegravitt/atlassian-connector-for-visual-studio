using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Atlassian.plvs.dialogs {
    public partial class MessageBoxWithHtml : Form {

        public static void showError(string title, string html) {
            MessageBoxWithHtml box = new MessageBoxWithHtml
                                     {
                                         Text = title,
                                         labelIcon = {Image = SystemIcons.Error.ToBitmap()},
                                     };
            box.webContent.DocumentText = getHtml(box.labelIcon.Font, html);
            box.ShowDialog();
        }

        private static string getHtml(Font font, string html) {
            string fontFamily = font.FontFamily.Name;
            return "<html><body style=\"margin:0;padding:0;font-family:" + fontFamily + ";font-size:12px;\">" + html + "</body>";
        }

        private MessageBoxWithHtml() {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterParent;
        }

        private void buttonOk_Click(object sender, EventArgs e) {
            Close();
        }

        private void webContent_Navigating(object sender, WebBrowserNavigatingEventArgs e) {
            if (e.Url.Equals("about:blank")) {
                return;
            }
            e.Cancel = true;
            string url = e.Url.ToString();
            try {
                Process.Start(url);
                // ReSharper disable EmptyGeneralCatchClause
            } catch {
                // ReSharper restore EmptyGeneralCatchClause
            }
        }
    }
}
