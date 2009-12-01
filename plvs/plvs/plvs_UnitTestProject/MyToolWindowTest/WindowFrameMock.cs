/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using Microsoft.VsSDK.UnitTestLibrary;
using Microsoft.VisualStudio.Shell.Interop;

namespace plvs_UnitTestProject.MyToolWindowTest {
    internal class WindowFrameMock {
        private const string propertiesName = "properties";

        private static GenericMockFactory frameFactory;

        /// <summary>
        /// Return a IVsWindowFrame without any special implementation
        /// </summary>
        /// <returns></returns>
        internal static IVsWindowFrame GetBaseFrame() {
            if (frameFactory == null)
                frameFactory = new GenericMockFactory("WindowFrame", new[] {typeof (IVsWindowFrame)});
            IVsWindowFrame frame = (IVsWindowFrame) frameFactory.GetInstance();
            return frame;
        }
    }
}