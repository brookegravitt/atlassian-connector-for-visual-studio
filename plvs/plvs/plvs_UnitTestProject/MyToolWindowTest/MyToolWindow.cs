/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Atlassian.plvs;

namespace plvs_UnitTestProject.MyToolWindowTest {
    /// <summary>
    ///This is a test class for MyToolWindowTest and is intended
    ///to contain all MyToolWindowTest Unit Tests
    ///</summary>
    [TestClass]
    public class MyToolWindowTest {
        /// <summary>
        ///AtlassianToolWindow Constructor test
        ///</summary>
        [TestMethod]
        public void MyToolWindowConstructorTest() {
            AtlassianToolWindow target = new AtlassianToolWindow();
            Assert.IsNotNull(target, "Failed to create an instance of AtlassianToolWindow");

            FieldInfo field = target.GetType().GetField("control", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(field.GetValue(target), "MyControl object was not instantiated");
        }

        public void WindowPropertyTest() {
            AtlassianToolWindow target = new AtlassianToolWindow();
            Assert.IsNotNull(target.Window, "Window property was null");
        }
    }
}