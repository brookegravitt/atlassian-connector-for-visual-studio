using System.Collections.Generic;
using System.Windows.Forms;
using Atlassian.plvs.api;
using Atlassian.plvs.models;

namespace Atlassian.plvs.ui.fields {
    public class IssueTypeFieldEditor : JiraFieldEditor {
        private readonly JiraNamedEntityComboBox combo = new JiraNamedEntityComboBox();

        public IssueTypeFieldEditor(JiraIssue issue, IEnumerable<JiraNamedEntity> issueTypes) {
            ImageList imageList = new ImageList();
            int i = 0;
            int selectedIndex = -1;
            foreach (var type in issueTypes) {
                ComboBoxWithImagesItem<JiraNamedEntity> item = new ComboBoxWithImagesItem<JiraNamedEntity>(type, i);
                imageList.Images.Add(ImageCache.Instance.getImage(type.IconUrl));
                combo.Items.Add(item);
                if (issue.IssueTypeId == type.Id) {
                    selectedIndex = i;
                }
                ++i;
            }
            
            combo.ImageList = imageList;
            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.Height = SINGLE_LINE_EDITOR_HEIGHT + 6;

            combo.ItemHeight = SINGLE_LINE_EDITOR_HEIGHT;

            if (selectedIndex != -1) {
                combo.SelectedIndex = selectedIndex;
            }
        }

        public override Control Widget {
            get { return combo; }
        }

        public override int VerticalSkip {
            get { return combo.Height; }
        }

        public override void resizeToWidth(int width) {
            combo.Width = width;
        }
    }
}