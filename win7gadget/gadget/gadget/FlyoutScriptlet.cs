// FlyoutScriptlet.cs
//

using System;
using System.DHTML;
using System.Gadgets;
using ScriptFX;
using ScriptFX.UI;

namespace gadget {

    public class FlyoutScriptlet {

        private Label labelShock;

        static FlyoutScriptlet() {
            if (Document.Body.ID == "gadgetFlyout") {
                ScriptHost.Run(typeof(FlyoutScriptlet), null);
            }
        }

        private FlyoutScriptlet() {
            DOMElement body = Document.Body;
            body.Style.Width = "300";
            body.Style.Height = "300";
            body.Style.BackgroundColor = "#ff0000";

            labelShock = new Label(Document.GetElementById("shock"));
            labelShock.Text = "shake a lot";
        }

        public static void Main(Dictionary arguments) {
            FlyoutScriptlet scriptlet = new FlyoutScriptlet();
        }
    }
}
