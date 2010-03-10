using System;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.windows;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;

namespace Atlassian.plvs.markers.vs2010.texttag {
    public class JiraIssueTag : IUrlTag, IGlyphTag {
        public SnapshotSpan Where { get; set; }
        public string IssueKey { get; private set; }

        public JiraIssueTag(SnapshotSpan where, string issueKey) {
            Where = where;
            IssueKey = issueKey;
        }

        public Uri Url {
            get { return createUrl(IssueKey); }
        }
                
        private static Uri createUrl(string key) {
            JiraServer server = AtlassianPanel.Instance.Jira.CurrentlySelectedServerOrDefault;
            return server != null ? new Uri(server.Url + "/browse/" + key) : new Uri("about:blank");
        }
    }
}


