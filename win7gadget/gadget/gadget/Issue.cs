namespace gadget {
    internal class Issue {
        public readonly string Key;
        public readonly string Link;
        public readonly string Summary;
        public readonly string IssueType;
        public readonly string IssueTypeIconUrl;

        public Issue(string key, string link, string summary, string issueType, string issueTypeIconUrl) {
            Key = key;
            Link = link;
            Summary = summary;
            IssueType = issueType;
            IssueTypeIconUrl = issueTypeIconUrl;
        }
    }
}
