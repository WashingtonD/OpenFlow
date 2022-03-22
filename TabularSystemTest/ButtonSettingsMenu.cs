using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TabularSystemTest
{
    public partial class ButtonSettingsMenu : Form
    {
        public Button returnButton = null;
        private CustomButton buttonSender = null;
        public ButtonSettingsMenu(object send)
        {
            InitializeComponent();
            buttonSender = send as CustomButton;
            string ext = Path.GetExtension(buttonSender.Text);
            switch (ext) {
                case ".txt":
                case ".TXT":
                    buttonColorBorder.Enabled = false;
                    label7.Enabled = false;
                    break;
            }
            setPropriateCollors();
        }

        private void setPropriateCollors()
        {
            if (buttonSender == null)
                return;
            buttonColor.BackColor = buttonSender.BackColor;
            buttonColorText.BackColor = buttonSender.TextColor;
            if (buttonSender.BorderRadius == 0)
                toggleButton1.Checked = false;
            else
                toggleButton1.Checked = true;
            customTestButton.Size = buttonSender.Size;
            if (buttonColorBorder.Enabled)
            {
                buttonColorBorder.BackColor = buttonSender.BorderColor;
                customTestButton.BorderSize = 5;
                customTestButton.BorderColor = buttonSender.BorderColor;
            }
            numericUpDown1.Value = buttonSender.Width;
            numericUpDown2.Value = buttonSender.Height;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if(colorDialog.ShowDialog() == DialogResult.OK)
            {
                buttonColor.BackColor = colorDialog.Color;
            }

        }

        private void buttonColorText_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                buttonColorText.BackColor = colorDialog.Color;
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            returnButton = new Button();
            returnButton.BackColor = buttonColor.BackColor;
            returnButton.ForeColor = buttonColorText.BackColor;
            returnButton.FlatAppearance.BorderColor = buttonColorBorder.BackColor;
            returnButton.Tag = toggleButton1.Checked.ToString();
            //returnButton.Size = buttonSender.Size;
            returnButton.Size = customTestButton.Size;

            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonColor_BackColorChanged(object sender, EventArgs e)
        {
            customTestButton.BackColor = (sender as Button).BackColor;
        }

        private void buttonColorText_BackColorChanged(object sender, EventArgs e)
        {
            customTestButton.TextColor = (sender as Button).BackColor;
        }

        private void toggleButton1_CheckedChanged(object sender, EventArgs e)
        {
            if(toggleButton1.Checked)
            {
                int x = customTestButton.Size.Width;
                customTestButton.BorderRadius = (Int32)(x * 0.32);
            }
            else
                customTestButton.BorderRadius = 0;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            customTestButton.Width = (Int32)(sender as NumericUpDown).Value;
            
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            customTestButton.Height = (Int32)(sender as NumericUpDown).Value;
        }

        private void buttonColorBorder_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                buttonColorBorder.BackColor = colorDialog.Color;
            }
        }

        private void buttonColorBorder_BackColorChanged(object sender, EventArgs e)
        {
            customTestButton.BorderColor = (sender as Button).BackColor;
        }
    }
}
