using System;
using System.Text;
using System.Net;
using System.IO;
using System.Diagnostics;
using Atlassian.plvs.dialogs;
using Atlassian.plvs.util;

namespace Atlassian.plvs.api.jira {
    internal class RestClient : JiraAuthenticatedClient {
        private readonly JiraServer server;

        public RestClient(JiraServer server) : base(server.Url, server.UserName, server.Password) {
            this.server = server;
        }

        public string getRenderedContent(string issueKey, int issueType, int projectId, string markup) {
            StringBuilder url = new StringBuilder(BaseUrl + "/rest/api/1.0/render");

            url.Append(appendAuthentication(true));

            try {

                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url.ToString());
                req.Proxy = server.NoProxy ? null : GlobalSettings.Proxy;

                req.Credentials = CredentialUtils.getCredentialsForUserAndPassword(url.ToString(), UserName, Password);
                req.Method = "POST";
                req.Timeout = GlobalSettings.NetworkTimeout * 1000;
                req.ReadWriteTimeout = GlobalSettings.NetworkTimeout * 2000;
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
                resp.Close();

                return text;
            } catch (Exception e) {
                Debug.WriteLine(e.Message);
                throw;
            }
        }
    }
}