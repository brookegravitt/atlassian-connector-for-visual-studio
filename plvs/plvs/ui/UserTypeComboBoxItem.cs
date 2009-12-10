using Atlassian.plvs.models;
using Atlassian.plvs.util;

namespace Atlassian.plvs.ui {
    class UserTypeComboBoxItem {
        public JiraCustomFilter.UserType Type { get; private set; }

        public UserTypeComboBoxItem(JiraCustomFilter.UserType type) {
            this.Type = type;
        }

        public override string ToString() {
            return Type.GetStringValue();
        }
    }
}
