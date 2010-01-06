using System.Drawing;
using System.Windows.Forms;
using Atlassian.plvs.util;

namespace Atlassian.plvs.ui.fields {
    public class UserFieldEditor : JiraFieldEditor {
        private readonly Panel panel = new Panel();

        private readonly TextBox userBox = new TextBox
                                  {
                                      Location = new Point(0, 0)
                                  };

        private readonly Label infoLabel = new Label
                                  {
                                      AutoSize = true,
                                      Text = Constants.USER_NOT_VALUDATED,
                                      Location = new Point(140, 3),
                                  };

        public UserFieldEditor(string userName, FieldValidListener validListener) : base(validListener) {
            userBox.Width = 120;
            if (userName != null) {
                userBox.Text = userName;
            }
            infoLabel.Font = new Font(infoLabel.Font.FontFamily, infoLabel.Font.Size - 2);

            panel.Height = userBox.Height;
            panel.Width = 300 + infoLabel.Location.X;

            panel.Controls.Add(userBox);
            panel.Controls.Add(infoLabel);
        }

        public override Control Widget {
            get { return panel; }
        }

        public override int VerticalSkip {
            get { return panel.Height; }
        }

        public override void resizeToWidth(int width) {
            panel.Width = width;
        }
    }
}