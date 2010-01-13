using System;
using System.Diagnostics;
using Atlassian.plvs.autoupdate;
using Atlassian.plvs.dialogs;
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
        public delegate ToolWindowPane CreateToolWindow();

        private readonly PlvsPackage package;
        private readonly CreateToolWindow createJiraWindow;
        private readonly CreateToolWindow createIssueDetailsWindow;

        public SolutionEventSink(PlvsPackage package, CreateToolWindow createJiraWindow, CreateToolWindow createIssueDetailsWindow) {
            this.package = package;
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
                GlobalSettings.checkFirstRun();

                JiraServerModel.Instance.clear();
                JiraServerModel.Instance.load();
                RecentlyViewedIssuesModel.Instance.load();
                JiraCustomFilter.load();
                ToolWindowManager.Instance.AtlassianWindow = createJiraWindow();
                IssueListWindow.Instance.reinitialize();
                ToolWindowManager.Instance.IssueDetailsWindow = createIssueDetailsWindow();

                DTE dte = (DTE) package.GetService(typeof(DTE));

                IssueDetailsWindow.Instance.Solution = dte.Solution;

                JiraEditorLinkManager.OnSolutionOpened();
                Autoupdate.Instance.initialize();
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
                IssueListWindow.Instance.FrameVisible = false;
                ParameterStoreManager.Instance.clear();
                JiraIssueListModelImpl.Instance.removeAllListeners();
                IssueDetailsWindow.Instance.clearAllIssues();
                IssueDetailsWindow.Instance.FrameVisible = false;
                ToolWindowManager.Instance.AtlassianWindow = null;
                Autoupdate.Instance.shutdown();
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