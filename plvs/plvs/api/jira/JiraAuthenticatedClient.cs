using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Atlassian.plvs.dialogs;

namespace Atlassian.plvs.api.jira {
    internal class JiraAuthenticatedClient : IDisposable {
        private readonly bool dontUseProxy;

        private const string NO_SESSION_COOKIE = "No session cookie found in response";
        private const string JSESSIONID = "JSESSIONID=";

        public string SessionCookie { get; set; }

        protected string UserName { get; private set; }
        protected string Password { get; private set; }
        protected string BaseUrl { get; private set; }

        public JiraAuthenticatedClient(string url, string userName, string password, bool dontUseProxy) {
            this.dontUseProxy = dontUseProxy;
            BaseUrl = url;
            UserName = userName;
            Password = password;
        }

        public string login() {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(BaseUrl + "/login.jsp");
            req.Proxy = dontUseProxy ? null : GlobalSettings.Proxy;
            req.Credentials = CredentialUtils.getCredentialsForUserAndPassword(BaseUrl, UserName, Password);
            req.Timeout = GlobalSettings.NetworkTimeout * 1000;
            req.ReadWriteTimeout = GlobalSettings.NetworkTimeout * 2000;
            req.Method = "POST";

            req.ContentType = "application/x-www-form-urlencoded";
            string pars = getLoginPostData(UserName, Password);
            req.ContentLength = pars.Length;
            using (StreamWriter outStream = new StreamWriter(req.GetRequestStream(), Encoding.ASCII)) {
                outStream.Write(pars);
                outStream.Flush();
                outStream.Close();

                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                using (resp.GetResponseStream()) {
                    if (!resp.StatusCode.Equals(HttpStatusCode.OK)) {
                        throw new LoginException(new Exception(resp.StatusDescription));
                    }

                    string cookies = resp.Headers["Set-Cookie"];
                    if (cookies == null) {
                        throw new LoginException(new Exception(NO_SESSION_COOKIE));
                    }
                    int idxStart = cookies.ToUpper().IndexOf(JSESSIONID);
                    if (idxStart == -1) {
                        throw new LoginException(new Exception(NO_SESSION_COOKIE));
                    }
                    int idxEnd = cookies.IndexOf(";", idxStart);
                    if (idxEnd == -1) {
                        throw new LoginException(new Exception(NO_SESSION_COOKIE));
                    }
                    SessionCookie = cookies.Substring(idxStart + JSESSIONID.Length, idxEnd - idxStart - JSESSIONID.Length);
                    return SessionCookie;
                }
            }
        }

#if OLDSKOOL_AUTH
        protected string appendAuthentication(bool first) {
            if (UserName != null) {
                return (first ? "?" : "&") + CredentialUtils.getOsAuthString(UserName, Password);
            }
            return "";
        }
#else
        protected void setSessionCookie(HttpWebRequest req) {
            if (SessionCookie != null) {
                req.Headers["Cookie"] = JSESSIONID + SessionCookie + ";";
            }
        }

        public static void setSessionCookie(WebHeaderCollection headers, string cookie) {
            headers["Cookie"] = getSessionCookieString(cookie);
        }

        public static string getSessionCookieString(string cookie) {
            return JSESSIONID + cookie + ";";
        }

        public static string getLoginPostData(string userName, string password) {
            return string.Format("os_username={0}&os_password={1}&os_cookie=true",
                HttpUtility.UrlEncode(CredentialUtils.getUserNameWithoutDomain(userName)),
                HttpUtility.UrlEncode(password));
        }
#endif

        public void Dispose() {
        }
    }
}
