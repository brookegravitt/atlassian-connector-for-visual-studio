using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
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

        private static void addBindingToButton(ToolStripItem button, object[] bindings) {
            if (bindings == null || bindings.Length == 0) return;

            string bindingText = bindings[0].ToString();
            string text = button.Text.Contains(" (") ? button.Text.Substring(0, button.Text.IndexOf(" (")) : button.Text;
            button.Text = text + " (" + bindingText.Substring("Global::".Length) + ")";
        }

        public static void showError(string msg) {
            MessageBox.Show(msg + "\n\nPress Ctrl+C to copy error text to clipboard", 
                Constants.ERROR_CAPTION, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }
}
