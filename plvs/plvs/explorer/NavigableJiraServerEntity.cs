using Atlassian.plvs.ui;

namespace Atlassian.plvs.explorer {
    interface NavigableJiraServerEntity {
        string getUrl();
        void onClick(StatusLabel status);
    }
}
