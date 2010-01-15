using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Ankh.ExtensionPoints.IssueTracker;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.util;
using Atlassian.plvs.windows;

namespace Atlassian.plvs.scm {
    [Guid("F6D2F9E0-0B03-42f2-A4BF-A3E4E0019685")]
    public class AnkhSvnJiraConnector : IssueRepositoryConnector {

        public const string ANKH_CONNECTOR_NAME = "Atlassian JIRA Connector";
        
        public override IssueRepository Create(IssueRepositorySettings settings) {
            return new JiraIssueRepository(settings.ConnectorName, settings.CustomProperties);
        }

        public override string Name {
            get { return ANKH_CONNECTOR_NAME; }
        }

        public override IssueRepositoryConfigurationPage ConfigurationPage {
            get { return new JiraRepositoryConfigurationPage(); }
        }

        private sealed class JiraIssueRepository : IssueRepository, IWin32Window {

            private readonly AnkhSvnJiraActiveIssueControl ctrl;

            public JiraIssueRepository(string connectorName, IDictionary<string, object> properties) : base(connectorName) {
                bool enabled = false;
                if (properties != null && properties.ContainsKey(Constants.INTEGRATE_WITH_ANKH)) {
                    string val = properties[Constants.INTEGRATE_WITH_ANKH].ToString();
                    enabled = val.Equals("1");
                    customProperties[Constants.INTEGRATE_WITH_ANKH] = val;
                }

                ctrl = new AnkhSvnJiraActiveIssueControl(enabled);   
            }

            public override Uri RepositoryUri { get { return new Uri("http://www.atlassian.com/software/jira"); } }

            public override string Label { get { return "JIRA"; } }

            public override string RepositoryId { get { return "JIRA"; } }

            public override string ConnectorName { get { return ANKH_CONNECTOR_NAME; } }

            readonly Dictionary<string, object> customProperties = new Dictionary<string, object>();

            public override IDictionary<string, object> CustomProperties {
                get { return customProperties; }
            }

            public IntPtr Handle {
                get { return ctrl.Handle; }
            }

            public override void PreCommit(PreCommitArgs args) {
                JiraIssue issue = AtlassianPanel.Instance.Jira.SelectedIssue;
                if (issue != null && (String.IsNullOrEmpty(args.CommitMessage))) {
                    args.CommitMessage = issue.Key + " - " + issue.Summary;
                }
            }
        }

        private class JiraRepositoryConfigurationPage : IssueRepositoryConfigurationPage, IWin32Window {
            private readonly AnkhSvnJiraConnectorControl ctrl = new AnkhSvnJiraConnectorControl();

            public JiraRepositoryConfigurationPage() {
                ctrl.PageChanged += ctrl_PageChanged;
            }

            void ctrl_PageChanged(object sender, ConfigPageEventArgs e) {
                base.ConfigurationPageChanged(e);
            }

            public override IssueRepositorySettings Settings {
                get { return ctrl.Settings; }
                set { ctrl.Settings = value; }
            }

            public IntPtr Handle {
                get { return ctrl.Handle; }
            }
        }
    }
}