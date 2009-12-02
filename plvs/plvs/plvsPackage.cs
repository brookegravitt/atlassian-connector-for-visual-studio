using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Atlassian.plvs.eventsinks;
using Atlassian.plvs.util;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;

namespace Atlassian.plvs {
    [ProvideLoadKey("Standard", "1.0", "Atlassian Connector for Visual Studio", "Atlassian", 1)]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\9.0")]
    [InstalledProductRegistration(true, null, null, null)]
    [ProvideMenuResource(1000, 1)]
    [ProvideToolWindow(typeof (AtlassianToolWindow), Transient = true, Style = VsDockStyle.Tabbed,
        Orientation = ToolWindowOrientation.Bottom)]
    [ProvideToolWindow(typeof (IssueDetailsToolWindow), Transient = true, Style = VsDockStyle.Tabbed,
        Orientation = ToolWindowOrientation.Bottom)]
    [ProvideAutoLoad(UIContextGuids.SolutionExists)]
    [ProvideToolWindowVisibility(typeof (AtlassianToolWindow), UIContextGuids.SolutionExists)]
    [ProvideToolWindowVisibility(typeof (IssueDetailsToolWindow), UIContextGuids.SolutionExists)]
    [Guid(GuidList.guidplvsPkgString)]
    public sealed class PlvsPackage : Package, IVsPersistSolutionOpts, IVsInstalledProduct {
        public new object GetService(Type serviceType) {
            return base.GetService(serviceType);
        }

        private ToolWindowPane createJiraWindow() {
            // Get the instance number 0 of this tool window. This window is single instance so this instance
            // is actually the only one.
            // The last flag is set to true so that if the tool window does not exists it will be created.
            ToolWindowPane window = FindToolWindow(typeof (AtlassianToolWindow), 0, true);
            if ((null == window) || (null == window.Frame)) {
                throw new NotSupportedException(Resources.CanNotCreateWindow);
            }
            IVsWindowFrame windowFrame = (IVsWindowFrame) window.Frame;
            ErrorHandler.ThrowOnFailure(windowFrame.Show());
            return window;
        }

        private ToolWindowPane createIssueDetailsWindow() {
            ToolWindowPane window = FindToolWindow(typeof (IssueDetailsToolWindow), 0, true);
            if ((null == window) || (null == window.Frame)) {
                throw new NotSupportedException(Resources.CanNotCreateWindow);
            }
            IVsWindowFrame windowFrame = (IVsWindowFrame) window.Frame;
            IssueDetailsWindow.Instance.DetailsFrame = window;
            ErrorHandler.ThrowOnFailure(windowFrame.Hide());
            return window;
        }

        private uint solutionEventCookie;
        private const string SERVER = "server";

        // this does not work anyway - oh well. No splash screen for us. too bad
        public int IdBmpSplash(out uint pIdBmp) {
            pIdBmp = 400;
            return VSConstants.S_OK;
        }

        public int IdIcoLogoForAboutbox(out uint pIdIco) {
            pIdIco = 600;
            return VSConstants.S_OK;
        }

        public int OfficialName(out string pbstrName) {
            pbstrName = VSPackage._110;
            return VSConstants.S_OK;
        }

        public int ProductID(out string pbstrPID) {
            pbstrPID = "1.0";
            return VSConstants.S_OK;
        }

        public int ProductDetails(out string pbstrProductDetails) {
            pbstrProductDetails = VSPackage._112;
            return VSConstants.S_OK;
        }

        public int SaveUserOptions(IVsSolutionPersistence pPersistence) {
            try {
                pPersistence.SavePackageUserOpts(this, SERVER);
            }
            finally {
                Marshal.ReleaseComObject(pPersistence);
            }
            return VSConstants.S_OK;
        }

        public int LoadUserOptions(IVsSolutionPersistence pPersistence, uint grfLoadOpts) {
            try {
                pPersistence.LoadPackageUserOpts(this, SERVER);
            }
            finally {
                Marshal.ReleaseComObject(pPersistence);
            }
            return VSConstants.S_OK;
        }

        public int WriteUserOptions(IStream pOptionsStream, string pszKey) {
            try {
                using (StreamEater wrapper = new StreamEater(pOptionsStream)) {
                    switch (pszKey) {
                        case SERVER:
                            writeOptions(wrapper);
                            break;
                        default:
                            break;
                    }
                }

                return VSConstants.S_OK;
            }
            finally {
                Marshal.ReleaseComObject(pOptionsStream);
            }
        }

        public int ReadUserOptions(IStream pOptionsStream, string pszKey) {
            try {
                using (StreamEater wrapper = new StreamEater(pOptionsStream)) {
                    switch (pszKey) {
                        case SERVER:
                            loadOptions(wrapper);
                            break;
                        default:
                            break;
                    }
                }
                return VSConstants.S_OK;
            }
            finally {
                Marshal.ReleaseComObject(pOptionsStream);
            }
        }

        private void writeOptions(Stream storageStream) {
            string text = "test";

            using (BinaryWriter bw = new BinaryWriter(storageStream)) {
                bw.Write(text);
            }
        }

        private void loadOptions(Stream storageStream) {
            using (BinaryReader bReader = new BinaryReader(storageStream)) {
                string Text = bReader.ReadString();
            }
        }

        protected override void Initialize() {
            base.Initialize();

            OleMenuCommandService mcs = GetService(typeof (IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs) {
                CommandID menuCommandId = new CommandID(GuidList.guidplvsCmdSet,
                                                        (int) PkgCmdIDList.cmdidToggleToolWindow);
                MenuCommand menuItem = new MenuCommand(MenuItemCallback, menuCommandId);
                mcs.AddCommand(menuItem);
            }

            DTE dte = (DTE) GetService(typeof (DTE));

            SolutionEventSink solutionEventSink = new SolutionEventSink(dte, createJiraWindow, createIssueDetailsWindow);

            IVsSolution solution = (IVsSolution) GetService(typeof (SVsSolution));
            ErrorHandler.ThrowOnFailure(solution.AdviseSolutionEvents(solutionEventSink, out solutionEventCookie));
        }

        protected override void Dispose(bool disposing) {
            IVsSolution solution = (IVsSolution) GetService(typeof (SVsSolution));
            try {
                solution.UnadviseSolutionEvents(solutionEventCookie);
            }
// ReSharper disable EmptyGeneralCatchClause
            catch {}
// ReSharper restore EmptyGeneralCatchClause
            base.Dispose(disposing);
        }

        private static void MenuItemCallback(object sender, EventArgs e) {
            ToolWindowManager.Instance.AtlassianWindowVisible = !ToolWindowManager.Instance.AtlassianWindowVisible;
        }
    }
}