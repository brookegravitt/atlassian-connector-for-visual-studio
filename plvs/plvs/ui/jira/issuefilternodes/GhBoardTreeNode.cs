using System;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.api.jira.gh;

namespace Atlassian.plvs.ui.jira.issuefilternodes {
    public sealed class GhBoardTreeNode : TreeNodeWithJiraServer {
        private readonly JiraServer server;

        public GhBoardTreeNode(JiraServer server, RapidBoard board, int imageIdx)
            : base(board.Name, imageIdx) {

            this.server = server;
            this.Board = board;
        }

        public override JiraServer Server {
            get { return server; }
            set { throw new NotImplementedException(); }
        }

        public RapidBoard Board { get; private set; }
    }
}