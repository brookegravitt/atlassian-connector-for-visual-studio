﻿using Atlassian.plvs.api;

namespace Atlassian.plvs.ui.issuefilternodes {
    public class JiraSavedFiltersGroupTreeNode : JiraFilterGroupTreeNode {
        public JiraSavedFiltersGroupTreeNode(JiraServer server, int imageIdx) : base(server, "Saved Filters", imageIdx) {}
    }
}