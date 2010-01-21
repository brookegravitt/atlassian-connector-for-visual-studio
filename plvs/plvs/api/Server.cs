using System;

namespace Atlassian.plvs.api {
    public abstract class Server {
        private Guid guid;
        private string name;
        private string url;
        private string userName;
        private string password;

        public static Guid JiraServerTypeGuid = new Guid("0C644383-BC4C-406d-B325-CA0AB1815B38");
        public static Guid BambooServerTypeGuid = new Guid("1C7A224E-52C4-4575-9212-7D731C13CFE9");

        protected Server(string name, string url, string userName, string password)
            : this(Guid.NewGuid(), name, url, userName, password) {}

        protected Server(Guid guid, string name, string url, string userName, string password) {
            this.guid = guid;
            this.name = name;
            this.url = url;
            this.userName = userName;
            this.password = password;
        }

        protected Server(Server other) {
            if (other != null) {
                guid = other.guid;
                name = other.name;
                url = other.url;
                userName = other.userName;
                password = other.password;
            }
            else {
                guid = Guid.NewGuid();
            }
        }

        public abstract Guid Type { get; }

        public string Name {
            get { return name; }
            set { name = value; }
        }

        public string Url {
            get { return url; }
            set { url = value; }
        }

        public string UserName {
            get { return userName; }
            set { userName = value; }
        }

        public string Password {
            get { return password; }
            set { password = value; }
        }

// ReSharper disable InconsistentNaming
        public Guid GUID
// ReSharper restore InconsistentNaming
        {
            get { return guid; }
            set { guid = value; }
        }

        public abstract string displayDetails();
    }
}
