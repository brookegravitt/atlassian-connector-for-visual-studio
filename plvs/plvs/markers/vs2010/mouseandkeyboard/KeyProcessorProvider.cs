using System.ComponentModel.Composition;
using System.Windows.Input;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Atlassian.plvs.markers.vs2010.mouseandkeyboard {
    [Export(typeof(IKeyProcessorProvider))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    [Name("JiraIssueKeyboardProcessorProvider")]
    internal class KeyProcessorProvider : IKeyProcessorProvider {

        [Import]
        public IViewTagAggregatorFactoryService TagAggregatorFactoryService;

        public KeyProcessor GetAssociatedProcessor(IWpfTextView wpfTextView) {
            return new MyKeyProcessor(this, wpfTextView);
        }
    }

    internal class MyKeyProcessor : KeyProcessor {
        private readonly KeyProcessorProvider provider;
        private readonly IWpfTextView view;
        public const string CONTROL_KEY_DOWN = "Plvs_ControlKeyDown";

        public MyKeyProcessor(KeyProcessorProvider provider, IWpfTextView view) {
            this.provider = provider;
            this.view = view;
        }

        public static bool isControlDown(IWpfTextView textView) {
            bool prop;
            return textView.Properties.TryGetProperty(CONTROL_KEY_DOWN, out prop) && prop;
        }

        public override void KeyDown(KeyEventArgs args) {
            if (isControlDown(view) && args.IsRepeat) return;

            if (args.Key == Key.LeftCtrl || args.Key == Key.RightCtrl) {
//                DebugMon.Instance().addText("KeyDown(): " + args.Key);
                setControlDown(view, provider.TagAggregatorFactoryService, true);
                args.Handled = true;
            }
        }

        public override void KeyUp(KeyEventArgs args) {
            if (args.Key == Key.LeftCtrl || args.Key == Key.RightCtrl) {
//                DebugMon.Instance().addText("KeyUp(): " + args.Key);
                setControlDown(view, provider.TagAggregatorFactoryService, false);
                args.Handled = true;
            }
        }

        internal static void setControlDown(IWpfTextView view, IViewTagAggregatorFactoryService tagAggregatorFactory, bool down) {
//            DebugMon.Instance().addText("setControlDown(): " + down);

            if (view.Properties.ContainsProperty(CONTROL_KEY_DOWN)) {
                view.Properties.RemoveProperty(CONTROL_KEY_DOWN);
            }
            view.Properties.AddProperty(CONTROL_KEY_DOWN, down);

            bool overIssue = MouseProcessor.getIssueTagUnderCursor(view, tagAggregatorFactory) != null;

            if (overIssue) {
                Mouse.OverrideCursor = down ? Cursors.Hand : null;
            }
            Mouse.UpdateCursor();
        }
    }
}
