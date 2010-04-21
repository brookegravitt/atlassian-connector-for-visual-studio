using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui;
using Atlassian.plvs.ui.jira;

namespace Atlassian.plvs.dialogs.jira {
    public class DeactivateIssue : LogWork {
        private readonly Action onFinished;
        private readonly CheckBox checkBoxLogWork;
        private CheckBox checkBoxRunAction;
        private ComboBox cbActions;

        public DeactivateIssue(
            Control parent, JiraIssueListModel model, JiraServerFacade facade,
            JiraIssue issue, StatusLabel status, JiraActiveIssueManager activeIssueManager, 
            IEnumerable<JiraNamedEntity> actions, Action onFinished) 
            : base(parent, model, facade, issue, status, activeIssueManager) {
            
            this.onFinished = onFinished;

            setOkButtonName("Stop Work");

            SuspendLayout();

            Controls.Remove(LogWorkPanel);

            checkBoxLogWork = new CheckBox { AutoSize = true, Text = "Log Work", Location = new Point(10, 10) };
            checkBoxLogWork.CheckedChanged += (s, e) => {
                                                  LogWorkPanel.Enabled = checkBoxLogWork.Checked;
                                                  updateOkButtonState();
                                              };
            GroupBox group = new GroupBox {
                                 Size = new Size(LogWorkPanel.Width + 20, LogWorkPanel.Height + 20),
                                 Location = new Point(10, checkBoxLogWork.Location.Y + checkBoxLogWork.Height - 3)
                             };
            group.Controls.Add(LogWorkPanel);

            LogWorkPanel.Location = new Point(5, 15);
            LogWorkPanel.Enabled = checkBoxLogWork.Checked;

            checkBoxRunAction = new CheckBox {
                                                 AutoSize = true,
                                                 Text = "Run Issue Action",
                                                 Location = new Point(10, group.Location.Y + group.Height + 20)
                                             };
            cbActions = new ComboBox {
                                         DropDownStyle = ComboBoxStyle.DropDownList,
                                         Location = new Point(150, group.Location.Y + group.Height + 17)
                                     };
            checkBoxRunAction.CheckedChanged += (s, e) => {
                                                    cbActions.Enabled = checkBoxRunAction.Checked;
                                                    updateOkButtonState();
                                                };

            foreach (var action in actions) {
                cbActions.Items.Add(action);
            }
            cbActions.SelectedValueChanged += (s, e) => updateOkButtonState();
            cbActions.Enabled = checkBoxRunAction.Checked;

            Size = new Size(Size.Width + 40, Size.Height + 100);
            
            Controls.Add(checkBoxLogWork);
            Controls.Add(group);
            Controls.Add(checkBoxRunAction);
            Controls.Add(cbActions);

            ButtonOk.Location = new Point(ButtonOk.Location.X + 40, ButtonOk.Location.Y + 100);
            ButtonCancel.Location = new Point(ButtonCancel.Location.X + 40, ButtonCancel.Location.Y + 100);
            ResumeLayout(true);
        }

        protected override void onOk(Action finished, bool closeDialogOnFinish) {
            if (checkBoxLogWork.Checked && !checkBoxRunAction.Checked) {
                base.onOk(onFinished, true);
            } else if (checkBoxLogWork.Checked && checkBoxRunAction.Checked) {
                base.onOk(() => {
                              // todo - run action
                              Close();
                              onFinished();
                          }, false);
            } else if (checkBoxRunAction.Checked) {
                // todo - run action
                Close();
                onFinished();
            } else {
                // todo - run action
                Close();
                onFinished();
            }
        }

        protected override void updateOkButtonState() {
            if (checkBoxLogWork.Checked) {
                base.updateOkButtonState();
                if (checkBoxRunAction.Checked && ButtonOk.Enabled) {
                    ButtonOk.Enabled = cbActions.SelectedItem != null;
                }
            } else {
                ButtonOk.Enabled = !checkBoxRunAction.Checked || cbActions.SelectedItem != null;
            }
        }

        protected override string getDialogName() {
            return "Stop Work on Issue " + issue.Key;
        }
    }
}
