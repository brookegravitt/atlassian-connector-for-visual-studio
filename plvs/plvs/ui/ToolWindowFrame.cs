using System.Windows.Forms;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Atlassian.plvs.ui {
    public class ToolWindowFrame : UserControl {

        public ToolWindowPane WindowFrame { get; set; }

        public bool FrameVisible {
            get { return WindowFrame != null && ((IVsWindowFrame)WindowFrame.Frame).IsVisible() == VSConstants.S_OK; }
            set {
                if (WindowFrame == null) {
                    return;
                }
                if (value) {
                    ((IVsWindowFrame)WindowFrame.Frame).Show();
                } else {
                    ((IVsWindowFrame)WindowFrame.Frame).Hide();
                }
            }
        }
    }
}
