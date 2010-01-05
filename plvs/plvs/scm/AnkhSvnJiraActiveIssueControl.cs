using System.Windows.Forms;

namespace Atlassian.plvs.scm {
    public partial class AnkhSvnJiraActiveIssueControl : UserControl {

        private const string INTEGRATION = "Atlassian Connector Integration Enabled";
        private const string NO_INTEGRATION = "Atlassian Connector Integration Disabled";

        public AnkhSvnJiraActiveIssueControl(bool enabled) {
            InitializeComponent();

            labelJira.Text = enabled ? INTEGRATION : NO_INTEGRATION;
        }
    }
}
