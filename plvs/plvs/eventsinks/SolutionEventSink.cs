using System;
using System.Diagnostics;
using Atlassian.plvs.models;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Atlassian.plvs.eventsinks {
    public sealed class SolutionEventSink : IVsSolutionEvents {
        public DTE dte { get; set; }

        private IVsWindowFrame jiraWindow;

        public delegate IVsWindowFrame CreateJiraWindow();

        private readonly CreateJiraWindow createJiraWindow;

        public SolutionEventSink(DTE dte, CreateJiraWindow createJiraWindow) {
            this.dte = dte;
            this.createJiraWindow = createJiraWindow;
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
                JiraServerModel.Instance.load(dte.Solution.Globals);
                RecentlyViewedIssuesModel.Instance.load(dte.Globals, dte.Solution.FullName);
                JiraCustomFilter.load(dte.Globals, dte.Solution.FullName);
                jiraWindow = createJiraWindow();
                IssueDetailsWindow.Instance.Solution = dte.Solution;
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
                if (jiraWindow == null) return VSConstants.S_OK;
                JiraServerModel.Instance.save(dte.Solution.Globals);
                JiraCustomFilter.save(dte.Globals, dte.Solution.FullName);
                JiraIssueListModel.Instance.removeAllListeners();
                jiraWindow.Hide();
                jiraWindow.CloseFrame((uint) __FRAMECLOSE.FRAMECLOSE_SaveIfDirty);
                jiraWindow = null;
                RecentlyViewedIssuesModel.Instance.save(dte.Globals, dte.Solution.FullName);
                IssueDetailsWindow.Instance.clearAllIssues();
                IssueDetailsWindow.Instance.FrameVisible = false;
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