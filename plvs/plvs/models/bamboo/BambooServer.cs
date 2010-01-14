using System;
using Atlassian.plvs.api;

namespace Atlassian.plvs.models.bamboo {
    public class BambooServer : Server {

        public bool UseFavourites { get; set; }

        public BambooServer(string name, string url, string userName, string password) : base(name, url, userName, password) {}
        public BambooServer(Guid guid, string name, string url, string userName, string password) : base(guid, name, url, userName, password) {}
        public BambooServer(BambooServer other) : base(other) {
            if (other == null) return;
            UseFavourites = other.UseFavourites;
        }

        public override Guid Type { get { return new Guid("1C7A224E-52C4-4575-9212-7D731C13CFE9"); } }
    }
}