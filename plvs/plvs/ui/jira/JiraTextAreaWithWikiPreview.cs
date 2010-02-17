﻿using System;
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

        public JiraIssue Issue { get; set; }

        public JiraProject Project { get; set; }

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
            if (Facade == null || Issue == null && !(Server != null && Project != null && IssueType > -1)) {
                Invoke(new MethodInvoker(delegate {
                                             webPreview.DocumentText =
                                                 "<html><head>" + Resources.summary_and_description_css
                                                 + "</head><body class=\"summary\">Unable to render preview</body></html>";
                                         }));
                return;
            }
            try {
                string renderedContent = Issue != null 
                    ? Facade.getRenderedContent(Issue, text) 
                    : Facade.getRenderedContent(Server, IssueType, Project, text);
                Invoke(new MethodInvoker(delegate {
                                             webPreview.DocumentText = 
                                                 "<html><head>" + Resources.summary_and_description_css
                                                 + "</head><body class=\"summary\">" + renderedContent + "</body></html>";
                                         }));
            } catch (Exception e) {
                // just log the problem. This is an informational functionality only, 
                // let's not make a big deal out of errors here
                Debug.WriteLine("JiraTextAreaWithWikiPreview.getMarkup() - exception: " + e.Message);
            }
        }

        private string getThrobberHtml() {
            if (throbberPath == null) {
                return "<html><head>" + Resources.summary_and_description_css + "</head><body class=\"summary\">Fetching preview...</body></html>";
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