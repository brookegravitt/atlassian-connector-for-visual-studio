// VsPkg.cs : Implementation of plvs
//

using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Atlassian.plvs.eventsinks;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;

namespace Atlassian.plvs {
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the registration utility (regpkg.exe) that this class needs
    // to be registered as package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // A Visual Studio component can be registered under different regitry roots; for instance
    // when you debug your package you want to register it in the experimental hive. This
    // attribute specifies the registry root to use if no one is provided to regpkg.exe with
    // the /root switch.
    [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\9.0")]
    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration(false, "#110", "#112", "1.0", IconResourceID = 400)]
    // In order be loaded inside Visual Studio in a machine that has not the VS SDK installed, 
    // package needs to have a valid load key (it can be requested at 
    // http://msdn.microsoft.com/vstudio/extend/). This attributes tells the shell that this 
    // package has a load key embedded in its resources.
    [ProvideLoadKey("Standard", "1.0", "Atlassian Connector for Visual Studio", "Atlassian", 1)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource(1000, 1)]
    // This attribute registers a tool window exposed by this package.
    [ProvideToolWindow(typeof (AtlassianToolWindow))]
    [ProvideToolWindow(typeof (IssueDetailsToolWindow))]
    [Guid(GuidList.guidplvsPkgString)]
    public sealed class PlvsPackage : Package {

        public new object GetService(Type serviceType) {
            return base.GetService(serviceType);
        }

#if (TOOLWINDOW_MENUITEM)
        private void ShowToolWindow(object sender, EventArgs e) {
            createJiraWindow();
        }
#endif

        private IVsWindowFrame createJiraWindow() {
            // Get the instance number 0 of this tool window. This window is single instance so this instance
            // is actually the only one.
            // The last flag is set to true so that if the tool window does not exists it will be created.
            ToolWindowPane window = FindToolWindow(typeof (AtlassianToolWindow), 0, true);
            if ((null == window) || (null == window.Frame)) {
                throw new NotSupportedException(Resources.CanNotCreateWindow);
            }
            IVsWindowFrame windowFrame = (IVsWindowFrame) window.Frame;
            ErrorHandler.ThrowOnFailure(windowFrame.Show());
            return windowFrame;
        }

        private void createIssueDetailsWindow() {
            ToolWindowPane window = FindToolWindow(typeof (IssueDetailsToolWindow), 0, true);
            if ((null == window) || (null == window.Frame)) {
                throw new NotSupportedException(Resources.CanNotCreateWindow);
            }
            IVsWindowFrame windowFrame = (IVsWindowFrame) window.Frame;
            IssueDetailsWindow.Instance.DetailsFrame = windowFrame;
            ErrorHandler.ThrowOnFailure(windowFrame.Hide());
        }

        private uint solutionEventCookie;

        protected override void Initialize() {
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof (IMenuCommandService)) as OleMenuCommandService;
            if (null == mcs) return;

            // Create the command for the menu item.
            CommandID menuCommandID = new CommandID(GuidList.guidplvsCmdSet, (int) PkgCmdIDList.cmdidToggleToolWindow);
            MenuCommand menuItem = new MenuCommand(MenuItemCallback, menuCommandID);
            mcs.AddCommand(menuItem);

#if (TOOLWINDOW_MENUITEM)
    // Create the command for the tool window
            CommandID toolwndCommandID = new CommandID(GuidList.guidplvsCmdSet,
                                                       (int) PkgCmdIDList.cmdidAtlassianToolWindow);
            MenuCommand menuToolWin = new MenuCommand(ShowToolWindow, toolwndCommandID);
            mcs.AddCommand(menuToolWin);
#endif

            DTE dte = (DTE) GetService(typeof (DTE));

            SolutionEventSink solutionEventSink = new SolutionEventSink(dte, createJiraWindow);

            IVsSolution solution = (IVsSolution) GetService(typeof (SVsSolution));
            ErrorHandler.ThrowOnFailure(solution.AdviseSolutionEvents(solutionEventSink, out solutionEventCookie));

            createIssueDetailsWindow();
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
    }
}