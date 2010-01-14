using System;
using System.Threading;
using System.Windows.Forms;

namespace Atlassian.plvs.autoupdate {
    public partial class ManualUpdate : Form {
        private readonly UpdateActionRunner runner;
        private readonly Action runOnSuccess;

        public delegate bool UpdateActionRunner();

        public ManualUpdate(UpdateActionRunner runner, Action runOnSuccess) {
            this.runner = runner;
            this.runOnSuccess = runOnSuccess;
            InitializeComponent();
        }

        private void ManualUpdate_Load(object sender, EventArgs e) {
            Thread t = new Thread(worker);
            t.Start();
        }

        private void worker() {
            Invoke(new MethodInvoker(delegate {
                                         if (runner()) {
                                             Close();
                                             runOnSuccess();
                                         } else {
                                             MessageBox.Show("Fail!");
                                             Close();
                                         }
                                     }));
        }
    }
}