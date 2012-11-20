using System;
using System.Globalization;
using System.Threading;
using Atlassian.plvs.api.jira;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Unit_Test {
    [TestClass]
    public class TestCreateIssueFromJson {
        private JiraServer server = new JiraServer(Guid.NewGuid(), "test", "http://localhost", "test", "test", false, false, true);

        [TestMethod]
        public void TestPlvs374() {
            try {
                var json = Resource.plvs_374_no_priority;
                var issue = JsonConvert.DeserializeObject(json);
                var t = issue as JToken;
                new JiraIssue(server, t);
            } catch (Exception e) {
                Assert.Fail(e.Message);   
            }
        }

        [TestMethod]
        public void TestPlvs374BadDate() {
            try {
                var info = new CultureInfo("de-DE");
                Thread.CurrentThread.CurrentCulture = info;
                Thread.CurrentThread.CurrentUICulture = info;
                var json = Resource.plvs_374_bad_date;
                var issue = JsonConvert.DeserializeObject(json);
                var t = issue as JToken;
                new JiraIssue(server, t);
            } catch (Exception e) {
                Assert.Fail(e.Message);
            }
        }
    }
}
