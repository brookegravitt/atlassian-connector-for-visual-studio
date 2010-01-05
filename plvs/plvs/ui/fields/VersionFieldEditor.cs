using System.Collections.Generic;
using System.Windows.Forms;
using Atlassian.plvs.api;

namespace Atlassian.plvs.ui.fields {
    public class VersionFieldEditor : JiraFieldEditor {
        private readonly ListBox list = new ListBox
                               {
                                   SelectionMode = SelectionMode.MultiExtended,
                                   Height = MULTI_LINE_EDITOR_HEIGHT
                               };

        public VersionFieldEditor(IEnumerable<string> selectedVersions, IEnumerable<JiraNamedEntity> versions) {
            List<JiraNamedEntity> selected = new List<JiraNamedEntity>();

            foreach (JiraNamedEntity version in versions) {
                foreach (var selectedVersion in selectedVersions) {
                    if (!version.Name.Equals(selectedVersion)) continue;
                    selected.Add(version);
                    break;
                }
                list.Items.Add(version);
            }
            foreach (JiraNamedEntity sel in selected) {
                list.SelectedItems.Add(sel);
            }
        }

        public override Control Widget {
            get { return list; }
        }

        public override int VerticalSkip {
            get { return list.Height; }
        }

        public override void resizeToWidth(int width) {
            list.Width = width;
        }
    }
}