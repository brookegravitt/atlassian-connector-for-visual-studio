using System;
using Atlassian.plvs.api.jira;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Unit_Test {
    [TestClass]
    public class TestParseIssuePlvs384 {
        [TestMethod]
        public void TestParseJson() {
            try {
                var json = Resource.plvs_384_issue_json_txt;
                var issue = JsonConvert.DeserializeObject(json);
                var t = issue as JToken;
                var s = new JiraServer("a", "http:/a", "a", "a", true, false);
                var issueObject = new JiraIssue(s, t);
            } catch(Exception e) {
                Assert.Fail(e.Message);
            }
        }
    }
}
