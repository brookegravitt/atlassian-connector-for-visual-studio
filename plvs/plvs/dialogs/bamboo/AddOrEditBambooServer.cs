using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Atlassian.plvs.api.bamboo;
using Atlassian.plvs.util;

namespace Atlassian.plvs.dialogs.bamboo {
    public partial class AddOrEditBambooServer : Form {
        private readonly BambooServerFacade facade;
        private static int invocations;

        private readonly bool editing;

        private readonly BambooServer server;

        private bool gettingPlans;

        private readonly List<string> planKeys = new List<string>();

        public AddOrEditBambooServer(BambooServer server, BambooServerFacade facade) {
            this.facade = facade;
            InitializeComponent();

            editing = server != null;

            this.server = new BambooServer(server);

            Text = editing ? "Edit Bamboo Server" : "Add Bamboo Server";
            buttonAddOrEdit.Text = editing ? "Apply Changes" : "Add Server";

            if (editing) {
                if (server != null) {
                    name.Text = server.Name;
                    url.Text = server.Url;
                    user.Text = server.UserName;
                    password.Text = server.Password;

                    radioUseFavourites.Checked = server.UseFavourites;
                    radioSelectManually.Checked = !server.UseFavourites;
                    if (server.PlanKeys != null) {
                        planKeys.AddRange(server.PlanKeys);
                    }

                    if (!server.UseFavourites) {
                        getPlans();
                    }

                    checkEnabled.Checked = server.Enabled;
                    checkDontUseProxy.Checked = server.NoProxy;
                }
            } else {
                ++invocations;
                name.Text = "Bamboo Server #" + invocations;
                buttonAddOrEdit.Enabled = false;

                radioUseFavourites.Checked = true;
                buttonGetBuilds.Enabled = false;
                checkedListBuilds.Enabled = false;

                checkEnabled.Checked = true;
                checkDontUseProxy.Checked = false;
            }

            StartPosition = FormStartPosition.CenterParent;
        }

        public override sealed string Text {
            get { return base.Text; }
            set { base.Text = value; }
        }

        private void buttonAddOrEdit_Click(object sender, EventArgs e) {
            fillServerData();

            server.UseFavourites = radioUseFavourites.Checked;

            planKeys.Clear();

            CheckedListBox.CheckedIndexCollection indices = checkedListBuilds.CheckedIndices;
            for (int i = 0; i < checkedListBuilds.Items.Count; i++) {
                if (indices.Contains(i)) {
                    planKeys.Add(((BambooPlan) checkedListBuilds.Items[i]).Key);
                }
            }

            server.PlanKeys = planKeys;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void fillServerData() {
            server.Name = name.Text.Trim();
            string fixedUrl = url.Text.Trim();
            if (!(fixedUrl.StartsWith("http://") || fixedUrl.StartsWith("https://"))) {
                fixedUrl = "http://" + fixedUrl;
            }
            if (fixedUrl.EndsWith("/")) {
                fixedUrl = fixedUrl.Substring(0, fixedUrl.Length - 1);
            }
            server.Url = fixedUrl;
            server.UserName = user.Text.Trim();
            server.Password = password.Text;
            server.Enabled = checkEnabled.Checked;
            server.NoProxy = checkDontUseProxy.Checked;
        }

        private void name_TextChanged(object sender, EventArgs e) {
            checkIfValid();
        }

        private void url_TextChanged(object sender, EventArgs e) {
            checkIfValid();
        }

        private void user_TextChanged(object sender, EventArgs e) {
            checkIfValid();
        }

        private void checkIfValid() {
            buttonAddOrEdit.Enabled = name.Text.Trim().Length > 0 && url.Text.Trim().Length > 0 &&
                                      user.Text.Trim().Length > 0 && buttonCancel.Enabled;
            buttonTestConnection.Enabled = buttonAddOrEdit.Enabled && buttonCancel.Enabled;
            buttonGetBuilds.Enabled = buttonAddOrEdit.Enabled && radioSelectManually.Checked;
            checkedListBuilds.Enabled = buttonAddOrEdit.Enabled && radioSelectManually.Checked;
        }

        public BambooServer Server {
            get { return server; }
        }

        private void addOrEditServerKeyPress(object sender, KeyPressEventArgs e) {
            if (e.KeyChar != (char) Keys.Escape) return;
            if (gettingPlans) return;
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void radioUseFavourites_CheckedChanged(object sender, EventArgs e) {
            checkIfValid();
        }

        private void radioSelectManually_CheckedChanged(object sender, EventArgs e) {
            checkIfValid();
        }

        private void buttonGetBuilds_Click(object sender, EventArgs e) {
            getPlans();
        }

        private void getPlans() {
            gettingPlans = true;
            buttonCancel.Enabled = false;
            buttonAddOrEdit.Enabled = false;
            radioUseFavourites.Enabled = false;
            radioSelectManually.Enabled = false;
            name.Enabled = false;
            url.Enabled = false;
            user.Enabled = false;
            password.Enabled = false;
            checkEnabled.Enabled = false;
            checkDontUseProxy.Enabled = false;

            buttonGetBuilds.Enabled = false;
            buttonTestConnection.Enabled = false;

            Thread t = new Thread(getPlansWorker);
            t.Start();
        }

        private void getPlansWorker() {
            fillServerData();
            try {
                ICollection<BambooPlan> plans  = facade.getPlanList(server);
                Invoke(new MethodInvoker(() => fillPlanList(plans)));
            } catch (Exception e) {
                Invoke(new MethodInvoker(() => PlvsUtils.showError("Unable to retrieve build plans", e)));
            } finally {
                gettingPlans = false;

                Invoke(new MethodInvoker(delegate {
                                             buttonCancel.Enabled = true;
                                             radioUseFavourites.Enabled = true;
                                             radioSelectManually.Enabled = true;
                                             name.Enabled = true;
                                             url.Enabled = true;
                                             user.Enabled = true;
                                             password.Enabled = true;
                                             checkEnabled.Enabled = true;
                                             checkDontUseProxy.Enabled = true;
                                             checkIfValid();
                }));
            }
        }

        private void fillPlanList(IEnumerable<BambooPlan> plans) {
            if (plans == null) {
                return;
            }
            checkedListBuilds.Items.Clear();
            int i = 0;
            foreach (var plan in plans) {
                checkedListBuilds.Items.Add(plan);
                BambooPlan planCopy = plan;
                if (planKeys.Any(key => planCopy.Key.Equals(key))) {
                    checkedListBuilds.SetItemChecked(i, true);
                }
                ++i;
            }
        }

        private void checkEnabled_CheckedChanged(object sender, EventArgs e) {
            checkIfValid();
        }

        private void buttonTestConnection_Click(object sender, EventArgs e) {
            fillServerData();
            new TestBambooConnection(facade, server).ShowDialog();
        }

        private void checkUseProxy_CheckedChanged(object sender, EventArgs e) {
            checkIfValid();
        }
    }
}