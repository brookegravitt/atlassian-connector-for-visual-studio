using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.ui;

namespace Atlassian.plvs.explorer.treeNodes {
    class VersionsNode : AbstractNavigableTreeNodeWithServer {
        private readonly Control parent;
        private readonly JiraProject project;

        private bool versionsLoaded;
        public VersionsNode(Control parent, JiraServer server, JiraProject project) : base(server, "Versions", 0) {
            this.parent = parent;
            this.project = project;
        }

        public override string getUrl(string authString) {
            return Server.Url + "/browse/" + project.Key
                + "?" + authString 
                + "#selectedTab=com.atlassian.jira.plugin.system.project%3Aversions-panel"; 
        }

        public override void onClick(JiraServerFacade facade, StatusLabel status) {
            if (versionsLoaded) return;
            versionsLoaded = true;
            Thread t = new Thread(() => loadVersions(facade, status));
            t.Start();
        }

        private void loadVersions(JiraServerFacade facade, StatusLabel status) {
            try {
                List<JiraNamedEntity> versions = facade.getVersions(Server, project);
                parent.Invoke(new MethodInvoker(()=> populateVersions(versions)));
            } catch (Exception e) {
                status.setError("Unable to retrieve versions list", e);
            }
        }

        private void populateVersions(List<JiraNamedEntity> versions) {
            versions.Reverse();
            foreach (JiraNamedEntity version in versions) {
                Nodes.Add(new VersionNode(Server, project, version));
            }
        }
    }
}
