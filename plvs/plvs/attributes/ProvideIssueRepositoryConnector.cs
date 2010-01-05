/* 
 * kalamon: Taken from CollabNetDesktop
 * 
 * ProvideIssueRepositoryConnector.cs
 * @author baybora
 * 
 * Copyright (c) 2008-2009 CollabNet Corporation ("CollabNet" - http://www.collab.net), 
 * Licensed under the Apache License, Version 2.0 (the "License"); 
 * you may not use this file except in compliance with the License. 
 * You may obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software 
 * distributed under the License is distributed on an "AS IS" BASIS, 
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
 * See the License for the specific language governing permissions and 
 * limitations under the License.
 * 
 **/

using System;
using Microsoft.VisualStudio.Shell;
using System.Globalization;

namespace Atlassian.plvs.attributes {
    /// <summary>
    /// This attribute registers the package as Issue Repository Connector.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
#if ORIGINAL
    [System.Runtime.InteropServices.Guid("072C0B48-7BCC-49ce-927C-1EC92279E8CC")]
#else 
    [System.Runtime.InteropServices.Guid("9D9C054A-BF4F-4e25-A1A3-B55EE950660D")]
#endif
    public sealed class ProvideIssueRepositoryConnector : RegistrationAttribute {
        private const string REG_KEY_CONNECTORS = "IssueRepositoryConnectors";
        private const string REG_KEY_NAME = "Name";
        private const string REG_VALUE_SERVICE = "Service";
        private const string REG_VALUE_PACKAGE = "Package";

        private Type _connectorService = null;
        private string _regName = null;
        private string _uiName = null;
        private Type _uiNamePkg = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectorServiceType">Type of the connector service</param>
        /// <param name="regName">Connector name in the registry</param>
        /// <param name="uiNamePkg">Unique identifier (Guid) of the package that proffers connector service</param>
        /// <param name="uiName">String resource id that represents ui name of the connector</param>
        public ProvideIssueRepositoryConnector(Type connectorServiceType, string regName, Type uiNamePkg, string uiName) {
            //System.Diagnostics.Debug.Assert(typeof(IIssueRepositoryConnector).IsAssignableFrom(connectorServiceType), "Issue Repository Connector must implement IIssueRepositoryConnector interface");
            _connectorService = connectorServiceType;
            _regName = regName;
            _uiNamePkg = uiNamePkg;
            _uiName = uiName;
        }

        /// <summary>
        /// Gets Issue repository connector service's global identifier.
        /// </summary>
        public Guid IssueRepositoryConnectorService {
            get { return _connectorService.GUID; }
        }

        /// <summary>
        /// Gets the name of the issue repository connector (used in registry)
        /// </summary>
        public string RegName {
            get { return _regName; }
        }

        /// <summary>
        /// Gets the global identifier used to register the issue repository connector
        /// </summary>
        public Guid RegGuid {
            get { return IssueRepositoryConnectorService; }
        }

        /// <summary>
        /// Gets the package identifier the proffers the connector service.
        /// </summary>
        public Guid UINamePkg {
            get { return _uiNamePkg.GUID; }
        }

        /// <summary>
        /// Gets the string resource identifier that represents the UI name of the issue tracker repository connector.
        /// </summary>
        public string UIName {
            get { return _uiName; }
        }

        public override void Register(RegistrationContext context) {
            // Write to the context's log what we are about to do
            context.Log.WriteLine(String.Format(CultureInfo.CurrentCulture,
                                                "Issue Repository Connector:\t\t{0}\n", RegName));

            // Declare the issue repository connector, its name, the provider's service 
            using (Key connectors = context.CreateKey(REG_KEY_CONNECTORS)) {
                using (Key connectorKey = connectors.CreateSubkey(RegGuid.ToString("B").ToUpperInvariant())) {
                    connectorKey.SetValue("", RegName);
                    connectorKey.SetValue(REG_VALUE_SERVICE,
                                          IssueRepositoryConnectorService.ToString("B").ToUpperInvariant());

                    using (Key connectorNameKey = connectorKey.CreateSubkey(REG_KEY_NAME)) {
                        connectorNameKey.SetValue("", UIName);
                        connectorNameKey.SetValue(REG_VALUE_PACKAGE, UINamePkg.ToString("B").ToUpperInvariant());

                        connectorNameKey.Close();
                    }
                    connectorKey.Close();
                }
                connectors.Close();
            }
        }

        public override void Unregister(RegistrationContext context) {
            context.RemoveKey(REG_KEY_CONNECTORS + "\\" + RegGuid.ToString("B"));
        }
    }
}
