using System.Collections.Generic;
using Atlassian.plvs.api;

namespace Atlassian.plvs.models {
    public interface JiraIssueListModel {

        ICollection<JiraIssue> Issues { get; }

        void addListener(JiraIssueListModelListener l);

        void removeListener(JiraIssueListModelListener l);

        void removeAllListeners();

        void clear(bool notify);

        void addIssues(ICollection<JiraIssue> newIssues);

        void updateIssue(JiraIssue issue);
    }
}