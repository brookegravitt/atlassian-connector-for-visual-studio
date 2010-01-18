using System;
using System.Windows.Forms;
using Atlassian.plvs.autoupdate;
using Atlassian.plvs.util;
using Microsoft.Win32;

namespace Atlassian.plvs.dialogs {
    public partial class GlobalSettings : Form {
        private const int DEFAULT_BAMBOO_POLLING_INTERVAL = 60;
        private const int DEFAULT_ISSUE_BATCH_SIZE = 25;
        private const string REG_AUTOUPDATE = "AutoupdateEnabled";
        private const string REG_BAMBOO_POLLING_INTERVAL = "BambooPollingInterval";
        private const string REG_CHECK_SNAPSHOTS = "AutoupdateCheckSnapshots";
        private const string REG_FIRST_RUN = "FirstRun";
        private const string REG_ISSUE_BATCH_SIZE = "JiraIssueBatchSize";
        private const string REG_MANUAL_UPDATE_STABLE_ONLY = "ManualUpdateCheckStableOnly";
        private const string REG_REPORT_USAGE = "AutoupdateReportUsage";

        private bool isRunningManualUpdateQuery;

        static GlobalSettings() {
            try {
                RegistryKey root = Registry.CurrentUser.CreateSubKey(Constants.PAZU_REG_KEY);
                if (root == null) {
                    throw new Exception();
                }
                JiraIssuesBatch = (int) root.GetValue(REG_ISSUE_BATCH_SIZE, DEFAULT_ISSUE_BATCH_SIZE);
                AutoupdateEnabled = (int) root.GetValue(REG_AUTOUPDATE, 1) > 0;
                AutoupdateSnapshots = (int) root.GetValue(REG_CHECK_SNAPSHOTS, 0) > 0;
                ReportUsage = (int) root.GetValue(REG_REPORT_USAGE, 1) > 0;
                CheckStableOnlyNow = (int) root.GetValue(REG_MANUAL_UPDATE_STABLE_ONLY, 1) > 0;
                BambooPollingInterval = (int) root.GetValue(REG_BAMBOO_POLLING_INTERVAL, DEFAULT_BAMBOO_POLLING_INTERVAL);
            } catch (Exception) {
                JiraIssuesBatch = DEFAULT_ISSUE_BATCH_SIZE;
                AutoupdateEnabled = true;
                AutoupdateSnapshots = false;
                ReportUsage = true;
                CheckStableOnlyNow = true;
                BambooPollingInterval = DEFAULT_BAMBOO_POLLING_INTERVAL;
            }
        }

        public GlobalSettings() {
            InitializeComponent();

            StartPosition = FormStartPosition.CenterParent;

            initializeWidgets();
        }

        public static int JiraIssuesBatch { get; private set; }
        public static bool AutoupdateEnabled { get; private set; }
        public static bool AutoupdateSnapshots { get; private set; }
        public static bool ReportUsage { get; private set; }
        public static bool CheckStableOnlyNow { get; private set; }
        public static int BambooPollingInterval { get; private set; }

        private void initializeWidgets() {
            numericJiraBatchSize.Value = Math.Min(Math.Max(JiraIssuesBatch, 10), 1000);
            numericBambooPollingInterval.Value = Math.Min(Math.Max(BambooPollingInterval, 10), 3600);
            checkAutoupdate.Checked = AutoupdateEnabled;
            checkUnstable.Checked = AutoupdateSnapshots;
            checkStats.Checked = ReportUsage;
            checkUnstable.Enabled = AutoupdateEnabled;
            checkStats.Enabled = AutoupdateEnabled;
            radioStable.Checked = CheckStableOnlyNow;
            radioUnstable.Checked = !CheckStableOnlyNow;
        }

        public static void checkFirstRun() {
            try {
                RegistryKey root = Registry.CurrentUser.CreateSubKey(Constants.PAZU_REG_KEY);
                if (root == null) {
                    throw new Exception();
                }
                var firstRun = (int) root.GetValue(REG_FIRST_RUN, 1);
                if (firstRun > 0) {
                    root.SetValue(REG_FIRST_RUN, 0);
                    handleFirstRun();
                }
            } catch (Exception e) {
                MessageBox.Show("Unable to read registry: " + e.Message, Constants.ERROR_CAPTION,
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void handleFirstRun() {
            DialogResult result = MessageBox.Show(
                "We would greatly appreciate it if you would allow us to collect anonymous"
                + " usage statistics to help us provide a better quality product. Is this OK?",
                Constants.QUESTION_CAPTION, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            ReportUsage = result == DialogResult.Yes;
            saveValues();
        }

        private void GlobalSettings_KeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar == (char) Keys.Escape && !isRunningManualUpdateQuery) {
                Close();
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e) {
            Close();
        }

        private void buttonOk_Click(object sender, EventArgs e) {
            JiraIssuesBatch = (int) numericJiraBatchSize.Value;
            AutoupdateEnabled = checkAutoupdate.Checked;
            AutoupdateSnapshots = checkUnstable.Checked;
            ReportUsage = checkStats.Checked;
            CheckStableOnlyNow = radioStable.Checked;
            BambooPollingInterval = (int) numericBambooPollingInterval.Value;

            saveValues();

            if (SettingsChanged != null) {
                SettingsChanged(this, null);
            }

            Close();
        }

        public static event EventHandler<EventArgs> SettingsChanged;

        private static void saveValues() {
            try {
                RegistryKey root = Registry.CurrentUser.CreateSubKey(Constants.PAZU_REG_KEY);
                if (root == null) {
                    throw new Exception();
                }
                root.SetValue(REG_ISSUE_BATCH_SIZE, JiraIssuesBatch);
                root.SetValue(REG_AUTOUPDATE, AutoupdateEnabled ? 1 : 0);
                root.SetValue(REG_CHECK_SNAPSHOTS, AutoupdateSnapshots ? 1 : 0);
                root.SetValue(REG_REPORT_USAGE, ReportUsage ? 1 : 0);
                root.SetValue(REG_MANUAL_UPDATE_STABLE_ONLY, CheckStableOnlyNow ? 1 : 0);
                root.SetValue(REG_BAMBOO_POLLING_INTERVAL, BambooPollingInterval);
            } catch (Exception e) {
                MessageBox.Show("Unable to save values to registry: " + e.Message, Constants.ERROR_CAPTION,
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonCheckNow_Click(object sender, EventArgs e) {
            setCloseButtonsEnabled(false);
            isRunningManualUpdateQuery = true;
            Autoupdate.Instance.runManualUpdate(radioStable.Checked, this, delegate
                                                                               {
                                                                                   setCloseButtonsEnabled(true);
                                                                                   isRunningManualUpdateQuery = false;
                                                                               });
        }

        private void setCloseButtonsEnabled(bool enabled) {
            buttonOk.Enabled = enabled;
            buttonCancel.Enabled = enabled;
        }

        private void checkAutoupdate_CheckedChanged(object sender, EventArgs e) {
            checkUnstable.Enabled = checkAutoupdate.Checked;
            checkStats.Enabled = checkAutoupdate.Checked;
        }
    }
}