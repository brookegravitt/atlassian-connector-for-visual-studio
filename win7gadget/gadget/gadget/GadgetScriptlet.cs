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
        private static string flyoutIssueDetailsText = "";
        private static string flyoutIssueKeyText = "";
        private static Label labelInfo;

        private static bool doShowFlyoutAgain;

//        private int settingsCommitCount;
//        private int issueCount;

        private static readonly ArrayList issues = new ArrayList();

        private string projectKey = "PL";
        private string serverUrl = "https://studio.atlassian.com";
        private string userName = "user";
        private string password = "password";

        private static Filter updatedRecently = new Filter("Updated Recently", "updated%3E%3D-1w+ORDER+BY+updated+DESC");

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

//            settingsUpdateLabel = new Label(Document.GetElementById("settingsText"));
//            tickerLabel = new Label(Document.GetElementById("ticker"));

            jiraResponse = Document.GetElementById("jiraResponse");

            setCurrentFilterLabel();
//            Window.SetInterval(OnTimer, 2000);
        }

        private void gotProjects(object result) {
            Dictionary d = (Dictionary) result;
            StringBuilder sb = new StringBuilder();
            foreach (DictionaryEntry entry in d) {
                int nr;
                try {
                    nr = (int) int.Parse(entry.Key);
                } catch (Exception) {
                    continue;
                }
                if (Number.IsNaN(nr)) continue;

                sb.Append(nr);
                sb.Append(": ");
                sb.Append(getProjectString(entry.Value));
                sb.Append("<br><hr><br>");
            }
            jiraResponse.InnerHTML = sb.ToString();
            labelInfo.Text = "got projects";
        }

        private static string getProjectString(object projectObject) {
            Dictionary d = (Dictionary)projectObject;
            StringBuilder sb = new StringBuilder();
            foreach (DictionaryEntry entry in d) {
                sb.Append("<br>&nbsp;&nbsp;&nbsp;&nbsp;" + entry.Key + ": " + entry.Value);
            }
            return sb.ToString();
        }

        private void errorGettingProjects(string error) {
            labelInfo.Text = "error getting projects: " + error;
        }

        private void setCurrentFilterLabel() {
            Document.GetElementById("currentFilter").InnerHTML = string.Format("{0}<br>{1}: {2}", serverUrl, projectKey, updatedRecently.Name);
        }

        private void pollNowButton_Click(object sender, EventArgs e) {

            pollJira();
        }

        private void gotLoginToken(string token) {
            rpc.getprojects(serverUrl, token, gotProjects, errorGettingProjects);
        }

//        private void OnTimer() {
//            tickerLabel.Text = new DateTime().Format("T");    
//        }

        public static void openFlyout(int issueId) {
//            labelDebug.Text = "openFlyout: " + issueId;
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
//                labelDebug.Text = "re-showing flyout";
                Window.SetTimeout(reShowFlyout, 300);
            }
        }

        private void reShowFlyout() {
            Gadget.Flyout.Show = true;
        }

        private void OnFlyoutShow() {
//            flyoutVisible = true;

//            labelDebug.Text = "setting flyout text";
            FlyoutScriptlet.setIssueDetailsText(flyoutIssueDetailsText);
            FlyoutScriptlet.setIssueKeyAndType(flyoutIssueKeyText);
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
                body.Style.Width = "250px";
                body.Style.Height = "400px";
            } else {
                Element.AddCSSClass(body, "undocked");
                Element.RemoveCSSClass(body, "docked");
                body.Style.Width = "500px";
                body.Style.Height = "400px";
            }
        }

        private void pollJira() {
            labelInfo.Text = "Polling JIRA server...";
            pollNowButton.DOMElement.Disabled = true;
            string url = 
                serverUrl 
                + "/sr/jira.issueviews:searchrequest-xml/temp/SearchRequest.xml?jqlQuery=project+%3D+" 
                + projectKey 
                + "+AND+" 
                + updatedRecently.FilterDef 
                + "&tempMax=1000";

            HTTPRequest req = HTTPRequest.CreateRequest(url, HTTPVerb.GET);
            req.Invoke(RequestCompleted);
        }

        private void RequestCompleted(HTTPRequest request, object userContext) {
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
