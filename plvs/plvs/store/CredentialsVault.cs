using System;
using Atlassian.plvs.api;
using Microsoft.Win32;
using System.Diagnostics;

namespace Atlassian.plvs.store {
    internal class CredentialsVault {
        private static readonly CredentialsVault INSTANCE = new CredentialsVault();

        public static CredentialsVault Instance {
            get { return INSTANCE; }
        }

        private const string ATL_KEY = "Software\\Atlassian";
        private const string PAZU_KEY = "PaZu";
        private const string USER_NAME = "UserName_";
        private const string USER_PASSWORD = "UserPassword_";

        private CredentialsVault() {}

        public string getUserName(JiraServer server) {
            try {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(ATL_KEY + "\\" + PAZU_KEY);
                if (key != null) return (string) key.GetValue(USER_NAME + server.GUID, "");
            }
            catch (Exception e) {
                Debug.WriteLine(e.Message);
            }
            return "";
        }

        public string getPassword(JiraServer server) {
            try {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(ATL_KEY + "\\" + PAZU_KEY);
                if (key != null) {
                    string password = DPApi.decrypt((string) key.GetValue(USER_PASSWORD + server.GUID, ""));
                    return password;
                }
            }
            catch (Exception e) {
                Debug.WriteLine(e.Message);
            }
            return "";
        }

        public void saveCredentials(JiraServer server) {
            RegistryKey atlKey = Registry.CurrentUser.CreateSubKey(ATL_KEY);
            if (atlKey == null) return;
            RegistryKey key = atlKey.CreateSubKey(PAZU_KEY);
            if (key == null) return;
            key.SetValue(USER_NAME + server.GUID, server.UserName);
            key.SetValue(USER_PASSWORD + server.GUID, DPApi.encrypt(server.Password));
        }

        public void deleteCredentials(JiraServer server) {
            RegistryKey atlKey = Registry.CurrentUser.CreateSubKey(ATL_KEY);
            if (atlKey == null) return;
            RegistryKey key = atlKey.CreateSubKey(PAZU_KEY);
            if (key == null) return;
            key.DeleteValue(USER_NAME + server.GUID, false);
            key.DeleteValue(USER_PASSWORD + server.GUID, false);
        }
    }
}