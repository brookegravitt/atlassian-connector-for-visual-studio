using System;
using Atlassian.plvs.attributes;

namespace Atlassian.plvs.api.bamboo {
    public class BambooBuild {
        public enum BuildResult {
            [StringValue("Passed")]
            PASSED,
            [StringValue("Failed")]
            FAILED,
            [StringValue("Unknown")]
            UNKNOWN
        }

        public BambooBuild(DateTime buildDate, BuildResult result) {
            BuildDate = buildDate;
            Result = result;
        }

        public DateTime BuildDate { get; private set; }
        public BuildResult Result { get; private set; }
    }
}
