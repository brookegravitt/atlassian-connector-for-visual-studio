using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
#if VS2010
using System.Windows.Media.Imaging;
#endif
using Atlassian.plvs.attributes;
using EnvDTE;

namespace Atlassian.plvs.util {
    public static class PlvsUtils {
        public static string GetStringValue(this Enum value) {
            Type type = value.GetType();
            FieldInfo fieldInfo = type.GetField(value.ToString());
            StringValueAttribute[] attribs = fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false) as StringValueAttribute[];
            if (attribs == null) return null;
            return attribs.Length > 0 ? attribs[0].StringValue : null;
        }

        public static bool compareLists<T>(IList<T> lhs, IList<T> rhs) {
            if (lhs == null && rhs == null) return true;
            if (lhs == null || rhs == null) return false;

            if (lhs.Count != rhs.Count) return false;
            for (int i = 0; i < lhs.Count; ++i) {
                if (!lhs[i].Equals(rhs[i])) return false;
            }
            return true;
        }

        // ok, this method officially sucks. I am only updating bindings on toolwindow creation.
        // Also, command names are hardcoded.
        // 
        // If anybody can tell me how to get notified about key bindings change, please let me know
        public static void updateKeyBindingsInformation(DTE dte, IDictionary<string, ToolStripItem> buttons) {
            if (dte == null) return;
            IEnumerator enumerator = dte.Commands.GetEnumerator();
            while (enumerator.MoveNext()) {
                Command c = (Command)enumerator.Current;
                if (buttons.ContainsKey(c.Name)) {
                    addBindingToButton(buttons[c.Name], c.Bindings as object[]);
                }
            }
        }

        public static string getAssemblyBasedLocalFilePath(string fileName) {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string name = assembly.EscapedCodeBase;

            if (name != null) {
                name = name.Substring(0, name.LastIndexOf("/"));
                return name + "/" + fileName;
            }
            throw new InvalidOperationException("Unable to retrieve assembly location");
        }

        private static void addBindingToButton(ToolStripItem button, object[] bindings) {
            if (bindings == null || bindings.Length == 0) return;

            string bindingText = bindings[0].ToString();
            string text = button.Text.Contains(" (") ? button.Text.Substring(0, button.Text.IndexOf(" (")) : button.Text;
            button.Text = text + " (" + bindingText.Substring("Global::".Length) + ")";
        }

        public static void showErrors(string msg, IEnumerable<Exception> exceptions) {
            StringBuilder sb = new StringBuilder();
            foreach (Exception exception in exceptions) {
                sb.Append(getExceptionMessage(exception)).Append("\n");
            }
            MessageBox.Show((msg != null ? msg + "\n\n" : "") + sb + "\n\nPress Ctrl+C to copy error text to clipboard",
                Constants.ERROR_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void showError(string msg, Exception e) {
            string exceptionMessage = getExceptionMessage(e);
            MessageBox.Show((msg != null ? msg + "\n\n" : "") + exceptionMessage + "\n\nPress Ctrl+C to copy error text to clipboard", 
                Constants.ERROR_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private const string RAE = "com.atlassian.jira.rpc.exception.RemoteAuthenticationException: ";
        private const string RVE = "com.atlassian.jira.rpc.exception.RemoteValidationException: ";

        private static string getExceptionMessage(Exception e) {
            if (e == null) {
                return "";
            }
            if (e.InnerException != null) {
                string message = e.InnerException.Message;
                if (message.Contains(RAE)) {
                    return message.Substring(message.LastIndexOf(RAE) + RAE.Length);
                }
                return message;
            }
            if (e.Message.Contains(RVE)) {
                return getJiraValidationErrorMessage(e);
            }
            return e.Message;
        }

        private static readonly Regex errorRegex = new Regex(@"Errors: \{(.+)\}");
        private static readonly Regex errorMsgRegex = new Regex(@"Error Messages: \[(.+)\]");

        private static string getJiraValidationErrorMessage(Exception e) {
            if (errorRegex.IsMatch(e.Message)) {
                return errorRegex.Match(e.Message).Groups[1].Value;
            }
            if (errorMsgRegex.IsMatch(e.Message)) {
                return errorMsgRegex.Match(e.Message).Groups[1].Value;
            }
            return e.Message;
        }

        public static string getTextDocument(Stream stream) {
            StringBuilder sb = new StringBuilder();

            byte[] buf = new byte[8192];

            int count;

            do {
                count = stream.Read(buf, 0, buf.Length);
                if (count == 0) continue;
                string tempString = Encoding.ASCII.GetString(buf, 0, count);
                sb.Append(tempString);
            } while (count > 0);

            return sb.ToString();
        }

        public static byte[] getBytesFromStream(Stream stream) {
            MemoryStream ms = new MemoryStream();
            byte[] buf = new byte[8192];

            int count;

            do {
                count = stream.Read(buf, 0, buf.Length);
                if (count == 0) continue;
                ms.Write(buf, 0, count);
            } while (count > 0);

            ms.Close();

            return ms.GetBuffer();
        }

        ///  FUNCTION Enquote Public Domain 2002 JSON.org 
        ///  @author JSON.org 
        ///  @version 0.1 
        ///  Ported to C# by Are Bjolseth, teleplan.no 
        public static string JsonEncode(string s) {
            if (string.IsNullOrEmpty(s)) {
                return "\"\"";
            }
            int i;
            int len = s.Length;
            StringBuilder sb = new StringBuilder(len + 4);
            string t;

            sb.Append('"');
            for (i = 0; i < len; i += 1) {
                char c = s[i];
                if ((c == '\\') || (c == '"') || (c == '>')) {
                    sb.Append('\\');
                    sb.Append(c);
                } else if (c == '\b')
                    sb.Append("\\b");
                else if (c == '\t')
                    sb.Append("\\t");
                else if (c == '\n')
                    sb.Append("\\n");
                else if (c == '\f')
                    sb.Append("\\f");
                else if (c == '\r')
                    sb.Append("\\r");
                else {
                    if (c < ' ') {
                        //t = "000" + Integer.toHexString(c); 
                        string tmp = new string(c, 1);
                        t = "000" + int.Parse(tmp, System.Globalization.NumberStyles.HexNumber);
                        sb.Append("\\u" + t.Substring(t.Length - 4));
                    } else {
                        sb.Append(c);
                    }
                }
            }
            sb.Append('"');
            return sb.ToString();
        }

#if VS2010
        // from http://social.msdn.microsoft.com/Forums/en-US/wpf/thread/16bce8a4-1ee7-4be9-bd7f-0cc2b0f80cf0/
        public static BitmapSource bitmapSourceFromPngImage(System.Drawing.Image img) {
            MemoryStream memStream = new MemoryStream();
            img.Save(memStream, System.Drawing.Imaging.ImageFormat.Png);
            PngBitmapDecoder decoder = new PngBitmapDecoder(memStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            return decoder.Frames[0];
        }
#endif

        public static void installSslCertificateHandler() {
            // This call solves PLVS-103
            // the code taken from http://www.codeproject.com/KB/webservices/web_service_over_SSL.aspx
            ServicePointManager.ServerCertificateValidationCallback =
                delegate(Object obj, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors) { return (true); };
        }

        public static void safeInvoke(this Control control, Delegate action) {
            try {
                control.Invoke(action);
            } catch (InvalidOperationException e) {
                Debug.WriteLine("PlvsUtils.safeInvoke() - exception: " + e.Message);
            }
        }
    }
}
