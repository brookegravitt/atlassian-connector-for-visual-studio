using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Atlassian.plvs.util {
    public sealed class JiraIssueUtils {

        public static readonly Regex ISSUE_REGEX = new Regex(@"([A-Z]+-\d+)");

        private const string JiraFormat = "ddd, d MMM yyyy HH:mm:ss zzz";
        private const string ShortFormat = "dd/MM/yy";

        public static DateTime getDateTimeFromJiraTimeString(string value) {
            int bracket = value.LastIndexOf("(");
            if (bracket != -1) {
                value = value.Substring(0, bracket);
            }

            try {
                return DateTime.ParseExact(value.Trim(), JiraFormat, new CultureInfo("en-US"), DateTimeStyles.None);
            }
            catch (FormatException) {
                return DateTime.MinValue;
            }
        }

        public static DateTime getDateTimeFromShortString(string value) {
            try {
                return DateTime.ParseExact(value.Trim(), ShortFormat, new CultureInfo("en-US"), DateTimeStyles.None);
            } catch (FormatException) {
                return DateTime.MinValue;
            }
        }

        public static string getTimeStringFromIssueDateTime(DateTime time) {
            if (time.Equals(DateTime.MinValue)) {
                return "Unknown";
            }
            return time.ToShortDateString() + " " + time.ToShortTimeString();
        }
    }
}