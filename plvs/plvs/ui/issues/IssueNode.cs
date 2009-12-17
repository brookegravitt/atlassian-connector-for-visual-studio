using System.Drawing;
using Atlassian.plvs.api;
using Atlassian.plvs.models;
using Atlassian.plvs.util;

namespace Atlassian.plvs.ui.issues {
    public class IssueNode : AbstractIssueTreeNode {
        public JiraIssue Issue { get; set; }

        public IssueNode(JiraIssue issue) {
            Issue = issue;
        }

        public override Image Icon {
            get { return ImageCache.Instance.getImage(Issue.IssueTypeIconUrl); }
        }

        public override string Name {
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
            get { return JiraIssueUtils.getTimeStringFromIssueDateTime(Issue.UpdateDate); }
        }
    }
}