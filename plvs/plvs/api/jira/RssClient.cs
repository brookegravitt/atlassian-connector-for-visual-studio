using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Xml.XPath;
using Atlassian.plvs.util;

namespace Atlassian.plvs.api.jira {
    internal class RssClient {
        private readonly JiraServer server;
        private readonly string baseUrl;
        private readonly string userName;
        private readonly string password;

        public RssClient(JiraServer server)
            : this(server.Url, server.UserName, server.Password) {
            this.server = server;
        }

        private RssClient(string url, string userName, string password) {
            baseUrl = url;
            this.userName = userName;
            this.password = password;
        }

        public List<JiraIssue> getSavedFilterIssues(int filterId, string sortBy, string sortOrder, int start, int max) {
            StringBuilder url = new StringBuilder(baseUrl + "/sr/jira.issueviews:searchrequest-xml/");
            url.Append(filterId).Append("/SearchRequest-").Append(filterId).Append(".xml");
            url.Append("?sorter/field=" + sortBy);
            url.Append("&sorter/order=" + sortOrder);
            url.Append("&pager/start=" + start);
            url.Append("&tempMax=" + max);

            url.Append(appendAuthentication(false));

            try {
                return createIssueList(getRssQueryResultStream(url));
            }
            catch (Exception e) {
                Debug.WriteLine(e.Message);
                throw;
            }
        }

        public List<JiraIssue> getCustomFilterIssues(string queryString, string sortBy, string sortOrder, int start,
                                                     int max) {
            StringBuilder url =
                new StringBuilder(baseUrl + "/sr/jira.issueviews:searchrequest-xml/temp/SearchRequest.xml?" +
                                  queryString);
            url.Append("&sorter/field=" + sortBy);
            url.Append("&sorter/order=" + sortOrder);
            url.Append("&pager/start=" + start);
            url.Append("&tempMax=" + max);

            url.Append(appendAuthentication(false));

            try {
                return createIssueList(getRssQueryResultStream(url));
            }
            catch (Exception e) {
                Debug.WriteLine(e.Message);
                throw;
            }
        }

        public JiraIssue getIssue(string key) {
            StringBuilder url = new StringBuilder(baseUrl + "/si/jira.issueviews:issue-xml/");
            url.Append(key).Append("/").Append(key).Append(".xml");

            url.Append(appendAuthentication(true));

            try {
                List<JiraIssue> list = createIssueList(getRssQueryResultStream(url));
                if (list.Count != 1) {
                    throw new ArgumentException("No such issue");
                }
                return list[0];
            }
            catch (Exception e) {
                Debug.WriteLine(e.Message);
                throw;
            }
        }

        private static Stream getRssQueryResultStream(StringBuilder url) {
            HttpWebRequest req = (HttpWebRequest) WebRequest.Create(url.ToString());
            req.Timeout = 5000;
            req.ReadWriteTimeout = 20000;
            HttpWebResponse resp = (HttpWebResponse) req.GetResponse();
            return resp.GetResponseStream();
        }

        private string appendAuthentication(bool first) {
            if (userName != null) {
                return (first ? "?" : "&") + "os_username=" + HttpUtility.UrlEncode(userName)
                       + "&os_password=" + HttpUtility.UrlEncode(password);
            }
            return "";
        }

        private List<JiraIssue> createIssueList(Stream s) {
            XPathDocument doc = XPathUtils.getDocument(s);

            XPathNavigator nav = doc.CreateNavigator();
            XPathExpression expr = nav.Compile("/rss/channel/item");
            XPathNodeIterator it = nav.Select(expr);

            List<JiraIssue> list = new List<JiraIssue>();
            while (it.MoveNext()) {
                list.Add(new JiraIssue(server, it.Current));
            }

            return list;
        }
    }
}