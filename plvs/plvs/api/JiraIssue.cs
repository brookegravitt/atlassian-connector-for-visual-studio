﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.XPath;
using Atlassian.plvs.util;

namespace Atlassian.plvs.api {
    public class JiraIssue {
        public const int UNKNOWN = -1;

        public class Comment {
            public string Body { get; internal set; }
            public string Created { get; internal set; }
            public string Author { get; internal set; }

            public bool Equals(Comment other) {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(other.Body, Body) && Equals(other.Created, Created) && Equals(other.Author, Author);
            }

            public override bool Equals(object obj) {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj.GetType() == typeof (Comment) && Equals((Comment) obj);
            }

            public override int GetHashCode() {
                unchecked {
                    int result = (Body != null ? Body.GetHashCode() : 0);
                    result = (result*397) ^ (Created != null ? Created.GetHashCode() : 0);
                    result = (result*397) ^ (Author != null ? Author.GetHashCode() : 0);
                    return result;
                }
            }
        }

        private readonly List<Comment> comments = new List<Comment>();

        private readonly List<string> versions = new List<string>();

        private readonly List<string> fixVersions = new List<string>();

        private readonly List<string> components = new List<string>();

        public JiraIssue(JiraServer server, XPathNavigator nav) {
            Server = server;

            nav.MoveToFirstChild();
            do {
                switch (nav.Name) {
                    case "key":
                        Key = nav.Value;
                        Id = getAttributeSafely(nav, "id", UNKNOWN);
                        ProjectKey = Key.Substring(0, Key.LastIndexOf('-'));
                        break;
                    case "summary":
                        Summary = nav.Value;
                        break;
                    case "status":
                        Status = nav.Value;
                        StatusIconUrl = getAttributeSafely(nav, "iconUrl", null);
                        StatusId = getAttributeSafely(nav, "id", UNKNOWN);
                        break;
                    case "priority":
                        Priority = nav.Value;
                        PriorityIconUrl = getAttributeSafely(nav, "iconUrl", null);
                        PriorityId = getAttributeSafely(nav, "id", UNKNOWN);
                        break;
                    case "description":
                        Description = nav.Value;
                        break;
                    case "type":
                        IssueType = nav.Value;
                        IssueTypeIconUrl = getAttributeSafely(nav, "iconUrl", null);
                        IssueTypeId = getAttributeSafely(nav, "id", UNKNOWN);
                        break;
                    case "assignee":
                        Assignee = nav.Value;
                        break;
                    case "reporter":
                        Reporter = nav.Value;
                        break;
                    case "created":
                        CreationDate = JiraIssueUtils.getDateTimeFromJiraTimeString(nav.Value);
                        break;
                    case "updated":
                        UpdateDate = JiraIssueUtils.getDateTimeFromJiraTimeString(nav.Value);
                        break;
                    case "resolution":
                        Resolution = nav.Value;
                        break;
                    case "timeestimate":
                        RemainingEstimate = nav.Value;
                        RemainingEstimateInSeconds = getAttributeSafely(nav, "seconds", UNKNOWN);
                        break;
                    case "timeoriginalestimate":
                        OriginalEstimate = nav.Value;
                        OriginalEstimateInSeconds = getAttributeSafely(nav, "seconds", UNKNOWN);
                        break;
                    case "timespent":
                        TimeSpent = nav.Value;
                        TimeSpentInSeconds = getAttributeSafely(nav, "seconds", UNKNOWN);
                        break;
                    case "version":
                        versions.Add(nav.Value);
                        break;
                    case "fixVersion":
                        fixVersions.Add(nav.Value);
                        break;
                    case "component":
                        components.Add(nav.Value);
                        break;
                    case "comments":
                        createComments(nav);
                        break;
                    case "environment":
                        Environment = nav.Value;
                        break;
                    default:
                        break;
                }
            } while (nav.MoveToNext());
            if (Key == null || Summary == null) {
                throw new InvalidDataException();
            }
        }

        private void createComments(XPathNavigator nav) {
            XPathExpression expr = nav.Compile("comment");
            XPathNodeIterator it = nav.Select(expr);

            if (!nav.MoveToFirstChild()) return;
            while (it.MoveNext()) {
                Comment c = new Comment
                            {
                                Body = it.Current.Value,
                                Author = getAttributeSafely(it.Current, "author", "Unknown"),
                                Created = getAttributeSafely(it.Current, "created", "Unknown")
                            };
                comments.Add(c);
            }
            nav.MoveToParent();
        }

        public JiraServer Server { get; private set; }

        public string IssueType { get; private set; }

        public int IssueTypeId { get; private set; }

        public string IssueTypeIconUrl { get; private set; }

        public string Description { get; private set; }

        public int Id { get; private set; }

        public string Key { get; private set; }

        public string Summary { get; private set; }

        public string Status { get; private set; }

        public int StatusId { get; private set; }

        public string StatusIconUrl { get; private set; }

        public string Priority { get; private set; }

        public string PriorityIconUrl { get; private set; }

        public int PriorityId { get; set; }

        public string Resolution { get; private set; }

        public string Reporter { get; private set; }

        public string Assignee { get; private set; }

        public DateTime CreationDate { get; private set; }

        public DateTime UpdateDate { get; private set; }

        public string ProjectKey { get; private set; }

        public string Environment { get; private set; }

        public string OriginalEstimate { get; private set; }

        public int OriginalEstimateInSeconds { get; private set; }

        public string RemainingEstimate { get; set; }

        public int RemainingEstimateInSeconds { get; set; }

        public string TimeSpent { get; set; }

        public int TimeSpentInSeconds { get; set; }

        public List<Comment> Comments {
            get { return comments; }
        }

        public List<string> Versions {
            get { return versions; }
        }

        public List<string> FixVersions {
            get { return fixVersions; }
        }

        public List<string> Components {
            get { return components; }
        }

        private static string getAttributeSafely(XPathNavigator nav, string name, string defaultValue) {
            if (nav.HasAttributes && nav.MoveToFirstAttribute()) {
                do {
                    if (!nav.Name.Equals(name)) continue;
                    string val = nav.Value;
                    nav.MoveToParent();
                    return val;
                } while (nav.MoveToNextAttribute());
                nav.MoveToParent();
            }
            return defaultValue;
        }

        private static int getAttributeSafely(XPathNavigator nav, string name, int defaultValue) {
            if (nav.HasAttributes && nav.MoveToFirstAttribute()) {
                do {
                    if (!nav.Name.Equals(name)) continue;
                    int val = nav.ValueAsInt;
                    nav.MoveToParent();
                    return val;
                } while (nav.MoveToNextAttribute());
                nav.MoveToParent();
            }
            return defaultValue;
        }

        public bool Equals(JiraIssue other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            bool eq = true;
            eq &= other.Server.GUID.Equals(Server.GUID);
            eq &= other.IssueType.Equals(IssueType);
            eq &= other.IssueTypeId.Equals(IssueTypeId);
            eq &= other.IssueTypeIconUrl.Equals(IssueTypeIconUrl);
            eq &= other.Description.Equals(Description);
            eq &= other.Id == Id;
            eq &= other.Key.Equals(Key);
            eq &= other.Summary.Equals(Summary);
            eq &= other.Status.Equals(Status);
            eq &= other.StatusIconUrl.Equals(StatusIconUrl);
            eq &= other.Priority.Equals(Priority);
            eq &= other.Resolution.Equals(Resolution);
            eq &= other.Reporter.Equals(Reporter);
            eq &= other.Assignee.Equals(Assignee);
            eq &= other.CreationDate.Equals(CreationDate);
            eq &= other.UpdateDate.Equals(UpdateDate);
            eq &= other.ProjectKey.Equals(ProjectKey);
            eq &= other.Environment.Equals(Environment);
            eq &= string.Equals(other.OriginalEstimate, OriginalEstimate);
            eq &= string.Equals(other.RemainingEstimate, RemainingEstimate);
            eq &= string.Equals(other.TimeSpent, TimeSpent);
            eq &= other.PriorityIconUrl.Equals(PriorityIconUrl);
            eq &= other.StatusId == StatusId;
            eq &= other.PriorityId == PriorityId;
            eq &= compareComments(other.comments, comments);
            eq &= compareVersions(other.versions, versions);
            eq &= compareFixVersions(other.fixVersions, fixVersions);
            eq &= compareComponents(other.components, components);

            return eq;
        }

        private static bool compareComments(IList<Comment> lhs, IList<Comment> rhs) {
            return compareLists(lhs, rhs);
        }

        private static bool compareFixVersions(IList<string> lhs, IList<string> rhs) {
            return compareLists(lhs, rhs);
        }

        private static bool compareVersions(IList<string> lhs, IList<string> rhs) {
            return compareLists(lhs, rhs);
        }

        private static bool compareComponents(IList<string> lhs, IList<string> rhs) {
            return compareLists(lhs, rhs);
        }

        private static bool compareLists<T>(IList<T> lhs, IList<T> rhs) {
            if (!ReferenceEquals(lhs, rhs)) return false;
            if (lhs.Count != rhs.Count) return false;
            for (int i = 0; i < lhs.Count; ++i) {
                if (!lhs[i].Equals(rhs[i])) return false;
            }
            return true;
        }

        public override int GetHashCode() {
            unchecked {
                int result = (comments != null ? comments.GetHashCode() : 0);
                result = (result*397) ^ (versions != null ? versions.GetHashCode() : 0);
                result = (result*397) ^ (fixVersions != null ? fixVersions.GetHashCode() : 0);
                result = (result*397) ^ (components != null ? components.GetHashCode() : 0);
                result = (result*397) ^ (Server != null ? Server.GUID.GetHashCode() : 0);
                result = (result*397) ^ (IssueType != null ? IssueType.GetHashCode() : 0);
                result = (result*397) ^ IssueTypeId;
                result = (result*397) ^ (IssueTypeIconUrl != null ? IssueTypeIconUrl.GetHashCode() : 0);
                result = (result*397) ^ (Description != null ? Description.GetHashCode() : 0);
                result = (result*397) ^ Id;
                result = (result*397) ^ (Key != null ? Key.GetHashCode() : 0);
                result = (result*397) ^ (Summary != null ? Summary.GetHashCode() : 0);
                result = (result*397) ^ (Status != null ? Status.GetHashCode() : 0);
                result = (result*397) ^ (StatusIconUrl != null ? StatusIconUrl.GetHashCode() : 0);
                result = (result*397) ^ (Priority != null ? Priority.GetHashCode() : 0);
                result = (result*397) ^ (Resolution != null ? Resolution.GetHashCode() : 0);
                result = (result*397) ^ (Reporter != null ? Reporter.GetHashCode() : 0);
                result = (result*397) ^ (Assignee != null ? Assignee.GetHashCode() : 0);
                result = (result*397) ^ CreationDate.GetHashCode();
                result = (result*397) ^ UpdateDate.GetHashCode();
                result = (result*397) ^ (ProjectKey != null ? ProjectKey.GetHashCode() : 0);
                result = (result*397) ^ (Environment != null ? Environment.GetHashCode() : 0);
                result = (result*397) ^ (OriginalEstimate != null ? OriginalEstimate.GetHashCode() : 0);
                result = (result*397) ^ (RemainingEstimate != null ? RemainingEstimate.GetHashCode() : 0);
                result = (result*397) ^ (TimeSpent != null ? TimeSpent.GetHashCode() : 0);
                result = (result*397) ^ (PriorityIconUrl != null ? PriorityIconUrl.GetHashCode() : 0);
                result = (result*397) ^ StatusId;
                result = (result*397) ^ PriorityId;
                return result;
            }
        }
    }
}