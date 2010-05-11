using System;
using Atlassian.plvs.markers;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Atlassian.plvs.eventsinks {
    public sealed class TextBufferDataEventSink : IVsTextBufferDataEvents {
        public IVsTextLines TextLines { get; set; }

        public IConnectionPoint ConnectionPoint { get; set; }

        public uint Cookie { get; set; }

        public void OnFileChanged(uint grfChange, uint dwFileAttrs) {}

        public int OnLoadCompleted(int fReload) {
            ConnectionPoint.Unadvise(Cookie);

            bool sharp = isCSharpOrCppOrC(TextLines);
            if (sharp || isVb(TextLines)) {
                JiraEditorLinkManager.OnDocumentOpened(TextLines, sharp 
                    ? JiraEditorLinkManager.BufferType.CSHARP_OR_C_OR_CPP
                    : JiraEditorLinkManager.BufferType.VISUAL_BASIC);
            }

            return VSConstants.S_OK;
        }

        private static bool isCSharpOrCppOrC(IVsTextLines textLines) {
            Guid languageServiceId;
            textLines.GetLanguageServiceID(out languageServiceId);
            return GuidList.CSHARP_LANGUAGE_GUID.Equals(languageServiceId) 
                || GuidList.C_AND_CPP_LANGUAGE_GUID.Equals(languageServiceId);
        }

        private static bool isVb(IVsTextLines textLines) {
            Guid languageServiceId;
            textLines.GetLanguageServiceID(out languageServiceId);
            return GuidList.VB_LANGUAGE_GUID.Equals(languageServiceId);
        }
    }
}