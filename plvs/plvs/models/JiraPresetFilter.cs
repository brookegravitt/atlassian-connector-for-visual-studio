using Atlassian.plvs.api;

namespace Atlassian.plvs.models {
    public abstract class JiraPresetFilter {

        public JiraProject Project { get; set; }

        public string Name { get; private set; }

        protected JiraPresetFilter(string name) {
            Name = name;
        }

        public string getFilterQueryString() {
            string query = getFilterQueryStringNoProject();
            if (Project != null) {
                return query + "&pid=" + Project.Id;
            }
            return query;
        }

        public abstract string getFilterQueryStringNoProject();

        public abstract string getSortBy();
    }
}
