using System;
using Newtonsoft.Json.Linq;

namespace Atlassian.plvs.api.jira {
    public class JiraNamedEntity {
        public JiraNamedEntity(int id, string name, string iconUrl) {
            Id = id;
            Name = name;
            IconUrl = iconUrl;
        }

        public JiraNamedEntity(JToken entity) : this(entity["id"].Value<int>(), entity["name"].Value<string>(), entity["iconUrl"].Value<string>()){
        }

        public int Id { get; private set; }
        public string Name { get; private set; }
        public string IconUrl { get; private set; }

        public override string ToString() {
            return Name;
        }
    }
}