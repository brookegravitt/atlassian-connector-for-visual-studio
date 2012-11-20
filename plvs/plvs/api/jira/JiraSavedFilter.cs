using Atlassian.plvs.util;
using Newtonsoft.Json.Linq;

namespace Atlassian.plvs.api.jira {
    public class JiraSavedFilter : JiraNamedEntity {
        public string Jql { get; private set; }
        public string ViewUrl { get; private set; }
        public string SearchUrl { get; private set; }

        public JiraSavedFilter(int id, string name)
            : base(id, name, null) {}

        public JiraSavedFilter(JToken filter) : this(filter["id"].Value<int>(), filter["name"].Value<string>()) {
            Jql = filter["jql"].Value<string>().unescape();
//            Jql = filter["jql"].Value<string>();
            ViewUrl = filter["viewUrl"].Value<string>();
            SearchUrl = filter["searchUrl"].Value<string>();
        }
    }
}