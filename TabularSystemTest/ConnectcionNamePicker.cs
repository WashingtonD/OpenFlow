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
    public partial class ConnectcionNamePicker : Form
    {
        public string conName { get; set; } 
        public ConnectcionNamePicker()
        {
            InitializeComponent();

           



        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if(textBoxConName.Text.Length != 0)
            {
                conName = textBoxConName.Text;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Connection name cannot be empty", "Try again!", MessageBoxButtons.OK);
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel; 
            this.Close();
        }
    }
}
