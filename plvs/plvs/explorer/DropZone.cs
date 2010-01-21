using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Atlassian.plvs.explorer {
    public sealed partial class DropZone : Form {

        private readonly Timer timer;

        private const string INITIAL_TEXT = "Drop zone is idle";

        public DropZone(string name) {
            InitializeComponent();

            StartPosition = FormStartPosition.CenterParent;

            Text = name;

            timer = new Timer {Interval = 2000, Enabled = false};

            labelInfo.Text = INITIAL_TEXT;
        }

        private static readonly Regex DROP_REGEX = new Regex(@"ISSUE:(\w+-\d+):SERVER:{(\S+)}");

        private void DropZone_DragEnter(object sender, DragEventArgs e) {
            e.Effect = DragDropEffects.None;

            labelInfo.Text = "format not accepted";

            if (!e.Data.GetDataPresent(DataFormats.UnicodeText))
                return;

            string txt = (string) e.Data.GetData(DataFormats.UnicodeText);

            if (!DROP_REGEX.IsMatch(txt)) return;

            e.Effect = DragDropEffects.Move;

            labelInfo.Text = "bring it on";
        }

        private void DropZone_DragLeave(object sender, System.EventArgs e) {
            labelInfo.Text = INITIAL_TEXT;
        }

        private void DropZone_DragDrop(object sender, DragEventArgs e) {

            if (!e.Data.GetDataPresent(DataFormats.UnicodeText)) return;

            string txt = (string)e.Data.GetData(DataFormats.UnicodeText);

            Match m = DROP_REGEX.Match(txt);
            if (m == null) return;

            Group @key = m.Groups[1];
            Group @guid = m.Groups[2];

            if (key == null || guid == null) return;

            labelInfo.Text = "dropped " + key.Value;

            AllowDrop = false;

            timer.Tick += timer_Tick;
            timer.Enabled = true;
            timer.Start();
        }

        void timer_Tick(object sender, System.EventArgs e) {
            timer.Tick -= timer_Tick;
            timer.Stop();

            AllowDrop = true;
            labelInfo.Text = INITIAL_TEXT;
        }
    }
}
