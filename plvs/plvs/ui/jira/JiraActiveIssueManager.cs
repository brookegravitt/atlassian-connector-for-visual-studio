using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.store;

namespace Atlassian.plvs.ui.jira {
    public class JiraActiveIssueManager {
        private readonly ToolStripButton buttonComment;
        private readonly ToolStripButton buttonLogWork;
        private readonly ToolStripButton buttonPause;
        private readonly ToolStripButton buttonStop;
        private readonly ToolStripSplitButton activeIssueDropDown;

        private const string PAST_ACTIVE_ISSUE_COUNT = "activeIssuePastIssueCount";
        private const string ACTIVE_ISSUE_SERVER_GUID = "activeIssueServerGuid";
        private const string ACTIVE_ISSUE_KEY = "activeIssueKey";
        private const string PAST_ACTIVE_ISSUE_SERVER_GUID = "activeIssuePastServerGuid_";
        private const string PAST_ACTIVE_ISSUE_KEY = "activeIssuePastIssueKey_";

        public class ActiveIssue {
            public ActiveIssue(string key, string serverGuid) {
                this.key = key;
                this.serverGuid = serverGuid;
            }

            public string key { get; private set; }
            public string serverGuid { get; private set; }
        }

        public ActiveIssue CurrentActiveIssue { get; private set; }
        private readonly LinkedList<ActiveIssue> pastActiveIssues = new LinkedList<ActiveIssue>();

        private const int ACTIVE_ISSUE_LIST_SIZE = 10;

        private bool paused;

        public JiraActiveIssueManager(StatusStrip container) {
            activeIssueDropDown = new ToolStripSplitButton();
            buttonStop = new ToolStripButton(Resources.ico_inactiveissue) {Text = "Stop Work on Active Issue"};
            buttonPause = new ToolStripButton(Resources.ico_pauseissue) {Text = "Pause Work on Active Issue"};
            buttonPause.Click += (s, e) => {
                                     paused = !paused;
                                     buttonPause.Text = paused
                                                            ? "Pause Work on Active Issue"
                                                            : "Resume Work on Active Issue";
                                 };
            buttonLogWork = new ToolStripButton(Resources.ico_logworkonissue) {Text = "Log Work on Active Issue"};
            buttonComment = new ToolStripButton(Resources.new_comment) {Text = "Comment on Active Issue"};

            container.Items.Add(activeIssueDropDown);
            container.Items.Add(buttonStop);
            container.Items.Add(buttonPause);
            container.Items.Add(buttonLogWork);
            container.Items.Add(buttonComment);

            activeIssueDropDown.Click += (s, e) => {
                                             if (CurrentActiveIssue != null) {
                                                 MessageBox.Show("clicked active issue: " + CurrentActiveIssue.key);
                                             }
                                         };

            activeIssueDropDown.ToolTipText = "Active Issue";
            setEnabled(false);
        }

        private void setEnabled(bool enabled) {
            foreach (ToolStripItem c in new ToolStripItem[] { activeIssueDropDown, buttonStop, buttonPause, buttonLogWork, buttonComment }) {
                c.Enabled = enabled;
                c.Visible = enabled;
            }
        }

        public void init() {
            ParameterStore store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.ACTIVE_ISSUES);
            string activeIssueKey = store.loadParameter(ACTIVE_ISSUE_KEY, null);
            string activeIssueServerGuidStr = store.loadParameter(ACTIVE_ISSUE_SERVER_GUID, null);
            if (activeIssueKey != null && activeIssueServerGuidStr != null) {
                ICollection<JiraServer> jiraServers = JiraServerModel.Instance.getAllEnabledServers();
                if (jiraServers.Any(server => server.GUID.ToString().Equals(activeIssueServerGuidStr))) {
                    setEnabled(true);
                    activeIssueDropDown.Text = activeIssueKey;
                    CurrentActiveIssue = new ActiveIssue(activeIssueKey, activeIssueServerGuidStr);
                }
            }
            loadPastActiveIssues(store);
        }

        private void loadPastActiveIssues(ParameterStore store) {
            int pastIssueCount = Math.Max(store.loadParameter(PAST_ACTIVE_ISSUE_COUNT, 0), ACTIVE_ISSUE_LIST_SIZE);
            for (int i = 0; i < pastIssueCount; ++i) {
                string key = store.loadParameter(PAST_ACTIVE_ISSUE_KEY + i, null);
                string guid = store.loadParameter(PAST_ACTIVE_ISSUE_SERVER_GUID + i, null);
                if (key != null && guid != null) {
                    ICollection<JiraServer> jiraServers = JiraServerModel.Instance.getAllEnabledServers();
                    if (jiraServers.Any(server => server.GUID.ToString().Equals(guid))) {
                        ActiveIssue issue = new ActiveIssue(key, guid);
                        pastActiveIssues.AddLast(issue);
                        activeIssueDropDown.DropDown.Items.Add(new PastActiveIssueMenuItem(issue));
                    }
                }
            }
            if (pastIssueCount > 0) {
                activeIssueDropDown.Enabled = true;
            }
        }

        private class PastActiveIssueMenuItem : ToolStripItem {
            public PastActiveIssueMenuItem(ActiveIssue issue) {
                Click += (s, e) => {
                             MessageBox.Show("clicked: " + issue.key);
                         };
            }
        }
    }
}

