using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.autoupdate;
using Atlassian.plvs.dialogs.jira;
using Atlassian.plvs.models;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.store;
using Atlassian.plvs.util;
using Atlassian.plvs.windows;
using Timer = System.Windows.Forms.Timer;

namespace Atlassian.plvs.ui.jira {
    public class JiraActiveIssueManager {
        private readonly ToolStrip container;
        private readonly ToolStripButton buttonComment;
        private readonly ToolStripButton buttonLogWork;
        private readonly ToolStripButton buttonPause;
        private readonly ToolStripButton buttonStop;
        private readonly ToolStripSplitButton activeIssueDropDown;
        private readonly ToolStripSeparator separator;
        private readonly ToolStripLabel labelMinuteTimer;

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
            public string summary { get; set; }
            public Image icon { get; set; }

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

        private const int ACTIVE_ISSUE_LIST_SIZE = 10;

        private bool paused;
        public int MinutesInProgress { get; private set; }

        private const string NO_ISSUE_ACTIVE = "No Issue Active";
        private const string STOP_WORK = "Stop Work on Active Issue";
        private const string PAUSE_WORK = "Pause Work on Active Issue";
        private const string RESUME_WORK = "Resume Work on Active Issue";
        private const string LOG_WORK = "Log Work on Active Issue";
        private const string COMMENT = "Comment on Active Issue";

        private readonly Timer minuteTimer;

        public JiraActiveIssueManager(ToolStrip container, StatusLabel jiraStatus) {
            this.container = container;
            activeIssueDropDown = new ToolStripSplitButton();
            labelMinuteTimer = new ToolStripLabel();
            buttonStop = new ToolStripButton(Resources.ico_inactiveissue) {Text = STOP_WORK, DisplayStyle = ToolStripItemDisplayStyle.Image};
            buttonStop.Click += (s, e) => deactivateActiveIssue(true);
            buttonPause = new ToolStripButton(Resources.ico_pauseissue) { Text = PAUSE_WORK, DisplayStyle = ToolStripItemDisplayStyle.Image };
            buttonPause.Click += (s, e) => {
                                     paused = !paused;
                                     setTimeSpentString();
                                     savePausedState();
                                     buttonPause.Text = paused ? RESUME_WORK : PAUSE_WORK;
                                     buttonPause.Image = paused ? Resources.ico_activateissue : Resources.ico_pauseissue;
                                 };
            buttonLogWork = new ToolStripButton(Resources.ico_logworkonissue) { Text = LOG_WORK, DisplayStyle = ToolStripItemDisplayStyle.Image };
            buttonLogWork.Click += (s, e) => 
                loadIssueAndRunAction((server, issue) => 
                    container.safeInvoke(new MethodInvoker(() => 
                        new LogWork(container, JiraIssueListModelImpl.Instance, JiraServerFacade.Instance, issue, jiraStatus, this).ShowDialog())), 
                    CurrentActiveIssue);
            buttonComment = new ToolStripButton(Resources.new_comment) { Text = COMMENT, DisplayStyle = ToolStripItemDisplayStyle.Image };
            buttonComment.Click += (s, e) => 
                loadIssueAndRunAction((server, issue) => 
                    container.safeInvoke(new MethodInvoker(() => addComment(issue, jiraStatus))), 
                    CurrentActiveIssue);

            separator = new ToolStripSeparator();

            container.Items.Add(activeIssueDropDown);
            container.Items.Add(buttonStop);
            container.Items.Add(buttonPause);
            container.Items.Add(buttonLogWork);
            container.Items.Add(buttonComment);
            container.Items.Add(labelMinuteTimer);
            container.Items.Add(separator);

            activeIssueDropDown.ButtonClick += (s, e) => {
                                                   if (CurrentActiveIssue == null) return;
                                                   JiraServer server = JiraServerModel.Instance.getServer(new Guid(CurrentActiveIssue.serverGuid));
                                                   if (server == null) return;
                                                   AtlassianPanel.Instance.Jira.findAndOpenIssue(CurrentActiveIssue.key, server, findFinished);
                                               };

            activeIssueDropDown.ToolTipText = "Active Issue";
            setEnabled(false);

            JiraIssueListModelImpl.Instance.IssueChanged += issueChanged;
            minuteTimer = new Timer {Interval = 5000};
            minuteTimer.Tick += (s, e) => updateMinutes();
            minuteTimer.Start();
        }

        private void addComment(JiraIssue issue, StatusLabel jiraStatus) {
            JiraServerFacade facade = JiraServerFacade.Instance;
            NewIssueComment dlg = new NewIssueComment(issue, facade);
            dlg.ShowDialog();
            if (dlg.DialogResult != DialogResult.OK) return;

            Thread addCommentThread =
                PlvsUtils.createThread(delegate {
                                           try {
                                               jiraStatus.setInfo("Adding comment to issue...");
                                               facade.addComment(issue, dlg.CommentBody);
                                               issue = facade.getIssue(issue.Server, issue.Key);
                                               jiraStatus.setInfo("Comment added");
                                               UsageCollector.Instance.bumpJiraIssuesOpen();
                                               container.safeInvoke(new MethodInvoker(() => JiraIssueListModelImpl.Instance.updateIssue(issue)));
                                           } catch (Exception ex) {
                                               jiraStatus.setError("Adding comment failed", ex);
                                           }
                                       });
            addCommentThread.Start();
        }

        private static void findFinished(bool success, string message, Exception e) {
            if (!success) {
                PlvsUtils.showError(message, e);
            }
        }

        private void issueChanged(object sender, IssueChangedEventArgs e) {
            if (CurrentActiveIssue == null) return;
            if (!e.Issue.Key.Equals(CurrentActiveIssue.key) ||
                !e.Issue.Server.GUID.ToString().Equals(CurrentActiveIssue.serverGuid)) return;
            ++generation;
            loadActiveIssueDetails();
        }

        private void savePausedState() {
            ParameterStore store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.ACTIVE_ISSUES);
            store.storeParameter(ACTIVE_ISSUE_IS_PAUSED, paused ? 1 : 0);
        }

        private void setTimeSpentString() {
            int hours = MinutesInProgress / 60;
            labelMinuteTimer.Text = "Time spent: " + (hours > 0 ? hours + "h " : "") + MinutesInProgress % 60 + "m";
            if (paused) {
                labelMinuteTimer.Text = labelMinuteTimer.Text + " (paused)";
            }
        }

        private void updateMinutes() {
            if (CurrentActiveIssue == null || paused) return;

            ++MinutesInProgress;
            storeTimeSpent();
            setTimeSpentString();
        }

        private void storeTimeSpent() {
            ParameterStore store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.ACTIVE_ISSUES);
            store.storeParameter(ACTIVE_ISSUE_TIMER_VALUE, MinutesInProgress);
        }

        private void setEnabled(bool enabled) {
            foreach (ToolStripItem c in new ToolStripItem[]
                                        {
                                            activeIssueDropDown, 
                                            labelMinuteTimer,
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

        private static int generation;
        private const int MAX_SUMMARY_LENGTH = 20;

        public void init() {
            ++generation;
            ParameterStore store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.ACTIVE_ISSUES);
            string activeIssueKey = store.loadParameter(ACTIVE_ISSUE_KEY, null);
            string activeIssueServerGuidStr = store.loadParameter(ACTIVE_ISSUE_SERVER_GUID, null);
            if (activeIssueKey != null && activeIssueServerGuidStr != null) {
                MinutesInProgress = store.loadParameter(ACTIVE_ISSUE_TIMER_VALUE, 0);
                paused = store.loadParameter(ACTIVE_ISSUE_IS_PAUSED, 0) > 0;
                if (paused) {
                    buttonPause.Text = RESUME_WORK;
                    buttonPause.Image = Resources.ico_activateissue;
                }
                setTimeSpentString();
                ICollection<JiraServer> jiraServers = JiraServerModel.Instance.getAllEnabledServers();
                if (jiraServers.Any(server => server.GUID.ToString().Equals(activeIssueServerGuidStr))) {
                    setEnabled(true);
                    CurrentActiveIssue = new ActiveIssue(activeIssueKey, activeIssueServerGuidStr);
                }
            }
            loadPastActiveIssues(store);
            if (CurrentActiveIssue != null) {
                activeIssueDropDown.Text = activeIssueKey;
            }

            Thread t = PlvsUtils.createThread(() => loadIssueInfosWorker(generation));
            t.Start();
        }

        private delegate void OnIssueLoaded(JiraServer server, JiraIssue issue);

        private void loadIssueInfosWorker(int gen) {
            if (CurrentActiveIssue != null) {
                loadActiveIssueDetailsWorker(gen);
            }
        }

        private void loadActiveIssueDetailsWorker(int gen) {
            loadIssueAndRunAction((server, issue) => container.safeInvoke(new MethodInvoker(delegate {
                                                                                                if (gen != generation) return;
                                                                                                setActiveIssueDropdownTextAndImage(server, issue);
                                                                                            })), CurrentActiveIssue);
        }

        private static void loadIssueAndRunAction(OnIssueLoaded loaded, ActiveIssue issue) {
            JiraServer server = JiraServerModel.Instance.getServer(new Guid(issue.serverGuid));
            if (server == null) return;

            JiraIssue jiraIssue = JiraServerFacade.Instance.getIssue(server, issue.key);
            if (jiraIssue != null) {
                loaded(server, jiraIssue);
            }
        }

        private static string getShortIssueSummary(JiraIssue issue) {
            if (issue.Summary == null) {
                return issue.Key;
            }
            if (issue.Summary.Length > MAX_SUMMARY_LENGTH) {
                return issue.Key + ": " + issue.Summary.Substring(0, MAX_SUMMARY_LENGTH) + "...";
            }
            return issue.Key + ": " + issue.Summary;
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
            savePastActiveIssuesAndSetupDropDown();
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

        private void savePastActiveIssuesAndSetupDropDown() {
            activeIssueDropDown.DropDown.Items.Clear();
            ParameterStore store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.ACTIVE_ISSUES);
            store.storeParameter(PAST_ACTIVE_ISSUE_COUNT, pastActiveIssues.Count);
            int i = 0;
            foreach (ActiveIssue issue in pastActiveIssues) {
                store.storeParameter(PAST_ACTIVE_ISSUE_KEY + i, issue.key);
                store.storeParameter(PAST_ACTIVE_ISSUE_SERVER_GUID + i, issue.serverGuid);
                ++i;
            }
            foreach (var issue in pastActiveIssues.Reverse()) {
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
            ++generation;
            List<ActiveIssue> toRemove = pastActiveIssues.Where(i => i.Equals(issue)).ToList();
            foreach (var i in toRemove) {
                pastActiveIssues.Remove(i);
            }
            if (CurrentActiveIssue != null) {
                deactivateActiveIssue(false);
            } else {
                savePastActiveIssuesAndSetupDropDown();
            }
            CurrentActiveIssue = new ActiveIssue(issue.key, issue.serverGuid);
            setEnabled(true);
            activeIssueDropDown.Text = CurrentActiveIssue.key;
            MinutesInProgress = 0;
            storeTimeSpent();
            setTimeSpentString();
            storeActiveIssue();
            activeIssueDropDown.Image = null;
            loadActiveIssueDetails();
            if (ActiveIssueChanged != null) {
                ActiveIssueChanged(this, null);
            }
        }

        private void loadActiveIssueDetails() {
            JiraServer server = JiraServerModel.Instance.getServer(new Guid(CurrentActiveIssue.serverGuid));
            if (server != null) {
                JiraIssue issue = JiraIssueListModelImpl.Instance.getIssue(CurrentActiveIssue.key, server);
                if (issue == null) {
                    Thread t = PlvsUtils.createThread(() => loadActiveIssueDetailsWorker(generation));
                    t.Start();
                } else {
                    setActiveIssueDropdownTextAndImage(server, issue);
                }
            }
        }

        private void setActiveIssueDropdownTextAndImage(JiraServer server, JiraIssue issue) {
            activeIssueDropDown.Text = getShortIssueSummary(issue);
            ImageCache.ImageInfo imageInfo = ImageCache.Instance.getImage(server, issue.IssueTypeIconUrl);
            activeIssueDropDown.Image = imageInfo != null ? imageInfo.Img : null;
        }

        private void storeActiveIssue() {
            ParameterStore store = ParameterStoreManager.Instance.getStoreFor(ParameterStoreManager.StoreType.ACTIVE_ISSUES);
            if (CurrentActiveIssue != null) {
                store.storeParameter(ACTIVE_ISSUE_KEY, CurrentActiveIssue.key);
                store.storeParameter(ACTIVE_ISSUE_SERVER_GUID, CurrentActiveIssue.serverGuid);
            } else {
                store.storeParameter(ACTIVE_ISSUE_KEY, null);
                store.storeParameter(ACTIVE_ISSUE_SERVER_GUID, null);
            }
        }

        private void deactivateActiveIssue(bool notifyListeners) {
            ++generation;
            pastActiveIssues.AddFirst(CurrentActiveIssue);
            while (pastActiveIssues.Count > ACTIVE_ISSUE_LIST_SIZE) {
                pastActiveIssues.RemoveLast();
            }
            CurrentActiveIssue = null;
            storeActiveIssue();
            activeIssueDropDown.Image = null;
            savePastActiveIssuesAndSetupDropDown();
            setEnabled(false);
            setNoIssueActiveInDropDown();
            if (notifyListeners && ActiveIssueChanged != null) {
                ActiveIssueChanged(this, null);
            }
        }

        public void resetTimeSpent() {
            MinutesInProgress = 0;
            storeTimeSpent();
            setTimeSpentString();
        }
    }
}

