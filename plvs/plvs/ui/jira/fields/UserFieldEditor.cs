using System.Collections.Generic;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;

namespace Atlassian.plvs.ui.jira.fields {
    public class UserFieldEditor : JiraFieldEditor {

        private readonly JiraUserPicker picker;

        public UserFieldEditor(JiraServer server, JiraField field, string userName, FieldValidListener validListener) 
            : base(field, validListener) {

            picker = new JiraUserPicker();
            picker.init(server, userName);
        }

        public override Control Widget {
            get { return picker; }
        }

        public override int VerticalSkip {
            get { return picker.Height; }
        }

        public override void resizeToWidth(int width) {}

        public override List<string> getValues() {
            return new List<string> { picker.Value };
        }
    }
}