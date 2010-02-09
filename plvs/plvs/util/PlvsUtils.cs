using System;
using System.Collections.Generic;
using System.Reflection;
using Atlassian.plvs.attributes;

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
    }
}
