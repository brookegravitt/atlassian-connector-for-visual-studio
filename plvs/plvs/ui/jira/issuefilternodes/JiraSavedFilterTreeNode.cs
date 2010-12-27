using System;
using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.ui.jira.issuefilternodes {
    public class JiraSavedFilterTreeNode : TreeNodeWithJiraServer {
        private readonly JiraServer server;

        public JiraSavedFilterTreeNode(JiraServer server, JiraSavedFilter filter, int imageIdx)
            : base(filter.Name, imageIdx) {
            this.server = server;
            Filter = filter;
            Tag = filter.Name;
        }

        public override JiraServer Server {
            get { return server; }
            set { throw new NotImplementedException(); }
        }

        public JiraSavedFilter Filter { get; private set; }
    }
}