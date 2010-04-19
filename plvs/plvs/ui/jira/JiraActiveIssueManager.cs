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

        private const string ACTIVE_ISSUE_SERVER_GUID = "activeIssueServerGuid";
        private const string ACTIVE_ISSUE_KEY = "activeIssueKey";
        private const string ACTIVE_ISSUE_TIMER_VALUE = "activeIssueTimerValue";
        private const string ACTIVE_ISSUE_IS_PAUSED = "activeIssueIsPaused";
        private const string PAST_ACTIVE_ISSUE_COUNT = "activeIssuePastIssueCount";
        private const string PAST_ACTIVE_ISSUE_SERVER_GUID = "activeIssuePastServerGuid_";
        private const string PAST_ACTIVE_ISSUE_KEY = "activeIssuePastIssueKey_";

        public event EventHandler<EventArgs> ActiveIssueChanged;

        public class ActiveIssue {
            public ActiveIssue(string key, string serverGuid) {
                this.key = key;
                this.serverGuid = serverGuid;
            }

            public string key { get; private set; }
            public string serverGuid { get; private set; }

            public bool Equals(ActiveIssue other) {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(other.key, key) && Equals(other.serverGuid, serverGuid);
            }

            public override bool Equals(object obj) {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj.GetType() == typeof (ActiveIssue) && Equals((ActiveIssue) obj);
            }

            public override int GetHashCode() {
                unchecked {
                    return ((key != null ? key.GetHashCode() : 0)*397) ^ (serverGuid != null ? serverGuid.GetHashCode() : 0);
                }
            }
        }

        public ActiveIssue CurrentActiveIssue { get; private set; }
        private readonly LinkedList<ActiveIssue> pastActiveIssues = new LinkedList<ActiveIssue>();

        private const int ACTIVE_ISSUE_LIST_SIZE = 3;

        private bool paused;
        private ToolStripSeparator separator;
        private const string NO_ISSUE_ACTIVE = "No Issue Active";
        private const string STOP_WORK = "Stop Work on Active Issue";
        private const string PAUSE_WORK = "Pause Work on Active Issue";
        private const string RESUME_WORK = "Resume Work on Active Issue";
        private const string LOG_WORK = "Log Work on Active Issue";
        private const string COMMENT = "Comment on Active Issue";

        public JiraActiveIssueManager(ToolStrip container) {
            activeIssueDropDown = new ToolStripSplitButton();
            buttonStop = new ToolStripButton(Resources.ico_inactiveissue) {Text = STOP_WORK, DisplayStyle = ToolStripItemDisplayStyle.Image};
            buttonStop.Click += (s, e) => deactivateActiveIssue(true);
            buttonPause = new ToolStripButton(Resources.ico_pauseissue) { Text = PAUSE_WORK, DisplayStyle = ToolStripItemDisplayStyle.Image };
            buttonPause.Click += (s, e) => {
                                     paused = !paused;
                                     buttonPause.Text = paused ? RESUME_WORK : PAUSE_WORK;
                                     buttonPause.Image = paused ? Resources.ico_activateissue : Resources.ico_pauseissue;
                                 };
            buttonLogWork = new ToolStripButton(Resources.ico_logworkonissue) { Text = LOG_WORK, DisplayStyle = ToolStripItemDisplayStyle.Image };
            buttonComment = new ToolStripButton(Resources.new_comment) { Text = COMMENT, DisplayStyle = ToolStripItemDisplayStyle.Image };

            separator = new ToolStripSeparator();

            container.Items.Add(activeIssueDropDown);
            container.Items.Add(buttonStop);
            container.Items.Add(buttonPause);
            container.Items.Add(buttonLogWork);
            container.Items.Add(buttonComment);
            container.Items.Add(separator);

            activeIssueDropDown.ButtonClick += (s, e) => {
                                             if (CurrentActiveIssue != null) {
                                                 MessageBox.Show("clicked active issue: " + CurrentActiveIssue.key);
                                             }
                                         };

            activeIssueDropDown.ToolTipText = "Active Issue";
            setEnabled(false);
        }

        private void setEnabled(bool enabled) {
            foreach (ToolStripItem c in new ToolStripItem[]
                                        {
                                            activeIssueDropDown, 
                                            buttonStop, 
                                            buttonPause, 
                                            buttonLogWork, 
                                            buttonComment, 
                                            separator
                                        }) {
                c.Enabled = enabled;
                c.Visible = enabled;
            }
        }

        public void init() {
            ParameterStore store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.ACTIVE_ISSUES);
            string activeIssueKey = store.loadParameter(ACTIVE_ISSUE_KEY, null);
            string activeIssueServerGuidStr = store.loadParameter(ACTIVE_ISSUE_SERVER_GUID, null);
            if (activeIssueKey != null && activeIssueServerGuidStr != null) {
                int time = store.loadParameter(ACTIVE_ISSUE_TIMER_VALUE, 0);
                paused = store.loadParameter(ACTIVE_ISSUE_IS_PAUSED, 0) > 0;
                if (paused) {
                    buttonPause.Text = RESUME_WORK;
                    buttonPause.Image = Resources.ico_activateissue;
                }
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
            int pastIssueCount = Math.Min(store.loadParameter(PAST_ACTIVE_ISSUE_COUNT, 0), ACTIVE_ISSUE_LIST_SIZE);
            for (int i = 0; i < pastIssueCount; ++i) {
                string key = store.loadParameter(PAST_ACTIVE_ISSUE_KEY + i, null);
                string guid = store.loadParameter(PAST_ACTIVE_ISSUE_SERVER_GUID + i, null);
                if (key != null && guid != null) {
                    ICollection<JiraServer> jiraServers = JiraServerModel.Instance.getAllEnabledServers();
                    if (jiraServers.Any(server => server.GUID.ToString().Equals(guid))) {
                        ActiveIssue issue = new ActiveIssue(key, guid);
                        pastActiveIssues.AddLast(issue);
                    }
                }
            }
            setupPastActiveIssuesDropDown();
            if (pastIssueCount > 0) {
                setNoIssueActiveInDropDown();
            }
        }

        private void setNoIssueActiveInDropDown() {
            activeIssueDropDown.Enabled = true;
            activeIssueDropDown.Visible = true;
            activeIssueDropDown.Text = NO_ISSUE_ACTIVE;
            separator.Enabled = true;
            separator.Visible = true;
        }

        private void setupPastActiveIssuesDropDown() {
            activeIssueDropDown.DropDown.Items.Clear();
            foreach (ActiveIssue issue in pastActiveIssues) {
                activeIssueDropDown.DropDown.Items.Add(new PastActiveIssueMenuItem(this, issue));
            }
        }

        private class PastActiveIssueMenuItem : ToolStripMenuItem {
            public PastActiveIssueMenuItem(JiraActiveIssueManager mgr, ActiveIssue issue): base(issue.key) {
                Click += (s, e) => mgr.setActive(issue);
            }
        }

        public bool isActive(JiraIssue issue) {
            if (CurrentActiveIssue == null) {
                return false;
            }
            return issue.Key.Equals(CurrentActiveIssue.key) &&
                   issue.Server.GUID.ToString().Equals(CurrentActiveIssue.serverGuid);
        }

        public void toggleActiveState(JiraIssue issue) {
            if (isActive(issue)) {
                deactivateActiveIssue(true);
            } else {
                setActive(issue);
            }
        }

        private void setActive(JiraIssue issue) {
            setActive(new ActiveIssue(issue.Key, issue.Server.GUID.ToString()));
        }

        private void setActive(ActiveIssue issue) {
            List<ActiveIssue> toRemove = pastActiveIssues.Where(i => i.Equals(issue)).ToList();
            foreach (var i in toRemove) {
                pastActiveIssues.Remove(i);
            }
            if (CurrentActiveIssue != null) {
                deactivateActiveIssue(false);
            } else {
                setupPastActiveIssuesDropDown();
            }
            CurrentActiveIssue = new ActiveIssue(issue.key, issue.serverGuid);
            setEnabled(true);
            activeIssueDropDown.Text = CurrentActiveIssue.key;
            if (ActiveIssueChanged != null) {
                ActiveIssueChanged(this, null);
            }
        }

        private void deactivateActiveIssue(bool notifyListeners) {
            pastActiveIssues.AddFirst(CurrentActiveIssue);
            while (pastActiveIssues.Count > ACTIVE_ISSUE_LIST_SIZE) {
                pastActiveIssues.RemoveLast();
            }
            CurrentActiveIssue = null;
            setupPastActiveIssuesDropDown();
            setEnabled(false);
            setNoIssueActiveInDropDown();
            if (notifyListeners && ActiveIssueChanged != null) {
                ActiveIssueChanged(this, null);
            }
        }
    }
}

