using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.util;

namespace Atlassian.plvs.ui.jira.fields {
    public class TimeTrackingEditor : JiraFieldEditor {

        private readonly Panel panel = new Panel();

        private readonly TextBox trackingBox = new TextBox {
                                                               Location = new Point(0, 0)
                                                           };

        private readonly Label infoLabel = new Label {
                                                         AutoSize = true,
                                                         Text = Constants.TIME_TRACKING_SYNTAX,
                                                         Location = new Point(140, 3),
                                                     };

        public TimeTrackingEditor(JiraField field, string value, FieldValidListener validListener)
            : base(field, validListener) {
            trackingBox.TextChanged += trackingBox_TextChanged;
            trackingBox.Width = 120;
            if (value != null) {
                trackingBox.Text = value;
            }
            infoLabel.Font = new Font(infoLabel.Font.FontFamily, infoLabel.Font.Size - 2);

            panel.Height = trackingBox.Height;
            panel.Width = 300 + infoLabel.Location.X;

            panel.Controls.Add(trackingBox);
            panel.Controls.Add(infoLabel);
        }

        private void trackingBox_TextChanged(object sender, EventArgs e) {
            Regex regex = new Regex(@"^\s*(\d+w\s*)?(\d+d\s*)?(\d+h\s*)?(\d+m\s*)?$"); 
            if (String.IsNullOrEmpty(trackingBox.Text) || regex.IsMatch(trackingBox.Text)) {
                trackingBox.ForeColor = Color.Black;
                FieldValid = true;
            } else {
                trackingBox.ForeColor = Color.Red;
                FieldValid = false;
            }
        }

        public override Control Widget {
            get { return panel; }
        }

        public override int VerticalSkip {
            get { return trackingBox.Height; }
        }

        public override void resizeToWidth(int width) {
            panel.Width = width;
        }

        public override string getFieldLabel(JiraIssue issue, JiraField field) {
            return issue.TimeSpent == null ? "Original Estimate" : "Remaining Estimate";
        }

        public override List<string> getValues() {
            return FieldValid ? new List<string> { trackingBox.Text.Trim()} : new List<string>();
        }
    }
}