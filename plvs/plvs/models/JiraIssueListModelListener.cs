using Atlassian.plvs.api;

namespace Atlassian.plvs.models {
    public interface JiraIssueListModelListener {
        void modelChanged();
        void issueChanged(JiraIssue issue);
    }
}