// FlyoutScriptlet.cs
//

using System;
using System.DHTML;
using System.Gadgets;
using ScriptFX;
using ScriptFX.UI;

namespace gadget {

    public class FlyoutScriptlet {

        private readonly Button buttonClose;

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

            buttonClose = new Button(Document.GetElementById("buttonClose"));
            buttonClose.Click += buttonClose_Click;
        }

        private static void buttonClose_Click(object sender, EventArgs e) {
            Gadget.Flyout.Show = false;
        }

        public static void setText(string text) {
            DOMElement element = Gadget.Flyout.Document.GetElementById("issueDetails");
            element.InnerHTML = text;
        }

        public static void Main(Dictionary arguments) {
            FlyoutScriptlet scriptlet = new FlyoutScriptlet();
        }
    }
}
