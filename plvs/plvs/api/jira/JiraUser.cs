using System;

namespace Atlassian.plvs.api.jira {

    //  well, it is almost the same as JiraNamedEntity. Consider merging
    public class JiraUser {
        public JiraUser(string id, string name) {
            Id = id;
            Name = name;
        }

        public string Id { get; private set; }
        public String Name { get; private set; }

        public override string ToString() {
            return Name != null ? Name + " (" + Id + ")" : Id;
        }
    }
}
