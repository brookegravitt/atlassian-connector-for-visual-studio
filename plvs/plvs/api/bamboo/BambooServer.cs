using System;
using System.Collections.Generic;
using System.Text;

namespace Atlassian.plvs.api.bamboo {
    public class BambooServer : Server {

        public bool UseFavourites { get; set; }

        public List<string> PlanKeys { get; set; }

        public BambooServer(string name, string url, string userName, string password) 
            : base(name, url, userName, password) {
        }
        
        public BambooServer(Guid guid, string name, string url, string userName, string password, bool enabled) 
            : base(guid, name, url, userName, password, enabled) {
        }

        public BambooServer(BambooServer other) : base(other) {
            if (other == null) return;
            UseFavourites = other.UseFavourites;
            if (other.PlanKeys != null) {
                PlanKeys = new List<string>(other.PlanKeys);
            }
        }

        public override Guid Type { get { return BambooServerTypeGuid; } }

        public override string displayDetails() {
            var sb = new StringBuilder();
            sb.Append("Name: ").Append(Name).Append("\r\n");
            sb.Append("Enabled: ").Append(Enabled ? "Yes" : "No").Append("\r\n");
            sb.Append("URL: ").Append(Url).Append("\r\n");
            sb.Append("User Name: ").Append(UserName).Append("\r\n");
            sb.Append("Use Favourite Builds: ").Append(UseFavourites ? "Yes" : "No").Append("\r\n");
            if (!UseFavourites) {
                if (PlanKeys != null && PlanKeys.Count > 0) {
                    sb.Append("Plans monitored: ");
                    int i = 1;
                    foreach (var key in PlanKeys) {
                        sb.Append(key);
                        if (i < PlanKeys.Count) {
                            sb.Append(", ");
                        }
                        ++i;
                    }
                } else {
                    sb.Append("No plans monitored");
                }
            }
            return sb.ToString();
        }
    }
}