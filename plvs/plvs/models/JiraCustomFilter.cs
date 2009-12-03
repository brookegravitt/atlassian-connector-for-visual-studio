using System;
using System.Collections.Generic;
using System.Text;
using Atlassian.plvs.api;
using Atlassian.plvs.store;

namespace Atlassian.plvs.models {
    public class JiraCustomFilter {
        private readonly JiraServer server;

        private const string ISSUE_NAVIGATOR =
            "/secure/IssueNavigator.jspa?refreshFilter=false&reset=update&show=View+%3E%3E";

        private const string BROWSER_QUERY_SUFFIX = "&pager/start=-1&tempMax=100";

        private const string FILTER_COUNT = "_jiraCustomFilterCount";
        private const string FILTER_GUID = "_jiraCustormFilterGuid_";
        private const string FILTER_SERVER_GUID = "_jiraCustomFilterServerGuid_";

        private const string FILTER_PROJECT_COUNT = "_jiraCustomFilterProjectCount_";
        private const string FILTER_PROJECT_ID = "_jiraCustomFilterProjectId_";
        private const string FILTER_PROJECT_KEY = "_jiraCustomFilterProjectKey_";

        private const string FILTER_ISSUE_TYPE_COUNT = "_jiraCustomFilterIssueTypeCount_";
        private const string FILTER_ISSUE_TYPE_ID = "_jiraCustomFilterIssueTypeId_";
        private const string FILTER_ISSUE_TYPE_NAME = "_jiraCustomFilterIssueTypeName_";

        private const string FILTER_FIXFORVERSIONS_COUNT = "_jiraCustomFilterFixForVersionsCount_";
        private const string FILTER_FIXFORVERSIONS_ID = "_jiraCustomFilterFixForVersionsId_";
        private const string FILTER_FIXFORVERSIONS_NAME = "_jiraCustomFilterFixForVersionsName_";

        private const string FILTER_AFFECTVERSIONS_COUNT = "_jiraCustomFilterAffectsVersionsCount_";
        private const string FILTER_AFFECTVERSIONS_ID = "_jiraCustomFilterAffectsVersionsId_";
        private const string FILTER_AFFECTVERSIONS_NAME = "_jiraCustomFilterAffectsVersionsName_";

        private const string FILTER_COMPONENTS_COUNT = "_jiraCustomFilterComponentsCount_";
        private const string FILTER_COMPONENTS_ID = "_jiraCustomFilterComponentsId_";
        private const string FILTER_COMPONENTS_NAME = "_jiraCustomFilterComponentsName_";

        public List<JiraProject> Projects { get; private set; }
        public List<JiraNamedEntity> IssueTypes { get; private set; }
        public List<JiraNamedEntity> FixForVersions { get; private set; }
        public List<JiraNamedEntity> AffectsVersions { get; private set; }
        public List<JiraNamedEntity> Components { get; private set; }

        private static readonly Dictionary<Guid, JiraCustomFilter> FILTERS = new Dictionary<Guid, JiraCustomFilter>();

        public bool Empty {
            get {
                return Projects.Count + IssueTypes.Count + FixForVersions.Count + AffectsVersions.Count + Components.Count == 0;
            }
        }

        private JiraCustomFilter(JiraServer server) {
            this.server = server;

            Projects = new List<JiraProject>();
            IssueTypes = new List<JiraNamedEntity>();
            FixForVersions = new List<JiraNamedEntity>();
            AffectsVersions = new List<JiraNamedEntity>();
            Components = new List<JiraNamedEntity>();
        }

        public static List<JiraCustomFilter> getAll(JiraServer server) {
            List<JiraCustomFilter> list = new List<JiraCustomFilter>(1);
            if (!FILTERS.ContainsKey(server.GUID))
                FILTERS[server.GUID] = new JiraCustomFilter(server);
            list.Add(FILTERS[server.GUID]);
            return list;
        }

        public static void clear() {
            FILTERS.Clear();
        }

        public string getBrowserQueryString() {
            StringBuilder sb = new StringBuilder();
            sb.Append(ISSUE_NAVIGATOR).Append("&");

            sb.Append(getQueryParameters());

            sb.Append(BROWSER_QUERY_SUFFIX);

            return sb.ToString();
        }

        public string getFilterQueryString() {
            StringBuilder sb = new StringBuilder();

            sb.Append(getQueryParameters());

            return sb.ToString();
        }

        private string getQueryParameters() {
            StringBuilder sb = new StringBuilder();
            int first = 0;
            foreach (JiraProject project in Projects)
                sb.Append(first++ == 0 ? "" : "&").Append("pid=").Append(project.Id);
            foreach (JiraNamedEntity issueType in IssueTypes)
                sb.Append(first++ == 0 ? "" : "&").Append("type=").Append(issueType.Id);
            foreach (JiraNamedEntity version in AffectsVersions)
                sb.Append(first++ == 0 ? "" : "&").Append("version=").Append(version.Id);
            foreach (JiraNamedEntity version in FixForVersions)
                sb.Append(first++ == 0 ? "" : "&").Append("fixfor=").Append(version.Id);
            foreach (JiraNamedEntity comp in Components)
                sb.Append(first++ == 0 ? "" : "&").Append("component=").Append(comp.Id);

            return sb.ToString();
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();

            if (Empty)
                return "Filter not defined\n\nRight-click to define the filter";

            sb.Append("Server URL: ").Append(server.Url);

            if (Projects.Count > 0) {
                sb.Append("\nProjects: ");
                foreach (JiraProject project in Projects)
                    sb.Append(project.Key).Append(" ");
            }
            if (IssueTypes.Count > 0) {
                sb.Append("\nIssue Types: ");
                foreach (JiraNamedEntity issueType in IssueTypes)
                    sb.Append(issueType.Name).Append(" ");
            }
            if (AffectsVersions.Count > 0) {
                sb.Append("\nAffects Versions: ");
                foreach (JiraNamedEntity version in AffectsVersions)
                    sb.Append(version.Name).Append(" ");
            }
            if (FixForVersions.Count > 0) {
                sb.Append("\nFix For Versions: ");
                foreach (JiraNamedEntity version in FixForVersions)
                    sb.Append(version.Name).Append(" ");
            }
            if (Components.Count > 0) {
                sb.Append("\nComponents: ");
                foreach (JiraNamedEntity comp in Components)
                    sb.Append(comp.Name).Append(" ");
            }
            sb.Append("\n\nRight-click to edit filter definition");

            return sb.ToString();
        }

        public static void load() {
            FILTERS.Clear();

            ParameterStore store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.SETTINGS);

            int filtersCount = store.loadParameter(FILTER_COUNT, 0);
            ICollection<JiraServer> servers = JiraServerModel.Instance.getAllServers();

            for (int i = 0; i < filtersCount; ++i) {
                string filterGuidStr = store.loadParameter(FILTER_GUID + i, null);
                Guid filterGuid = new Guid(filterGuidStr);
                string filterServerGuidStr = store.loadParameter(getParamKey(filterGuid, FILTER_SERVER_GUID + filterGuidStr), null);
                Guid serverGuid = new Guid(filterServerGuidStr);
                JiraServer server = null;
                foreach (JiraServer s in servers) {
                    if (!s.GUID.Equals(serverGuid)) continue;
                    server = s;
                    break;
                }
                if (server == null) continue;

                JiraCustomFilter filter = new JiraCustomFilter(server);

                loadProjects(store, filterGuid, filter);
                loadIssueTypes(store, filterGuid, filter);
                loadFixVersions(store, filterGuid, filter);
                loadAffectsVersions(store, filterGuid, filter);
                loadComponents(store, filterGuid, filter);

                FILTERS[filterGuid] = filter;
            }
        }

        public static void save() {
            ParameterStore store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.SETTINGS);

            store.storeParameter(FILTER_COUNT, FILTERS.Count);
            int i = 0;
            foreach (var filter in FILTERS) {
                store.storeParameter(FILTER_GUID + i, filter.Key.ToString());
                store.storeParameter(getParamKey(filter.Key, FILTER_SERVER_GUID + filter.Key),
                                                     filter.Key.ToString());

                JiraCustomFilter f = filter.Value;

                storeProjects(store, filter.Key, f);
                storeIssueTypes(store, filter.Key, f);
                storeFixVersions(store, filter.Key, f);
                storeAffectsVersions(store, filter.Key, f);
                storeComponents(store, filter.Key, f);

                ++i;
            }
        }

        private static void storeComponents(ParameterStore store, Guid key, JiraCustomFilter f) {
            int i = 0;

            store.storeParameter(getParamKey(key, FILTER_COMPONENTS_COUNT), f.Components.Count);
            foreach (JiraNamedEntity comp in f.Components) {
                store.storeParameter(getParamKey(key, FILTER_COMPONENTS_ID + i), comp.Id);
                store.storeParameter(getParamKey(key, FILTER_COMPONENTS_NAME + i), comp.Name);
                ++i;
            }
        }

        private static void loadComponents(ParameterStore store, Guid key, JiraCustomFilter f) {
            int count = store.loadParameter(getParamKey(key, FILTER_COMPONENTS_COUNT), 0);
            for (int i = 0; i < count; ++i) {
                int id = store.loadParameter(getParamKey(key, FILTER_COMPONENTS_ID + i), 0);
                string name = store.loadParameter(getParamKey(key, FILTER_COMPONENTS_NAME + i), null);
                JiraNamedEntity comp = new JiraNamedEntity(id, name, null);
                f.Components.Add(comp);
            }
        }

        private static void storeAffectsVersions(ParameterStore store, Guid key, JiraCustomFilter f) {
            int i = 0;
            store.storeParameter(getParamKey(key, FILTER_AFFECTVERSIONS_COUNT), f.AffectsVersions.Count);
            foreach (JiraNamedEntity version in f.AffectsVersions) {
                store.storeParameter(getParamKey(key, FILTER_AFFECTVERSIONS_ID + i), version.Id);
                store.storeParameter(getParamKey(key, FILTER_AFFECTVERSIONS_NAME + i), version.Name);
                ++i;
            }
        }

        private static void loadAffectsVersions(ParameterStore store, Guid key, JiraCustomFilter f) {
            int count = store.loadParameter(getParamKey(key, FILTER_AFFECTVERSIONS_COUNT), 0);
            for (int i = 0; i < count; ++i) {
                int id = store.loadParameter(getParamKey(key, FILTER_AFFECTVERSIONS_ID + i), 0);
                string name = store.loadParameter(getParamKey(key, FILTER_AFFECTVERSIONS_NAME + i), null);
                JiraNamedEntity affectsVersion = new JiraNamedEntity(id, name, null);
                f.AffectsVersions.Add(affectsVersion);
            }
        }

        private static void storeFixVersions(ParameterStore store, Guid key, JiraCustomFilter f) {
            int i = 0;

            store.storeParameter(getParamKey(key, FILTER_FIXFORVERSIONS_COUNT), f.FixForVersions.Count);
            foreach (JiraNamedEntity version in f.FixForVersions) {
                store.storeParameter(getParamKey(key, FILTER_FIXFORVERSIONS_ID + i), version.Id);
                store.storeParameter(getParamKey(key, FILTER_FIXFORVERSIONS_NAME + i), version.Name);
                ++i;
            }
        }

        private static void loadFixVersions(ParameterStore store, Guid key, JiraCustomFilter f) {
            int count = store.loadParameter(getParamKey(key, FILTER_FIXFORVERSIONS_COUNT), 0);
            for (int i = 0; i < count; ++i) {
                int id = store.loadParameter(getParamKey(key, FILTER_FIXFORVERSIONS_ID + i), 0);
                string name = store.loadParameter(getParamKey(key, FILTER_FIXFORVERSIONS_NAME + i), null);
                JiraNamedEntity fixVersion = new JiraNamedEntity(id, name, null);
                f.FixForVersions.Add(fixVersion);
            }
        }

        private static void storeIssueTypes(ParameterStore store, Guid key, JiraCustomFilter f) {
            int i = 0;

            store.storeParameter(getParamKey(key, FILTER_ISSUE_TYPE_COUNT), f.IssueTypes.Count);
            foreach (JiraNamedEntity issueType in f.IssueTypes) {
                store.storeParameter(getParamKey(key, FILTER_ISSUE_TYPE_ID + i), issueType.Id);
                store.storeParameter(getParamKey(key, FILTER_ISSUE_TYPE_NAME + i), issueType.Name);
                ++i;
            }
        }

        private static void loadIssueTypes(ParameterStore store, Guid key, JiraCustomFilter f) {
            int count = store.loadParameter(getParamKey(key, FILTER_ISSUE_TYPE_COUNT), 0);
            for (int i = 0; i < count; ++i) {
                int id = store.loadParameter(getParamKey(key, FILTER_ISSUE_TYPE_ID + i), 0);
                string name = store.loadParameter(getParamKey(key, FILTER_ISSUE_TYPE_NAME + i), null);
                JiraNamedEntity issueType = new JiraNamedEntity(id, name, null);
                f.IssueTypes.Add(issueType);
            }
        }

        private static void storeProjects(ParameterStore store, Guid key, JiraCustomFilter f) {
            store.storeParameter(getParamKey(key, FILTER_PROJECT_COUNT), f.Projects.Count);
            int i = 0;
            foreach (JiraProject project in f.Projects) {
                store.storeParameter(getParamKey(key, FILTER_PROJECT_ID + i), project.Id);
                store.storeParameter(getParamKey(key, FILTER_PROJECT_KEY + i), project.Key);
                ++i;
            }
        }

        private static void loadProjects(ParameterStore store, Guid key, JiraCustomFilter f) {
            int count = store.loadParameter(getParamKey(key, FILTER_PROJECT_COUNT), 0);
            for (int i = 0; i < count; ++i) {
                int id = store.loadParameter(getParamKey(key, FILTER_PROJECT_ID + i), 0);
                string projectKey = store.loadParameter(getParamKey(key, FILTER_PROJECT_KEY + i), null);
                JiraProject proj = new JiraProject(id, projectKey, projectKey);
                f.Projects.Add(proj);
            }
        }

        private static string getParamKey(Guid serverGuid, string paramName) {
            return paramName + serverGuid;
        }
    }
}