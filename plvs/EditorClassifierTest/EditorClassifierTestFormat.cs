using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace EditorClassifierTest {
    #region Format definition
    /// <summary>
    /// Defines an editor format for the EditorClassifierTest type that has a purple background
    /// and is underlined.
    /// </summary>
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "EditorClassifierTest")]
    [Name("EditorClassifierTest")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class EditorClassifierTestFormat : ClassificationFormatDefinition {
        /// <summary>
        /// Defines the visual format for the "EditorClassifierTest" classification type
        /// </summary>
        public EditorClassifierTestFormat() {
            this.DisplayName = "EditorClassifierTest"; //human readable version of the name
//            this.BackgroundColor = Colors.BlueViolet;
//            this.TextDecorations = System.Windows.TextDecorations.Underline;
        }
    }
    #endregion //Format definition
}
