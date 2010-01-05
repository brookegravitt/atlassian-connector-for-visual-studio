using System.Windows.Forms;

namespace Atlassian.plvs.ui.fields {
    public abstract class JiraFieldEditor {
        public static int SINGLE_LINE_EDITOR_HEIGHT = 16;
        public static int MULTI_LINE_EDITOR_HEIGHT = 80;
        public abstract Control Widget { get; }
        public abstract int VerticalSkip { get; }
        public abstract void resizeToWidth(int width);
    }
}
