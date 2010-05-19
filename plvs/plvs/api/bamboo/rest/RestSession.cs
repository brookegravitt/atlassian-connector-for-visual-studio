using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Xml.XPath;
using Atlassian.plvs.dialogs;
using Atlassian.plvs.util;
using Atlassian.plvs.util.bamboo;
using Atlassian.plvs.util.jira;

namespace Atlassian.plvs.api.bamboo.rest {
    public class RestSession {

        private readonly BambooServer server;
        private string authToken;
        private string userName;
        private string password;

        private string cookie;
        private const string LATEST_BUILDS_FOR_FAVOURITE_PLANS_ACTION = "/rest/api/latest/build?favourite&expand=builds.build";

        private const string LATEST_BUILDS_FOR_PLANS_ACTION = "/rest/api/latest/build/{0}?expand=builds[0].build";

        private const string ALL_PLANS_ACTION = "/rest/api/latest/plan?expand=plans.plan";
        private const string FAVOURITE_PLANS_ACTION = "/rest/api/latest/plan?favourite&expand=plans.plan";

        private const string RUN_BUILD_ACTION_NEW_AND_IT_DOES_NOT_WORK = "/rest/api/latest/queue";
        private const string RUN_BUILD_ACTION_OLD = "/api/rest/executeBuild.action";

        private const string LOGIN_ACTION = "/api/rest/login.action";
    	private const string LOGOUT_ACTION = "/api/rest/logout.action";

        public RestSession(BambooServer server) {
            this.server = server;
        }

        public bool LoggedIn { get; private set; }

        public RestSession login(string username, string pwd) {

            string endpoint = server.Url + LOGIN_ACTION 
                + "?username=" + HttpUtility.UrlEncode(CredentialUtils.getUserNameWithoutDomain(username), Encoding.UTF8) 
                + "&password=" + HttpUtility.UrlEncode(pwd, Encoding.UTF8) 
                + "&os_username=" + HttpUtility.UrlEncode(CredentialUtils.getUserNameWithoutDomain(username), Encoding.UTF8) 
                + "&os_password=" + HttpUtility.UrlEncode(pwd, Encoding.UTF8);

            using (Stream stream = getQueryResultStream(endpoint, false)) {
                XPathDocument doc = XPathUtils.getXmlDocument(stream);

                string exceptions = getRemoteExceptionMessages(doc);
                if (exceptions != null) {
                    throw new Exception(exceptions);
                }

                XPathNavigator nav = doc.CreateNavigator();
                XPathExpression expr = nav.Compile("/response/auth");
                XPathNodeIterator it = nav.Select(expr);
                if (it.Count == 0) {
                    throw new Exception("Server did not return any authentication token");
                }
                if (it.Count != 1) {
                    throw new Exception("Server returned unexpected number of authentication tokens (" + it.Count + ")");
                }
                it.MoveNext();
                authToken = it.Current.Value;
                userName = username;
                password = pwd;

                LoggedIn = true;
                return this;
            }
        }

        public void logout() {
            if (!LoggedIn) return;
            try {
                string endpoint = server.Url + LOGOUT_ACTION + "?auth=" + HttpUtility.UrlEncode(authToken, Encoding.UTF8);
                Stream stream = getQueryResultStream(endpoint, false);
                stream.Close();
            } catch (Exception e) {
                Debug.WriteLine("RestSession.logout() - exception (ignored): " + e.Message);
            }
            authToken = null;
            userName = null;
            password = null;
            LoggedIn = false;
        }

        public ICollection<BambooPlan> getAllPlans() {
            return getPlansFromUrl(server.Url + ALL_PLANS_ACTION);
        }

        public ICollection<BambooPlan> getFavouritePlans() {
            return getPlansFromUrl(server.Url + FAVOURITE_PLANS_ACTION);
        }

        private ICollection<BambooPlan> getPlansFromUrl(string endpoint) {
            return getPlansFromUrlWithStartIndex(endpoint, 0);
        }

        private ICollection<BambooPlan> getPlansFromUrlWithStartIndex(string endpoint, int start) {
            using (Stream stream = getQueryResultStream(endpoint + getBasicAuthParameter(endpoint) + "&start-index=" + start, true)) {

                XPathDocument doc = XPathUtils.getXmlDocument(stream);

                string code = getRestErrorStatusCode(doc);
                if (code != null) {
                    throw new Exception(code);
                }

                XPathNavigator nav = doc.CreateNavigator();

                XPathExpression expr = nav.Compile("/plans/plans");
                XPathNodeIterator it = nav.Select(expr);
                int totalPlansCount = 0;
                int maxResult = 0;
                int startIndex = 0;
                if (it.MoveNext()) {
                    totalPlansCount = int.Parse(XPathUtils.getAttributeSafely(it.Current, "size", "0"));
                    maxResult = int.Parse(XPathUtils.getAttributeSafely(it.Current, "max-result", "0"));
                    startIndex = int.Parse(XPathUtils.getAttributeSafely(it.Current, "start-index", "0"));
                }

                expr = nav.Compile("/plans/plans/plan");
                it = nav.Select(expr);

                List<BambooPlan> plans = new List<BambooPlan>();

                while (it.MoveNext()) {
                    string enabledValue = XPathUtils.getAttributeSafely(it.Current, "enabled", "true");
                    string key = XPathUtils.getAttributeSafely(it.Current, "key", null);
                    string name = XPathUtils.getAttributeSafely(it.Current, "name", null);
                    bool enabled = true;
                    if (enabledValue != null) {
                        enabled = Boolean.Parse(enabledValue);
                    }
                    it.Current.MoveToFirstChild();
                    bool favourite = false;
                    do {
                        switch (it.Current.Name) {
                            case "isFavourite":
                                favourite = it.Current.Value.Equals("true");
                                break;
                        }
                    } while (it.Current.MoveToNext());
                    if (key == null || name == null) continue;
                    BambooPlan plan = new BambooPlan(key, name, enabled, favourite);
                    plans.Add(plan);
                }

                // Yes, recursion here. I hope it works as I think it should. If not, we are all doomed
                if (totalPlansCount > maxResult + startIndex) {
                    plans.AddRange(getPlansFromUrlWithStartIndex(endpoint, startIndex + maxResult));
                }

                return plans;
            }
        }

        public ICollection<BambooBuild> getLatestBuildsForFavouritePlans() {
            string endpoint = server.Url + LATEST_BUILDS_FOR_FAVOURITE_PLANS_ACTION;
            return getBuildsFromUrl(endpoint, true);
        }
    
        public ICollection<BambooBuild> getLatestBuildsForPlanKeys(ICollection<string> keys) {
            List<BambooBuild> result = new List<BambooBuild>();
            foreach (string key in keys) {
                string buildUrl = string.Format(LATEST_BUILDS_FOR_PLANS_ACTION, key);
                string endpoint = server.Url + buildUrl;
                ICollection<BambooBuild> builds = getBuildsFromUrl(endpoint, false);
                if (builds != null) {
                    result.AddRange(builds);
                }
            }
            return result;
        }

        private ICollection<BambooBuild> getBuildsFromUrl(string endpoint, bool withRecursion) {
            return getBuildsFromUrlWithStartIndex(endpoint, 0, withRecursion);
        }

        private ICollection<BambooBuild> getBuildsFromUrlWithStartIndex(string endpoint, int start, bool withRecursion) {

            using (Stream stream = getQueryResultStream(endpoint + getBasicAuthParameter(endpoint) + (withRecursion ? "&start-index=" + start : ""), true)) {
                XPathDocument doc = XPathUtils.getXmlDocument(stream);

                string code = getRestErrorStatusCode(doc);
                if (code != null) {
                    throw new Exception(code);
                }

                XPathNavigator nav = doc.CreateNavigator();

                XPathExpression expr = nav.Compile("/builds/builds");
                XPathNodeIterator it = nav.Select(expr);
                int totalBuildsCount = 0;
                int maxResult = 0;
                int startIndex = 0;
                if (it.MoveNext()) {
                    totalBuildsCount = int.Parse(XPathUtils.getAttributeSafely(it.Current, "size", "0"));
                    maxResult = int.Parse(XPathUtils.getAttributeSafely(it.Current, "max-result", "0"));
                    startIndex = int.Parse(XPathUtils.getAttributeSafely(it.Current, "start-index", "0"));
                }

                expr = nav.Compile("/builds/builds/build");
                it = nav.Select(expr);

                List<BambooBuild> builds = new List<BambooBuild>();

                while (it.MoveNext()) {
                    int number = int.Parse(XPathUtils.getAttributeSafely(it.Current, "number", "-1"));
                    string key = XPathUtils.getAttributeSafely(it.Current, "key", null);
                    string state = XPathUtils.getAttributeSafely(it.Current, "state", null);
                    it.Current.MoveToFirstChild();
                    string buildRelativeTime = null;
                    string buildDurationDescription = null;
                    int successfulTestCount = 0;
                    int failedTestCount = 0;
                    string buildReason = null;
                    do {
                        switch (it.Current.Name) {
                            case "buildRelativeTime":
                                buildRelativeTime = it.Current.Value;
                                break;
                            case "buildDurationDescription":
                                buildDurationDescription = it.Current.Value;
                                break;
                            case "successfulTestCount":
                                successfulTestCount = int.Parse(it.Current.Value);
                                break;
                            case "failedTestCount":
                                failedTestCount = int.Parse(it.Current.Value);
                                break;
                            case "buildReason":
                                buildReason = it.Current.Value;
                                break;
                        }
                    } while (it.Current.MoveToNext());
                    if (key == null) continue;
                    BambooBuild build = new BambooBuild(server,
                        key, BambooBuild.stringToResult(state), number, buildRelativeTime,
                        buildDurationDescription, successfulTestCount, failedTestCount, buildReason);
                    builds.Add(build);
                }

                // Yes, recursion here. I hope it works as I think it should. If not, we are all doomed
                if (withRecursion && totalBuildsCount > maxResult + startIndex) {
                    builds.AddRange(getBuildsFromUrlWithStartIndex(endpoint, startIndex + maxResult, true));
                }

                return builds;
            }
        }

        public void runBuild(string planKey) {
#if false
            string endpoint = server.Url + RUN_BUILD_ACTION_NEW_AND_IT_DOES_NOT_WORK + "/" + planKey;

            Stream stream = postWithNullBody(endpoint + getBasicAuthParameter(endpoint), true);

            XPathDocument doc = XPathUtils.getDocument(stream);

            string code = getRestErrorStatusCode(doc);
            if (code != null) {
                throw new Exception(code);
            }
#else 
            string endpoint = server.Url + RUN_BUILD_ACTION_OLD 
                + "?buildKey=" + planKey 
                + "&auth=" + HttpUtility.UrlEncode(authToken, Encoding.UTF8);

            using (Stream stream = getQueryResultStream(endpoint, false)) {
                XPathDocument doc = XPathUtils.getXmlDocument(stream);

                string code = getRemoteExceptionMessages(doc);
                if (code != null) {
                    throw new Exception(code);
                }
            }
#endif
        }

        public string getBuildLog(BambooBuild build) {
            string endpoint = server.Url + "/download/" + BambooBuildUtils.getPlanKey(build) + "/build_logs/" + build.Key + ".log";
            using (Stream stream = getQueryResultStream(endpoint, true)) {
                StreamReader reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
        }

        private Stream getQueryResultStream(string endpoint, bool setBasicAuth) {
            var req = (HttpWebRequest)WebRequest.Create(endpoint);
            req.Proxy = server.NoProxy ? null : GlobalSettings.Proxy;
            req.Timeout = GlobalSettings.NetworkTimeout * 1000;
            req.ReadWriteTimeout = GlobalSettings.NetworkTimeout * 2000;
            req.ContentType = "application/xml";
            req.Method = "GET";
            // required for PLVS-83
            req.Accept = "application/xml";

            if (setBasicAuth) {
                setBasicAuthHeader(endpoint, req);
            } else {
                restoreSessionContext(req);
            }
            var resp = (HttpWebResponse)req.GetResponse();
            if (!setBasicAuth) {
                saveSessionContext(resp);
            }
            return resp.GetResponseStream();
        }

        private Stream postWithNullBody(string endpoint, bool setBasicAuth) {
            var req = (HttpWebRequest)WebRequest.Create(endpoint);
            req.Proxy = server.NoProxy ? null : GlobalSettings.Proxy;
            req.Timeout = GlobalSettings.NetworkTimeout * 1000;
            req.ReadWriteTimeout = GlobalSettings.NetworkTimeout * 2000;
            req.Method = "POST";
            req.ContentType = "text/xml";
            // required for PLVS-83
            req.Accept = "application/xml";
            req.ContentLength = 0;

            const string postData = "";

            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            if (setBasicAuth) {
                setBasicAuthHeader(endpoint, req);
            } else {
                restoreSessionContext(req);
            }

            Stream dataStream = req.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            var resp = (HttpWebResponse)req.GetResponse();
            if (!setBasicAuth) {
                saveSessionContext(resp);
            }
            return resp.GetResponseStream();
        }

        private static string getBasicAuthParameter(string url) {
            return url.Contains("?") ? "&os_authType=basic" : "?os_authType=basic";
        }

        private void setBasicAuthHeader(string url, WebRequest req) {
            if (userName == null || password == null) {
                return;
            }
#if true
            req.Credentials = CredentialUtils.getCredentialsForUserAndPassword(url, userName, password);
#else
            string authInfo = CredentialUtils.getUserNameWithoutDomain(userName) + ":" + password;
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            req.Headers["Authorization"] = "Basic " + authInfo;
#endif
        }

       	private static string getRemoteExceptionMessages(IXPathNavigable doc) {
            XPathNavigator nav = doc.CreateNavigator();
            XPathExpression expr = nav.Compile("/errors/error");
            XPathNodeIterator it = nav.Select(expr);

       	    return messagesFromNode(it);
	    }

        private static string getRestErrorStatusCode(IXPathNavigable doc) {
            XPathNavigator nav = doc.CreateNavigator();
            XPathExpression expr = nav.Compile("/status/status-code");
            XPathNodeIterator it = nav.Select(expr);
            string code = messagesFromNode(it);
            expr = nav.Compile("/status/message");
            it = nav.Select(expr);
            string message = messagesFromNode(it);
            if (code == null || message == null) {
                return null;
            }
            return "Status code: " + code + ", Message: " + message;
        }

        private static string messagesFromNode(XPathNodeIterator it) {
            if (it.Count <= 0) {
                return null;
            }
            StringBuilder msg = new StringBuilder();
            while (it.MoveNext()) {
                msg.Append(it.Current.Value);
                msg.Append("\n");
            }
            return msg.ToString().Trim(new[] { '\n' });
        }

        private void saveSessionContext(WebResponse resp) {
            if (cookie != null) {
                return;
            }

            if (resp.Headers["Set-Cookie"] == null) {
                return;
            }

            cookie = resp.Headers["Set-Cookie"];
        }

        private void restoreSessionContext(WebRequest req) {
            if (cookie != null) {
                req.Headers["Cookie"] = cookie;
            }
        }
    }
}
