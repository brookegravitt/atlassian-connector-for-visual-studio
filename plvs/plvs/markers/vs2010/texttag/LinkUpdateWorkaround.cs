﻿using System;
using System.ComponentModel.Composition;
using Atlassian.plvs.windows;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace Atlassian.plvs.markers.vs2010.texttag {

    /// <summary>
    /// 
    /// This class is required as a workaround for an issue described here:
    /// http://social.msdn.microsoft.com/Forums/en-SG/vseditorprerelease/thread/0ca39bad-785d-474b-89b9-84981dee5534
    /// 
    /// Microsoft acknowledged the issue in their code. Unfortunately, 
    /// they will not be able to provide the fix before RTM is launched
    /// 
    /// </summary>
    
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    class LinkUpdateWorkaround : IWpfTextViewCreationListener {
        public void TextViewCreated(IWpfTextView textView) {
            new ViewListener(textView);
        }
    }

    class ViewListener {
        private bool disposed;
        private readonly ITextView view;

        public ViewListener(ITextView view) {
            this.view = view;
            AtlassianPanel.Instance.Jira.SelectedServerChanged += jiraSelectedServerChanged;
        }

        private void jiraSelectedServerChanged(object sender, EventArgs e) {
            var options = view.Options;
            if (!options.GetOptionValue(DefaultTextViewOptions.DisplayUrlsAsHyperlinksId)) return;
            options.SetOptionValue(DefaultTextViewOptions.DisplayUrlsAsHyperlinksId, false);
            options.SetOptionValue(DefaultTextViewOptions.DisplayUrlsAsHyperlinksId, true);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {
            if (disposed) return;
            if (disposing) {
                AtlassianPanel.Instance.Jira.SelectedServerChanged -= jiraSelectedServerChanged;
            }
            disposed = true;
        }
    }
}
