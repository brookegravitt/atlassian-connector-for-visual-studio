using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui.jira;
using Atlassian.plvs.windows;

namespace Atlassian.plvs.util.jira {
    public sealed class JiraIssueUtils {

        public static readonly Regex ISSUE_REGEX = new Regex(@"(([A-Z]+)-\d+)");

        private const string JiraFormat = "ddd, d MMM yyyy HH:mm:ss zzz";
        private const string ShortFormatFromJira = "dd/MM/yy";
        private const string ShortFormatToJira = "dd/MMM/yy";

        public static DateTime getDateTimeFromJiraTimeString(string locale, string value) {
            int bracket = value.LastIndexOf("(");
            if (bracket != -1) {
                value = value.Substring(0, bracket);
            }

            try {
                return DateTime.ParseExact(value.Trim(), JiraFormat, new CultureInfo(locale ?? "en-US"), DateTimeStyles.None);
            }
            catch (FormatException) {
                return DateTime.MinValue;
            }
        }

        public static DateTime getDateTimeFromShortString(string locale, string value) {
            // let's try both formats
            try {
                return DateTime.ParseExact(value.Trim(), ShortFormatFromJira, new CultureInfo(locale ?? "en-US"), DateTimeStyles.None);
            } catch (FormatException) {
                try {
                    return DateTime.ParseExact(value.Trim(), ShortFormatToJira, new CultureInfo(locale ?? "en-US"), DateTimeStyles.None);
                } catch (FormatException) {
                    return DateTime.MinValue;
                }
            }
        }

        public static string getTimeStringFromIssueDateTime(DateTime time) {
            if (time.Equals(DateTime.MinValue)) {
                return "Unknown";
            }
            return time.ToShortDateString() + " " + time.ToShortTimeString();
        }

        public static string getShortDateStringFromDateTime(string locale, DateTime time) {
            return time.ToString(ShortFormatToJira, new CultureInfo(locale ?? "en-US"));
        }

        public static string addSpacesToTimeSpec(string text) {
            Regex regex = new Regex(Constants.TIME_TRACKING_REGEX);

            if (!regex.IsMatch(text)) {
                throw new ArgumentException("Time specification must be in the " + Constants.TIME_TRACKING_SYNTAX + " format");
            }

            Match match = regex.Match(text);
            Group @groupWeeks = match.Groups[2];
            Group @groupDays = match.Groups[4];
            Group @groupHours = match.Groups[6];
            Group @groupMinutes = match.Groups[8];

            string result = "";
            if (groupWeeks != null && groupWeeks.Success) result = result + groupWeeks.Value + "w ";
            if (groupDays != null && groupDays.Success) result = result + groupDays.Value + "d ";
            if (groupHours != null && groupHours.Success) result = result + groupHours.Value + "h ";
            if (groupMinutes != null && groupMinutes.Success) result = result + groupMinutes.Value + "m";
            return result.Trim();
        }

        public static T getIssueSoapObjectPropertyValue<T>(object soapObject, string name) {
            if (soapObject == null) {
                return default(T);
            }
            PropertyInfo property = soapObject.GetType().GetProperty(name);
            if (property == null) {
                return default(T);
            }
            return (T) property.GetValue(soapObject, null);
        }

        public static void openInIde(string issueKey) {
            if (issueKey == null) return;

            bool found = false;
            foreach (JiraIssue issue in JiraIssueListModelImpl.Instance.Issues.Where(issue => issue.Key.Equals(issueKey))) {
                IssueDetailsWindow.Instance.openIssue(issue, AtlassianPanel.Instance.Jira.ActiveIssueManager);
                found = true;
                break;
            }
            if (!found) {
                AtlassianPanel.Instance.Jira.findAndOpenIssue(issueKey, findFinished);
            }
        }

        private static void findFinished(bool success, string message, Exception e) {
            if (!success) {
                PlvsUtils.showError(message, e);
            }
        }

        public static void launchBrowser(string issueKey) {
            if (issueKey == null) {
                return;
            }
            try {
                JiraServer server = AtlassianPanel.Instance.Jira.CurrentlySelectedServerOrDefault;
                if (server != null) {
                    Process.Start(server.Url + "/browse/" + issueKey);
                } else {
                    MessageBox.Show("No JIRA server selected", Constants.ERROR_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
// ReSharper disable EmptyGeneralCatchClause
            } catch { }
// ReSharper restore EmptyGeneralCatchClause
        }
    }
}