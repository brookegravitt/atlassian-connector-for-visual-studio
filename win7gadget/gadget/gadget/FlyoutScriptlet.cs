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
            body.Style.Width = "400";
            body.Style.Height = "600";
            body.Style.BackgroundColor = "#eeeeee";

            buttonClose = new Button(Document.GetElementById("buttonClose"));
            buttonClose.Click += buttonClose_Click;
        }

        private static void buttonClose_Click(object sender, EventArgs e) {
            Gadget.Flyout.Show = false;
        }

        public static void setIssueDetailsText(string text) {
            DOMElement element = Gadget.Flyout.Document.GetElementById("issueDetails");
            element.InnerHTML = text;
        }

        public static void setIssueKeyAndType(string text) {
            DOMElement element = Gadget.Flyout.Document.GetElementById("issueKeyLink");
            element.InnerHTML = text;
        }

        public static void Main(Dictionary arguments) {
            FlyoutScriptlet scriptlet = new FlyoutScriptlet();
        }
    }
}
