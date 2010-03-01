using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Atlassian.plvs.api.jira;
using Atlassian.plvs.autoupdate;
using Atlassian.plvs.models.jira;
using Atlassian.plvs.ui;
using Atlassian.plvs.util;
using Atlassian.plvs.util.jira;

namespace Atlassian.plvs.dialogs.jira {
    public sealed partial class LogWork : Form {
        private readonly Control parent;
        private readonly JiraIssueListModel model;
        private readonly JiraServerFacade facade;
        private readonly JiraIssue issue;
        private readonly StatusLabel status;

        private DateTime endTime;

        public LogWork(Control parent, JiraIssueListModel model, JiraServerFacade facade, JiraIssue issue, StatusLabel status) {
            this.parent = parent;
            this.model = model;
            this.facade = facade;
            this.issue = issue;
            this.status = status;
            InitializeComponent();

            Text = "Log for for issue " + issue.Key;

            endTime = DateTime.Now;

            setEndTimeLabelText();

            textRemainingEstimate.Enabled = false;
            radioAutoUpdate.Checked = true;
            textExplanation.Font = new Font(textExplanation.Font.FontFamily, textExplanation.Font.Size - 1);

            updateOkButtonState();

            StartPosition = FormStartPosition.CenterParent;
        }

        private void setEndTimeLabelText() {
            labelEndTime.Text = endTime.ToShortDateString() + " " + endTime.ToShortTimeString();
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            Close();
        }

        private void buttonChange_Click(object sender, EventArgs e) {
            SetEndTime dlg = new SetEndTime(endTime);
            DialogResult result = dlg.ShowDialog();
            if (result != DialogResult.OK) {
                return;
            }
            endTime = dlg.DateTime;
            setEndTimeLabelText();
        }

        private void radioUpdateManually_CheckedChanged(object sender, EventArgs e) {
            textRemainingEstimate.Enabled = radioUpdateManually.Checked;
            updateOkButtonState();
        }

        private void textTimeSpent_TextChanged(object sender, EventArgs e) {
            updateOkButtonState();
        }

        private void textRemainingEstimate_TextChanged(object sender, EventArgs e) {
            updateOkButtonState();
        }

        private void updateOkButtonState() {
            bool timeSpentOk;
            Regex regex = new Regex(Constants.TIME_TRACKING_REGEX);
            if (textTimeSpent.Text.Length > 0 && regex.IsMatch(textTimeSpent.Text)) {
                textTimeSpent.ForeColor = Color.Black;
                timeSpentOk = true;
            } else {
                textTimeSpent.ForeColor = Color.Red;
                timeSpentOk = false;
            }

            bool remainingOk;

            if (!radioUpdateManually.Checked
                || (textRemainingEstimate.Text.Length > 0 && regex.IsMatch(textRemainingEstimate.Text))) {
                textRemainingEstimate.ForeColor = Color.Black;
                remainingOk = true;
            } else {
                textRemainingEstimate.ForeColor = Color.Red;
                remainingOk = false;
            }

            buttonOk.Enabled = timeSpentOk && remainingOk;
        }

        private void buttonOk_Click(object sender, EventArgs e) {
            Action a;

            if (radioAutoUpdate.Checked) {
                a = logWorkAndAutoUpdateRemaining;
            } else if (radioLeaveUnchanged.Checked) {
                a = logWorkAndLeaveRemainingUnchanged;
            } else {
                a = logWorkAndUpdateRemainingManually;
            }

            Close();
            Thread t = new Thread(() => logWorkWorker(a));
            t.Start();
        }

        private void logWorkAndAutoUpdateRemaining() {
            facade.logWorkAndAutoUpdateRemaining(issue, JiraIssueUtils.addSpacesToTimeSpec(textTimeSpent.Text), getStartTime());
        }

        private void logWorkAndLeaveRemainingUnchanged() {
            facade.logWorkAndLeaveRemainingUnchanged(issue, JiraIssueUtils.addSpacesToTimeSpec(textTimeSpent.Text), getStartTime());
        }

        private void logWorkAndUpdateRemainingManually() {
            facade.logWorkAndUpdateRemainingManually(issue, JiraIssueUtils.addSpacesToTimeSpec(textTimeSpent.Text),
                                                     getStartTime(), JiraIssueUtils.addSpacesToTimeSpec(textRemainingEstimate.Text));
        }

        private void logWorkWorker(Action action) {
            try {
                status.setInfo("Logging work for issue " + issue.Key + "...");
                action();
                status.setInfo("Logged work for issue " + issue.Key);
                UsageCollector.Instance.bumpJiraIssuesOpen();
                JiraIssue updatedIssue = facade.getIssue(issue.Server, issue.Key);
                parent.safeInvoke(new MethodInvoker(() => model.updateIssue(updatedIssue)));
            } catch (Exception e) {
                status.setError("Failed to log work for issue " + issue.Key, e);
            }
        }

        private DateTime getStartTime() {
            Regex regex = new Regex(Constants.TIME_TRACKING_REGEX);
            Match match = regex.Match(textTimeSpent.Text);
            Group @groupWeeks = match.Groups[2];
            Group @groupDays = match.Groups[4];
            Group @groupHours = match.Groups[6];
            Group @groupMinutes = match.Groups[8];

            DateTime result = endTime;
            if (groupWeeks != null && groupWeeks.Success) {
                result = result.AddDays(-7*double.Parse(groupWeeks.Value));
            }
            if (groupDays != null && groupDays.Success) {
                result = result.AddDays(-1*double.Parse(groupDays.Value));
            }
            if (groupHours != null && groupHours.Success) {
                result = result.AddHours(-1*double.Parse(groupHours.Value));
            }
            if (groupMinutes != null && groupMinutes.Success) {
                result = result.AddMinutes(-1*double.Parse(groupMinutes.Value));
            }
            return result;
        }

        private void logWorkKeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == (char)Keys.Escape) {
                Close();
            }
        }
    }
}