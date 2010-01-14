using System;
using System.Collections.Generic;
using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.models.jira {
    class JiraIssueListModelImpl: JiraIssueListModel {

        #region member fields
        
        private static readonly JiraIssueListModel INSTANCE = new JiraIssueListModelImpl();

        private readonly List<JiraIssue> issues = new List<JiraIssue>();

        #endregion

        public ICollection<JiraIssue> Issues {
            get { return issues; }
        }

        public static JiraIssueListModel Instance {
            get { return INSTANCE; }
        }

        public void removeAllListeners() {
            ModelChanged = null;
            IssueChanged = null;
        }

        public void clear(bool notify) {
            lock (issues) {
                issues.Clear();
                if (notify) {
                    notifyListenersOfModelChange();
                }
            }
        }

        public void addIssues(ICollection<JiraIssue> newIssues) {
            lock (issues) {
                foreach (var issue in newIssues) {
                    issues.Add(issue);
                }
                notifyListenersOfModelChange();
            }
        }

        public void updateIssue(JiraIssue issue) {
            lock (issues) {
                foreach (var i in Issues) {
                    if (!i.Id.Equals(issue.Id)) continue;
                    if (!i.Server.GUID.Equals(issue.Server.GUID)) continue;
                    if (!i.Equals(issue)) {
                        issues.Remove(i);
                        issues.Add(issue);
                        notifyListenersOfIssueChange(issue);
                    }
                    break;
                }
            }
        }

        public event EventHandler<EventArgs> ModelChanged;
        public event EventHandler<IssueChangedEventArgs> IssueChanged;

        #region private parts

        private void notifyListenersOfIssueChange(JiraIssue issue) {
            if (IssueChanged != null) {
                IssueChanged(this, new IssueChangedEventArgs(issue));
            }
        }

        private void notifyListenersOfModelChange() {
            if (ModelChanged != null) {
                ModelChanged(this, new EventArgs());
            }
        }

        private JiraIssueListModelImpl() { }

        #endregion
    }
}