using System.Collections.Generic;
using System.Drawing;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.models;
using Atlassian.plvs.util.jira;

namespace Atlassian.plvs.ui.jira.issues {
    public class IssueNode : AbstractIssueTreeNode {
        public JiraIssue Issue { get; set; }

        public List<IssueNode> SubtaskNodes { get; private set; }

        public IssueNode(JiraIssue issue) {
            Issue = issue;
            if (issue.HasSubtasks) {
                SubtaskNodes = new List<IssueNode>();
            }
        }

        public override Image Icon {
            get { return ImageCache.Instance.getImage(Issue.IssueTypeIconUrl).Img; }
        }

        public override string Name {
            get { return Issue.Key + " - " + Issue.Summary; }
        }

        public Image PriorityIcon {
            get { return ImageCache.Instance.getImage(Issue.PriorityIconUrl).Img; }
        }

        public string StatusText {
            get { return Issue.Status; }
        }

        public Image StatusIcon {
            get { return ImageCache.Instance.getImage(Issue.StatusIconUrl).Img; }
        }

        public string Updated {
            get { return JiraIssueUtils.getTimeStringFromIssueDateTime(Issue.UpdateDate); }
        }
    }
}