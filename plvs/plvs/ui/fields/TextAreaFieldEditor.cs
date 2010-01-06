using System.Windows.Forms;

namespace Atlassian.plvs.ui.fields {
    public class TextAreaFieldEditor : JiraFieldEditor {
        private readonly Control editor = new TextBox
                                          {
                                              Multiline = true,
                                              ScrollBars = ScrollBars.Both,
                                              AcceptsReturn = true,
                                              Height = MULTI_LINE_EDITOR_HEIGHT
                                          };

        public TextAreaFieldEditor(string value, FieldValidListener validListener) : base(validListener) {
            if (value == null) return;

            string fixedValue = value.Replace("\r\n", "\n").Replace("\n", "\r\n");
            editor.Text = fixedValue;
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