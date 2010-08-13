namespace gadget {
    internal class Issue {
        public readonly string Key;
        public readonly string Link;
        public readonly string Summary;
        public readonly string IssueType;
        public readonly string IssueTypeIconUrl;
        public readonly string Priority;
        public readonly string PriorityIconUrl;
        public readonly string Status;
        public readonly string StatusIconUrl;
        public readonly string Reporter;
        public readonly string Assignee;
        public readonly string Created;
        public readonly string Updated;
        public readonly string Resolution;
        public readonly string Description;
        public readonly string Environment;
        public readonly string Votes;

        public Issue(
            string key, string link, string summary, string issueType, string issueTypeIconUrl, 
            string priority, string priorityIconUrl, string status, string statusIconUrl, 
            string reporter, string assignee, string created, string updated, string resolution, 
            string description, string environment, string votes) {

            Key = key;
            Votes = votes;
            Environment = environment;
            Description = description;
            Resolution = resolution;
            Updated = updated;
            Created = created;
            Assignee = assignee;
            Reporter = reporter;
            StatusIconUrl = statusIconUrl;
            Status = status;
            PriorityIconUrl = priorityIconUrl;
            Priority = priority;
            Link = link;
            Summary = summary;
            IssueType = issueType;
            IssueTypeIconUrl = issueTypeIconUrl;
        }
    }
}
