using System.Drawing;
using System.Windows.Forms;

namespace Atlassian.plvs.ui {
    public sealed class OwnerDrawListBox : ListBox {
        public OwnerDrawListBox() {
            DrawMode = DrawMode.OwnerDrawFixed;
            DrawItem += listBoxWithProgrammableHeightDrawItem;
        }

        private void listBoxWithProgrammableHeightDrawItem(object sender, DrawItemEventArgs e) {
            e.DrawBackground();
            e.Graphics.DrawString(Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), e.Bounds);
            e.DrawFocusRectangle();
        }
    }
}
