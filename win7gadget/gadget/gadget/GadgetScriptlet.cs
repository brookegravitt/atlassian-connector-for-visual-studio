using System;
using System.Collections;
using System.Html;
using System.Gadgets;
using System.Net;
using System.Xml;
using jQueryApi;

namespace gadget {

    public class GadgetScriptlet {

        private static InputElement pollNowButton;
        private static Element jiraResponse;
        private static string flyoutIssueDetailsText = "";
        private static string flyoutIssueKeyText = "";
        private static Element labelInfo;

        private static bool doShowFlyoutAgain;

        private static readonly ArrayList Issues = new ArrayList();

        private static string projectKey = "";
        private static string serverUrl = "";
        private static string userName = "";
        private static string password = "";

        private static Filter currentFilter;

        private static bool haveValidSettings;

        private GadgetScriptlet() {
            Gadget.OnDock = OnDock;
            Gadget.OnUndock = OnUndock;

            Gadget.Flyout.File = "Flyout.htm";
            Gadget.Flyout.OnShow = OnFlyoutShow;
            Gadget.Flyout.OnHide = OnFlyoutHide;

            Gadget.SettingsUI = "Settings.htm";
            Gadget.OnSettingsClosed = SettingsClosed;

            UpdateDockedState();

            pollNowButton = (InputElement) Document.GetElementById("pollNowButton");
            pollNowButton.AttachEvent("onclick", pollNowButtonClick);

            labelInfo = Document.GetElementById("info");

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

        private static void pollNowButtonClick() {
            pollJira();
        }

        public static void openFlyout(int issueId) {
            if (issueId >= 0) {
                Issue issue = (Issue) Issues[issueId];
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
                pollNowButton.Disabled = true;
                labelInfo.InnerHTML = "Set Up Server Connection First";
                return;
            }
            currentFilter = new Filter(
                Gadget.Settings.ReadString(SettingsScriptlet.SETTING_FILTERNAME), 
                Gadget.Settings.ReadString(SettingsScriptlet.SETTING_FILTERVALUE)
            );
            projectKey = Gadget.Settings.ReadString(SettingsScriptlet.SETTING_PROJECTKEY);

            haveValidSettings = true;
            pollNowButton.Disabled = false;
            pollJira();
        }

        private static void UpdateDockedState() {
            if (Gadget.Docked) {
                jQuery.Select("body").RemoveClass("undocked");
                jQuery.Select("body").AddClass("docked");
                jQuery.Select("body").Width("250px");
                jQuery.Select("body").Height("400px");
            } else {
                jQuery.Select("body").AddClass("undocked");
                jQuery.Select("body").RemoveClass("docked");
                jQuery.Select("body").Width("500px");
                jQuery.Select("body").Height("400px");
            }
        }

        private static XmlHttpRequest req;

        private static void pollJira() {
            pollNowButton.Disabled = true;
            string url = 
                serverUrl 
                + "/sr/jira.issueviews:searchrequest-xml/temp/SearchRequest.xml?jqlQuery=project+%3D+" 
                + projectKey 
                + "+AND+" 
                + currentFilter.FilterDef 
                + "&tempMax=1000";

            labelInfo.InnerHTML = "Polling JIRA server...";

            req = new XmlHttpRequest();
            req.Open("GET", getAuthenticatedUrl(url));
            req.OnReadyStateChange = onReadyStateChange;
            req.Send();
        }

        private static void onReadyStateChange() {
            if (req.ReadyState != ReadyState.Loaded) {
                return;
            }
            pollNowButton.Disabled = false;
            if (req.Status != 200) {
                labelInfo.InnerHTML = "Error. Status code is " + req.ResponseText;
            } else {
                pollNowButton.Disabled = false;
                labelInfo.InnerHTML = "Last Polled: " + DateTime.Now.ToLocaleDateString() + " " + DateTime.Now.ToLocaleTimeString();
                createIssueListFromResponseXml(req.ResponseXml);
                jiraResponse.InnerHTML = createIssueListHtmlFromIssueList();
            }
        }

        private static string getAuthenticatedUrl(string url) {
            if (string.IsNullOrEmpty(userName)) return url;
            if (string.IsNullOrEmpty(password)) {
                return url + "&os_username=" + userName;
            }
            return url + "&os_username=" + userName + "&os_password=" + password;
        }

        private static void createIssueListFromResponseXml(XmlDocument resp) {
            XmlNodeList issuesXml = resp.SelectNodes("/rss/channel/item");
            Issues.Clear();
            for (int i = 0; i < issuesXml.Count; ++i) {
                XmlNode key = issuesXml[i].SelectSingleNode("key");
                XmlNode link = issuesXml[i].SelectSingleNode("link");
                XmlNode summary = issuesXml[i].SelectSingleNode("summary");
                XmlNode type = issuesXml[i].SelectSingleNode("type");
                XmlNode priority = issuesXml[i].SelectSingleNode("priority");
                XmlNode status = issuesXml[i].SelectSingleNode("status");
                XmlNode reporter = issuesXml[i].SelectSingleNode("reporter");
                XmlNode assignee = issuesXml[i].SelectSingleNode("assignee");
                XmlNode created = issuesXml[i].SelectSingleNode("created");
                XmlNode updated = issuesXml[i].SelectSingleNode("updated");
                XmlNode resolution = issuesXml[i].SelectSingleNode("resolution");
                XmlNode description = issuesXml[i].SelectSingleNode("description");
                XmlNode env = issuesXml[i].SelectSingleNode("environment");
                XmlNode votes = issuesXml[i].SelectSingleNode("votes");
                Issue issue = new Issue(
                    safeText(key), safeText(link), safeText(summary), safeText(type), safeAttribute(type, "iconUrl"),
                    safeText(priority), safeAttribute(priority, "iconUrl"),
                    safeText(status), safeAttribute(status, "iconUrl"),
                    safeText(reporter), safeText(assignee),
                    safeText(created), safeText(updated),
                    safeText(resolution), safeText(description), 
                    safeText(env), safeText(votes)
                    );

                Issues.Add(issue);
            }
        }

        private static string safeText(XmlNode node) {
            return node == null ? "" : node.Value;
        }

        private static string safeAttribute(XmlNode node, string attr) {
            if (node == null) return "";
            XmlNode a = node.Attributes.GetNamedItem(attr);
            return a != null ? a.Value : "";
        }

        private static string createIssueListHtmlFromIssueList() {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Issues.Count; ++i) {
                Issue issue = (Issue) Issues[i];

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
