using System;
using System.Text;

namespace Atlassian.plvs.api.jira {
    public class JiraServer : Server {
        public JiraServer(string name, string url, string userName, string password) : base(name, url, userName, password) {}
        public JiraServer(Guid guid, string name, string url, string userName, string password) : base(guid, name, url, userName, password) {}
        public JiraServer(Server other) : base(other) {}
        
        public override Guid Type { get { return new Guid("0C644383-BC4C-406d-B325-CA0AB1815B38"); } }

        public override string displayDetails() {
            var sb = new StringBuilder();
            sb.Append("Name: ").Append(Name).Append("\r\n");
            sb.Append("URL: ").Append(Url).Append("\r\n");
            sb.Append("User Name: ").Append(UserName);
            return sb.ToString();
        }
    }
}