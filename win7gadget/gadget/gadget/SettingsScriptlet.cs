// SettingsScriptlet.cs
//

using System;
using System.DHTML;
using System.Gadgets;
using ScriptFX;
using ScriptFX.UI;

namespace gadget {

    public class SettingsScriptlet {

        private readonly DOMElement dropDownProjects;
        private readonly Button buttonTestConnection;
        private readonly Button buttonGetProjects;
        private readonly TextBox txtUrl;
        private readonly TextBox txtLogin;
        private readonly TextBox txtPassword;
        private readonly Label labelInfo;

        static SettingsScriptlet() {
            if (Document.Body.ID == "gadgetSettings") {
                ScriptHost.Run(typeof(SettingsScriptlet), null);
            }
        }

        private SettingsScriptlet() {
            DOMElement body = Document.Body;
            body.Style.Width = "300";
            body.Style.Height = "300";

            dropDownProjects = Document.GetElementById("projects");
            dropDownProjects.Disabled = true;

            txtUrl = new TextBox(Document.GetElementById("url"));
            txtUrl.TextChanged += txtUrl_TextChanged;
            txtLogin = new TextBox(Document.GetElementById("login"));
            txtPassword = new TextBox(Document.GetElementById("password"));

            buttonTestConnection = new Button(Document.GetElementById("testConnection"));
            buttonTestConnection.Click += buttonTestConnection_Click;

            buttonGetProjects = new Button(Document.GetElementById("retrieveProjects"));
            buttonGetProjects.Click += buttonGetProjects_Click;

            updateButtonStates();

            labelInfo = new Label(Document.GetElementById("info"));
        }

        private void buttonGetProjects_Click(object sender, EventArgs e) {
            labelInfo.DOMElement.Style.Color = "#000000";
            labelInfo.Text = "Retrieving Projects...";
            rpc.login(txtUrl.Text, txtLogin.Text, txtPassword.Text, gotTokenForGetProjects, connectionError);
        }

        private void gotTokenForGetProjects(string token) {
            rpc.getprojects(txtUrl.Text, token, gotProjects, connectionError);
        }

        private void gotProjects(object result) {
            labelInfo.DOMElement.Style.Color = "#000000";
            labelInfo.Text = "Retrieved Projects";
        }

        private void txtUrl_TextChanged(object sender, EventArgs e) {
            updateButtonStates();
        }

        private void updateButtonStates() {
            bool disabled = txtUrl.Text.Length == 0;
            buttonTestConnection.DOMElement.Disabled = disabled;
            buttonGetProjects.DOMElement.Disabled = disabled;
        }

        private void buttonTestConnection_Click(object sender, EventArgs e) {
            labelInfo.DOMElement.Style.Color = "#000000";
            labelInfo.Text = "Testing Server Connection...";
            rpc.login(txtUrl.Text, txtLogin.Text, txtPassword.Text, gotLoginToken, connectionError);
        }

        private void connectionError(string error) {
            labelInfo.DOMElement.Style.Color = "#ff0000";
            labelInfo.Text = "Connection error: " + error;
        }

        private void gotLoginToken(string token) {
            labelInfo.DOMElement.Style.Color = "#000000";
            labelInfo.Text = "Connection successful";
        }

        public static void Main(Dictionary arguments) {
            SettingsScriptlet scriptlet = new SettingsScriptlet();
        }
    }
}
