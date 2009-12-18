using System.Collections.Generic;
using Atlassian.plvs.api;

namespace Atlassian.plvs.models {
    class JiraIssueListSearchingModel : JiraIssueListModel, JiraIssueListModelListener {
        private JiraIssueListModel model;
        private string query;
        public string Query { get { return query; } set { setQuery(value); } }
        private readonly List<JiraIssueListModelListener> listeners = new List<JiraIssueListModelListener>();

        public JiraIssueListSearchingModel(JiraIssueListModel model) {
            reinit(model);
        }

        public void reinit(JiraIssueListModel m) {
            model = m;
            shutdown();
            model.addListener(this);
        }

        public void shutdown() {
            model.removeListener(this);
            removeAllListeners();
        }

        private void setQuery(string q) {
            if (q.Equals(Query)) {
                return;
            }
            query = q;
            foreach (var listener in listeners) {
                listener.modelChanged();
            }
        }

        public ICollection<JiraIssue> Issues {
            get { return filter(model.Issues); }
        }

        private ICollection<JiraIssue> filter(ICollection<JiraIssue> issues) {
            if (string.IsNullOrEmpty(Query)) {
                return issues;
            }
            List<JiraIssue> list = new List<JiraIssue>();
            foreach (var issue in issues) {
                if (matches(issue)) {
                    list.Add(issue);
                }
            }
            return list;
        }

        private bool matches(JiraIssue issue) {
            return issue.Key.ToLower().Contains(Query.ToLower()) 
                || issue.Summary.ToLower().Contains(Query.ToLower());
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
            model.clear(notify);
        }

        public void addIssues(ICollection<JiraIssue> newIssues) {
            model.addIssues(newIssues);
        }

        public void updateIssue(JiraIssue issue) {
            model.updateIssue(issue);
        }

        public void modelChanged() {
            foreach (var listener in listeners) {
                listener.modelChanged();
            }
        }

        public void issueChanged(JiraIssue issue) {
            if (!string.IsNullOrEmpty(Query) && !matches(issue)) {
                return;
            }
            foreach (var listener in listeners) {
                listener.issueChanged(issue);
            }
        }
    }
}
