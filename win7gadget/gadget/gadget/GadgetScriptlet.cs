// GadgetScriptlet.cs
//

using System;
using System.DHTML;
using System.Gadgets;
using ScriptFX;
using ScriptFX.UI;

namespace gadget {

    public class GadgetScriptlet {

        private const string FLYOUT_SHOW = "Show Details";
        private const string FLYOUT_HIDE = "Hide Details";

        private bool flyoutVisible;

        private readonly Button flyoutButton;
        private readonly Label settingsUpdateLabel;

        private int settingsCommitCount;

        static GadgetScriptlet() {
            if (Document.Body.ID == "gadget") {
                ScriptHost.Run(typeof(GadgetScriptlet), null);
            }
        }

        private GadgetScriptlet() {
            Gadget.OnDock = OnDock;
            Gadget.OnUndock = OnUndock;

            Gadget.Flyout.File = "Flyout.htm";
            Gadget.Flyout.OnShow = OnFlyoutShow;
            Gadget.Flyout.OnHide = OnFlyoutHide;

            Gadget.SettingsUI = "Settings.htm";
            Gadget.OnSettingsClosed = SettingsClosed;

            UpdateDockedState();

            flyoutButton = new Button(Document.GetElementById("flyoutButton"));
            flyoutButton.DOMElement.InnerText = FLYOUT_SHOW;
            flyoutButton.Click += flyoutButton_Click;

            settingsUpdateLabel = new Label(Document.GetElementById("settingsText"));
        }

        private void flyoutButton_Click(object sender, EventArgs e) {
            flyoutButton.DOMElement.InnerText = flyoutVisible ? FLYOUT_HIDE : FLYOUT_SHOW;
            Gadget.Flyout.Show = !flyoutVisible;
        }

        public static void Main(Dictionary arguments) {
            GadgetScriptlet scriptlet = new GadgetScriptlet();
        }

        private static void OnDock() {
            UpdateDockedState();
        }

        private static void OnUndock() {
            UpdateDockedState();
        }

        private void OnFlyoutHide() {
            // TODO: Use Gadget.Flyout.Document to get to the HTML document within
            //       the Flyout page

            flyoutVisible = false;
            flyoutButton.DOMElement.InnerText = FLYOUT_SHOW;
        }

        private void OnFlyoutShow() {
            flyoutVisible = true;
            flyoutButton.DOMElement.InnerText = FLYOUT_HIDE;

            // TODO: Use Gadget.Flyout.Document to get to the HTML document within
            //       the Flyout page
        }

        private void SettingsClosed(GadgetSettingsEvent e) {
            if (e.CloseAction == GadgetSettingsCloseAction.Cancel) return;

            ++settingsCommitCount;

            settingsUpdateLabel.Text = string.Format("Settings updated {0} times", settingsCommitCount);
        }

        private static void UpdateDockedState() {
            DOMElement body = Document.Body;

            if (Gadget.Docked) {
                Element.RemoveCSSClass(body, "undocked");
                Element.AddCSSClass(body, "docked");
                body.Style.Width = "100";
                body.Style.Height = "200";
            } else {
                Element.AddCSSClass(body, "undocked");
                Element.RemoveCSSClass(body, "docked");
                body.Style.Width = "200";
                body.Style.Height = "200";
            }
        }
    }
}
