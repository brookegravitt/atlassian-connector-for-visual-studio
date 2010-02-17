using System;
using System.Text;

namespace Atlassian.plvs.api.jira {
    public class JiraServer : Server {
        public JiraServer(string name, string url, string userName, string password) : base(name, url, userName, password) {}
        public JiraServer(Guid guid, string name, string url, string userName, string password, bool enabled) 
            : base(guid, name, url, userName, password, enabled) {}
        public JiraServer(Server other) : base(other) {}
        
        public override Guid Type { get { return JiraServerTypeGuid; } }
    }
}