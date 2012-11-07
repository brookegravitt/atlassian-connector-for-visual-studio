using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Atlassian.plvs.api.jira {
    public class JiraField {
        public List<string> Values { get; set; }

        public JiraField(string id, string name) {
            Id = id;
            Name = name;
            Values = new List<string>();
            // oh well
            Required = true;
        }

        public JiraField(JiraField other) {
            Id = other.Id;
            Name = other.Name;
            Values = new List<string>(other.Values);
            Required = other.Required;
            fieldDefinition = other.fieldDefinition;
        }

        public JiraField(JToken fields, string name) {
            fieldDefinition = fields[name];
            Id = name;
            Name = fieldDefinition["name"].Value<string>();
            Required = fieldDefinition["required"].Value<bool>();
            Values = new List<string>();
        }

        public void setRawIssueObject(object rawIssueObject) {
            var issue = rawIssueObject as JToken;
            if (issue == null) return;
            fieldDefinition = issue["editmeta"]["fields"][Id];
        }

        public string Id { get; private set; }
        public string Name { get; private set; }
        public bool Required { get; private set; }
        public string SettablePropertyName { get; set; }
        private JToken fieldDefinition;

        public object getJsonValue() {
            if (fieldDefinition == null) return null;

            var simple = !"user".Equals(fieldDefinition["schema"]["type"].Value<string>()) && fieldDefinition["allowedValues"] == null;
            if (simple) {
                return Values.Count == 0 ? null : Values[0];
            }

            if ("array".Equals(fieldDefinition["schema"]["type"].Value<string>())) {
                if (Values.Count == 0) return new List<object>();

                var stringItems = "string".Equals(fieldDefinition["schema"]["items"].Value<string>());
                return Values.Select(value => stringItems ? value : getPair(SettablePropertyName, value)).ToList();
            }

            return getPair(SettablePropertyName, Values[0]);
        }

        private static object getPair(string key, object val) {
            var d = new Dictionary<string, object> {{key, val}};
            string there = JsonConvert.SerializeObject(d);
            var back = JsonConvert.DeserializeObject(there) as JContainer;
            return back;
        }
    }
}