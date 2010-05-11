using Atlassian.plvs.attributes;
using Atlassian.plvs.util;

namespace Atlassian.plvs.api.bamboo {
    public class BambooBuild {
        public enum BuildResult {
            [StringValue("Successful")]
            SUCCESSFUL,
            [StringValue("Failed")]
            FAILED,
            [StringValue("Unknown")]
            UNKNOWN
        }

        public static BuildResult stringToResult(string str) {
            if (BuildResult.SUCCESSFUL.GetStringValue().Equals(str)) {
                return BuildResult.SUCCESSFUL;
            }
            if (BuildResult.FAILED.GetStringValue().Equals(str)) {
                return BuildResult.FAILED;
            }
            return BuildResult.UNKNOWN;
        }

        public BambooBuild(
            BambooServer server, string key, BuildResult result, int number, string relativeTime, 
            string duration, int successfulTests, int failedTests, string reason) {
            Server = server;
            Key = key;
            Result = result;
            Number = number;
            RelativeTime = relativeTime;
            Duration = duration;
            SuccessfulTests = successfulTests;
            FailedTests = failedTests;
            Reason = reason;
        }

        public BambooServer Server { get; private set; }
        public string Key { get; private set; }
        public BuildResult Result { get; private set; }
        public int Number { get; private set; }
        public string RelativeTime { get; private set; }
        public string Duration { get; private set; }
        public int SuccessfulTests { get; private set; }
        public int FailedTests { get; private set; }
        public string Reason { get; private set; }


    }
}
