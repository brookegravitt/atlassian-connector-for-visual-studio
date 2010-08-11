// SettingsScriptlet.cs
//

using System;
using System.DHTML;
using System.Gadgets;
using ScriptFX;
using ScriptFX.UI;

namespace gadget {

    public class SettingsScriptlet {

        static SettingsScriptlet() {
            if (Document.Body.ID == "gadgetSettings") {
                ScriptHost.Run(typeof(SettingsScriptlet), null);
            }
        }

        private SettingsScriptlet() {
            DOMElement body = Document.Body;
            body.Style.Width = "300";
            body.Style.Height = "300";
//            body.Style.BackgroundColor = "#0000ff";
        }

        public static void Main(Dictionary arguments) {
            SettingsScriptlet scriptlet = new SettingsScriptlet();
        }
    }
}
