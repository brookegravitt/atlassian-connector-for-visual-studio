using System;
using Atlassian.plvs.api.jira;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Unit_Test {
    [TestClass]
    public class TestCreateIssueFromJson {
        [TestMethod]
        public void TestPlvs374() {
            try {
                var plvs374 = Resource.plvs_374;
                var issue = JsonConvert.DeserializeObject(plvs374);
                var s = new JiraServer(Guid.NewGuid(), "test", "http://localhost", "test", "test", false, false, true);
                var t = issue as JToken;
                new JiraIssue(s, t);
            } catch (Exception e) {
                Assert.Fail(e.Message);   
            }
        }
    }
}
