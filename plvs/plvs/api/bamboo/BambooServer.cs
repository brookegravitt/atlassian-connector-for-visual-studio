using System;
using System.Text;

namespace Atlassian.plvs.api.bamboo {
    public class BambooServer : Server {

        public bool UseFavourites { get; set; }

        public BambooServer(string name, string url, string userName, string password) : base(name, url, userName, password) {}
        public BambooServer(Guid guid, string name, string url, string userName, string password) : base(guid, name, url, userName, password) {}
        public BambooServer(BambooServer other) : base(other) {
            if (other == null) return;
            UseFavourites = other.UseFavourites;
        }

        public override Guid Type { get { return new Guid("1C7A224E-52C4-4575-9212-7D731C13CFE9"); } }

        public override string displayDetails() {
            var sb = new StringBuilder();
            sb.Append("Name: ").Append(Name).Append("\r\n");
            sb.Append("URL: ").Append(Url).Append("\r\n");
            sb.Append("User Name: ").Append(UserName).Append("\r\n");
            sb.Append("Use Favourite Builds: ").Append(UseFavourites ? "Yes" : "No");
            return sb.ToString();
        }
    }
}