using System;

namespace Atlassian.plvs.api.jira {
    public class JiraServer : Server {
        public JiraServer(string name, string url, string userName, string password) : base(name, url, userName, password) {}
        public JiraServer(Guid guid, string name, string url, string userName, string password) : base(guid, name, url, userName, password) {}
        public JiraServer(Server other) : base(other) {}
        
        public override Guid Type { get { return new Guid("0C644383-BC4C-406d-B325-CA0AB1815B38"); } }
    }
}