using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.util.jira;

namespace Atlassian.plvs.ui.jira.fields {
    internal class DateFieldEditorProvider : FixedHeightFieldEditorProvider {
        private readonly DateTimePicker picker = new DateTimePicker
                                                 {
                                                     ShowCheckBox = true
                                                 };

        public DateFieldEditorProvider(JiraField field, DateTime? date, FieldValidListener validListener)
            : base(field, validListener) {
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

        public override List<string> getValues() {
            return picker.Checked 
                ? new List<string> { JiraIssueUtils.getShortDateStringFromDateTime(picker.Value)} 
                : new List<string>();
        }
    }
}