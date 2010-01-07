using System.Collections.Generic;
using System.Windows.Forms;
using Atlassian.plvs.api;

namespace Atlassian.plvs.ui.fields {
    public class TextLineFieldEditor : JiraFieldEditor {
        private readonly Control editor = new TextBox();

        public TextLineFieldEditor(JiraField field, string value, FieldValidListener validListener)
            : base(field, validListener) {
            if (value != null) {
                editor.Text = value;
            }
        }

        public override Control Widget {
            get { return editor; }
        }

        public override int VerticalSkip {
            get { return editor.Height; }
        }

        public override void resizeToWidth(int width) {
            editor.Width = width;
        }

        public override List<string> getValues() {
            return new List<string> { editor.Text };
        }
    }
}
