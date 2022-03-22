using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TabularSystemTest
{
    class TreeGraph
    {
        public List<NodeOfTree> listOfNodes;
        public List<Connection> listOfConnections;
        private string name;
        //public TabPage placeOfTree;
        public TreeGraph()
        {
            listOfNodes = new List<NodeOfTree>();
            listOfConnections = new List<Connection>();
            name = null; 
        }
       /* public TreeGraph(TabPage tp)
        {
            listOfNodes = new List<NodeOfTree>();
            listOfConnections = new List<Connection>();
            placeOfTree = tp;
        }*/
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

    }
}
