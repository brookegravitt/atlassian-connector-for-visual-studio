﻿using System.Drawing;
using System.Text.RegularExpressions;
using Atlassian.plvs.api.bamboo;

namespace Atlassian.plvs.ui.bamboo {
    public class BuildNode {

        private const int PROBABLE_GARBAGE_REASON_LENGTH = 300;

        public BambooBuild Build { get; set; }

        public BuildNode(BambooBuild build) {
            Build = build;
        }


        public Image Icon {
            get {
                switch (Build.Result) {
                    case BambooBuild.BuildResult.SUCCESSFUL:
                        return Resources.icn_plan_passed;
                    case BambooBuild.BuildResult.FAILED:
                        return Resources.icn_plan_failed;
                    default:
                        // todo: fixme - this is not right, disabled plans should be disabled, 
                        // unknown results should have a separate icon
                        return Resources.icn_plan_disabled;
                }
            }
        }

        public string Key { get { return Build.Key; } }

        public string Tests { 
            get {
                if (Build.SuccessfulTests == 0 && Build.FailedTests == 0) {
                    return "No tests";
                }
                return Build.SuccessfulTests + "/" + (Build.SuccessfulTests + Build.FailedTests) + " tests passed";
            } 
        }

        public string Reason {
            get {
                return Build.Reason.Length > PROBABLE_GARBAGE_REASON_LENGTH ? "[garbage received?]" : stripHtml(Build.Reason);
            }
        }

        private static string stripHtml(string html) {
            return Regex.Replace(html, @"<(.|\n)*?>", string.Empty); 
        }

        public string Completed { get { return Build.RelativeTime; } }

        public string Duration { get { return Build.Duration; } }

        public string Server { get { return Build.Server.Name; } }
    }
}