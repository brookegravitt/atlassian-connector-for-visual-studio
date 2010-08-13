using System;
using System.DHTML;
using System.Gadgets;
using System.XML;
using ScriptFX.Net;
using ScriptFX.UI;

namespace gadget {

    public class GadgetScriptlet {

        private static Button pollNowButton;
        private static DOMElement jiraResponse;
        private static string flyoutIssueDetailsText = "";
        private static string flyoutIssueKeyText = "";
        private static Label labelInfo;

        private static bool doShowFlyoutAgain;

        private static readonly ArrayList issues = new ArrayList();

        private static string projectKey = "";
        private static string serverUrl = "";
        private static string userName = "";
        private static string password = "";

        private static Filter currentFilter;

        private static bool haveValidSettings;

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

            labelInfo = new Label(Document.GetElementById("info"));

            jiraResponse = Document.GetElementById("jiraResponse");

            reloadSettingsAndPollNow();
            setCurrentFilterLabel();
        }

        private static void setCurrentFilterLabel() {
            Document.GetElementById("currentFilter").InnerHTML = 
                haveValidSettings
                    ? string.Format("{0}<br>{1}: {2}", serverUrl, projectKey, currentFilter.Name)
                    : "";
        }

        private static void pollNowButton_Click(object sender, EventArgs e) {
            pollJira();
        }

        public static void openFlyout(int issueId) {
            if (issueId >= 0) {
                Issue issue = (Issue) issues[issueId];
                flyoutIssueDetailsText = createIssueDetailsHtml(issue);
                flyoutIssueKeyText = createIssueKeyHtml(issue);
                doShowFlyoutAgain = true;
            } else {
                flyoutIssueDetailsText = null;
                doShowFlyoutAgain = false;
            }
            if (Gadget.Flyout.Show) {
                Gadget.Flyout.Show = false;
            } else {
                Gadget.Flyout.Show = issueId >= 0;
            }
        }

        public static void Main(Dictionary arguments) {
#pragma warning disable 168
            GadgetScriptlet scriptlet = new GadgetScriptlet();
#pragma warning restore 168
        }

        private static void OnDock() {
            UpdateDockedState();
        }

        private static void OnUndock() {
            UpdateDockedState();
        }

        private static void OnFlyoutHide() {
            if (doShowFlyoutAgain) {
                Window.SetTimeout(reShowFlyout, 300);
            }
        }

        private static void reShowFlyout() {
            Gadget.Flyout.Show = true;
        }

        private static void OnFlyoutShow() {
            FlyoutScriptlet.setIssueDetailsText(flyoutIssueDetailsText);
            FlyoutScriptlet.setIssueKeyAndType(flyoutIssueKeyText);
            doShowFlyoutAgain = false;
        }

        private static void SettingsClosed(GadgetSettingsEvent e) {
            if (e.CloseAction == GadgetSettingsCloseAction.Cancel) return;
            reloadSettingsAndPollNow();
            setCurrentFilterLabel();
        }

        private static void reloadSettingsAndPollNow() {
            serverUrl = Gadget.Settings.ReadString(SettingsScriptlet.SETTING_URL);
            userName = Gadget.Settings.ReadString(SettingsScriptlet.SETTING_LOGIN);
            password = Gadget.Settings.ReadString(SettingsScriptlet.SETTING_PASSWORD);
            if (string.IsNullOrEmpty(serverUrl)) {
                haveValidSettings = false;
                pollNowButton.DOMElement.Disabled = true;
                labelInfo.Text = "Set Up Server Connection First";
                return;
            }
            currentFilter = new Filter(
                Gadget.Settings.ReadString(SettingsScriptlet.SETTING_FILTERNAME), 
                Gadget.Settings.ReadString(SettingsScriptlet.SETTING_FILTERVALUE)
            );
            projectKey = Gadget.Settings.ReadString(SettingsScriptlet.SETTING_PROJECTKEY);

            haveValidSettings = true;
            pollNowButton.DOMElement.Disabled = false;
            pollJira();
        }

        private static void UpdateDockedState() {
            DOMElement body = Document.Body;

            if (Gadget.Docked) {
                Element.RemoveCSSClass(body, "undocked");
                Element.AddCSSClass(body, "docked");
                body.Style.Width = "250px";
                body.Style.Height = "400px";
            } else {
                Element.AddCSSClass(body, "undocked");
                Element.RemoveCSSClass(body, "docked");
                body.Style.Width = "500px";
                body.Style.Height = "400px";
            }
        }

        private static void pollJira() {
            labelInfo.Text = "Polling JIRA server...";
            pollNowButton.DOMElement.Disabled = true;
            string url = 
                serverUrl 
                + "/sr/jira.issueviews:searchrequest-xml/temp/SearchRequest.xml?jqlQuery=project+%3D+" 
                + projectKey 
                + "+AND+" 
                + currentFilter.FilterDef 
                + "&tempMax=1000";

            HTTPRequest req = HTTPRequest.CreateRequest(getAuthenticatedUrl(url), HTTPVerb.GET);
            req.Invoke(RequestCompleted);
        }

        private static string getAuthenticatedUrl(string url) {
            if (string.IsNullOrEmpty(userName)) return url;
            if (string.IsNullOrEmpty(password)) {
                return url + "&os_username=" + userName;
            }
            return url + "&os_username=" + userName + "&os_password=" + password;
        }

        private static void RequestCompleted(HTTPRequest request, object userContext) {
            HTTPStatusCode statusCode = request.Response.StatusCode;
            pollNowButton.DOMElement.Disabled = false;
            if (statusCode != HTTPStatusCode.OK) {
                labelInfo.Text = "Error. Status code is " + statusCode;
            } else {
                labelInfo.Text = "Last Polled: " + DateTime.Now.ToLocaleDateString() + " " + DateTime.Now.ToLocaleTimeString();
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
                sb.Append("<div class=\"filler\">A</div></div>\r\n");
            }
            return sb.ToString();
        }

        private static string createIssueDetailsHtml(Issue issue) {
            StringBuilder sb = new StringBuilder();
            sb.Append("<div class=\"issueDetails\">");
            sb.Append("<table class=\"issueTable\" ><tr><td valign=\"top\" class=\"issueTableHeader\">");
            sb.Append("Type</td><td valign=\"top\" class=\"issueTableContent\">");
            sb.Append(issue.IssueType);
            sb.Append("</td></tr><tr><td valign=\"top\" class=\"issueTableHeader\">");
            sb.Append("Summary</td><td valign=\"top\" class=\"issueTableContent\">");
            sb.Append(issue.Summary);
            sb.Append("</td></tr></table></div>");
            return sb.ToString();
        }

        private static string createIssueKeyHtml(Issue issue) {
            StringBuilder sb = new StringBuilder();
            sb.Append("<img align=absmiddle src=\"");
            sb.Append(issue.IssueTypeIconUrl);
            sb.Append("\"> ");
            sb.Append("<a href=\"");
            sb.Append(issue.Link);
            sb.Append("\"><b>");
            sb.Append(issue.Key);
            sb.Append("</b></a><br><br>");
            return sb.ToString();
        }
    }
}
