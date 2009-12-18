using System.Collections.Generic;
using Atlassian.plvs.api;

namespace Atlassian.plvs.models {
    class JiraIssueListModelImpl: JiraIssueListModel {

        #region member fields
        
        private static readonly JiraIssueListModel INSTANCE = new JiraIssueListModelImpl();

        private readonly List<JiraIssueListModelListener> listeners = new List<JiraIssueListModelListener>();
        private readonly List<JiraIssue> issues = new List<JiraIssue>();

        #endregion

        public ICollection<JiraIssue> Issues {
            get { return issues; }
        }

        public static JiraIssueListModel Instance {
            get { return INSTANCE; }
        }

        public void addListener(JiraIssueListModelListener l) {
            listeners.Add(l);
        }

        public void removeListener(JiraIssueListModelListener l) {
            listeners.Remove(l);
        }

        public void removeAllListeners() {
            listeners.Clear();
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

        #region private parts

        private void notifyListenersOfIssueChange(JiraIssue issue) {
            foreach (var l in listeners) {
                l.issueChanged(issue);
            }
        }

        private void notifyListenersOfModelChange() {
            foreach (var l in listeners) {
                l.modelChanged();
            }
        }

        private JiraIssueListModelImpl() { }

        #endregion
    }
}
