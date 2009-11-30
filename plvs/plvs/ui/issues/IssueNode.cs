using System.Drawing;
using Atlassian.plvs.api;
using Atlassian.plvs.models;

namespace Atlassian.plvs.ui.issues {
    public class IssueNode {
        public JiraIssue Issue { get; set; }

        public IssueNode(JiraIssue issue) {
            Issue = issue;
        }

        public Image IssueTypeIcon {
            get { return ImageCache.Instance.getImage(Issue.IssueTypeIconUrl); }
        }

        public string KeyAndSummary {
            get { return Issue.Key + " - " + Issue.Summary; }
        }

        public Image PriorityIcon {
            get { return ImageCache.Instance.getImage(Issue.PriorityIconUrl); }
        }

        public string StatusText {
            get { return Issue.Status; }
        }

        public Image StatusIcon {
            get { return ImageCache.Instance.getImage(Issue.StatusIconUrl); }
        }

        public string Updated {
            get { return Issue.UpdateDate; }
        }
    }
}