using System;
using Atlassian.plvs.api;
using Atlassian.plvs.models;

namespace Atlassian.plvs.ui.issuefilternodes {
    public class JiraCustomFilterTreeNode : TreeNodeWithServer {
        private readonly JiraServer server;

        public JiraCustomFilterTreeNode(JiraServer server, JiraCustomFilter filter, int imageIdx) : base(filter.Name, imageIdx) {
            this.server = server;
            Filter = filter;
            Tag = filter;
        }

        public JiraCustomFilter Filter { get; private set; }

        public void setFilterName(string newName) {
            Text = newName;
        }

        public override JiraServer Server {
            get { return server; }
            set { throw new NotImplementedException(); }
        }
    }
}