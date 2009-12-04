﻿using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.TextManager.Interop;
using System.Runtime.InteropServices;

namespace Atlassian.plvs.markers {
    [Guid(GuidList.GUID_JIRA_LINK_MARKER_SERVICE_STRING)]
    public sealed class JiraLinkMarkerTypeProvider : IVsTextMarkerTypeProvider {
        private readonly JiraLinkMarginMarkerType marginMarkerType = new JiraLinkMarginMarkerType();
        private readonly JiraLinkTextMarkerType textMarkerType = new JiraLinkTextMarkerType();

        public int GetTextMarkerType(ref Guid pguidMarker, out IVsPackageDefinedTextMarkerType ppMarkerType) {
            // This method is called by Visual Studio when it needs the marker
            // type information in order to retrieve the implementing objects.
            if (pguidMarker == GuidList.JiraLinkMarginMarker) {
                ppMarkerType = marginMarkerType;
                return VSConstants.S_OK;
            }

            if (pguidMarker == GuidList.JiraLinkTextMarker) {
                ppMarkerType = textMarkerType;
                return VSConstants.S_OK;
            }

            ppMarkerType = null;
            return VSConstants.E_FAIL;
        }

        internal static void InitializeMarkerIds(PlvsPackage package) {
            // Retrieve the Text Marker IDs. We need them to be able to create instances.
            IVsTextManager textManager = (IVsTextManager) package.GetService(typeof (SVsTextManager));

            int markerId;
            Guid markerGuid = GuidList.JiraLinkMarginMarker;
            ErrorHandler.ThrowOnFailure(textManager.GetRegisteredMarkerTypeID(ref markerGuid, out markerId));
            JiraLinkMarginMarkerType.Id = markerId;

            markerGuid = GuidList.JiraLinkTextMarker;
            ErrorHandler.ThrowOnFailure(textManager.GetRegisteredMarkerTypeID(ref markerGuid, out markerId));
            JiraLinkTextMarkerType.Id = markerId;
        }
    }
}