using System;
using System.ComponentModel.Design;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using Atlassian.plvs.attributes;
using Atlassian.plvs.autoupdate;
using Atlassian.plvs.eventsinks;
using Atlassian.plvs.markers;
using Atlassian.plvs.scm;
using Atlassian.plvs.store;
using Atlassian.plvs.ui.jira;
using Atlassian.plvs.windows;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.Win32;

namespace Atlassian.plvs {
    [ProvideLoadKey(MINIMUM_VISUAL_STUDIO_EDITION, "1.0", PRODUCT_NAME, COMPANY, 1)]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [DefaultRegistryRoot("Software\\Microsoft\\VisualStudio\\9.0")]
    [InstalledProductRegistration(true, null, null, null)]
    [ProvideMenuResource(1000, 1)]
    [ProvideToolWindow(typeof (AtlassianToolWindow), Transient = true, Style = VsDockStyle.Tabbed,
        Orientation = ToolWindowOrientation.Bottom)]
    [ProvideToolWindow(typeof (IssueDetailsToolWindow), Transient = true, Style = VsDockStyle.Tabbed,
        Orientation = ToolWindowOrientation.Bottom)]
    [ProvideAutoLoad(UIContextGuids.NoSolution)]
    [ProvideToolWindowVisibility(typeof (AtlassianToolWindow), UIContextGuids.SolutionExists)]
    [ProvideToolWindowVisibility(typeof (IssueDetailsToolWindow), UIContextGuids.SolutionExists)]
    [ProvideIssueRepositoryConnector(typeof(AnkhSvnJiraConnector), AnkhSvnJiraConnector.ANKH_CONNECTOR_NAME, typeof(PlvsPackage), "#113")]
    [Guid(GuidList.guidplvsPkgString)]
    public sealed class PlvsPackage : Package, IVsPersistSolutionOpts, IVsInstalledProduct {
        public const string MINIMUM_VISUAL_STUDIO_EDITION = "Standard";
        public const string PRODUCT_NAME = "Atlassian Connector for Visual Studio";
        public const string DESCRIPTION = "Blah";
        public const string COMPANY = "Atlassian";

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
            AtlassianPanel.Instance.WindowFrame = window;
            ErrorHandler.ThrowOnFailure(windowFrame.Show());
            return window;
        }

        private ToolWindowPane createIssueDetailsWindow() {
            ToolWindowPane window = FindToolWindow(typeof (IssueDetailsToolWindow), 0, true);
            if ((null == window) || (null == window.Frame)) {
                throw new NotSupportedException(Resources.CanNotCreateWindow);
            }
            IVsWindowFrame windowFrame = (IVsWindowFrame) window.Frame;
            IssueDetailsWindow.Instance.WindowFrame = window;
            IVsWindowFrame2 wf2 = (IVsWindowFrame2) windowFrame;
            uint cookie;
            wf2.Advise(IssueDetailsWindow.Instance, out cookie);
            ErrorHandler.ThrowOnFailure(windowFrame.Hide());
            return window;
        }

        private uint solutionEventCookie;
        private IConnectionPoint tmConnectionPoint;
        private uint tmConnectionCookie;
        private uint rdtEventCookie;

        // this method is no longer used
        public int IdBmpSplash(out uint pIdBmp) {
            pIdBmp = 0;
            return VSConstants.E_NOTIMPL;
        }

        public int IdIcoLogoForAboutbox(out uint pIdIco) {
            pIdIco = 600;
            return VSConstants.S_OK;
        }

        public int OfficialName(out string pbstrName) {
            pbstrName = VSPackage._110;
            return VSConstants.S_OK;
        }

        public int ProductID(out string str) {
            str = PlvsVersionInfo.Version + "-" + PlvsVersionInfo.Stamp;
            return VSConstants.S_OK;
        }

        public int ProductDetails(out string pbstrProductDetails) {
            pbstrProductDetails = VSPackage._112;
            return VSConstants.S_OK;
        }

        public int SaveUserOptions(IVsSolutionPersistence pPersistence) {
            return ParameterStoreManager.Instance.SaveUserOptions(pPersistence);
        }

        public int LoadUserOptions(IVsSolutionPersistence pPersistence, uint grfLoadOpts) {
            return ParameterStoreManager.Instance.LoadUserOptions(pPersistence, grfLoadOpts);
        }

        public int WriteUserOptions(IStream pOptionsStream, string pszKey) {
            return ParameterStoreManager.Instance.WriteUserOptions(pOptionsStream, pszKey);
        }

        public int ReadUserOptions(IStream pOptionsStream, string pszKey) {
            return ParameterStoreManager.Instance.ReadUserOptions(pOptionsStream, pszKey);
        }

        protected override void Initialize() {
            base.Initialize();

            if (InCommandLineMode) {
                return;
            }

            OleMenuCommandService mcs = GetService(typeof (IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs) {
                CommandID menuCommandId = new CommandID(GuidList.guidplvsCmdSet,
                                                        (int) PkgCmdIDList.cmdidToggleToolWindow);
                OleMenuCommand menuItem = new OleMenuCommand(MenuItemCallback, menuCommandId);
                menuItem.BeforeQueryStatus += menuItem_BeforeQueryStatus;

                mcs.AddCommand(menuItem);
            }

            JiraLinkMarkerTypeProvider markerTypeProvider = new JiraLinkMarkerTypeProvider();
            ((IServiceContainer) this).AddService(markerTypeProvider.GetType(), markerTypeProvider, true);

            // Now it's time to initialize our copies of the marker IDs. We need them to be
            // able to create marker instances.
            JiraLinkMarkerTypeProvider.InitializeMarkerIds(this);

            SolutionEventSink solutionEventSink = new SolutionEventSink(this, createJiraWindow, createIssueDetailsWindow);
            TextManagerEventSink textManagerEventSink = new TextManagerEventSink();
            RunningDocTableEventSink runningDocTableEventSink = new RunningDocTableEventSink();

            IVsSolution solution = (IVsSolution) GetService(typeof (SVsSolution));
            ErrorHandler.ThrowOnFailure(solution.AdviseSolutionEvents(solutionEventSink, out solutionEventCookie));

            IConnectionPointContainer textManager = (IConnectionPointContainer) GetService(typeof (SVsTextManager));
            Guid interfaceGuid = typeof (IVsTextManagerEvents).GUID;
            try {
                textManager.FindConnectionPoint(ref interfaceGuid, out tmConnectionPoint);
                tmConnectionPoint.Advise(textManagerEventSink, out tmConnectionCookie);
// ReSharper disable EmptyGeneralCatchClause
            }
            catch {}
// ReSharper restore EmptyGeneralCatchClause

            IVsRunningDocumentTable rdt = (IVsRunningDocumentTable) GetService(typeof (SVsRunningDocumentTable));
            ErrorHandler.ThrowOnFailure(rdt.AdviseRunningDocTableEvents(runningDocTableEventSink, out rdtEventCookie));

            // Since we register custom text markers we have to ensure the font and color
            // cache is up-to-date.
            ValidateFontAndColorCacheManagerIsUpToDate();

            ((IServiceContainer)this).AddService(typeof(AnkhSvnJiraConnector), new ServiceCreatorCallback(CreateAnkhSvnConnector), true);
        }

        private object CreateAnkhSvnConnector(IServiceContainer container, Type servicetype) {
            if (container != this) {
                return null;
            }

            if (typeof(AnkhSvnJiraConnector) == servicetype) {
                AnkhSvnJiraConnector provider = new AnkhSvnJiraConnector();
                return provider;
            }

            return null;
        }

        // 
        // no idea what this does really :) - resets some sort of cache. Apparently this is needed
        // for text markers to work properly. Taken from http://www.codeplex.com/CloneDetectiveVS
        //
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void ValidateFontAndColorCacheManagerIsUpToDate() {
            IVsFontAndColorCacheManager cacheManager =
                (IVsFontAndColorCacheManager) GetService(typeof (SVsFontAndColorCacheManager));
            if (cacheManager == null)
                return;

            bool alreadyInitialized = false;

            try {
                const string registryValueName = "InstalledVersion";
                string expectedVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

                using (RegistryKey rootKey = UserRegistryRoot)
                using (RegistryKey ourKey = rootKey.CreateSubKey(PRODUCT_NAME)) {
                    if (ourKey != null) {
                        object registryValue = ourKey.GetValue(registryValueName);
                        string initializedVersion = Convert.ToString(registryValue, CultureInfo.InvariantCulture);
                        alreadyInitialized = (initializedVersion == expectedVersion);
                        ourKey.SetValue(registryValueName, expectedVersion, RegistryValueKind.String);
                    }
                }
            }
// ReSharper disable EmptyGeneralCatchClause
            catch
// ReSharper restore EmptyGeneralCatchClause
            {
                // Ignore any errors since it's not a big deal if we can't read
                // this setting. We just always refresh the cache in that case.
            }

            // Actually refresh the Fonts and Colors cache now if we detected we have
            // to do so.
            if (alreadyInitialized) return;

            ErrorHandler.ThrowOnFailure(cacheManager.ClearAllCaches());
            Guid categoryGuid = Guid.Empty;
            ErrorHandler.ThrowOnFailure(cacheManager.RefreshCache(ref categoryGuid));
            categoryGuid = GuidList.GuidFontsAndColorsTextEditor;
            ErrorHandler.ThrowOnFailure(cacheManager.RefreshCache(ref categoryGuid));
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

            // Remove text manager event notifications.
            if (tmConnectionPoint != null) {
                tmConnectionPoint.Unadvise(tmConnectionCookie);
                tmConnectionPoint = null;
            }

            // Remove running document table (RDT) event notifications.
            // Ignore any errors that might occur since we're shutting down.
            IVsRunningDocumentTable rdt = (IVsRunningDocumentTable) GetService(typeof (SVsRunningDocumentTable));
            if (rdt != null) {
                try {
                    rdt.UnadviseRunningDocTableEvents(rdtEventCookie);
                } catch (Exception) {}
            }
        }

        private bool? inCommandLineMode;

        //
        // taken from AnkhSVN code
        //
        public bool InCommandLineMode {
            get {
                if (!inCommandLineMode.HasValue) {
                    IVsShell shell = (IVsShell) GetService(typeof (SVsShell));

                    if (shell == null)
                        inCommandLineMode = false; // Probably running in a testcase; the shell loads us!
                    else {
                        object value;
                        if (ErrorHandler.Succeeded(shell.GetProperty((int) __VSSPROPID.VSSPROPID_IsInCommandLineMode, out value))) {
                            inCommandLineMode = Convert.ToBoolean(value);
                        }
                    }
                }

                return inCommandLineMode.Value;
            }
        }

        private static void MenuItemCallback(object sender, EventArgs e) {
            ToolWindowManager.Instance.AtlassianWindowVisible = !ToolWindowManager.Instance.AtlassianWindowVisible;
        }

        private static void menuItem_BeforeQueryStatus(object sender, EventArgs e) {
            bool enable = ToolWindowManager.Instance.AtlassianWindow != null;
            var myCommand = sender as OleMenuCommand;
            if (myCommand != null) myCommand.Enabled = enable;
        }
    }
}