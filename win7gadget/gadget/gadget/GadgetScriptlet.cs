// GadgetScriptlet.cs
//

using System;
using System.DHTML;
using System.Gadgets;
using System.XML;
using ScriptFX;
using ScriptFX.Net;
using ScriptFX.UI;

namespace gadget {

    public class GadgetScriptlet {

//        private bool flyoutVisible;

        private readonly Button pollNowButton;
        private readonly DOMElement jiraResponse;
        private static string flyoutText = "";
        private static Label labelDebug;

        private static bool doShowFlyoutAgain;

//        private int settingsCommitCount;
//        private int issueCount;

        private static readonly ArrayList issues = new ArrayList();

        static GadgetScriptlet() {
            if (Document.Body.ID == "gadget") {
                ScriptHost.Run(typeof(GadgetScriptlet), null);
            }
        }

        private GadgetScriptlet() {
            Gadget.OnDock = OnDock;
            Gadget.OnUndock = OnUndock;

            Gadget.Flyout.File = "Flyout.htm";
            Gadget.Flyout.OnShow = OnFlyoutShow;
            Gadget.Flyout.OnHide = OnFlyoutHide;

            Gadget.SettingsUI = "Settings.htm";
            Gadget.OnSettingsClosed = SettingsClosed;

            UpdateDockedState();

            pollNowButton = new Button(Document.GetElementById("pollNowButton"));
            pollNowButton.Click += pollNowButton_Click;

            labelDebug = new Label(Document.GetElementById("debug"));
//            settingsUpdateLabel = new Label(Document.GetElementById("settingsText"));
//            tickerLabel = new Label(Document.GetElementById("ticker"));

            jiraResponse = Document.GetElementById("jiraResponse");
            jiraResponse.Style.Width = Gadget.Docked ? "180px" : "380px";
            jiraResponse.Style.Height = "280px";

//            Window.SetInterval(OnTimer, 2000);
        }

        private void pollNowButton_Click(object sender, EventArgs e) {
            pollJira();
        }

//        private void OnTimer() {
//            tickerLabel.Text = new DateTime().Format("T");    
//        }

        public static void openFlyout(int issueId) {
            labelDebug.Text = "openFlyout: " + issueId;
            if (issueId >= 0) {
                Issue issue = (Issue) issues[issueId];
                flyoutText = createIssueHtml(issue);
                doShowFlyoutAgain = true;
            } else {
                flyoutText = null;
                doShowFlyoutAgain = false;
            }
            if (Gadget.Flyout.Show) {
                Gadget.Flyout.Show = false;
            } else {
                Gadget.Flyout.Show = issueId >= 0;
            }
        }

        public static void Main(Dictionary arguments) {
            GadgetScriptlet scriptlet = new GadgetScriptlet();
        }

        private void OnDock() {
            UpdateDockedState();
        }

        private void OnUndock() {
            UpdateDockedState();
        }

        private void OnFlyoutHide() {
//            flyoutVisible = false;
            if (doShowFlyoutAgain) {
                labelDebug.Text = "re-showing flyout";
                Window.SetTimeout(reShowFlyout, 300);
            }
        }

        private void reShowFlyout() {
            Gadget.Flyout.Show = true;
        }

        private void OnFlyoutShow() {
//            flyoutVisible = true;

            labelDebug.Text = "setting flyout text";
            FlyoutScriptlet.setText(flyoutText);
            doShowFlyoutAgain = false;
        }

        private void SettingsClosed(GadgetSettingsEvent e) {
            if (e.CloseAction == GadgetSettingsCloseAction.Cancel) return;

//            ++settingsCommitCount;

//            settingsUpdateLabel.Text = string.Format("Settings updated {0} times", settingsCommitCount);
        }

        private void UpdateDockedState() {
            DOMElement body = Document.Body;

            if (Gadget.Docked) {
                Element.RemoveCSSClass(body, "undocked");
                Element.AddCSSClass(body, "docked");
                body.Style.Width = "200px";
                body.Style.Height = "400px";
            } else {
                Element.AddCSSClass(body, "undocked");
                Element.RemoveCSSClass(body, "docked");
                body.Style.Width = "400px";
                body.Style.Height = "400px";
            }
            if (jiraResponse != null) jiraResponse.Style.Width = Gadget.Docked ? "180px" : "380px";
        }

        private const string UpdatedRecently =
            "https://studio.atlassian.com/sr/jira.issueviews:searchrequest-xml/temp/SearchRequest.xml?jqlQuery=project+%3D+PL+AND+updated%3E%3D-1w+ORDER+BY+updated+DESC&tempMax=1000";
        
        private void pollJira() {
            jiraResponse.InnerText = "starting...";
            pollNowButton.DOMElement.Disabled = true;
            HTTPRequest req = HTTPRequest.CreateRequest(UpdatedRecently, HTTPVerb.GET);
            req.Invoke(RequestCompleted);
        }

        private void RequestCompleted(HTTPRequest request, object userContext) {
            HTTPStatusCode statusCode = request.Response.StatusCode;
            pollNowButton.DOMElement.Disabled = false;
            if (statusCode != HTTPStatusCode.OK) {
                jiraResponse.InnerText = "Error. Status code is " + statusCode;
            } else {
                XMLDocument resp = request.Response.GetXML();
                createIssueListFromResponseXml(resp);
                jiraResponse.InnerHTML = createIssueListHtmlFromIssueList();
            }
        }

        private static void createIssueListFromResponseXml(XMLDocument resp) {
            XMLNodeList issuesXml = resp.SelectNodes("/rss/channel/item");
            issues.Clear();
            for (int i = 0; i < issuesXml.Length; ++i) {
                XMLNode key = issuesXml[i].SelectSingleNode("key");
                XMLNode link = issuesXml[i].SelectSingleNode("link");
                XMLNode summary = issuesXml[i].SelectSingleNode("summary");
                XMLNode type = issuesXml[i].SelectSingleNode("type");

                Issue issue = new Issue(key.Text, link.Text, summary.Text, type.Text, type.Attributes.GetNamedItem("iconUrl").Text);

                issues.Add(issue);
            }
        }

        private static string createIssueListHtmlFromIssueList() {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < issues.Length; ++i) {
                Issue issue = (Issue) issues[i];

                sb.Append("<div onclick=\"javascript:gadget.GadgetScriptlet.openFlyout(");
                sb.Append(i);
                sb.Append(")\" class=\"");
                sb.Append(i % 2 > 0 ? "oddRow" : "evenRow");
                sb.Append("\" id=\"issue");
                sb.Append(i);
                sb.Append("\">");
                sb.Append("<img align=absmiddle src=\"");
                sb.Append(issue.IssueTypeIconUrl);
                sb.Append("\">");
                sb.Append("<a href=\"");
                sb.Append(issue.Link);
                sb.Append("\"> ");
                sb.Append(issue.Key);
                sb.Append("</a> ");
                sb.Append(issue.Summary);
                sb.Append("</div>\r\n");
            }
            return sb.ToString();
        }

        private static string createIssueHtml(Issue issue) {
            StringBuilder sb = new StringBuilder();
            sb.Append("<img align=absmiddle src=\"");
            sb.Append(issue.IssueTypeIconUrl);
            sb.Append("\"> ");
            sb.Append("<a href=\"");
            sb.Append(issue.Link);
            sb.Append("\">");
            sb.Append(issue.Key);
            sb.Append("</a><br><br>");
            sb.Append("<table class=\"issueTable\" ><tr><td valign=\"top\" class=\"issueTableHeader\">");
            sb.Append("Type</td><td valign=\"top\" class=\"issueTableContent\">");
            sb.Append(issue.IssueType);
            sb.Append("</td></tr><tr><td valign=\"top\" class=\"issueTableHeader\">");
            sb.Append("Summary</td><td valign=\"top\" class=\"issueTableContent\">");
            sb.Append(issue.Summary);
            sb.Append("</td></tr></table>");
            return sb.ToString();
        }
    }
}
