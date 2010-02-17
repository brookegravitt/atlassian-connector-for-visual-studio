using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.ui.jira {
    public partial class JiraTextAreaWithWikiPreview : UserControl {
        
        private string throbberPath;

        public JiraTextAreaWithWikiPreview() {
            IssueType = -1;
            InitializeComponent();

            Assembly assembly = Assembly.GetExecutingAssembly();
            string name = assembly.EscapedCodeBase;

            if (name != null) {
                name = name.Substring(0, name.LastIndexOf("/"));
                throbberPath = name + "/ajax-loader.gif";
            }
        }

        public override string Text {
            get { return textMarkup.Text; }
            set { textMarkup.Text = value; }
        }

        public JiraServerFacade Facade { get; set; }

        public JiraServer Server { get; set; }

        public JiraIssue JiraIssue { get; set; }

        public JiraProject JiraProject { get; set; }

        public int IssueType { get; set; }

        private void tabContents_Selected(object sender, TabControlEventArgs e) {
            if (e.TabPage != tabPreview) return;
            if (textMarkup.Text.Length == 0) {
                webPreview.DocumentText = "";
                return;
            }
            webPreview.DocumentText = getThrobberHtml();
            Thread t = new Thread(() => getMarkup(textMarkup.Text));
            t.Start();
        }

        private void getMarkup(string text) {
            if (Facade == null || JiraIssue == null && !(Server != null && JiraProject != null && IssueType > -1)) {
                Invoke(new MethodInvoker(delegate {
                    webPreview.DocumentText = "Unable to render preview";
                }));
                return;
            }
            try {
                string renderedContent = JiraIssue != null 
                    ? Facade.getRenderedContent(JiraIssue, text) 
                    : Facade.getRenderedContent(Server, IssueType, JiraProject, text);
                Invoke(new MethodInvoker(delegate {
                                             webPreview.DocumentText = renderedContent;
                                         }));
            } catch (Exception e) {
                // just log the problem. This is an informational functionality only, 
                // let's not make a big deal out of errors here
                Debug.WriteLine("JiraTextAreaWithWikiPreview.getMarkup() - exception: " + e.Message);
            }
        }

        private string getThrobberHtml() {
            if (throbberPath == null) {
                return "<html><head>" + Resources.summary_and_description_css + "</head><body>Fetching preview...</body></html>";
            }
            return string.Format(Resources.throbber_html, throbberPath);
        }

        private void textMarkup_TextChanged(object sender, EventArgs e) {
            if (MarkupTextChanged != null) {
                MarkupTextChanged(this, new EventArgs());
            }
        }

        public event EventHandler<EventArgs> MarkupTextChanged;
    }
}
