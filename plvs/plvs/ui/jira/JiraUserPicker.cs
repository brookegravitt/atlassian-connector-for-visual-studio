using System.Collections.Generic;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.ui.jira {
    public partial class JiraUserPicker : UserControl {
        public JiraUserPicker() {
            InitializeComponent();
        }

        public void init(JiraServer server, string currentUserId) {
            ICollection<JiraUser> users = JiraServerCache.Instance.getUsers(server).getAllUsers();

            JiraUser selected = null;

            foreach (JiraUser user in users) {
                if (currentUserId != null && currentUserId.Equals(user.Id)) {
                    selected = user;
                }
                comboUsers.Items.Add(user);
            }

            if (selected != null) {
                comboUsers.SelectedItem = selected;
            }
        }

        public string Value { 
            get {
                if (!(comboUsers.SelectedItem is JiraUser)) {
                    return comboUsers.Text;
                }
                return ((JiraUser) comboUsers.SelectedItem).Id;        
            }
        }
    }
}
