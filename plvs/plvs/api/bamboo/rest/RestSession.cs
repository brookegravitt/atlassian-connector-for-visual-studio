using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Xml.XPath;
using Atlassian.plvs.util;

namespace Atlassian.plvs.api.bamboo.rest {
    public class RestSession {
        private readonly string url;
        private string authToken;
        private string connection;

        private const string LIST_PLAN_ACTION = "/api/rest/listBuildNames.action";
        private const string LATEST_USER_BUILDS_ACTION = "/api/rest/getLatestUserBuilds.action";
        private const string LATEST_BUILD_FOR_PLAN_ACTION = "/api/rest/getLatestBuildResults.action";

        private const string LOGIN_ACTION = "/api/rest/login.action";
    	private const string LOGOUT_ACTION = "/api/rest/logout.action";

        public RestSession(string url) {
            this.url = url;
        }

        public bool LoggedIn { get; private set; }

        public RestSession login(string userName, string password) {

            string endpoint = url + LOGIN_ACTION 
                + "?username=" + HttpUtility.UrlEncode(userName, Encoding.UTF8) + "&password=" + HttpUtility.UrlEncode(password, Encoding.UTF8) 
                + "&os_username=" + HttpUtility.UrlEncode(userName, Encoding.UTF8) + "&os_password=" + HttpUtility.UrlEncode(password, Encoding.UTF8);

            Stream stream = getQueryResultStream(endpoint);

            XPathDocument doc = XPathUtils.getDocument(stream);

            XPathNavigator nav = doc.CreateNavigator();
            XPathExpression expr = nav.Compile("/response/auth");
            XPathNodeIterator it = nav.Select(expr);
            if (it == null || it.Count == 0) {
                throw new Exception("Server did not return any authentication token");
            }
            if (it.Count != 1) {
                throw new Exception("Server returned unexpected number of authentication tokens (" + it.Count + ")");
            }
            it.MoveNext();
            authToken = it.Current.Value;

            LoggedIn = true;
            return this;
        }

        public void logout() {
            if (!LoggedIn) return;
            try {
                string endpoint = url + LOGOUT_ACTION + "?auth=" + HttpUtility.UrlEncode(authToken, Encoding.UTF8);
                getQueryResultStream(endpoint);
            } catch (Exception e) {
                Debug.WriteLine("RestSession.logout() - exception (ignored): " + e.Message);
            }
            authToken = null;
            LoggedIn = false;
        }

        public ICollection<BambooPlan> getPlanList() {

            String endpoint = url + LIST_PLAN_ACTION + "?auth=" + HttpUtility.UrlEncode(authToken, Encoding.UTF8);

            Stream stream = getQueryResultStream(endpoint);

            XPathDocument doc = XPathUtils.getDocument(stream);

            XPathNavigator nav = doc.CreateNavigator();
            XPathExpression expr = nav.Compile("/response/build");
            XPathNodeIterator it = nav.Select(expr);

            List<BambooPlan> plans = new List<BambooPlan>();

            while(it.MoveNext()) {
                string enabledValue = XPathUtils.getAttributeSafely(it.Current, "enabled", "true");
				bool enabled = true;
				if (enabledValue != null) {
					enabled = Boolean.Parse(enabledValue);
				}
                it.Current.MoveToFirstChild();
                string key = null;
                string name = null;
                do {
                    switch (it.Current.Name) {
                        case "key":
                            key = it.Current.Value;
                            break;
                        case "name":
                            name = it.Current.Value;
                            break;
                    }
                    if (key == null || name == null) continue;
                    BambooPlan plan = new BambooPlan(key, name, enabled);
                    plans.Add(plan);
                } while (it.Current.MoveToNext());
            }
            return plans;
        }

        private List<string> getFavouriteUserPlans() {
            String endpoint = url + LATEST_USER_BUILDS_ACTION + "?auth=" + HttpUtility.UrlEncode(authToken, Encoding.UTF8);
            Stream stream = getQueryResultStream(endpoint);
            XPathDocument doc = XPathUtils.getDocument(stream);
            XPathNavigator nav = doc.CreateNavigator();
            XPathExpression expr = nav.Compile("/response/build");
            XPathNodeIterator it = nav.Select(expr);

            List<string> result = new List<string>();
            while (it.MoveNext()) {
                it.Current.MoveToFirstChild();
                do {
                    if (it.Current.Name.Equals("key")) {
                        result.Add(it.Current.Value);
                    }
                } while (it.Current.MoveToNext());
            }
            return result;
        }

        public ICollection<BambooBuild> getLatestBuildsForFavouritePlans() {
            List<BambooBuild> builds = new List<BambooBuild>();
            foreach (string plan in getFavouriteUserPlans()) {
                
            }
            foreach (BambooPlan plan in getPlanList()) {
                if (plan.Favourite == null || !plan.Favourite.Value) continue;

                String endpoint = url + LATEST_BUILD_FOR_PLAN_ACTION + "?auth=" 
                    + HttpUtility.UrlEncode(authToken, Encoding.UTF8) + "&buildKey=" + HttpUtility.UrlEncode(plan.Key);

                Stream stream = getQueryResultStream(endpoint);

                XPathDocument doc = XPathUtils.getDocument(stream);

                XPathNavigator nav = doc.CreateNavigator();
                XPathExpression expr = nav.Compile("/response");
                XPathNodeIterator it = nav.Select(expr);
            }
            return builds;
        }

        private Stream getQueryResultStream(string url) {
            var req = (HttpWebRequest)WebRequest.Create(url);
            if (connection != null) {
                req.Connection = connection;
            }
            req.KeepAlive = true;
            req.Timeout = 10000;
            req.ReadWriteTimeout = 20000;
            var resp = (HttpWebResponse)req.GetResponse();
            if (connection == null) {
                connection = req.Connection;
            }
            return resp.GetResponseStream();
        }
    }
}
