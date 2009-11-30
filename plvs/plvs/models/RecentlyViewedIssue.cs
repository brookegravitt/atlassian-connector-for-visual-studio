using System;
using Atlassian.plvs.api;

namespace Atlassian.plvs.models {
    internal class RecentlyViewedIssue {
        public Guid ServerGuid { get; private set; }
        public string IssueKey { get; private set; }

        public RecentlyViewedIssue(JiraIssue issue) {
            ServerGuid = issue.Server.GUID;
            IssueKey = issue.Key;
        }

        public RecentlyViewedIssue(Guid serverGuid, string issueKey) {
            ServerGuid = serverGuid;
            IssueKey = issueKey;
        }
    }
}