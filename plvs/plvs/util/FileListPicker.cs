using System.Collections.Generic;
using System.Windows.Forms;
using EnvDTE;

namespace Atlassian.plvs.util {
    internal partial class FileListPicker : Form {

        public ProjectItem SelectedFile { get; private set; }

        public FileListPicker(IEnumerable<ProjectItem> files) {
            InitializeComponent();

            foreach (ProjectItem file in files) {
                listFiles.Items.Add(file);
            }
        }

        private void listFiles_MouseDoubleClick(object sender, MouseEventArgs e) {
            if (listFiles.SelectedItem == null) return;

            SelectedFile = listFiles.SelectedItem as ProjectItem;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void listFiles_KeyPress(object sender, KeyPressEventArgs e) {
            switch (e.KeyChar) {
                case (char) Keys.Enter:
                    SelectedFile = listFiles.SelectedItem as ProjectItem;
                    DialogResult = DialogResult.OK;
                    Close();
                    break;
                case (char) Keys.Escape:
                    DialogResult = DialogResult.Cancel;
                    Close();
                    break;
            }
        }
    }
}
