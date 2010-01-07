using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Atlassian.plvs.util {
    public sealed class JiraIssueUtils {

        public static readonly Regex ISSUE_REGEX = new Regex(@"([A-Z]+-\d+)");

        private const string JiraFormat = "ddd, d MMM yyyy HH:mm:ss zzz";
        private const string ShortFormatFromJira = "dd/MM/yy";
        private const string ShortFormatToJira = "dd/MMM/yy";

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
            // let's try both formats
            try {
                return DateTime.ParseExact(value.Trim(), ShortFormatFromJira, new CultureInfo("en-US"), DateTimeStyles.None);
            } catch (FormatException) {
                try {
                    return DateTime.ParseExact(value.Trim(), ShortFormatToJira, new CultureInfo("en-US"), DateTimeStyles.None);
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

        public static string getShortDateStringFromDateTime(DateTime time) {
            return time.ToString(ShortFormatToJira, new CultureInfo("en-US"));
        }
    }
}