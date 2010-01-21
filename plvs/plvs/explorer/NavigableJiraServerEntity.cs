using Atlassian.plvs.api.jira;
using Atlassian.plvs.ui;

namespace Atlassian.plvs.explorer {
    interface NavigableJiraServerEntity {
        string getUrl(string authString);
        void onClick(JiraServerFacade facade, StatusLabel status);
    }
}
