using System;
using System.Collections.Generic;
using Atlassian.plvs.api.soap;
using Atlassian.plvs.models;

namespace Atlassian.plvs.api {
    public class JiraServerFacade {
        private readonly SortedDictionary<string, SoapSession> sessionMap = new SortedDictionary<string, SoapSession>();

        private static readonly JiraServerFacade INSTANCE = new JiraServerFacade();

        public static JiraServerFacade Instance {
            get { return INSTANCE; }
        }

        private JiraServerFacade() {}

        private SoapSession getSoapSession(JiraServer server) {
            SoapSession s;
            if (!sessionMap.TryGetValue(server.Url + server.UserName, out s)) {
                s = new SoapSession(server.Url);
                s.login(server.UserName, server.Password);
                sessionMap.Add(getSessionKey(server), s);
            }
            return s;
        }

        private static string getSessionKey(JiraServer server) {
            return server.Url + server.UserName;
        }

        private void removeSession(JiraServer server) {
            sessionMap.Remove(getSessionKey(server));
        }

        public void login(JiraServer server) {
            new SoapSession(server.Url).login(server.UserName, server.Password);
        }

        public List<JiraProject> getProjects(JiraServer server) {
            try {
                return getSoapSession(server).getProjects();
            } catch (Exception) {
                removeSession(server);
                throw;
            }
        }

        public List<JiraNamedEntity> getIssueTypes(JiraServer server) {
            try {
                return getSoapSession(server).getIssueTypes();
            } catch (Exception) {
                removeSession(server);
                throw;
            }
        }

        public List<JiraNamedEntity> getIssueTypes(JiraServer server, JiraProject project) {
            try {
                return getSoapSession(server).getIssueTypes(project);
            } catch (Exception) {
                removeSession(server);
                throw;
            }
        }

        public List<JiraSavedFilter> getSavedFilters(JiraServer server) {
            try {
                return getSoapSession(server).getSavedFilters();
            } catch (Exception) {
                removeSession(server);
                throw;
            }
        }

        public List<JiraIssue> getSavedFilterIssues(JiraServer server, JiraSavedFilter filter, int start, int count) {
            RssClient rss = new RssClient(server);
            return rss.getSavedFilterIssues(filter.Id, "priority", "DESC", start, count);
        }

        public List<JiraIssue> getCustomFilterIssues(JiraServer server, JiraCustomFilter filter, int start, int count) {
            RssClient rss = new RssClient(server);
            return rss.getCustomFilterIssues(filter.getFilterQueryString(), "priority", "DESC", start, count);
        }

        public List<JiraIssue> getPresetFilterIssues(JiraServer server, JiraPresetFilter filter, int start, int count) {
            RssClient rss = new RssClient(server);
            return rss.getCustomFilterIssues(filter.getFilterQueryString(), filter.getSortBy(), "DESC", start, count);
        }

        public JiraIssue getIssue(JiraServer server, string key) {
            RssClient rss = new RssClient(server);
            return rss.getIssue(key);
        }

        public List<JiraNamedEntity> getPriorities(JiraServer server) {
            try {
                return getSoapSession(server).getPriorities();
            } catch (Exception) {
                removeSession(server);
                throw;
            }
        }

        public List<JiraNamedEntity> getStatuses(JiraServer server) {
            try {
                return getSoapSession(server).getStatuses();
            } catch (Exception) {
                removeSession(server);
                throw;
            }
        }

        public void addComment(JiraIssue issue, string comment) {
            try {
                getSoapSession(issue.Server).addComment(issue, comment);
            } catch (Exception) {
                removeSession(issue.Server);
                throw;
            }
        }

        public List<JiraNamedEntity> getActionsForIssue(JiraIssue issue) {
            try {
                return getSoapSession(issue.Server).getActionsForIssue(issue);
            } catch (Exception) {
                removeSession(issue.Server);
                throw;
            }
        }

        public List<JiraField> getFieldsForAction(JiraIssue issue, int actionId) {
            try {
                return getSoapSession(issue.Server).getFieldsForAction(issue, actionId);
            } catch (Exception) {
                removeSession(issue.Server);
                throw;
            }
        }

        public void runIssueActionWithoutParams(JiraIssue issue, JiraNamedEntity action) {
            try {
                getSoapSession(issue.Server).runIssueActionWithoutParams(issue, action.Id);
            } catch (Exception) {
                removeSession(issue.Server);
                throw;
            }
        }

        public List<JiraNamedEntity> getComponents(JiraServer server, JiraProject project) {
            try {
                return getSoapSession(server).getComponents(project);
            } catch (Exception) {
                removeSession(server);
                throw;
            }
        }

        public List<JiraNamedEntity> getVersions(JiraServer server, JiraProject project) {
            try {
                return getSoapSession(server).getVersions(project);
            } catch (Exception) {
                removeSession(server);
                throw;
            }
        }

        public List<JiraNamedEntity> getResolutions(JiraServer server) {
            try {
                return getSoapSession(server).getResolutions();
            } catch (Exception) {
                removeSession(server);
                throw;
            }
        }
    }
}