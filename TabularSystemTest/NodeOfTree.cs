using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TabularSystemTest
{
    class NodeOfTree
    {
        private CustomButton ConnectedButton;
        private string name;
        private string path;

        public NodeOfTree(CustomButton b, string Name, string Path)
        {
            ConnectedButton = b;
            name = Name;
            path = Path;
        }
        public NodeOfTree()
        {
            ConnectedButton = null;
            name = null;
        }
        public string Path
        {
            get { return path; }
            set { path = value; }
        }
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
        public CustomButton buttonNode
        {
            get
            {
                return ConnectedButton;
            }
            set
            {
                ConnectedButton = value;
            }
        }

        }

    }
