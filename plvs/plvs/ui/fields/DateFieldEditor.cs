using System;
using System.Windows.Forms;

namespace Atlassian.plvs.ui.fields {
    internal class DateFieldEditor : JiraFieldEditor {
        private readonly DateTimePicker picker = new DateTimePicker
                                                 {
                                                     ShowCheckBox = true
                                                 };

        public DateFieldEditor(DateTime? date) {
            if (date != null) {
                picker.Value = (DateTime) date;
                picker.Checked = true;
            } else {
                picker.Checked = false;
            }
        }

        public override Control Widget {
            get { return picker; }
        }

        public override int VerticalSkip {
            get { return picker.Height; }
        }

        public override void resizeToWidth(int width) {}
    }
}