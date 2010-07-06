using System;
using System.Diagnostics;
using System.Windows.Forms;
using Atlassian.plvs.api.bamboo;
using Atlassian.plvs.autoupdate;
using Atlassian.plvs.util.bamboo;

namespace Atlassian.plvs.ui.bamboo {
    public sealed partial class BambooBuildHistoryTable : UserControl {

        private BambooBuild lastBuild;

        public BambooBuildHistoryTable() {
            InitializeComponent();
            Dock = DockStyle.Fill;
            updateButtons(false);
        }

        private void updateButtons(bool enabled) {
            buttonOpen.Enabled = enabled;
            buttonViewInBrowser.Enabled = enabled;
        }

        public void showHistoryForBuild(BambooBuild build) {
            lastBuild = build;
            updateButtons(false);

            if (build == null) {
                grid.Rows.Clear();
            } else {
                for (int i = 0; i < 10; i++) {
                    BuildNode bn = new BuildNode(lastBuild);
                    object[] b = new object[] { bn.Icon, bn.Key, bn.Completed };
                    grid.Rows.Insert(i, b);
                }
            }
        }

        private void grid_CellDoubleClick(object sender, DataGridViewCellEventArgs e) {
            openBuild(lastBuild);
        }

        private void buttonOpen_Click(object sender, EventArgs e) {
            openBuild(lastBuild);
        }

        private void grid_SelectionChanged(object sender, EventArgs e) {
            updateButtons(grid.SelectedRows.Count > 0);
        }

        private void buttonViewInBrowser_Click(object sender, EventArgs e) {
            BambooBuild b = lastBuild;
            if (b == null) return;
            try {
                Process.Start(b.Server.Url + "/build/viewBuildResults.action?buildKey="
                              + BambooBuildUtils.getPlanKey(b) + "&buildNumber=" + b.Number);
            } catch (Exception ex) {
                Debug.WriteLine("buttonViewInBrowser_Click - exception: " + ex.Message);
            }
            UsageCollector.Instance.bumpBambooBuildsOpen();

        }

        private void grid_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                openBuild(lastBuild);
            }
        }

        private void grid_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                e.Handled = true;
            }
        }

        private static void openBuild(BambooBuild build) {
            BuildDetailsWindow.Instance.openBuild(build);
        }
    }
}
