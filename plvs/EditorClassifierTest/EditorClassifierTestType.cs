using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace EditorClassifierTest {
    internal static class EditorClassifierTestClassificationDefinition {
        /// <summary>
        /// Defines the "EditorClassifierTest" classification type.
        /// </summary>
        [Export(typeof(ClassificationTypeDefinition))]
        [Name("EditorClassifierTest")]
        internal static ClassificationTypeDefinition EditorClassifierTestType = null;
    }
}
