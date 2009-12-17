using System;
using Atlassian.plvs.api;
using Atlassian.plvs.models;

namespace Atlassian.plvs.ui.issuefilternodes {
    internal class JiraCustomFilterTreeNode : TreeNodeWithServer {
        private readonly JiraServer server;

        public JiraCustomFilterTreeNode(JiraServer server, JiraCustomFilter filter, int imageIdx)
            : base("Custom Filter", imageIdx) {
            this.server = server;
            Filter = filter;

            Tag = filter;
        }

        public JiraCustomFilter Filter { get; private set; }

        public override JiraServer Server {
            get { return server; }
            set { throw new NotImplementedException(); }
        }
    }
}