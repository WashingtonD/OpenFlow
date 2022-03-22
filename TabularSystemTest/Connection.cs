using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TabularSystemTest
{
    class Connection
    {
        private NodeOfTree firstNode;
        private NodeOfTree secondNode;
        private string title;

        public Connection()
        {
            firstNode = null;
            secondNode = null;
            title = null;
        }
        public Connection(NodeOfTree first, NodeOfTree second, string _title)
        {
            firstNode = first;
            secondNode = second;
            title = _title;
        }

        public NodeOfTree FirstNode
        {
            get { return firstNode; }
            set { firstNode = value; }
        }

        public NodeOfTree SecondNode
        {
            get { return secondNode; }
            set { secondNode = value; }
        }
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public bool Compare(Connection second)
        {
            if(this.firstNode == second.firstNode || this.firstNode == second.SecondNode)
            {
                if (this.secondNode == second.firstNode || this.secondNode == second.secondNode)
                {
                    if (this.Title == second.Title)
                        return true;
                }
            }
            return false;
        }
    }
}
