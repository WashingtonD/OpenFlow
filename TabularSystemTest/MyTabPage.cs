using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TabularSystemTest
{
    public partial class MyTabPage : UserControl
    {
        public MainForm mainForm;
        public MyRichTextBox _myRichTextBox = new MyRichTextBox();

        public MyTabPage(MainForm mf) 
        {
            mainForm = mf;

            this._myRichTextBox.Dock = DockStyle.Fill;
            this._myRichTextBox.richTextBox1.Text = "";
            _myRichTextBox.richTextBox1.Font = new System.Drawing.Font("Monospaced", 11, FontStyle.Regular);
            this._myRichTextBox.richTextBox1.Select();

            _myRichTextBox.richTextBox1.TextChanged += new EventHandler(this.richTextBox1_TextChanged);


        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            String str = this.Text;
            if (str.Contains("*"))
            {

            }
            else
            {
                this.Text = str + "*";
            }


        }

        private void richTextBox1_SelectionChanged(object sender, EventArgs e)
        {
            int sel = _myRichTextBox.richTextBox1.SelectionStart;
            int line = _myRichTextBox.richTextBox1.GetLineFromCharIndex(sel) + 1;
            int col = sel - _myRichTextBox.richTextBox1.GetFirstCharIndexFromLine(line - 1) + 1;

        }


        private void richTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            //Process.Start(e.LinkText);
        }
    }
}
