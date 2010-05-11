using System;
using Atlassian.plvs.api.bamboo;

namespace Atlassian.plvs.util.jira {
    public static class BambooBuildUtils {
        
        public static string getPlanKey(BambooBuild build) {
            int idx = build.Key.LastIndexOf("-");
            if (idx < 0) {
                throw new ArgumentException("Build key does not seem to contain plan key: " + build.Key);
            }
            return build.Key.Substring(0, idx);
        }
    }
}
