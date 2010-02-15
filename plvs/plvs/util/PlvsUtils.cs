using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
            button.Text = button.Text + " (" + bindingText.Substring("Global::".Length) + ")";
        }
    }
}
