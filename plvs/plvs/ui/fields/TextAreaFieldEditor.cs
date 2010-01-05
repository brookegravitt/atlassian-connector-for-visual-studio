using System.Windows.Forms;

namespace Atlassian.plvs.ui.fields {
    public class TextAreaFieldEditor : JiraFieldEditor {
        private readonly Control editor = new TextBox
                                          {
                                              Multiline = true,
                                              WordWrap = true,
                                              Height = MULTI_LINE_EDITOR_HEIGHT
                                          };

        public TextAreaFieldEditor(string value) {
            if (value != null) {
                editor.Text = value;
            }
        }

        public override Control Widget {
            get { return editor; }
        }

        public override int VerticalSkip {
            get { return 80; }
        }

        public override void resizeToWidth(int width) {
            editor.Width = width;
        }
    }
}