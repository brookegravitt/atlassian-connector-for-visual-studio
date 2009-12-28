using System;
using System.Collections.Generic;
using Atlassian.plvs.api;

namespace Atlassian.plvs.models {
    public interface JiraIssueListModel {

        ICollection<JiraIssue> Issues { get; }

        event EventHandler<EventArgs> ModelChanged;
        event EventHandler<IssueChangedEventArgs> IssueChanged;

        void removeAllListeners();

        void clear(bool notify);

        void addIssues(ICollection<JiraIssue> newIssues);

        void updateIssue(JiraIssue issue);
    }

    public class IssueChangedEventArgs : EventArgs {
        public JiraIssue Issue { get; private set; }
        public IssueChangedEventArgs(JiraIssue issue) {
            Issue = issue;
        }
    }
}