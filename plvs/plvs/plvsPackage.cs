using System;
using System.IO;
using System.Runtime.InteropServices;
using Atlassian.plvs.eventsinks;
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

#if (TOOLWINDOW_MENUITEM)
        private void ShowToolWindow(object sender, EventArgs e) {
            createJiraWindow();
        }
#endif

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

        int IVsPersistSolutionOpts.LoadUserOptions(IVsSolutionPersistence pPersistence, uint grfLoadOpts) {
            try {
                pPersistence.LoadPackageUserOpts(this, "server");
                return VSConstants.S_OK;
            }
            finally {
                Marshal.ReleaseComObject(pPersistence);
            }
        }

        int IVsPersistSolutionOpts.ReadUserOptions(IStream pOptionsStream, string pszKey) {
            try {
                using (Stream wrapper = (Stream) pOptionsStream) {
                    switch (pszKey) {
                        case "server":
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

        public int SaveUserOptions(IVsSolutionPersistence pPersistence) {
            return VSConstants.S_OK;
        }

        protected override void Initialize() {
            base.Initialize();

#if (MENUITEMS)
    // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof (IMenuCommandService)) as OleMenuCommandService;
            if (null == mcs) return;

            // Create the command for the menu item.
            CommandID menuCommandID = new CommandID(GuidList.guidplvsCmdSet, (int) PkgCmdIDList.cmdidToggleToolWindow);
            MenuCommand menuItem = new MenuCommand(MenuItemCallback, menuCommandID);
            mcs.AddCommand(menuItem);

    // Create the command for the tool window
            CommandID toolwndCommandID = new CommandID(GuidList.guidplvsCmdSet,
                                                       (int) PkgCmdIDList.cmdidAtlassianToolWindow);
            MenuCommand menuToolWin = new MenuCommand(ShowToolWindow, toolwndCommandID);
            mcs.AddCommand(menuToolWin);
#endif

            DTE dte = (DTE) GetService(typeof (DTE));

            SolutionEventSink solutionEventSink = new SolutionEventSink(dte, createJiraWindow, createIssueDetailsWindow);

            IVsSolution solution = (IVsSolution) GetService(typeof (SVsSolution));
            ErrorHandler.ThrowOnFailure(solution.AdviseSolutionEvents(solutionEventSink, out solutionEventCookie));

            //            createIssueDetailsWindow();
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

#if (MENUITEMS)
        private void MenuItemCallback(object sender, EventArgs e) {
            // Show a Message Box to prove we were here
            IVsUIShell uiShell = (IVsUIShell) GetService(typeof (SVsUIShell));
            Guid clsid = Guid.Empty;
            int result;
            ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(
                                            0,
                                            ref clsid,
                                            "Atlassian Connector for Visual Studio",
                                            string.Format(CultureInfo.CurrentCulture,
                                                          "Inside {0}.MenuItemCallback()",
                                                          this.ToString()),
                                            string.Empty,
                                            0,
                                            OLEMSGBUTTON.OLEMSGBUTTON_OK,
                                            OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                                            OLEMSGICON.OLEMSGICON_INFO,
                                            0, // false
                                            out result));
        }
#endif
    }
}