using System.Collections.Generic;
using Atlassian.plvs.attributes;
using Atlassian.plvs.util;

namespace Atlassian.plvs.api.bamboo {
    public class BambooBuild {
        public enum BuildResult {
            [StringValue("Successful")]
            [ColorValue("#339933")]
            SUCCESSFUL,
            [StringValue("Failed")]
            [ColorValue("#ff0000")]
            FAILED,
            [StringValue("Unknown")]
            [ColorValue("#808080")]
            UNKNOWN
        }

        public enum PlanState {
            UNKNOWN,
            IDLE,
            IN_QUEUE,
            BUILDING
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

        public class Artifact {
            public string Name { get; private set; }
            public string Url { get; private set; }
            public Artifact(string name, string url) {
                Name = name;
                Url = url;
            }
        }

        public class RelatedIssue {
            public string Key { get; private set; }
            public string Url { get; private set; }
            public RelatedIssue(string key, string url) {
                Key = key;
                Url = url;
            }
        }

        public BambooBuild(
            BambooServer server, string key, BuildResult result, int number, string relativeTime,
            string duration, int successfulTests, int failedTests, string reason, PlanState state,
            ICollection<Artifact> artifacts, ICollection<RelatedIssue> relatedIssues) {

            Server = server;
            Key = key;
            Result = result;
            Number = number;
            RelativeTime = relativeTime;
            Duration = duration;
            SuccessfulTests = successfulTests;
            FailedTests = failedTests;
            Reason = reason;
            State = state;
            Artifacts = artifacts;
            RelatedIssues = relatedIssues;
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
        public PlanState State { get; private set; }
        public ICollection<Artifact> Artifacts { get; private set; }
        public ICollection<RelatedIssue> RelatedIssues { get; private set; }
    }
}
