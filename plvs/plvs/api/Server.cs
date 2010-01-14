﻿using System;

namespace Atlassian.plvs.api {
    public abstract class Server {
        private Guid guid;
        private string name;
        private string url;
        private string userName;
        private string password;

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
