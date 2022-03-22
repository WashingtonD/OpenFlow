using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TabularSystemTest
{
    public delegate bool PreRemoveTab(int indx);

    class CrossTabControl : TabControl
    {
        public MainForm currentForm = null;
        public CrossTabControl(MainForm form) : base()
        {
            currentForm = form;
            PreRemoveTabPage = null;
            this.DrawMode = TabDrawMode.OwnerDrawFixed;
        }

        public PreRemoveTab PreRemoveTabPage;

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            Rectangle r = e.Bounds;
            r = GetTabRect(e.Index);
            r.Offset(2, 2);
            r.Width = 5;
            r.Height = 5;
            Brush b = new SolidBrush(Color.Black);
            Pen p = new Pen(b);
            e.Graphics.DrawLine(p, r.X, r.Y, r.X + r.Width, r.Y + r.Height);
            e.Graphics.DrawLine(p, r.X + r.Width, r.Y, r.X, r.Y + r.Height);

            string title = this.TabPages[e.Index].Text;
            Font f = this.Font;
            e.Graphics.DrawString(title, f, b, new PointF(r.X + 5, r.Y));
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            Point p = e.Location;
            for (int i = 0; i < TabCount; i++)
            {
                Rectangle r = GetTabRect(i);
                r.Offset(2, 2);
                r.Width = 5;
                r.Height = 5;
                if (r.Contains(p))
                {
                    DialogResult result = MessageBox.Show("Would you like to close this file?", "File Closing", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        if (this.TabPages[i].Text.Contains("*"))
                        {
                            DialogResult res = MessageBox.Show("Would you like to save the file?", "File Saving", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (res == DialogResult.Yes && currentForm != null)
                            {
                                 string path = "";
                                 foreach(string tn in currentForm.OpenedFilesList)
                                 {
                                    if(Path.GetFileName(tn) + "*" == this.TabPages[i].Text)
                                    {
                                        path = tn;
                                    }
                                 }
                                if (path != "")
                                    currentForm.targetOfSave = true;
                                currentForm.saveCurrentFileToolStripMenuItem_Click(null, null);
                                //this.TabPages.Remove(this.SelectedTab);
 
                            }
                            else
                            {
                                //this.TabPages.Remove(this.SelectedTab);
                            }
                            CloseTab(i);
                        }
                        else
                        {
                            this.TabPages.Remove(this.SelectedTab);
                        }
                    }
                }
            }
        }
        private void CloseTab(int i)
        {
            if (PreRemoveTabPage != null)
            {
                bool closeIt = PreRemoveTabPage(i);
                if (!closeIt)
                    return;
            }
            TabPages.Remove(TabPages[i]);
        }
    }
}
