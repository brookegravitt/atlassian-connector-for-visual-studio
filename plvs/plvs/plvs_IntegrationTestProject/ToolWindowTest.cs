using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VsSDK.IntegrationTestLibrary;
using Microsoft.VSSDK.Tools.VsIdeTesting;

namespace plvs_IntegrationTestProject {
    [TestClass]
    public class ToolWindowTest {
        private delegate void ThreadInvoker();

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        /// <summary>
        ///A test for showing the toolwindow
        ///</summary>
        [TestMethod]
        [HostType("VS IDE")]
        public void ShowToolWindow() {
            UIThreadInvoker.Invoke((ThreadInvoker) delegate {
                                                       CommandID toolWindowCmd =
                                                           new CommandID(Atlassian.plvs.GuidList.guidplvsCmdSet,
                                                                         (int)
                                                                         Atlassian.plvs.PkgCmdIDList.
                                                                             cmdidAtlassianToolWindow);

                                                       TestUtils testUtils = new TestUtils();
                                                       testUtils.ExecuteCommand(toolWindowCmd);

                                                       Assert.IsTrue(
                                                           testUtils.CanFindToolwindow(
                                                               new Guid(
                                                                   Atlassian.plvs.GuidList.
                                                                       guidToolWindowPersistanceString)));
                                                   });
        }
    }
}