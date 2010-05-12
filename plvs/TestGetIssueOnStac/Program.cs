using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace TestGetIssueOnStac {
    class Program {
        private static string crowdToken;
        private static string jsessionIdCookie;

        private const string JSESSIONID = "JSESSIONID=";
        private const string CROWD_TOKEN = "studio.crowd.tokenkey=";

        private const string NO_SESSION_COOKIE = "no session cookie";

        private const string StacUrl = "https://studio.atlassian.com";

        static void Main(string[] args) {
            if (args.Count() < 3) {
                printUsage();
            }

            try {
                login(args[1], args[2]);
            } catch (Exception e) {
                Console.Write(e.ToString());
                Console.WriteLine();
                Console.ReadKey();
                Environment.Exit(0);
            }

            StringBuilder url = new StringBuilder(StacUrl + "/si/jira.issueviews:issue-xml/");
            url.Append(args[0]).Append("/").Append(args[0]).Append(".xml");

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url.ToString());
            req.Credentials = new NetworkCredential(args[1], args[2]);
            req.Timeout = 10000;
            req.ReadWriteTimeout = 20000;

            setSessionCookie(req);

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
            Stream stream = resp.GetResponseStream();

            StringBuilder sb = new StringBuilder();

            // used on each read operation
            byte[] buf = new byte[8192];

            int count;

            do {
                // fill the buffer with data
                count = stream.Read(buf, 0, buf.Length);

                // make sure we read some data
                if (count == 0) continue;
                // translate from bytes to ASCII text
                string tempString = Encoding.ASCII.GetString(buf, 0, count);

                // continue building the string
                sb.Append(tempString);
            }
            while (count > 0); // any more data to read?

            Console.Write(sb.ToString());
            Console.WriteLine();
            Console.WriteLine("**** Press any key to exit ****");
            Console.ReadKey();
        }

        private static void printUsage() {
            Console.WriteLine("usage: prog <issue key> <use name> <password>");
            Environment.Exit(0);
        }

        public static void login(string userName, string password) {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(StacUrl + "/login.jsp");
            req.Credentials = new NetworkCredential(userName, password);
            req.Timeout = 10000;
            req.ReadWriteTimeout = 20000;
            req.Method = "POST";

            req.ContentType = "application/x-www-form-urlencoded";
            string pars = getLoginPostData(userName, password);
            req.ContentLength = pars.Length;
            using (StreamWriter outStream = new StreamWriter(req.GetRequestStream(), Encoding.ASCII)) {
                outStream.Write(pars);
                outStream.Flush();
                outStream.Close();

                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                using (resp.GetResponseStream()) {
                    if (!resp.StatusCode.Equals(HttpStatusCode.OK)) {
                        throw new Exception(resp.StatusDescription);
                    }

                    string cookies = resp.Headers["Set-Cookie"];
                    if (cookies == null) {
                        throw new Exception(NO_SESSION_COOKIE);
                    }
                    Console.WriteLine("Cookies: " + cookies);
                    Console.WriteLine();

                    jsessionIdCookie = getSessionToken(cookies, JSESSIONID);
                    crowdToken = getSessionToken(cookies, CROWD_TOKEN);
                }
            }
        }

        private static string getSessionToken(string cookies, string tokenName) {
            int idxStart = cookies.LastIndexOf(tokenName);
            if (idxStart == -1) {
                throw new Exception(NO_SESSION_COOKIE);
            }
            int idxEnd = cookies.IndexOf(";", idxStart);
            if (idxEnd == -1) {
                throw new Exception(NO_SESSION_COOKIE);
            }
            return cookies.Substring(idxStart + tokenName.Length, idxEnd - idxStart - tokenName.Length);
        }

        private static string getLoginPostData(string userName, string password) {
            return string.Format("os_username={0}&os_password={1}&os_cookie=true", HttpUtility.UrlEncode(userName), HttpUtility.UrlEncode(password));
        }

        private static void setSessionCookie(HttpWebRequest req) {
            StringBuilder sb = new StringBuilder();
            if (jsessionIdCookie != null) {
                sb.Append(JSESSIONID).Append(jsessionIdCookie).Append(";");
            }
            if (crowdToken != null) {
                sb.Append(CROWD_TOKEN).Append(crowdToken).Append(";");
            }
            if (sb.Length > 0) {
                req.Headers["Cookie"] = sb.ToString();
            }
        }
    }
}
