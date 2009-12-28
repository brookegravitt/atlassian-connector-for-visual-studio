using System;
using System.Diagnostics;
using Atlassian.plvs.markers;
using Atlassian.plvs.models;
using Atlassian.plvs.store;
using Atlassian.plvs.windows;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Atlassian.plvs.eventsinks {
    public sealed class SolutionEventSink : IVsSolutionEvents {
        public DTE dte { get; set; }

        public delegate ToolWindowPane CreateToolWindow();

        private readonly CreateToolWindow createJiraWindow;
        private readonly CreateToolWindow createIssueDetailsWindow;

        public SolutionEventSink(DTE dte, CreateToolWindow createJiraWindow, CreateToolWindow createIssueDetailsWindow) {
            this.dte = dte;
            this.createJiraWindow = createJiraWindow;
            this.createIssueDetailsWindow = createIssueDetailsWindow;
        }

        public int OnAfterOpenProject(IVsHierarchy pHierarchy, int fAdded) {
            return VSConstants.S_OK;
        }

        public int OnQueryCloseProject(IVsHierarchy pHierarchy, int fRemoving, ref int pfCancel) {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseProject(IVsHierarchy pHierarchy, int fRemoved) {
            return VSConstants.S_OK;
        }

        public int OnAfterLoadProject(IVsHierarchy pStubHierarchy, IVsHierarchy pRealHierarchy) {
            return VSConstants.S_OK;
        }

        public int OnQueryUnloadProject(IVsHierarchy pRealHierarchy, ref int pfCancel) {
            return VSConstants.S_OK;
        }

        public int OnBeforeUnloadProject(IVsHierarchy pRealHierarchy, IVsHierarchy pStubHierarchy) {
            return VSConstants.S_OK;
        }

        public int OnAfterOpenSolution(object pUnkReserved, int fNewSolution) {
            try {
                JiraServerModel.Instance.clear();
                JiraServerModel.Instance.load();
                RecentlyViewedIssuesModel.Instance.load();
                JiraCustomFilter.load();
                ToolWindowManager.Instance.AtlassianWindow = createJiraWindow();
                IssueListWindow.Instance.reinitialize();
                ToolWindowManager.Instance.IssueDetailsWindow = createIssueDetailsWindow();
                IssueDetailsWindow.Instance.Solution = dte.Solution;

                JiraEditorLinkManager.OnSolutionOpened();
            }
            catch (Exception e) {
                Debug.WriteLine(e);
            }

            return VSConstants.S_OK;
        }

        public int OnQueryCloseSolution(object pUnkReserved, ref int pfCancel) {
            return VSConstants.S_OK;
        }

        public int OnBeforeCloseSolution(object pUnkReserved) {
            try {
                JiraEditorLinkManager.OnSolutionClosed();
                if (ToolWindowManager.Instance.AtlassianWindow == null) return VSConstants.S_OK;
                JiraServerModel.Instance.clear();
                ParameterStoreManager.Instance.clear();
                JiraIssueListModelImpl.Instance.removeAllListeners();
                IssueDetailsWindow.Instance.clearAllIssues();
                IssueDetailsWindow.Instance.FrameVisible = false;
                ToolWindowManager.Instance.AtlassianWindow = null;
            }
            catch (Exception e) {
                Debug.WriteLine(e);
            }

            return VSConstants.S_OK;
        }

        public int OnAfterCloseSolution(object pUnkReserved) {
            return VSConstants.S_OK;
        }
    }
}