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
                XMLNode priority = issuesXml[i].SelectSingleNode("priority");
                XMLNode status = issuesXml[i].SelectSingleNode("status");
                XMLNode reporter = issuesXml[i].SelectSingleNode("reporter");
                XMLNode assignee = issuesXml[i].SelectSingleNode("assignee");
                XMLNode created = issuesXml[i].SelectSingleNode("created");
                XMLNode updated = issuesXml[i].SelectSingleNode("updated");
                XMLNode resolution = issuesXml[i].SelectSingleNode("resolution");
                XMLNode description = issuesXml[i].SelectSingleNode("description");
                XMLNode env = issuesXml[i].SelectSingleNode("environment");
                XMLNode votes = issuesXml[i].SelectSingleNode("votes");
                Issue issue = new Issue(
                    safeText(key), safeText(link), safeText(summary), safeText(type), safeAttribute(type, "iconUrl"),
                    safeText(priority), safeAttribute(priority, "iconUrl"),
                    safeText(status), safeAttribute(status, "iconUrl"),
                    safeText(reporter), safeText(assignee),
                    safeText(created), safeText(updated),
                    safeText(resolution), safeText(description), 
                    safeText(env), safeText(votes)
                    );

                issues.Add(issue);
            }
        }

        private static string safeText(XMLNode node) {
            return node == null ? "" : node.Text;
        }

        private static string safeAttribute(XMLNode node, string attr) {
            if (node == null) return "";
            XMLNode a = node.Attributes.GetNamedItem(attr);
            return a != null ? a.Text : "";
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
            sb.Append("<table class=\"issueTable\" >");
            sb.Append(row("Summary", issue.Summary));
            sb.Append(row("Type", string.Format("<img align=absmiddle src=\"{0}\"> {1}", issue.IssueTypeIconUrl, issue.IssueType)));
            sb.Append(row("Status", string.Format("<img align=absmiddle src=\"{0}\"> {1}", issue.StatusIconUrl, issue.Status)));
            sb.Append(row("Priority", string.Format("<img align=absmiddle src=\"{0}\"> {1}", issue.PriorityIconUrl, issue.Priority)));
            sb.Append(row("Resolution", issue.Resolution));
            sb.Append(row("Reporter", issue.Reporter));
            sb.Append(row("Assignee", issue.Assignee));
            sb.Append(row("Created", issue.Created));
            sb.Append(row("Updated", issue.Updated));
            sb.Append(row("Environment", issue.Environment));
            sb.Append(row("Description", issue.Description));
            sb.Append(row("Votes", issue.Votes));
            sb.Append("</table></div>");
            return sb.ToString();
        }

        private static string row(string title, string value) {
            return string.Format(
                "<tr><td valign=\"top\" class=\"issueTableHeader\">{0}"
                + "</td><td valign=\"top\" class=\"issueTableContent\">{1}"
                + "</td></tr>", title, value);
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
