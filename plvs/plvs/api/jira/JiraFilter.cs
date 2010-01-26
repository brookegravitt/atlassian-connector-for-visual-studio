namespace Atlassian.plvs.api.jira {
    public interface JiraFilter {
        string getFilterQueryString();
        string getSortBy();
    }
}
