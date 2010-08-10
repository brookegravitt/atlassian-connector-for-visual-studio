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

        private const string FLYOUT_SHOW = "Show Details";
        private const string FLYOUT_HIDE = "Hide Details";

        private bool flyoutVisible;

        private static Button flyoutButton;
        private readonly Button pollNowButton;
        private readonly Label settingsUpdateLabel;
        private readonly Label tickerLabel;
        private readonly DOMElement jiraResponse;

        private int settingsCommitCount;
        private int issueCount;

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

            flyoutButton = new Button(Document.GetElementById("flyoutButton"));
            flyoutButton.DOMElement.InnerText = FLYOUT_SHOW;
            flyoutButton.Click += flyoutButton_Click;

            pollNowButton = new Button(Document.GetElementById("pollNowButton"));
            pollNowButton.Click += pollNowButton_Click;

            settingsUpdateLabel = new Label(Document.GetElementById("settingsText"));
            tickerLabel = new Label(Document.GetElementById("ticker"));

            jiraResponse = Document.GetElementById("jiraResponse");
            jiraResponse.Style.Width = Gadget.Docked ? "180px" : "380px";
            jiraResponse.Style.Height = "280px";

            Window.SetInterval(OnTimer, 2000);
        }

        private void pollNowButton_Click(object sender, EventArgs e) {
            pollJira();
        }

        private void OnTimer() {
            tickerLabel.Text = new DateTime().Format("T");    
        }

        private void flyoutButton_Click(object sender, EventArgs e) {
            openFlyout(!flyoutVisible);
        }

        public static void openFlyout(bool open) {
            flyoutButton.DOMElement.InnerText = open ? FLYOUT_HIDE : FLYOUT_SHOW;
            Gadget.Flyout.Show = open;
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
            // TODO: Use Gadget.Flyout.Document to get to the HTML document within
            //       the Flyout page

            flyoutVisible = false;
            flyoutButton.DOMElement.InnerText = FLYOUT_SHOW;
        }

        private void OnFlyoutShow() {
            flyoutVisible = true;
            flyoutButton.DOMElement.InnerText = FLYOUT_HIDE;

            // TODO: Use Gadget.Flyout.Document to get to the HTML document within
            //       the Flyout page
        }

        private void SettingsClosed(GadgetSettingsEvent e) {
            if (e.CloseAction == GadgetSettingsCloseAction.Cancel) return;

            ++settingsCommitCount;

            settingsUpdateLabel.Text = string.Format("Settings updated {0} times", settingsCommitCount);
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
            "https://studio.atlassian.com/sr/jira.issueviews:searchrequest-xml/temp/SearchRequest.xml?jqlQuery=project+%3D+PLVS+AND+updated%3E%3D-1w+ORDER+BY+updated+DESC&tempMax=1000";
        
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
//                issueCount = getIssueCountFromXml(resp);
                jiraResponse.InnerHTML = createHtmlFromResponseXml(resp);
//                tickerLabel.Text = Document.GetElementById("issue0").InnerHTML;
//                jiraResponse.Style.Width = "1000px";
//                jiraResponse.Style.Height = "1000px";
            }
        }

        private void clickedIssue() {
            tickerLabel.Text = "issue clicked";
            Gadget.Flyout.Show = true;
        }

        private int getIssueCountFromXml(XMLDocument resp) {
            return resp.SelectNodes("/rss/channel/item").Length;    
        }

        private string createHtmlFromResponseXml(XMLDocument resp) {
            XMLNodeList issues = resp.SelectNodes("/rss/channel/item");
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < issues.Length; ++i) {
                sb.Append("<div onclick=\"javascript:gadget.GadgetScriptlet.openFlyout(true)\" class=\"");
                sb.Append(i%2 > 0 ? "oddRow" : "evenRow");
                sb.Append("\" id=\"issue" );
                sb.Append(i);
                sb.Append("\">");
                XMLNode key = issues[i].SelectSingleNode("key");
                XMLNode link = issues[i].SelectSingleNode("link");
                XMLNode summary = issues[i].SelectSingleNode("summary");
                XMLNode type = issues[i].SelectSingleNode("type");
                sb.Append("<img src=\"");
                sb.Append(type.Attributes.GetNamedItem("iconUrl").Text);
                sb.Append("\">");
                sb.Append("<a href=\"");
                sb.Append(link.Text);
                sb.Append("\"> ");
                sb.Append(key.Text);
                sb.Append("</a>");
                sb.Append(summary.Text);
                sb.Append("</div>\r\n");
            }
            return sb.ToString();
        }
    }
}
