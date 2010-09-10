using System;
using System.DHTML;
using System.Gadgets;
using ScriptFX.UI;

namespace gadget {

    public class SettingsScriptlet {

        private readonly DOMElement dropDownProjects;
        private readonly Button buttonTestConnection;
        private readonly Button buttonGetProjects;
        private static TextBox txtUrl;
        private static TextBox txtLogin;
        private static TextBox txtPassword;
        private static Label labelInfo;
        
        private const string FILTERS_SELECT = "filters";
        private const string PROJECTS_SELECT = "projects";

        public const string SETTING_URL = "url";
        public const string SETTING_LOGIN = "login";
        public const string SETTING_PASSWORD = "password";

        public const string SETTING_FILTERNAME = "filterName";
        public const string SETTING_FILTERVALUE = "filterValue";

        public const string SETTING_PROJECTNAME = "projectName";
        public const string SETTING_PROJECTKEY = "projectKey";

        private static bool haveProject;

        static SettingsScriptlet() {
            if (Document.Body.ID == "gadgetSettings") {
                ScriptHost.Run(typeof(SettingsScriptlet), null);
            }
        }

        private SettingsScriptlet() {

            Gadget.OnSettingsClosing = SettingsClosing;

            DOMElement body = Document.Body;
            body.Style.Width = "350";
            body.Style.Height = "400";

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

            txtUrl.Text = Gadget.Settings.ReadString(SETTING_URL);
            txtLogin.Text = Gadget.Settings.ReadString(SETTING_LOGIN);
            txtPassword.Text = Gadget.Settings.ReadString(SETTING_PASSWORD);

            string val = Gadget.Settings.ReadString(SETTING_FILTERVALUE);
            if (!string.IsNullOrEmpty(val)) {
                optionreader.setselectedval(FILTERS_SELECT, val);
            }

            string projectKey = Gadget.Settings.ReadString(SETTING_PROJECTKEY);
            string projectName = Gadget.Settings.ReadString(SETTING_PROJECTNAME);
            if (!(string.IsNullOrEmpty(projectKey) || string.IsNullOrEmpty(projectName))) {
                optionreader.clearoptions(PROJECTS_SELECT);
                optionreader.addoption(PROJECTS_SELECT, projectKey, projectName);
                haveProject = true;
            }
        }

        private static void SettingsClosing(GadgetSettingsEvent e) {
            if (e.CloseAction == GadgetSettingsCloseAction.Cancel) return;
            e.Cancel = !saveSettings();
        }

        private static void buttonGetProjects_Click(object sender, EventArgs e) {
            labelInfo.DOMElement.Style.Color = "#000000";
            labelInfo.Text = "Retrieving Projects...";
            rpc.login(txtUrl.Text, txtLogin.Text, txtPassword.Text, gotTokenForGetProjects, connectionError);
        }

        private static void gotTokenForGetProjects(string token) {
            rpc.getprojects(txtUrl.Text, token, gotProjects, connectionError);
        }

        private static void gotProjects(object result) {
            labelInfo.DOMElement.Style.Color = "#000000";
            labelInfo.Text = "Retrieved Projects";

            string curKey = null;

            if (haveProject) {
                curKey = optionreader.getselectedval(PROJECTS_SELECT);
            }

            optionreader.clearoptions(PROJECTS_SELECT);

            Dictionary d = (Dictionary)result;
            foreach (DictionaryEntry entry in d) {
                int nr;
                try {
                    nr = (int)int.Parse(entry.Key);
                } catch (Exception) {
                    continue;
                }
                if (Number.IsNaN(nr)) continue;

                addProject(entry.Value);
            }

            Document.GetElementById(PROJECTS_SELECT).Disabled = false;

            if (haveProject) {
                optionreader.setselectedval(PROJECTS_SELECT, curKey);
            }
            haveProject = true;
        }

        private static void addProject(object projectObject) {
            Dictionary d = (Dictionary)projectObject;
            string key = null;
            string name = null;
            foreach (DictionaryEntry entry in d) {
                if (entry.Key == "key") key = entry.Value.ToString();
                if (entry.Key == "name") name = entry.Value.ToString();
            }
            if (key == null || name == null) return;
            optionreader.addoption(PROJECTS_SELECT, key, name);
        }

        private void txtUrl_TextChanged(object sender, EventArgs e) {
            updateButtonStates();
        }

        private void updateButtonStates() {
            string url = txtUrl.Text;
            bool disabled = !isValidUrl(url);
            buttonTestConnection.DOMElement.Disabled = disabled;
            buttonGetProjects.DOMElement.Disabled = disabled;
        }

        private static bool isValidUrl(string url) {
            return !(url.Length == 0 || !(url.StartsWith("https://") || url.StartsWith("http://")) || url.EndsWith("://"));
        }

        private static void buttonTestConnection_Click(object sender, EventArgs e) {
            labelInfo.DOMElement.Style.Color = "#000000";
            labelInfo.Text = "Testing Server Connection...";
            rpc.login(txtUrl.Text, txtLogin.Text, txtPassword.Text, gotLoginToken, connectionError);
        }

        private static void connectionError(string error) {
            labelInfo.DOMElement.Style.Color = "#ff0000";
            labelInfo.Text = "Connection error: " + error;
        }

        private static void gotLoginToken(string token) {
            labelInfo.DOMElement.Style.Color = "#000000";
            labelInfo.Text = "Connection successful";
        }

        public static void Main(Dictionary arguments) {
#pragma warning disable 168
            SettingsScriptlet scriptlet = new SettingsScriptlet();
#pragma warning restore 168
        }

        private static bool saveSettings() {
            if (!isValidUrl(txtUrl.Text) || !haveProject) return false;
            Gadget.Settings.WriteString(SETTING_URL, txtUrl.Text);
            Gadget.Settings.WriteString(SETTING_LOGIN, txtLogin.Text);
            Gadget.Settings.WriteString(SETTING_PASSWORD, txtPassword.Text);
            Gadget.Settings.WriteString(SETTING_FILTERVALUE, optionreader.getselectedval(FILTERS_SELECT));
            Gadget.Settings.WriteString(SETTING_FILTERNAME, optionreader.getselectedtext(FILTERS_SELECT));
            Gadget.Settings.Write(SETTING_PROJECTKEY, optionreader.getselectedval(PROJECTS_SELECT));
            Gadget.Settings.Write(SETTING_PROJECTNAME, optionreader.getselectedtext(PROJECTS_SELECT));

            return true;
        }
    }
}
