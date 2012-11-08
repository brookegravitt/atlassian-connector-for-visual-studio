using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Diagnostics;
using Atlassian.plvs.dialogs;
using Atlassian.plvs.util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Atlassian.plvs.api.jira {
    internal class RestClient : JiraAuthenticatedClient {
        private readonly JiraServer server;

        private const string REST = "/rest/api/2/";
        private const string UNEXPECTED = "Unexpected response code: ";

        public RestClient(JiraServer server) : base(server.Url, server.UserName, server.Password, server.NoProxy) {
            this.server = server;
        }

        public string getRenderedContent(string issueKey, int issueType, int projectId, string markup) {
            var url = new StringBuilder(BaseUrl + "/rest/api/1.0/render");

            if (server.OldSkoolAuth) {
                url.Append(appendAuthentication(true));
            }

            try {

                var req = (HttpWebRequest) WebRequest.Create(url.ToString());
                req.Proxy = server.NoProxy ? null : GlobalSettings.Proxy;

                req.Credentials = CredentialUtils.getCredentialsForUserAndPassword(url.ToString(), UserName, Password);
                req.Method = "POST";
                req.Timeout = GlobalSettings.NetworkTimeout * 1000;
                req.ReadWriteTimeout = GlobalSettings.NetworkTimeout * 2000;
                req.ContentType = "application/json";

                setSessionCookie(req);

                var requestStream = req.GetRequestStream();
                var encoding = new ASCIIEncoding();

                object json = new {
                                      rendererType = "atlassian-wiki-renderer",
                                      unrenderedMarkup = markup,
                                      issueKey = issueKey,
                                      issueType = issueType,
                                      projectId = projectId
                                  };
                    
                var serialized = JsonConvert.SerializeObject(json);
                var buffer = encoding.GetBytes(serialized);
                
                requestStream.Write(buffer, 0, buffer.Length);
                requestStream.Flush();
                requestStream.Close();

                var resp = (HttpWebResponse)req.GetResponse();
                var stream = resp.GetResponseStream();
                var text = PlvsUtils.getTextDocument(stream);
                if (stream != null) stream.Close();
                resp.Close();

                return text;
            } catch (Exception e) {
                Debug.WriteLine(e.Message);
                throw;
            }
        }

        public bool restSupported() {
            try {
                var url = BaseUrl + REST + "serverInfo";
                var resp = getJson(url);
                var jToken = resp["buildNumber"];
                if (jToken == null) return false;
                var buildNumber = jToken.Value<int>();
                server.BuildNumber = buildNumber;
                server.Version = resp["version"].Value<string>();
                server.ServerTitle = resp["serverTitle"].Value<string>();

                 // JIRA 5.0.1)
                return buildNumber >= 721;
            } catch(WebException) {
                return false;
            }
        }

        public List<JiraProject> getProjects() {
            var url = BaseUrl + REST + "project";
            var resp = getJson(url);
            return resp.Select(project => new JiraProject(project)).ToList();
        }

        public List<JiraNamedEntity> getIssueTypes(bool subtasks) {
            return getIssueTypes(subtasks, null);
        }

        public List<JiraNamedEntity> getIssueTypes(bool subtasks, JiraProject project) {
            var url = BaseUrl + REST + (project != null ? "project/" + project.Key : "issuetype");
            JToken resp = getJson(url);
            if (project != null) {
                resp = resp["issueTypes"];
            }
            return (from type in resp where subtasks == type["subtask"].Value<bool>() select new JiraNamedEntity(type)).ToList();
        }

        public List<JiraNamedEntity> getPriorities() {
            return getNamedEntities("priority");
        }

        public List<JiraNamedEntity> getStatuses() {
            return getNamedEntities("status");
        }

        public List<JiraNamedEntity> getResolutions() {
            return getNamedEntities("resolution");
        }

        public List<JiraSavedFilter> getSavedFilters() {
            var url = BaseUrl + REST + "filter/favourite";
            var resp = getJson(url);
            return resp.Select(filter => new JiraSavedFilter(filter)).ToList();
        }

        public List<JiraNamedEntity> getComponents(JiraProject project) {
            return getNamedEntities("project/" + project.Key, "components");
        }

        public List<JiraNamedEntity> getVersions(JiraProject project) {
            return getNamedEntities("project/" + project.Key, "versions");
        }

        private List<JiraNamedEntity> getNamedEntities(string what) {
            return getNamedEntities(what, null);
        }

        private List<JiraNamedEntity> getNamedEntities(string what, string sub) {
            var url = BaseUrl + REST + what;
            var resp = getJson(url);
            return sub != null 
                ? resp[sub].Select(item => new JiraNamedEntity(item)).ToList() 
                : resp.Select(item => new JiraNamedEntity(item)).ToList();
        }

        public List<JiraIssue> getSavedFilterIssues(JiraSavedFilter filter, string sortBy, string sortOrder, int start, int count) {
            var res = getJson(BaseUrl + REST + "search?jql=" + filter.Jql + " order by " + sortBy + " " + sortOrder + "&startAt=" + start + "&maxResults=" + count + "&expand=renderedFields");
            return res["issues"].Select(issue => new JiraIssue(server, issue)).ToList();
        }

        public List<JiraIssue> getCustomFilterIssues(JiraFilter filter, string sortOrder, int start, int count) {
            var res = getJson(BaseUrl + REST + "search?jql=" + filter.getJql() + " order by " + filter.getSortBy() + " " + sortOrder + "&startAt=" + start + "&maxResults=" + count + "&expand=renderedFields");
            return res["issues"].Select(issue => new JiraIssue(server, issue)).ToList();
        }

        public JiraIssue getIssue(string key) {
            var res = getRawIssueObject(key);
            return new JiraIssue(server, res);
        }

        public List<JiraNamedEntity> getActionsForIssue(JiraIssue issue) {
            var res = getJson(BaseUrl + REST + "issue/" + issue.Key + "/transitions");
            return res["transitions"].Select(t => new JiraNamedEntity(t)).ToList();
        }

        public List<JiraField> getFieldsForAction(JiraIssue issue, int actionId) {
            var res = getJson(BaseUrl + REST + "issue/" + issue.Key + "/transitions?expand=transitions.fields");
            return (
                from tr in res["transitions"] 
                where tr["id"].Value<int>() == actionId
                select tr["fields"].Select(fld => new JiraField(tr["fields"], ((JProperty)fld).Name)).ToList()
            ).FirstOrDefault();
        }

        public JiraNamedEntity getSecurityLevel(JiraIssue issue) {
            var i = getJson(BaseUrl + REST + "issue/" + issue.Key);
            var sec = i["security"];
            if (sec == null || !sec.HasValues) return null;
            return new JiraNamedEntity(sec);
        }

        public JToken getRawIssueObject(string key) {
            return getJson(BaseUrl + REST + "issue/" + key + "?expand=renderedFields,editmeta");
        }

        public void runIssueActionWithoutParams(JiraIssue issue, JiraNamedEntity action) {
            var data = new {
                transition = new {
                    id = action.Id
                }
            };
            postJson(BaseUrl + REST + "issue/" + issue.Key + "/transitions", data);
        }

        public void runIssueActionWithParams(JiraIssue issue, JiraNamedEntity action, ICollection<JiraField> fields, string comment) {
            object data;
            var fldsObj = new Dictionary<string, object>();
            foreach (var field in fields) {
                fldsObj[field.Id] = field.getJsonValue();
            }
            if (comment != null) {
                var commentObj = new List<object> { new { add = new { body = comment } } };
                data = new {
                    update = new { comment = commentObj },
                    transition = new { id = action.Id },
                    fields = fldsObj
                };
            } else {
                data = new {
                    transition = new { id = action.Id },
                    fields = fldsObj
                };
            }

            postJson(BaseUrl + REST + "issue/" + issue.Key + "/transitions", data);
        }

        public void updateIssue(JiraIssue issue, ICollection<JiraField> fields) {
            var fldsObj = new Dictionary<string, object>();
            foreach (var field in fields) {
                fldsObj[field.Id] = field.getJsonValue();
            }
            object data = new { fields = fldsObj };

            putJson(BaseUrl + REST + "issue/" + issue.Key, data);
        }

        public void addComment(JiraIssue issue, string comment) {
            var data = new { body = comment };
            postJson(BaseUrl + REST + "issue/" + issue.Key + "/comment", data, HttpStatusCode.Created);
        }

        private JContainer getJson(string url) {
            return jsonOp("GET", url, null, HttpStatusCode.OK);
        }

        private void postJson(string url, object data) {
            postJson(url, data, HttpStatusCode.NoContent);
        }

        private void postJson(string url, object data, HttpStatusCode code) {
            jsonOp("POST", url, data, code);
        }

        private void putJson(string url, object data) {
            jsonOp("PUT", url, data, HttpStatusCode.NoContent);
        }

        private void setBasicAuthHeader(WebRequest req) {
            var u = CredentialUtils.getUserNameWithoutDomain(server.UserName);
            var p = server.Password;
            var authInfo = u + ":" + p;
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            req.Headers["Authorization"] = "Basic " + authInfo;
        }

        private JContainer jsonOp(string method, string tgtUrl, object json, HttpStatusCode expectedCode) {
            var url = new StringBuilder(tgtUrl);

            if (server.OldSkoolAuth) {
                url.Append(appendAuthentication(true));
            }

            var req = (HttpWebRequest) WebRequest.Create(url.ToString());
            req.Proxy = server.NoProxy ? null : GlobalSettings.Proxy;

            req.Method = method;
            req.Timeout = GlobalSettings.NetworkTimeout*1000;
            req.ReadWriteTimeout = GlobalSettings.NetworkTimeout*2000;
            req.ContentType = "application/json";
            setBasicAuthHeader(req);

            string data = null;

            if (!method.Equals("GET")) {
                using (var requestStream = req.GetRequestStream()) {
                    var sw = new StreamWriter(requestStream);
                    data = JsonConvert.SerializeObject(json);
                    sw.Write(data);
                    sw.Flush();
                }
            }

            HttpWebResponse response;
            try {
                response = (HttpWebResponse) req.GetResponse();
                if (response.StatusCode == expectedCode) {
                    using (var stream = response.GetResponseStream()) {
                        if (stream != null) {
                            var reader = new StreamReader(stream);
                            var value = reader.ReadToEnd();
                            var result = JsonConvert.DeserializeObject(value) as JContainer;
                            return result;
                        }
                        return null;
                    }
                }
            } catch (WebException e) {
                if (e.Response != null) {
                    using (var stream = e.Response.GetResponseStream()) {
                        if (stream != null) {
                            var reader = new StreamReader(stream);
                            var value = reader.ReadToEnd();
                            //                        var result = JsonConvert.DeserializeObject(value) as JContainer;
                            throw new WebException(e.Message + "<br><br>Url: " + tgtUrl + (data != null ? ("<br>Data: " + data) : "") + "<br>Response: " + value + "<br>", e.InnerException, e.Status,
                                                   e.Response);
                        }
                    }
                }
                throw new WebException(e.Message + "<br><br>Url: " + tgtUrl + (data != null ? ("<br>Data: " + data) : "") + "<br>", e.InnerException, e.Status, e.Response);
            }


            throw new WebException(UNEXPECTED + response.StatusCode);
        }
    }
}