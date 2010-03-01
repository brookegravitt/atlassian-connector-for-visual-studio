using System.Web;

namespace Atlassian.plvs.api.jira {
    internal class JiraAuthenticatedClient {

        protected string UserName { get; private set; }
        protected string Password { get; private set; }
        protected string BaseUrl { get; private set; }

        public JiraAuthenticatedClient(string url, string userName, string password) {
            BaseUrl = url;
            UserName = userName;
            Password = password;
        }

        protected string appendAuthentication(bool first) {
            if (UserName != null) {
                return (first ? "?" : "&") + "os_username=" + HttpUtility.UrlEncode(UserName)
                       + "&os_password=" + HttpUtility.UrlEncode(Password);
            }
            return "";
        }

    }
}
