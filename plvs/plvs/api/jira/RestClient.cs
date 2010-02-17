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
    internal class RestClient {
        private readonly string baseUrl;
        private readonly string userName;
        private readonly string password;

        public RestClient(JiraServer server) : this(server.Url, server.UserName, server.Password) {
        }

        private RestClient(string url, string userName, string password) {
            baseUrl = url;
            this.userName = userName;
            this.password = password;
        }

        public string getRenderedContent(string issueKey, int issueType, int projectId, string markup) {
            StringBuilder url = new StringBuilder(baseUrl + "/rest/api/1.0/render");

            url.Append(appendAuthentication(true));

            try {

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url.ToString());
                req.Method = "POST";
                req.Timeout = 5000;
                req.ReadWriteTimeout = 20000;
                req.ContentType = "application/json";
                Stream requestStream = req.GetRequestStream();
                ASCIIEncoding encoding = new ASCIIEncoding();
                StringBuilder query = new StringBuilder();
                query.Append("{\"rendererType\":\"atlassian-wiki-renderer\",\"unrenderedMarkup\":");
                query.Append(PlvsUtils.JsonEncode(markup));
                query.Append(",\"issueKey\":\"");
                if (issueKey != null) {
                    query.Append(issueKey);
                }
                query.Append("\"");
                if (issueType > -1) {
                    query.Append(",\"issueType\":\"").Append(issueType).Append("\"");
                }
                if (projectId > -1) {
                    query.Append(",\"projectId\":\"").Append(projectId).Append("\"");
                }
                query.Append("}");
                byte[] buffer = encoding.GetBytes(query.ToString());
                requestStream.Write(buffer, 0, buffer.Length);
                requestStream.Flush();
                requestStream.Close();

                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                Stream stream = resp.GetResponseStream();
                string text = PlvsUtils.getTextDocument(stream);
                stream.Close();
                return text;
            } catch (Exception e) {
                Debug.WriteLine(e.Message);
                throw;
            }
        }

        private string appendAuthentication(bool first) {
            if (userName != null) {
                return (first ? "?" : "&") + "os_username=" + HttpUtility.UrlEncode(userName)
                       + "&os_password=" + HttpUtility.UrlEncode(password);
            }
            return "";
        }
    }
}