using System;
using Atlassian.plvs.models;
using Atlassian.plvs.api;

namespace Atlassian.plvs.ui.issuefilternodes {
    internal class JiraPresetFilterTreeNode : TreeNodeWithServer {
        private readonly JiraServer server;

        public JiraPresetFilterTreeNode(JiraServer server, JiraPresetFilter filter, int imageIdx)
            : base(filter.Name, imageIdx) {

            this.server = server;
            Filter = filter;

            Tag = filter;
        }

        public JiraPresetFilter Filter { get; private set; }

        public override JiraServer Server {
            get { return server; }
            set { throw new NotImplementedException(); }
        }
    }
}