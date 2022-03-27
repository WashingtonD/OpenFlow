using Microsoft.VisualBasic;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace TabularSystemTest
{
    public partial class MainForm : Form
    {
        public string currentFileName;
        public string currentPath;
        private Point tempPoint;
        private Graphics graphics;
        private Point tempPositionOfButton;
        private Pen myPen;
        private CustomButton tempButton = null;
        public bool targetOfSave = false;
        private NodeOfTree tempNode = null;
        private SolidBrush mySolidBrush;
        private static string DirOfNotes;
        private TreeGraph treeGraph;
        private SqlConnection sqlConnection = null;
        public List<String> OpenedFilesList = new List<String> { };
        //private static List<TreeGraph> OpenedTreesList = new List<TreeGraph> { };
        private static Dictionary<TabPage, TreeGraph> OpenedTreesDict = new Dictionary<TabPage, TreeGraph>();
        private static Dictionary<TabPage, List<(NodeOfTree, Size)>> DefaultTreesModel = new Dictionary<TabPage, List<(NodeOfTree, Size)>>();
        //private static Dictionary<TabPage, List<Size>> DefaultTreesModel = new Dictionary<TabPage, List<Size>>();
        CrossTabControl tabControl1 = null;
        //public enum Colors { Red = 1, Green = 2, LtBlue = 3, Blue = 4, };
        private Random rnd = new Random();


        public MainForm()
        {
            InitializeComponent();
            InitializeDBConnection();
            /// Do przenoszenia
            //treeView1.ContextMenuStrip = contextMenuTreeView;
            treeView1.MouseClick += TreeView1_MouseClick1;
            ///
            

            new PanelWndProc().AssignHandle(splitContainer1.Panel2.Handle);


            createOrCheckDirectory();
            refreshTreeView();
            mySolidBrush = new SolidBrush(Color.Black);
            myPen = new Pen(Color.Black, 5);
            myPen.StartCap = LineCap.Round;
            myPen.EndCap = LineCap.Round;
            treeGraph = new TreeGraph();
            scaleStatus.Add(treeGraph, new Tuple<int, int>(0, 0));

            tabControl1 = new CrossTabControl(this);
            tabControl1.SelectedIndexChanged += TabControl1_SelectedIndexChanged;
            splitContainer1.Panel2.AutoScroll = true;
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
            TabPage a = new TabPage("Cool");
            Panel bb = new Panel();
            bb.Dock = DockStyle.Fill;
            bb.AutoScroll = true;
            


            PictureBox pp = new PictureBox();
            bb.Controls.Add(pp);
            pp.Size = new Size(3000, 3000);
            pp.MouseWheel += pictureBox1_MouseWheel;
            CustomButton cb = new CustomButton();
            cb.Text = "testing button";
            cb.Location = new Point(100, 100);
            cb.Size = new Size(200, 150);
            CustomButton ql = new CustomButton();
            ql.Size = new Size(400, 500);
            ql.Location = new Point(1500, 1500);
            pp.Controls.Add(cb);
            pp.Controls.Add(ql);
            pp.Name = "pictureBox";
            //a.Controls.Add(bb);
            //splitContainer1.Panel2.Controls.Add(pp);
            //Graphics cgi = null;
            Bitmap bm = new Bitmap(3000,3000);//tabControl1.SelectedTab.Size.Width, tabControl1.SelectedTab.Size.Height);
            pp.Image = bm;
            // pb.Dock = DockStyle.Fill;
            graphics = Graphics.FromImage(pp.Image);
            graphics.Clear(Color.Black);

            a.Controls.Add(bb);
            tabControl1.TabPages.Add(a);
           splitContainer1.Panel2.Controls.Add(tabControl1);

        }

        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Panel pn = null;
            foreach(Control c in tabControl1.SelectedTab.Controls)
            {
                if(c.Name.Contains("panel"))
                {
                    pn = c as Panel;
                }
            }
            if(pn != null)
                new PanelWndProc().AssignHandle(pn.Handle);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch(keyData)
                {
                case (Keys.Control | Keys.S):
                    saveToolStripMenuItem_Click(null, null);
                    return true;
                case (Keys.Control | Keys.N):
                    newToolStripMenuItem_Click(null, null);
                    return true;
                case (Keys.Control | Keys.Shift | Keys.N):
                    newTreeToolStripMenuItem_Click(null, null);
                    return true;
            } 
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void TreeView1_MouseClick1(object sender, MouseEventArgs e)
        {
         /*   if(e.Button == MouseButtons.Right)
            {
                foreach(TreeNode c in treeView1.Nodes)
                {
                    if (c.Bounds.Contains(e.Location))
                        return;
                }
                    
                  contextMenuTreeView.Show();
            }*/
        }

        private void refreshTreeView()
        {
            saveTheNodes();
            treeView1.Nodes.Clear();
            try
            {
                using (SqlCommand sqlCom = new SqlCommand($"SELECT * FROM [Folder]"))
                {
                    sqlCom.Connection = sqlConnection;
                    sqlConnection.Open();
                    SqlDataReader sqlRead = sqlCom.ExecuteReader();
                    while (sqlRead.Read())
                    {
                        string path = sqlRead["Path"].ToString();
                        loadFromDirectory(treeView1, path, 1, 0);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                sqlConnection.Close();
            }
            openNodes();
        }

        /*private bool checkDBExistance(string Path)
        {
            
            try
            {
                string pathAnswer = null;
                using (SqlCommand sqlCom = new SqlCommand($"SELECT * FROM [Backup]"))
                {
                    sqlCom.Connection = sqlConnection;
                    sqlConnection.Open();
                    SqlDataReader sqlRead = sqlCom.ExecuteReader();
                    while (sqlRead.Read())
                    {
                        pathAnswer = sqlRead["Path"].ToString();
                        if (pathAnswer == Path)
                            return true;
                    }
                    sqlConnection.Close();
                    return false;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }*/


        private void InitializeDBConnection()
        {
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["OpenFlowDB"].ConnectionString);
        }

        private void addPathToDB(string Path,bool f)
        {
            try
            {
                using (SqlCommand sqlCom = new SqlCommand($"INSERT INTO [Folder] VALUES(@Path,@isOpen)"))
                {
                    sqlCom.Connection = sqlConnection;
                    sqlConnection.Open();
                    sqlCom.Parameters.AddWithValue("@Path", Path);
                    if (f)
                        sqlCom.Parameters.AddWithValue("@isOpen", 1);
                    else
                        sqlCom.Parameters.AddWithValue("@isOpen", 0);
                    sqlCom.ExecuteNonQuery();
                }
                
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        private void RichTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (!tabControl1.SelectedTab.Text.Contains("*"))
            {
                tabControl1.SelectedTab.Text += "*";
            }
        }

        private static void loadFromDirectory(TreeView trv, string directory, int folder_img, int file_img)
        {
            DirectoryInfo dir_info = new DirectoryInfo(directory);
            AddDirectoryNodes(trv, dir_info, null, folder_img, file_img);
        }


        private void createOrCheckDirectory()
        {
            string dir = Directory.GetCurrentDirectory();
            dir += "\\Notes";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                addPathToDB(dir,true);
            }
            DirOfNotes = dir;
        }

        private static void AddDirectoryNodes(TreeView trv, DirectoryInfo dir_info, TreeNode parent, int folder_img, int file_img)
        {
            TreeNode dir_node;
            if (parent == null)
                dir_node = trv.Nodes.Add(dir_info.Name);
            else
                dir_node = parent.Nodes.Add(dir_info.Name);

            if (folder_img >= 0)
            {
                dir_node.ImageIndex = folder_img;
                dir_node.SelectedImageIndex = folder_img;
            }
            foreach (DirectoryInfo subdir in dir_info.GetDirectories())
                AddDirectoryNodes(trv, subdir, dir_node, folder_img, file_img);

            foreach (FileInfo file_info in dir_info.GetFiles())
            {
                TreeNode file_node = dir_node.Nodes.Add(file_info.Name);
                if (file_img >= 0)
                {
                    file_node.ImageIndex = file_img;
                    file_node.SelectedImageIndex = file_img;
                    file_node.Tag = file_info.FullName;
                }
            }
        }





        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabPage tp = new TabPage();
            tp.Text = "New File";
            tabControl1.TabPages.Add(tp);
            MyRichTextBox myRichTextBox = new MyRichTextBox();
            myRichTextBox.richTextBox1.TextChanged += RichTextBox1_TextChanged;
            tp.Controls.Add(myRichTextBox);
            myRichTextBox.Dock = DockStyle.Fill;
            tabControl1.SelectedTab = tp;
        }




        private void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabControl.TabPageCollection pages = tabControl1.TabPages;
            foreach (TabPage page in pages)
            {
///TODO Fix
/// 
                saveCurrentFileToolStripMenuItem_Click(null,null);
            }

        }

        public class forJson
        {
            public int X1 { get; set; }
            public int X2 { get; set; }
            public int Y1 { get; set; }
            public int Y2 { get; set; }
            public int sizeW1 { get; set; }
            public int sizeH1 { get; set; }
            public int sizeW2 { get; set; }
            public int sizeH2 { get; set; }
            public int textcolor1 { get; set; }
            public int textcolor2 { get; set; }
            public int backcolor1 { get; set; }
            public int backcolor2 { get; set; }
            public int borderRad1 { get; set; }
            public int borderRad2 { get; set; }
            public int bordercolor1 { get; set; }
            public int bordercolor2 { get; set; }
            public int bordersize1 { get; set; }
            public int bordersize2 { get; set; }
            public string FirstPath { get; set; }
            public string SecondPath { get; set; }
            public string Title { get; set; }
        }


        public void saveCurrentFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (targetOfSave)
            {
                targetOfSave = false;
                string reg = ".*" + tabControl1.SelectedTab.Text + "$";
                Regex re = new Regex(reg);
                string res = "";
                foreach(string s in OpenedFilesList)
                {
                    if (re.IsMatch(s))
                    {
                        res = s;
                        break;
                    }
                }
                if (res != "")
                {
                    int indexOfTab = tabControl1.SelectedIndex;
                    Control[] tabOfControls = tabControl1.TabPages[indexOfTab].Controls.Find("myRichTextBox", true);
                    if (tabOfControls != null && tabOfControls.Length != 0)
                    {
                        MyRichTextBox myRtb = tabOfControls[0] as MyRichTextBox;
                        File.WriteAllText(res, myRtb.richTextBox1.Text);
                        if (tabControl1.SelectedTab.Text.Contains("*"))
                        {
                            int place = tabControl1.SelectedTab.Text.Length;
                            tabControl1.SelectedTab.Text = tabControl1.SelectedTab.Text.Remove(place - 1);
                        }
                        deleteAndSaveTextFile(res);
                    }
                    else
                    {
                        tabOfControls = tabControl1.TabPages[indexOfTab].Controls.Find("pictureBox", true);
                        if (tabOfControls != null && tabOfControls.Length != 0)
                        {
                            deleteFromDataBase(res);
                            saveIntoJSON(res);
                            insertDataIntoDb(res, res);
                        }
                    }
                }
                else
                {
                    saveCurrentFileToolStripMenuItem_Click(null,null);
                }
            }
            else
            {
                SaveFileDialog filedialog = new SaveFileDialog();
                //filedialog.FileName = Path.GetFileNameWithoutExtension(tabControl1.SelectedTab.Text); /// TUT NADA ЗАХУЯРИТЬ ФУЛЛ ПАС
                filedialog.InitialDirectory = DirOfNotes;
               
                ///// OGRANICZYC Typ File`a
                filedialog.Filter = "Text files (txt) | *.txt|Tree File (.json)| *.JSON";
                int indexOfTab = tabControl1.SelectedIndex;
                Control[] tabOfControls = tabControl1.TabPages[indexOfTab].Controls.Find("myRichTextBox", true);
                if (tabOfControls != null && tabOfControls.Length != 0)
                {
                    MyRichTextBox myRtb = tabOfControls[0] as MyRichTextBox;
                    if (filedialog.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllText(filedialog.FileName, myRtb.richTextBox1.Text);
                        if (tabControl1.SelectedTab.Text.Contains("*"))
                        {
                            int place = tabControl1.SelectedTab.Text.Length;
                            tabControl1.SelectedTab.Text = tabControl1.SelectedTab.Text.Remove(place - 1);
                        }
                        deleteAndSaveTextFile(filedialog.FileName);
                    }
                    /* try
                    {
                        bool folderExistance = false;
                         int folderId = -1;
                         using (SqlCommand sqlCom = new SqlCommand($"SELECT * FROM [Folder]"))
                         {
                             sqlCom.Connection = sqlConnection;
                             if (sqlConnection.State == ConnectionState.Closed)
                                 sqlConnection.Open();
                             SqlDataReader sqlReader = sqlCom.ExecuteReader();
                             while (sqlReader.Read())
                             {
                                 if (sqlReader["Path"].ToString() == Path.GetDirectoryName(filedialog.FileName))
                                 {
                                     folderExistance = true;
                                     folderId = (Int32)sqlReader["Id"];
                                     break;
                                 }
                             }
                             sqlReader.Close();
                         }
                         if (!folderExistance)
                         {
                             using (SqlCommand sqlCom = new SqlCommand($"INSERT INTO OUTPUT INSERTED.Id [FOLDER] VALUES(@PATH)"))
                             {
                                 sqlCom.Connection = sqlConnection;
                                 if (sqlConnection.State == ConnectionState.Closed)
                                     sqlConnection.Open();
                                 sqlCom.Parameters.AddWithValue("@PATH", Path.GetDirectoryName(filedialog.FileName));
                                 folderId = (int)sqlCom.ExecuteScalar();
                             }
                         }
                         string text = File.ReadAllText(filedialog.FileName);
                         using (SqlCommand sqlCom = new SqlCommand("$INSERT INTO [Backup] VALUES(@Name, @Text, @isOpen, @FolderId)"))
                         {
                             sqlCom.Connection = sqlConnection;
                             if (sqlConnection.State == ConnectionState.Closed)
                                 sqlConnection.Open();
                             sqlCom.Parameters.AddWithValue("@Name", Path.GetFileName(filedialog.FileName));
                             sqlCom.Parameters.AddWithValue("@Text", text);
                             //sqlCom.Parameters.AddWithValue("@isOpen",);
                         }
                     }
                     catch (Exception exc)
                     {
                         throw exc;
                     }
                     finally
                     {
                         sqlConnection.Close();
                     }*/

                }
                else
                {
                    tabOfControls = tabControl1.TabPages[indexOfTab].Controls.Find("pictureBox", true);
                    if (tabOfControls != null && tabOfControls.Length != 0)
                    {
                        if (filedialog.ShowDialog() == DialogResult.OK)
                        {
                            string path = filedialog.FileName;
                            if (checkExistanceOfGraphInDB(path))//File.Exists(path))///
                            {
                                deleteFromDataBase(path);     
                                /*int idOfTree = -1;
                                int idOfFolder = -1;
                                try
                                {
                                    using (SqlCommand sqlCom = new SqlCommand($"SELECT * FROM [Folder]"))
                                    {
                                        sqlCom.Connection = sqlConnection;
                                        if (sqlConnection.State == ConnectionState.Closed)
                                            sqlConnection.Open();
                                        SqlDataReader reader = sqlCom.ExecuteReader();
                                        while (reader.Read())
                                        {
                                            if (reader["Path"].ToString() == path)
                                            {
                                                idOfFolder = (Int32)reader["Id"];
                                                break;
                                            }
                                        }
                                        reader.Close();
                                    }
                                    using (SqlCommand sqlC = new SqlCommand($"SELECT * FROM [Tree]"))
                                    {
                                        sqlC.Connection = sqlConnection;
                                        if (sqlConnection.State == ConnectionState.Closed)
                                            sqlConnection.Open();
                                        SqlDataReader reader = sqlC.ExecuteReader();
                                        while (reader.Read())
                                        {
                                            if (reader["FolderId"].ToString() == idOfFolder.ToString())
                                            {
                                                idOfTree = (Int32)reader["Id"];
                                                break;
                                            }
                                        }
                                        reader.Close();
                                    }
                                    if (idOfTree != -1)
                                    {
                                        List<int> listOfConnections = new List<int>();
                                        List<int> listOfBackups = new List<int>();
                                        using (SqlCommand sqlCom = new SqlCommand($"SELECT * FROM [Connection] WHERE [TreeId] = @IdOfTree"))
                                        {
                                            sqlCom.Connection = sqlConnection;
                                            if (sqlConnection.State == ConnectionState.Closed)
                                                sqlConnection.Open();
                                            sqlCom.Parameters.AddWithValue("@IdOfTree", idOfTree);
                                            SqlDataReader sqlReader = sqlCom.ExecuteReader();
                                            while (sqlReader.Read())
                                            {
                                                listOfConnections.Add((Int32)sqlReader["Id"]);
                                            }
                                            sqlReader.Close();
                                        }
                                        foreach (int i in listOfConnections)
                                        {
                                            using (SqlCommand sqlCom = new SqlCommand($"DELETE FROM [NodesConnection] WHERE" +
                                                "[ConnectionIdFirst] = @FirstId OR [ConnectionIdSecond] = @FirstId"))
                                            {
                                                sqlCom.Connection = sqlConnection;
                                                if (sqlConnection.State == ConnectionState.Closed)
                                                    sqlConnection.Open();
                                                sqlCom.Parameters.AddWithValue("@FirstId", listOfConnections[i]);
                                                sqlCom.ExecuteNonQuery();
                                            }
                                        }
                                        foreach (int i in listOfConnections)
                                        {
                                            using (SqlCommand sqlC = new SqlCommand($"SELECT [BackupId] FROM [Connection] WHERE [Id] = @idOfConneciton"))
                                            {
                                                sqlC.Connection = sqlConnection;
                                                if (sqlConnection.State == ConnectionState.Closed)
                                                    sqlConnection.Open();
                                                sqlC.Parameters.AddWithValue(@"idOfConnection", i);
                                                SqlDataReader reader = sqlC.ExecuteReader();
                                                while (reader.Read())
                                                {
                                                    listOfBackups.Add((Int32)reader["BackupId"]);
                                                }
                                                reader.Close();
                                            }
                                        }
                                        listOfBackups = listOfBackups.Distinct().ToList();
                                        foreach (int i in listOfConnections)
                                        {
                                            using (SqlCommand sqlCom = new SqlCommand($"DELETE FROM [Connection] WHERE [Id] = @FirstId"))
                                            {
                                                sqlCom.Connection = sqlConnection;
                                                if (sqlConnection.State == ConnectionState.Closed)
                                                    sqlConnection.Open();
                                                sqlCom.Parameters.AddWithValue("@FirstId", listOfConnections[i]);
                                                sqlCom.ExecuteNonQuery();
                                            }
                                        }
                                        foreach (int i in listOfBackups)
                                        {
                                            using (SqlCommand sqlC = new SqlCommand($"DELETE FROM [Backup] WHERE [Id] = @IdOfBackup"))
                                            {
                                                sqlC.Connection = sqlConnection;
                                                if (sqlConnection.State == ConnectionState.Closed)
                                                    sqlConnection.Open();
                                                sqlC.Parameters.AddWithValue("@IdOfBackup", i);
                                                sqlC.ExecuteNonQuery();
                                            }
                                        }
                                        using (SqlCommand sqlC = new SqlCommand($"DELETE FROM [Tree] WHERE [Id] = @IdOfTree"))
                                        {
                                            sqlC.Connection = sqlConnection;
                                            if (sqlConnection.State == ConnectionState.Closed)
                                                sqlConnection.Open();
                                            sqlC.Parameters.AddWithValue("IdOfTree", idOfTree);
                                            sqlC.ExecuteNonQuery();
                                        }
                                    }

                                }
                                catch (Exception exception)
                                {
                                    throw exception;
                                }
                                finally
                                {
                                    sqlConnection.Close();
                                }*/
                            }
                                //// SAVING INTO JSON FILE HERE AND CREATING PATH VAR
                                /* TreeGraph a = OpenedTreesDict.FirstOrDefault(tabpage => tabpage.Key == tabControl1.SelectedTab).Value;
                                 a.Name = Path.GetFileName(filedialog.FileName);
                                 JObject o;
                                 List<String> list = new List<String>();

                                 string filename = path;
                                 list.Clear();
                                 foreach (Connection con in a.listOfConnections)
                                 {
                                     o = new JObject();
                                     o["X1"] = con.FirstNode.buttonNode.Location.X;
                                     o["Y1"] = con.FirstNode.buttonNode.Location.Y;
                                     o["FirstPath"] = con.FirstNode.Path;
                                     o["X2"] = con.SecondNode.buttonNode.Location.X;
                                     o["Y2"] = con.SecondNode.buttonNode.Location.Y;
                                     o["SecondPath"] = con.SecondNode.Path;
                                     o["Title"] = con.Title;
                                     o["sizeW1"] = con.FirstNode.buttonNode.Size.Width;
                                     o["sizeH1"] = con.FirstNode.buttonNode.Size.Height;
                                     o["sizeW2"] = con.SecondNode.buttonNode.Size.Width;
                                     o["sizeH2"] = con.SecondNode.buttonNode.Size.Height;
                                     o["textcolor1"] = con.FirstNode.buttonNode.ForeColor.ToArgb();
                                     o["textcolor2"] = con.SecondNode.buttonNode.ForeColor.ToArgb();
                                     o["backcolor1"] = con.FirstNode.buttonNode.BackColor.ToArgb();
                                     o["backcolor2"] = con.SecondNode.buttonNode.BackColor.ToArgb();
                                     o["borderRad1"] = con.FirstNode.buttonNode.BorderRadius;
                                     o["borderRad2"] = con.SecondNode.buttonNode.BorderRadius;
                                     o["bordersize1"] = con.FirstNode.buttonNode.BorderSize;
                                     o["bordersize2"] = con.SecondNode.buttonNode.BorderSize;
                                     o["bordercolor1"] = con.FirstNode.buttonNode.BorderColor.ToArgb();
                                     o["bordercolor2"] = con.SecondNode.buttonNode.BorderColor.ToArgb();
                                     list.Add(o.ToString());
                                 }
                                 writeFileAsync(list, filename);*/
                            saveIntoJSON(path); 
                            insertDataIntoDb(path, Path.GetFullPath(filedialog.FileName));
                        }
                    }
                }
            }
            refreshTreeView();
        }

        private void insertDataIntoDb(string path, string Patth)
        {
            try
            {
                string directory = Path.GetDirectoryName(path);
                int identify = -1;
                using (SqlCommand sqlCom = new SqlCommand($"SELECT * FROM [Folder]"))
                {
                    sqlCom.Connection = sqlConnection;
                    if (sqlConnection.State == ConnectionState.Closed)
                        sqlConnection.Open();
                    //sqlConnection.Open();
                    SqlDataReader sqlRead = sqlCom.ExecuteReader();
                    while (sqlRead.Read())
                    {
                        if (sqlRead["Path"].ToString() == directory)
                        {
                            Int32.TryParse((sqlRead["Id"].ToString()), out identify);
                            break;
                        }
                    }
                    sqlRead.Close();
                }
                if(identify == -1)
                {
                    using(SqlCommand sqlC = new SqlCommand($"INSERT INTO [Folder] OUTPUT INSERTED.Id VALUES(@Path,@isOpen)"))
                    {
                        sqlC.Connection = sqlConnection;
                        if (sqlConnection.State == ConnectionState.Closed)
                            sqlConnection.Open();
                        sqlC.Parameters.AddWithValue("@Path", directory);
                        sqlC.Parameters.AddWithValue("@isOpen", false);
                        identify = (Int32)sqlC.ExecuteScalar();
                    }
                }
                int TreeId = -1;
                try
                {
                    using (SqlCommand sqlCom = new SqlCommand($"INSERT INTO [Tree] OUTPUT INSERTED.Id VALUES(@Path,@IsOpen,@FolderId)"))
                    {
                        sqlCom.Connection = sqlConnection;
                        if (sqlConnection.State == ConnectionState.Closed)
                            sqlConnection.Open();
                        sqlCom.Parameters.AddWithValue("@Path", Patth);
                        sqlCom.Parameters.AddWithValue("@IsOpen", 1);
                        if (identify != -1)
                            sqlCom.Parameters.AddWithValue("@FolderId", identify);
                        else
                            sqlCom.Parameters.AddWithValue("@FolderId", null);
                        TreeId = (int)sqlCom.ExecuteScalar();
                    }
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                }


                TreeGraph currentGraph = null;
                OpenedTreesDict.TryGetValue(tabControl1.SelectedTab, out currentGraph);
                if (currentGraph != null)
                {
                    int lastIdOfBackup = -1;
                    List<String> check = new List<String>();
                    Dictionary<NodeOfTree, int> tempDict = new Dictionary<NodeOfTree, int>();
                    foreach (NodeOfTree treeNode in currentGraph.listOfNodes)
                    {
                        if (check.FirstOrDefault(s => s == treeNode.Path) == null)
                        {
                            check.Add(treeNode.Path);
                            string patth = treeNode.Path;
                            string textOfNote = File.ReadAllText(patth);
                            bool Open = false;
                            foreach (TabPage tp in tabControl1.TabPages)
                            {
                                if (tp.Text == Path.GetFileName(patth))
                                { Open = true; break; }
                            }
                            using (SqlCommand sqlCom = new SqlCommand($"INSERT INTO [Backup] OUTPUT INSERTED.Id VALUES(@Name,@Text,@IsOpen,@FolderId)"))
                            {
                                sqlCom.Connection = sqlConnection;
                                if (sqlConnection.State == ConnectionState.Closed)
                                    sqlConnection.Open();
                                sqlCom.Parameters.AddWithValue("@Name", Path.GetFileName(patth));
                                sqlCom.Parameters.AddWithValue("@IsOpen", Open);
                                sqlCom.Parameters.AddWithValue("@Text", textOfNote);
                                if (identify == -1)
                                    sqlCom.Parameters.AddWithValue("@FolderId", null);
                                else
                                    sqlCom.Parameters.AddWithValue("@FolderId", identify);
                                lastIdOfBackup = (int)sqlCom.ExecuteScalar();
                            }

                        }

                        Point position = treeNode.buttonNode.Location;

                        Size defaultSize = new Size(200,150);
                        defaultSize = DefaultTreesModel[tabControl1.SelectedTab].Find(x => x.Item1 == treeNode).Item2;

                        using (SqlCommand sqlCom = new SqlCommand($"INSERT INTO [Connection] OUTPUT INSERTED.Id VALUES(@PositionX,@PositionY,@BackupId,@TreeId,@BackColor,@BorderColor,@BorderSize,@TextColor,@Width,@Height)"))
                        {
                            sqlCom.Connection = sqlConnection;
                            if (sqlConnection.State == ConnectionState.Closed)
                                sqlConnection.Open();
                            sqlCom.Parameters.AddWithValue("@PositionX", position.X);
                            sqlCom.Parameters.AddWithValue("@PositionY", position.Y);
                            sqlCom.Parameters.AddWithValue("@BackupId", lastIdOfBackup);
                            sqlCom.Parameters.AddWithValue("@TreeId", TreeId);
                            sqlCom.Parameters.AddWithValue("@BackColor", treeNode.buttonNode.BackColor.ToArgb());
                            sqlCom.Parameters.AddWithValue("@BorderColor", treeNode.buttonNode.BorderColor.ToArgb());
                            sqlCom.Parameters.AddWithValue("@BorderSize", treeNode.buttonNode.BorderSize);
                            sqlCom.Parameters.AddWithValue("@TextColor", treeNode.buttonNode.TextColor.ToArgb());
                            sqlCom.Parameters.AddWithValue("@Width", defaultSize.Width);
                            sqlCom.Parameters.AddWithValue("@Height", defaultSize.Height);
                            tempDict.Add(treeNode, (int)sqlCom.ExecuteScalar());
                        }
                    }
                    int first = -1;
                    int second = -1;
                    string title;
                    foreach (Connection con in currentGraph.listOfConnections)
                    {
                        title = con.Title;
                        first = tempDict.First(node => node.Key == con.FirstNode).Value;
                        second = tempDict.First(node => node.Key == con.SecondNode).Value;
                        using (SqlCommand sqlCom = new SqlCommand($"INSERT INTO [NodesConnection] VALUES(@Title,@ConnectionIdFirst,@ConnectionIdSecond,@TreeId)"))
                        {
                            sqlCom.Connection = sqlConnection;
                            if (sqlConnection.State == ConnectionState.Closed)
                                sqlConnection.Open();
                            sqlCom.Parameters.AddWithValue("@Title", title);
                            sqlCom.Parameters.AddWithValue("@ConnectionIdFirst", first);
                            sqlCom.Parameters.AddWithValue("@ConnectionIdSecond", second);
                            sqlCom.Parameters.AddWithValue("@TreeId", TreeId);
                            sqlCom.ExecuteNonQuery();
                            sqlConnection.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                 throw ex;
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        private void saveIntoJSON(string path)
        {
            TreeGraph a = OpenedTreesDict.FirstOrDefault(tabpage => tabpage.Key == tabControl1.SelectedTab).Value;
            a.Name = Path.GetFileName(path);
            JObject o;
            List<String> list = new List<String>();

            string filename = path;
            list.Clear();
            foreach (Connection con in a.listOfConnections)
            {
                o = new JObject();
                Size originalOne = new Size(0,0);
                Size originalSecond = new Size(0,0);
                o["X1"] = con.FirstNode.buttonNode.Location.X;
                o["Y1"] = con.FirstNode.buttonNode.Location.Y;
                o["FirstPath"] = con.FirstNode.Path;
                o["X2"] = con.SecondNode.buttonNode.Location.X;
                o["Y2"] = con.SecondNode.buttonNode.Location.Y;
                o["SecondPath"] = con.SecondNode.Path;
                o["Title"] = con.Title;
                
                originalOne = DefaultTreesModel[tabControl1.SelectedTab].Find(x => x.Item1 == con.FirstNode).Item2;
                originalSecond = DefaultTreesModel[tabControl1.SelectedTab].Find(x => x.Item1 == con.SecondNode).Item2;

                o["sizeW1"] = originalOne.Width ;//con.FirstNode.buttonNode.Size.Width;
                o["sizeH1"] = originalOne.Height;//con.FirstNode.buttonNode.Size.Height;
                o["sizeW2"] = originalSecond.Width;//con.SecondNode.buttonNode.Size.Width;
                o["sizeH2"] = originalSecond.Height;//con.SecondNode.buttonNode.Size.Height;
                o["textcolor1"] = con.FirstNode.buttonNode.ForeColor.ToArgb();
                o["textcolor2"] = con.SecondNode.buttonNode.ForeColor.ToArgb();
                o["backcolor1"] = con.FirstNode.buttonNode.BackColor.ToArgb();
                o["backcolor2"] = con.SecondNode.buttonNode.BackColor.ToArgb();
                o["borderRad1"] = con.FirstNode.buttonNode.BorderRadius;
                o["borderRad2"] = con.SecondNode.buttonNode.BorderRadius;
                o["bordersize1"] = con.FirstNode.buttonNode.BorderSize;
                o["bordersize2"] = con.SecondNode.buttonNode.BorderSize;
                o["bordercolor1"] = con.FirstNode.buttonNode.BorderColor.ToArgb();
                o["bordercolor2"] = con.SecondNode.buttonNode.BorderColor.ToArgb();
                list.Add(o.ToString());
            }
            writeFileAsync(list, filename);
        }

        private bool checkExistanceOfGraphInDB(string path)
        {
            int idOfTree = -1;
            int idOfFolder = -1;
            using (SqlCommand sqlCom = new SqlCommand($"SELECT * FROM [Folder]"))
            {
                sqlCom.Connection = sqlConnection;
                if (sqlConnection.State == ConnectionState.Closed)
                    sqlConnection.Open();
                SqlDataReader reader = sqlCom.ExecuteReader();
                while (reader.Read())
                {
                    if (reader["Path"].ToString() == path)
                    {
                        idOfFolder = (Int32)reader["Id"];
                        break;
                    }
                }
                reader.Close();
            }

            /// OPPPP
            using (SqlCommand sqlC = new SqlCommand($"SELECT * FROM [Tree] WHERE Path = @path"))
            {
                sqlC.Connection = sqlConnection;
                if (sqlConnection.State == ConnectionState.Closed)
                    sqlConnection.Open();
                sqlC.Parameters.AddWithValue("@path", path);
                SqlDataReader reader = sqlC.ExecuteReader();

                while (reader.Read())
                {
                    if (reader["FolderId"].ToString() == idOfFolder.ToString())
                    {
                        idOfTree = (Int32)reader["Id"];
                        break;
                    }
                }
                reader.Close();
            }
            if (idOfTree != -1)
                return true;
            return false;
        }
        private void deleteFromDataBase(string path)
        {
            int idOfTree = -1;
            int idOfFolder = -1;
            string folderP = Path.GetDirectoryName(path);
            try
            {
                using (SqlCommand sqlCom = new SqlCommand($"SELECT * FROM [Folder]"))
                {
                    sqlCom.Connection = sqlConnection;
                    if (sqlConnection.State == ConnectionState.Closed)
                        sqlConnection.Open();
                    SqlDataReader reader = sqlCom.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader["Path"].ToString() == path)
                        {
                            idOfFolder = (Int32)reader["Id"];
                            break;
                        }
                    }
                    reader.Close();
                }

                /// OPPPP
                using (SqlCommand sqlC = new SqlCommand($"SELECT * FROM [Tree] WHERE Path = @path"))
                {
                    sqlC.Connection = sqlConnection;
                    if (sqlConnection.State == ConnectionState.Closed)
                        sqlConnection.Open();
                    sqlC.Parameters.AddWithValue("@path", path);
                    SqlDataReader reader = sqlC.ExecuteReader();

                    while (reader.Read())
                    {
                        if (reader["FolderId"].ToString() == idOfFolder.ToString())
                        {
                            idOfTree = (Int32)reader["Id"];
                            break;
                        }
                    }
                    reader.Close();
                }
                if (idOfTree != -1)
                {
                    List<int> listOfConnections = new List<int>();
                    List<int> listOfBackups = new List<int>();
                    using (SqlCommand sqlCom = new SqlCommand($"SELECT * FROM [Connection] WHERE [TreeId] = @IdOfTree"))
                    {
                        sqlCom.Connection = sqlConnection;
                        if (sqlConnection.State == ConnectionState.Closed)
                            sqlConnection.Open();
                        sqlCom.Parameters.AddWithValue("@IdOfTree", idOfTree);
                        SqlDataReader sqlReader = sqlCom.ExecuteReader();
                        while (sqlReader.Read())
                        {
                            listOfConnections.Add((Int32)sqlReader["Id"]);
                        }
                        sqlReader.Close();
                    }
                    foreach (int i in listOfConnections)
                    {
                        using (SqlCommand sqlCom = new SqlCommand($"DELETE FROM [NodesConnection] WHERE" +
                            "[ConnectionIdFirst] = @FirstId OR [ConnectionIdSecond] = @FirstId"))
                        {
                            sqlCom.Connection = sqlConnection;
                            if (sqlConnection.State == ConnectionState.Closed)
                                sqlConnection.Open();
                            sqlCom.Parameters.AddWithValue("@FirstId", listOfConnections[i]);
                            sqlCom.ExecuteNonQuery();
                        }
                    }
                    foreach (int i in listOfConnections)
                    {
                        using (SqlCommand sqlC = new SqlCommand($"SELECT [BackupId] FROM [Connection] WHERE [Id] = @idOfConneciton"))
                        {
                            sqlC.Connection = sqlConnection;
                            if (sqlConnection.State == ConnectionState.Closed)
                                sqlConnection.Open();
                            sqlC.Parameters.AddWithValue(@"idOfConnection", i);
                            SqlDataReader reader = sqlC.ExecuteReader();
                            while (reader.Read())
                            {
                                listOfBackups.Add((Int32)reader["BackupId"]);
                            }
                            reader.Close();
                        }
                    }
                    listOfBackups = listOfBackups.Distinct().ToList();
                    foreach (int i in listOfConnections)
                    {
                        using (SqlCommand sqlCom = new SqlCommand($"DELETE FROM [Connection] WHERE [Id] = @FirstId"))
                        {
                            sqlCom.Connection = sqlConnection;
                            if (sqlConnection.State == ConnectionState.Closed)
                                sqlConnection.Open();
                            sqlCom.Parameters.AddWithValue("@FirstId", listOfConnections[i]);
                            sqlCom.ExecuteNonQuery();
                        }
                    }
                    foreach (int i in listOfBackups)
                    {
                        using (SqlCommand sqlC = new SqlCommand($"DELETE FROM [Backup] WHERE [Id] = @IdOfBackup"))
                        {
                            sqlC.Connection = sqlConnection;
                            if (sqlConnection.State == ConnectionState.Closed)
                                sqlConnection.Open();
                            sqlC.Parameters.AddWithValue("@IdOfBackup", i);
                            sqlC.ExecuteNonQuery();
                        }
                    }
                    using (SqlCommand sqlC = new SqlCommand($"DELETE FROM [Tree] WHERE [Id] = @IdOfTree"))
                    {
                        sqlC.Connection = sqlConnection;
                        if (sqlConnection.State == ConnectionState.Closed)
                            sqlConnection.Open();
                        sqlC.Parameters.AddWithValue("IdOfTree", idOfTree);
                        sqlC.ExecuteNonQuery();
                    }
                }

            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                sqlConnection.Close();
            }
    }


        private void deleteAndSaveTextFile(string path)
        {
            try
            {
                bool folderExistance = false;
                int folderId = -1;
                using (SqlCommand sqlCom = new SqlCommand($"SELECT * FROM [Folder]"))
                {
                    sqlCom.Connection = sqlConnection;
                    if (sqlConnection.State == ConnectionState.Closed)
                        sqlConnection.Open();
                    SqlDataReader sqlReader = sqlCom.ExecuteReader();
                    while (sqlReader.Read())
                    {
                        if (sqlReader["Path"].ToString() == Path.GetDirectoryName(path))
                        {
                            folderExistance = true;
                            folderId = (Int32)sqlReader["Id"];
                            break;
                        }
                    }
                    sqlReader.Close();
                }
                if (!folderExistance)
                {
                    using (SqlCommand sqlCom = new SqlCommand($"INSERT INTO OUTPUT INSERTED.Id [FOLDER] VALUES(@PATH)"))
                    {
                        sqlCom.Connection = sqlConnection;
                        if (sqlConnection.State == ConnectionState.Closed)
                            sqlConnection.Open();
                        sqlCom.Parameters.AddWithValue("@PATH", Path.GetDirectoryName(path));
                        folderId = (int)sqlCom.ExecuteScalar();
                    }

                    string text = File.ReadAllText(path);
                    using (SqlCommand sqlCom = new SqlCommand($"INSERT INTO [Backup] VALUES(@Name, @Text, @isOpen, @FolderId)"))
                    {
                        sqlCom.Connection = sqlConnection;
                        if (sqlConnection.State == ConnectionState.Closed)
                            sqlConnection.Open();
                        sqlCom.Parameters.AddWithValue("@Name", Path.GetFileName(path));
                        sqlCom.Parameters.AddWithValue("@Text", text);
                        //sqlCom.Parameters.AddWithValue("@isOpen",);
                    }
                }
                else
                {
                    //int idOfBackup = -1;
                    List<int> idOfBackups = new List<int>();
                    using(SqlCommand sqlCom = new SqlCommand($"SELECT Id FROM [Backup] WHERE [FolderId] = @folderId AND [Name] = @name"))
                    {
                        sqlCom.Connection = sqlConnection;
                        if (sqlConnection.State == ConnectionState.Closed)
                            sqlConnection.Open();
                        sqlCom.Parameters.AddWithValue("@folderId", folderId);
                        sqlCom.Parameters.AddWithValue("@name", Path.GetFileName(path));
                        //idOfBackup = Convert.ToInt32(sqlCom.ExecuteScalar());
                        SqlDataReader reader = sqlCom.ExecuteReader();
                        while (reader.Read())
                        {
                            idOfBackups.Add((Int32)reader["Id"]);
                        }
                        reader.Close();
                    }
                    if(idOfBackups.Count != 0)
                    {
                        string text = File.ReadAllText(path);
                        using(SqlCommand sqlCom = new SqlCommand("UPDATE [Backup] SET [Text] = @text WHERE [Id] = @id"))
                        {
                            sqlCom.Connection = sqlConnection;
                            if (sqlConnection.State == ConnectionState.Closed)
                                sqlConnection.Open();
                            foreach (int id in idOfBackups) 
                            {
                                sqlCom.Parameters.AddWithValue("@text", text);
                                sqlCom.Parameters.AddWithValue("@id", id);
                                sqlCom.ExecuteNonQuery();        
                            }
                        }
                        sqlConnection.Close();
                    }

                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
            finally
            {
                sqlConnection.Close();
            }
        }




        private void writeFileAsync(List<string> list, string filename)
        {
            
            using (StreamWriter sw = File.AppendText(filename))
            {
                foreach (string s in list)
                {
                    sw.WriteLine(s);
                }
            }
        }
        public static partial class JsonExtensions
        {
            public static IEnumerable<T> FromDelimitedJson<T>(TextReader reader, JsonSerializerSettings settings = null)
            {
                using (var jsonReader = new JsonTextReader(reader) { CloseInput = false, SupportMultipleContent = true })
                {
                    var serializer = JsonSerializer.CreateDefault(settings);

                    while (jsonReader.Read())
                    {
                        if (jsonReader.TokenType == JsonToken.Comment)
                            continue;
                        yield return serializer.Deserialize<T>(jsonReader);
                    }
                }
            }
        }



        private void newTreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TabPage treeTabPage = new TabPage();
            treeTabPage.Text = "New Tree";
            Panel pn = new Panel();
            pn.Name = "panel";
            pn.AutoScroll = true;
            pn.Dock = DockStyle.Fill;
            PictureBox pictureBox = new PictureBox();
            new PanelWndProc().AssignHandle(pn.Handle);
            //VScrollBar vscroll = new VScrollBar();
            //vscroll.Dock = DockStyle.Right;
            //pictureBox.Controls.Add(vscroll);
            pictureBox.Name = "pictureBox";
            pictureBox.MouseWheel += pictureBox1_MouseWheel;
           // CustomButton n = new CustomButton();
            //n.Name = "but";
            //n.Text = "Butt";
            //pictureBox.Controls.Add(n);
            //pictureBox.Dock = DockStyle.None;
            pictureBox.Size = new Size(defaultPictureBoxWidth, defaultPictureBoxHeigth);
            pn.Controls.Add(pictureBox);
            //treeTabPage.Controls.Add(pictureBox);
            treeTabPage.Controls.Add(pn);
            
            
            
            TreeGraph tg = new TreeGraph();
            OpenedTreesDict.Add(treeTabPage, tg);
            List<(NodeOfTree, Size)> nodes = new List<(NodeOfTree, Size)>();
            DefaultTreesModel.Add(treeTabPage, nodes);
            scaleStatus.Add(tg, new Tuple<int, int>(0, 0));
            
            tabControl1.TabPages.Add(treeTabPage);
            pictureBox.MouseDown += PictureBox_MouseDown;
            pictureBox.MouseUp += PictureBox_MouseUp;
            tabControl1.SelectedTab = treeTabPage;
            CreatePicture(pictureBox);
            //pictureBox.Resize += PictureBox_Resize;
           
            //graphics.DrawString("Sd", new Font("Comic Sans MS", 10), mySolidBrush, new Point(125, 55));
            pictureBox.Invalidate();
        }

        private void PictureBox_Resize(object sender, EventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            pb.Image = null;
            CreatePicture(pb);
            //RefreshAfterPosChange();
        }

        private void CreatePicture(PictureBox pb)
        {
            Bitmap bm = new Bitmap(defaultPictureBoxWidth, defaultPictureBoxHeigth);//tabControl1.SelectedTab.Size.Width, tabControl1.SelectedTab.Size.Height);
            pb.Image = bm;
           // pb.Dock = DockStyle.Fill;
            graphics = Graphics.FromImage(pb.Image);
            graphics.Clear(Color.White);
            RefreshAfterPosChange();
        }

        private void PictureBox_MouseUp(object sender, MouseEventArgs e){}

        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left) tempPoint = e.Location;
        }


        private double Angle(int x1, int y1, int x2, int y2)
        {
            double degrees;

            if (x2 - x1 == 0)
            {
                if (y2 > y1)
                    degrees = 90;
                else
                    degrees = 270;
            }
            else
            {
                double overrun = (double)(y2 - y1) / (double)(x2 - x1);
                double radians = Math.Atan(overrun);
                degrees = radians * ((Double)180 / Math.PI);

                if ((x2 - x1) < 0 || (y2 - y1) < 0)
                    degrees += 180;
                if ((x2 - x1) > 0 && (y2 - y1) < 0)
                    degrees -= 180;
                if (degrees < 0)
                    degrees += 360;
            }
            return degrees;
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (String.IsNullOrEmpty(Path.GetExtension(e.Node.Text)))
                return;
            string path = treeView1.SelectedNode.Tag as string;
            string name = treeView1.SelectedNode.Text;
            foreach (TabPage tpp in tabControl1.TabPages)
            {
                if(tpp.Name ==  name)
                {
                    tabControl1.SelectedTab = tpp;
                    return;
                }
            }
            TabPage tp = new TabPage();
            tp.Text = name;
            tp.Name = name;
            string extension = Path.GetExtension(path);
            tabControl1.TabPages.Add(tp);
            switch (extension)
            {
                case ".JSON":
                    string text = File.ReadAllText(path);
                    var lst = JsonExtensions.FromDelimitedJson<forJson>(new StringReader(text)).ToList();
                    tp.Text = Path.GetFileName(path);
                    //treeTabPage.Text = "New Tree";
                    Panel pn = new Panel();
                    pn.Name = "panel";
                    pn.AutoScroll = true;
                    pn.Dock = DockStyle.Fill;
                    new PanelWndProc().AssignHandle(pn.Handle);

                    PictureBox pictureBox = new PictureBox();
                    pictureBox.MouseWheel += pictureBox1_MouseWheel;
                    //tp.Controls.Add(pictureBox);
                    pictureBox.Name = "pictureBox";
                    //pictureBox.Dock = DockStyle.Fill;
                    pictureBox.Size = new Size(defaultPictureBoxWidth, defaultPictureBoxHeigth);
                    pn.Controls.Add(pictureBox);
                    tp.Controls.Add(pn);
                    TreeGraph tg = new TreeGraph();
                    scaleStatus.Add(tg, new Tuple<int, int>(0, 0));
                    OpenedTreesDict.Add(tp, tg);
                    List<(NodeOfTree, Size)> nodes = new List<(NodeOfTree, Size)>();
                    DefaultTreesModel.Add(tp, nodes);
                   // tabControl1.TabPages.Add(tp);
                    pictureBox.MouseDown += PictureBox_MouseDown;
                    pictureBox.MouseUp += PictureBox_MouseUp;
                    OpenedFilesList.Add(path);
                    tabControl1.SelectedTab = tp;
                    List<Button> checklist = new List<Button>();

                        
                    foreach (forJson node in lst)
                    {
                        //MessageBox.Show((node as forJson).X1.ToString() + (node as forJson).Y1.ToString());
                        Control[] array = { };
                        array = pictureBox.Controls.Find(Path.GetFileName(node.FirstPath), true);
                        NodeOfTree nodeFirst = new NodeOfTree();
                        if (array == null || array.Length == 0)
                        {
                            CustomButton buttonFirst = new CustomButton();
                            buttonFirst.Width = node.sizeW1;
                            buttonFirst.Height = node.sizeH1;
                            buttonFirst.Location = new Point(node.X1, node.Y1);
                            buttonFirst.BackColor = Color.FromArgb(node.backcolor1);
                            buttonFirst.ForeColor = Color.FromArgb(node.textcolor1);
                            buttonFirst.ContextMenuStrip = contextMenuStrip1;
                            buttonFirst.BorderRadius = node.borderRad1;
                            buttonFirst.Text = Path.GetFileName(node.FirstPath);
                            buttonFirst.MouseDown += Btn_MouseDown;
                            buttonFirst.MouseUp += Btn_MouseUp;
                            buttonFirst.Click += Btn_Click;
                            buttonFirst.BorderSize = node.bordersize1;
                            if (buttonFirst.BorderSize != 0)
                                buttonFirst.BorderColor = Color.FromArgb(node.bordercolor1);
                            pictureBox.Controls.Add(buttonFirst);
                           nodeFirst = new NodeOfTree(buttonFirst, Path.GetFileName(node.FirstPath), node.FirstPath);
                            tg.listOfNodes.Add(nodeFirst);
                            DefaultTreesModel[tp].Add((nodeFirst, buttonFirst.Size));
                        }
                        else
                        {
                            CustomButton buttonFirst = array[0] as CustomButton;
                            nodeFirst = tg.listOfNodes.Find(x => x.Name == buttonFirst.Name);
                        }
                        Array.Clear(array, 0, array.Length);


                        array = pictureBox.Controls.Find(Path.GetFileName(node.SecondPath), true);
                        NodeOfTree nodeSecond = new NodeOfTree();
                        if (array == null || array.Length == 0)
                        {
                            CustomButton buttonSecond = new CustomButton();
                            buttonSecond.Width = node.sizeW2;
                            buttonSecond.Height = node.sizeH2;
                            buttonSecond.Location = new Point(node.X2, node.Y2);
                            buttonSecond.BackColor = Color.FromArgb(node.backcolor2);
                            buttonSecond.ForeColor = Color.FromArgb(node.textcolor2);
                            buttonSecond.BorderRadius = node.borderRad2;
                            buttonSecond.Text = Path.GetFileName(node.SecondPath);
                            buttonSecond.MouseDown += Btn_MouseDown;
                            buttonSecond.MouseUp += Btn_MouseUp;
                            buttonSecond.ContextMenuStrip = contextMenuStrip1;
                            buttonSecond.Click += Btn_Click;
                            buttonSecond.BorderSize = node.bordersize2;
                            if (buttonSecond.BorderSize != 0)
                                buttonSecond.BorderColor = Color.FromArgb(node.bordercolor2);
                            pictureBox.Controls.Add(buttonSecond);
                            nodeSecond = new NodeOfTree(buttonSecond, Path.GetFileName(node.SecondPath), node.SecondPath);
                            tg.listOfNodes.Add(nodeSecond);
                            DefaultTreesModel[tp].Add((nodeSecond, buttonSecond.Size));
                        }
                        else 
                        {
                            CustomButton buttonSecond = array[0] as CustomButton;
                            nodeSecond = tg.listOfNodes.Find(x => x.Name == buttonSecond.Name);
                        }
                        Connection conn = new Connection(nodeFirst, nodeSecond, node.Title);
                        tg.listOfConnections.Add(conn);
                    }
                    CreatePicture(pictureBox);
                    pictureBox.Invalidate();
                        break;
                case ".txt":
                    MyRichTextBox myRichTextBox = new MyRichTextBox();
                    myRichTextBox.Dock = DockStyle.Fill;
                    tp.Controls.Add(myRichTextBox);
                    tabControl1.SelectedTab = tp;
                    string textt = "Cannot read the file";
                    if (path != null && File.Exists(path))
                    {
                        textt = File.ReadAllText(path);
                    }
                    RichTextBox rtb = myRichTextBox.Controls.Find("richTextBox1", true)[0] as RichTextBox;
                    rtb.Text = textt;
                    rtb.TextChanged += RichTextBox1_TextChanged;
                    OpenedFilesList.Add(path);
                    break;
                case null:
                    break;
                default:
                    MessageBox.Show("This format is not supported yet and cannot be opened.");
                    break;
                    ///DOPISAC!!!!
                
            }


        /*    PictureBox pb = new PictureBox();
                foreach(Control c in tabControl1.SelectedTab.Controls)
                {
                    if(c.Name.Contains("Box"))
                     {
                        pb = c as PictureBox;
                        break;
                    }
                }
            Control [] arrayOfControls = tabControl1.SelectedTab.Controls.Find("pictureBox", true);
            if (arrayOfControls != null && arrayOfControls.Length != 0)
            {
                CustomButton btn = new CustomButton();
                btn.Name = treeView1.SelectedNode.Text;
                btn.Text = treeView1.SelectedNode.Text;
                btn.ContextMenuStrip = contextMenuStrip1;
                btn.Location = new Point(300, 50);
                btn.Size = new Size(200, 100);
                btn.MouseDown += Btn_MouseDown;
                btn.MouseUp += Btn_MouseUp;
                btn.Click += Btn_Click;

                string path = treeView1.SelectedNode.Tag as string;
                string name = treeView1.SelectedNode.Text;

                NodeOfTree node = new NodeOfTree(btn, name, path);

                TabPage ourPage = tabControl1.SelectedTab;
                TreeGraph thisGraph = null;

                if(OpenedTreesDict.TryGetValue(ourPage,out thisGraph))
                {
                    thisGraph.listOfNodes.Add(node);
                }
                pb.Controls.Add(btn);
            }*/
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            if (tempButton != null && tempButton != sender as CustomButton && tempNode != null)
            {
                CustomButton tempButt = tempButton;
                tempButton = null;
                NodeOfTree currentNode = new NodeOfTree();
                TreeGraph currentTree = null;
                Connection connect = new Connection();
                if (OpenedTreesDict.TryGetValue(tabControl1.SelectedTab, out currentTree))
                {
                    foreach (NodeOfTree not in currentTree.listOfNodes)
                    {
                        if (not.buttonNode == (sender as CustomButton))
                        {
                            currentNode = not;
                        }
                    }

                    string Answer = "";
                    using(ConnectcionNamePicker form = new ConnectcionNamePicker())
                    {
                        DialogResult result = form.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            Answer = form.conName;
                        }
                        else
                            return;
                    }
                    if (String.IsNullOrEmpty(Answer))
                        Answer = "New Connection";
                    connect = new Connection(currentNode, tempNode, Answer);
                    tempNode = null;
                    foreach (Connection conn in currentTree.listOfConnections)
                    {
                        if (conn.Compare(connect))
                        {
                            return;
                        }
                    }
                    currentTree.listOfConnections.Add(connect);
                    Point centerOfConnection = getCenterOfCon(connect);
                    double angle = Angle(tempButt.Location.X, tempButt.Location.Y, (sender as CustomButton).Location.X, (sender as CustomButton).Location.Y);
                    //DrawTextWithAngle(graphics, angle, connect.Title, centerOfConnection, new Font("Comic Sans MS", 10), mySolidBrush);
                    //graphics.RotateTransform((float)angle);
                    // graphics.DrawString(connect.Title, new Font("Comic Sans MS", 20),mySolidBrush,centerOfConnection);


                    graphics.DrawLine(myPen, new Point(tempButt.Location.X + 100, tempButt.Location.Y + 50), new Point((sender as CustomButton).Location.X + 100, (sender as CustomButton).Location.Y + 50));
                   // graphics.RotateTransform((float)angle);
                    graphics.DrawString(connect.Title, new Font("Comic Sans MS", 10), mySolidBrush, centerOfConnection);


                    TabPage tp = tabControl1.SelectedTab;
                    PictureBox pb = tp.Controls.Find("pictureBox", true)[0] as PictureBox;

                    pb.Invalidate();
                }
            }
        }

        private void DrawTextWithAngle(Graphics graphics, double angle, string text, Point pos, Font font, Brush brush)
        {
            GraphicsState state = graphics.Save();
            graphics.ResetTransform();

            graphics.RotateTransform((float)angle);

            graphics.TranslateTransform(pos.X, pos.Y, MatrixOrder.Append);

            graphics.DrawString(text, font, brush,0,0);

            graphics.Restore(state);

        }


        private Point getCenterOfCon(Connection conn)
        {
            int x = conn.FirstNode.buttonNode.Location.X + conn.SecondNode.buttonNode.Location.X;
            int y = conn.FirstNode.buttonNode.Location.Y + conn.SecondNode.buttonNode.Location.Y;
            return new Point(x / 2, y / 2);
        }

        private void Btn_MouseUp(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                Point justDistance = new Point();
                justDistance.X = Math.Abs(e.X - tempPositionOfButton.X);
                justDistance.Y = Math.Abs(e.Y - tempPositionOfButton.Y);

                Point newPostion = new Point((sender as CustomButton).Location.X, (sender as CustomButton).Location.Y);
                if (e.X > tempPositionOfButton.X)
                    newPostion.X = newPostion.X + justDistance.X;
                else if (e.X < tempPositionOfButton.X)
                    newPostion.X = newPostion.X - justDistance.X;
                if (e.Y > tempPositionOfButton.Y)
                    newPostion.Y = newPostion.Y + justDistance.Y;
                else if (e.Y < tempPositionOfButton.Y)
                    newPostion.Y = newPostion.Y - justDistance.Y;
                (sender as CustomButton).Location = newPostion;
                RefreshAfterPosChange();
            }
        }



        private void RefreshAfterPosChange()
        {
            graphics.Clear(Color.White);
            TreeGraph currentTree = null;
            if (OpenedTreesDict.TryGetValue(tabControl1.SelectedTab, out currentTree))
            {
                foreach (Connection con in currentTree.listOfConnections)
                {
                    graphics.DrawLine(myPen, new Point(con.FirstNode.buttonNode.Location.X + (con.FirstNode.buttonNode.Size.Width/2) , 
                        con.FirstNode.buttonNode.Location.Y + (con.FirstNode.buttonNode.Size.Height/2)), 
                        new Point(con.SecondNode.buttonNode.Location.X + (con.SecondNode.buttonNode.Size.Width/2), 
                        con.SecondNode.buttonNode.Location.Y +(con.SecondNode.buttonNode.Size.Height/2)));
                    //graphics.DrawString(con.Title, new Font("Comic Sans MS", 10), mySolidBrush, getCenterOfCon(con));
                    double angle = Angle(con.FirstNode.buttonNode.Location.X, con.FirstNode.buttonNode.Location.Y, con.SecondNode.buttonNode.Location.X, con.SecondNode.buttonNode.Location.Y);
                    Point centerOfConnection = getCenterOfCon(con);
                    //DrawTextWithAngle(graphics, (float)angle, con.Title, centerOfConnection, new Font("Comic Sans MS", 10), mySolidBrush);
                    Point midOne = new Point(con.FirstNode.buttonNode.Location.X + con.FirstNode.buttonNode.Size.Width / 2,
                        con.FirstNode.buttonNode.Location.Y + con.FirstNode.buttonNode.Size.Height / 2);
                    Point midTwo = new Point(con.SecondNode.buttonNode.Location.X + con.SecondNode.buttonNode.Size.Width / 2,
                        con.SecondNode.buttonNode.Location.Y + con.SecondNode.buttonNode.Size.Height / 2);
                    Point topLeft = new Point(Math.Min(midOne.X, midTwo.X), Math.Min(midOne.Y, midTwo.Y));
                    Point bottomRight = new Point(Math.Max(midOne.X, midTwo.X),Math.Max(midOne.Y, midTwo.Y));

                    TextRenderer.DrawText(graphics, con.Title, this.Font, new Rectangle(topLeft, new Size(bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y)), Color.Black, Color.White,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }
            }
            TabPage tp = tabControl1.SelectedTab;
            PictureBox pb = tp.Controls.Find("pictureBox", true)[0] as PictureBox;
            pb.Invalidate();
        }

        private void Btn_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button == MouseButtons.Left)
            {
                tempPositionOfButton = e.Location;
            }    
        }

        private void openNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CustomButton source = new CustomButton();
            //source.Text = treeView1.SelectedNode.Text;
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (menuItem != null)
            {
                ContextMenuStrip owner = menuItem.Owner as ContextMenuStrip;
                if (owner != null)
                {
                    source = owner.SourceControl as CustomButton;
                }
            }
            else
            {
                MessageBox.Show("Ooops! Something went wrong, try again!");
                return;
            }

            TreeGraph currentGraph = null;
            OpenedTreesDict.TryGetValue(tabControl1.SelectedTab, out currentGraph);
            string res = null;
            foreach(String s in OpenedFilesList)
            {
                /// TODO CHAMNGEEEEEEEEEEEEEEEEEEEEEEEEEEEE (wyr regular)
                if(s.Contains(source.Text))
                {
                    res = source.Text;
                    break;
                }
            }
            string path = null;
            foreach(NodeOfTree noT in currentGraph.listOfNodes)
            {
               if(noT.buttonNode == source)
                    path = noT.Path;
            }

            if (res == null)
            {
                TabPage tp = new TabPage();
                tp.Text = source.Text;
                tabControl1.TabPages.Add(tp);
                MyRichTextBox myRichTextBox = new MyRichTextBox();
                myRichTextBox.Dock = DockStyle.Fill;
                tp.Controls.Add(myRichTextBox);
                tabControl1.SelectedTab = tp;
                string text = "Cannot read the file";
                if (path != null && File.Exists(path))
                {
                    text = File.ReadAllText(path);
                }
                RichTextBox rtb = myRichTextBox.Controls.Find("richTextBox1", true)[0] as RichTextBox;
                rtb.Text = text;
                rtb.TextChanged += RichTextBox1_TextChanged;
                OpenedFilesList.Add(path);
            }
            else
            {
                TabPage tp = new TabPage();
                foreach(TabPage t in tabControl1.TabPages)
                {
                    if(t.Text == source.Text)
                    {
                        tp = t;
                        break;
                    }
                }
                tabControl1.SelectedTab = tp;
            }
        }

        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Title = "Choose file to open ";
            openFile.Filter = "Text Files | *.txt|Tree File| *.JSON";

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                
                switch (Path.GetExtension(openFile.FileName))
                {
                    case ".txt":
                        currentFileName = openFile.FileName;
                        OpenedFilesList.Add(currentFileName);
                        TabPage tp = new TabPage();
                        tp.Text = Path.GetFileName(currentFileName);
                        tabControl1.TabPages.Add(tp);
                        MyRichTextBox nrtb = new MyRichTextBox();
                        nrtb.Dock = DockStyle.Fill;
                        tp.Controls.Add(nrtb);
                        tabControl1.SelectedTab = tp;
                        currentPath = openFile.FileName;
                        //OpenedFilesList.Add(currentPath);
                        RichTextBox rtb = nrtb.Controls.Find("richTextBox1", true)[0] as RichTextBox;
                        rtb.Text = File.ReadAllText(openFile.FileName);
                        rtb.TextChanged += RichTextBox1_TextChanged;
                        break;
                    case ".JSON":
                        string text = File.ReadAllText(openFile.FileName);
                        var lst = JsonExtensions.FromDelimitedJson<forJson>(new StringReader(text)).ToList();
                        TabPage treeTabPage = new TabPage();
                        treeTabPage.Text = Path.GetFileName(openFile.FileName);
                        //treeTabPage.Text = "New Tree";
                        Panel pn = new Panel();
                        pn.Name = "panel";
                        pn.AutoScroll = true;
                        pn.Dock = DockStyle.Fill;
                        new PanelWndProc().AssignHandle(pn.Handle);
                        PictureBox pictureBox = new PictureBox();
                        pictureBox.MouseWheel += pictureBox1_MouseWheel;
                        pictureBox.Name = "pictureBox";
                        //pictureBox.Dock = DockStyle.Fill;
                        pictureBox.Size = new Size(defaultPictureBoxWidth, defaultPictureBoxHeigth);
                        pn.Controls.Add(pictureBox);
                        treeTabPage.Controls.Add(pn);
                        TreeGraph tg = new TreeGraph();
                        scaleStatus.Add(tg, new Tuple<int, int>(0, 0));
                        OpenedTreesDict.Add(treeTabPage, tg);
                        List<(NodeOfTree, Size)> nodes = new List<(NodeOfTree, Size)>();
                        DefaultTreesModel.Add(treeTabPage, nodes);
                        tabControl1.TabPages.Add(treeTabPage);
                        pictureBox.MouseDown += PictureBox_MouseDown;
                        pictureBox.MouseUp += PictureBox_MouseUp;
                        tabControl1.SelectedTab = treeTabPage;
                        List<Button> checklist = new List<Button>();

                        foreach (forJson node in lst)
                        {
                            Control[] array = { }; 
                            array = pictureBox.Controls.Find(Path.GetFileName(node.FirstPath), true);
                            NodeOfTree nodeFirst = new NodeOfTree();
                            if (array == null ||  array.Length == 0 )
                            {
                                CustomButton buttonFirst = new CustomButton();
                                buttonFirst.Width = node.sizeW1;
                                buttonFirst.Height = node.sizeH1;
                                buttonFirst.Location = new Point(node.X1, node.Y1);
                                buttonFirst.BackColor = Color.FromArgb(node.backcolor1);
                                buttonFirst.ForeColor = Color.FromArgb(node.textcolor1);
                                buttonFirst.BorderRadius = node.borderRad1;
                                buttonFirst.Text = Path.GetFileName(node.FirstPath);
                                buttonFirst.Name = Path.GetFileName(node.FirstPath);
                                buttonFirst.ContextMenuStrip = contextMenuStrip1;
                                buttonFirst.MouseDown += Btn_MouseDown;
                                buttonFirst.MouseUp += Btn_MouseUp;
                                buttonFirst.Click += Btn_Click;
                                buttonFirst.BorderSize = node.bordersize1;
                                if (buttonFirst.BorderSize != 0)
                                    buttonFirst.BorderColor = Color.FromArgb(node.bordercolor1);
                                pictureBox.Controls.Add(buttonFirst);
                                nodeFirst = new NodeOfTree(buttonFirst, Path.GetFileName(node.FirstPath), node.FirstPath);
                                tg.listOfNodes.Add(nodeFirst);
                                DefaultTreesModel[treeTabPage].Add((nodeFirst, buttonFirst.Size));
                            }
                            else
                            {
                                CustomButton buttonFirst = array[0] as CustomButton;
                                nodeFirst = tg.listOfNodes.Find(x => x.Name == buttonFirst.Name);
                            }
                            Array.Clear(array,0,array.Length);

                            array = pictureBox.Controls.Find(Path.GetFileName(node.SecondPath),true);
                            NodeOfTree nodeSecond = new NodeOfTree();
                            if (array == null || array.Length == 0)
                            {
                                CustomButton buttonSecond = new CustomButton();
                                buttonSecond.Width = node.sizeW2;
                                buttonSecond.Height = node.sizeH2;
                                buttonSecond.Location = new Point(node.X2, node.Y2);
                                buttonSecond.BackColor = Color.FromArgb(node.backcolor2);
                                buttonSecond.ForeColor = Color.FromArgb(node.textcolor2);
                                buttonSecond.BorderRadius = node.borderRad2;
                                buttonSecond.Text = Path.GetFileName(node.SecondPath);
                                buttonSecond.Name = Path.GetFileName(node.SecondPath);
                                buttonSecond.ContextMenuStrip = contextMenuStrip1;
                                buttonSecond.MouseDown += Btn_MouseDown;
                                buttonSecond.MouseUp += Btn_MouseUp;
                                buttonSecond.Click += Btn_Click;
                                buttonSecond.BorderSize = node.bordersize2;
                                if (buttonSecond.BorderSize != 0)
                                    buttonSecond.BorderColor = Color.FromArgb(node.bordercolor2);
                                pictureBox.Controls.Add(buttonSecond);
                                nodeSecond = new NodeOfTree(buttonSecond, Path.GetFileName(node.SecondPath), node.SecondPath);
                                tg.listOfNodes.Add(nodeSecond);
                                DefaultTreesModel[treeTabPage].Add((nodeSecond,buttonSecond.Size));
                            }
                            else
                            {
                                CustomButton buttonSecond = array[0] as CustomButton;
                                nodeSecond = tg.listOfNodes.Find(x => x.Name == buttonSecond.Name);
                            }
                            Connection conn = new Connection(nodeFirst, nodeSecond, node.Title);
                            tg.listOfConnections.Add(conn);
                        }
                        CreatePicture(pictureBox);
                        pictureBox.Invalidate();
                        break;
                    default:
                        MessageBox.Show("Sorry we do not support such format at this time!");
                        break;
                }

            
            }
        }

        private void connectNotesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if(menuItem != null)
            {
                ContextMenuStrip owner = menuItem.Owner as ContextMenuStrip;
                if (owner != null)
                {
                    CustomButton button = owner.SourceControl as CustomButton;
                    button.Font = new Font(button.Font.Name, button.Font.Size, FontStyle.Bold);
                    TreeGraph currentTree = null;
                    tempButton = button;
                    if (OpenedTreesDict.TryGetValue(tabControl1.SelectedTab, out currentTree))
                    {
                        foreach (NodeOfTree not in currentTree.listOfNodes)
                        {
                            if (not.buttonNode == button)
                            {
                                tempNode = not;
                            }
                        }
                    }
                }
            }
        }

        private void addDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "C:\\Users";
            dialog.IsFolderPicker = true;
            if(dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string path = dialog.FileName;
                try
                {
                    int id = -1;
                    bool isOpen = false;
                    using(SqlCommand Sqlc = new SqlCommand($"SELECT * FROM [Folder]"))
                    {
                        Sqlc.Connection = sqlConnection;
                        if (sqlConnection.State == ConnectionState.Closed)
                            sqlConnection.Open();
                        SqlDataReader readerS = Sqlc.ExecuteReader();
                        while (readerS.Read())
                        {
                            if(readerS["Path"].ToString() == path)
                            {
                                isOpen = (bool)readerS["isOpen"];
                                id = (Int32)readerS["Id"];
                                break;
                            }    
                        }
                        readerS.Close();
                        sqlConnection.Close();
                        if(id == -1)
                        {
                            using (SqlCommand com = new SqlCommand($"INSERT INTO [Folder] VALUES (@Path, @isOpen)"))
                            {
                                com.Connection = sqlConnection;
                                if (sqlConnection.State == ConnectionState.Closed)
                                    sqlConnection.Open();
                                com.Parameters.AddWithValue("@Path", path);
                                com.Parameters.AddWithValue("@isOpen", 1);
                                com.ExecuteNonQuery();
                                sqlConnection.Close();
                            }
                            try
                            {
                                loadFromDirectory(treeView1, path, 1, 0);
                            }
                            catch (Exception ex)
                            {
                                return;
                            }
                        }
                        else
                        {
                            if(isOpen == false)
                            {
                                using (SqlCommand sqlcom = new SqlCommand("UPDATE [Folder] SET [isOpen] = 1 WHERE [Id] = @Id"))
                                {
                                    sqlcom.Connection = sqlConnection;
                                    if (sqlConnection.State == ConnectionState.Closed)
                                        sqlConnection.Open();
                                    sqlcom.Parameters.AddWithValue("@Id", id);
                                    sqlcom.ExecuteNonQuery();
                                }
                                try
                                {
                                    loadFromDirectory(treeView1, path, 1, 0);
                                }
                                catch (Exception ex)
                                {
                                    return;
                                }
                            }             
                        }
                    }
                }
                catch(Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }

        private void newDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
           string Answer =  Interaction.InputBox("Enter Folder Name", "Create Local Folder");
            string Path = null;
            if(Answer != null && Answer != "" )
            {
                Path = Directory.GetCurrentDirectory() + "\\";
                Path += Answer;
                bool existance = Directory.Exists(Path);
                DirectoryInfo a = Directory.CreateDirectory(Path);
                if (!existance)
                {
                    loadFromDirectory(treeView1, Path, 1, 0);
                    addPathToDB(Path,true);
                }
                else
                {
                    MessageBox.Show("Directory with this name already exists!");
                }
            }
        }

      

        private void tabControl1_MouseUp(object sender, MouseEventArgs e)
        {
            /*if(e.Button == MouseButtons.Left)
            {
                for(int i=0;i<tabControl1.TabCount;i++)
                {
                    Rectangle r = tabControl1.GetTabRect(i);
                    Rectangle closeButton = new Rectangle(r.Right - 15, r.Top + 4, 9, 7);
                    if(closeButton.Contains(e.Location))
                    {
                        DialogResult result = MessageBox.Show("Would you like to close this file?", "File Closing", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (result == DialogResult.Yes)
                        {
                            if (tabControl1.TabPages[i].Text.Contains("*")) 
                            {
                                DialogResult res = MessageBox.Show("Would you like to save the file?", "File Saving", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (res == DialogResult.Yes)
                                {
                                    saveCurrentFileToolStripMenuItem_Click(null,null);
                                    tabControl1.TabPages.Remove(tabControl1.SelectedTab);
                                }
                                else
                                {
                                    tabControl1.TabPages.Remove(tabControl1.SelectedTab);
                                }
                            }
                            else
                            {
                                tabControl1.TabPages.Remove(tabControl1.SelectedTab);
                            }
                        }
                    }
                }
            }*/
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab != null)
            {
                Control[] arrayOfControls = tabControl1.SelectedTab.Controls.Find("pictureBox", true);
                if (arrayOfControls != null && arrayOfControls.Length != 0)
                {
                    PictureBox pb = arrayOfControls[0] as PictureBox;
                    CreatePicture(pb);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ButtonSettingsMenu a = new ButtonSettingsMenu(sender);
            a.ShowDialog();
            CustomButton butt = sender as CustomButton;
            if (a.returnButton != null)
            {
                butt.BackColor = a.returnButton.BackColor;
                butt.ForeColor = a.returnButton.ForeColor;
                if((a.returnButton.Tag as string).ToLower() == "true")
                {
                    int x = a.returnButton.Size.Width;
                    int borderRadius = (Int32)(x * 0.32);
                    butt.BorderRadius = borderRadius;
                }
                else
                {
                    butt.BorderRadius = 0;
                }
                butt.Size = a.returnButton.Size;
                sender = butt;
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = sender as ToolStripMenuItem;
            if (menuItem != null)
            {
                ContextMenuStrip owner = menuItem.Owner as ContextMenuStrip;
                if (owner != null)
                {
                    CustomButton button = owner.SourceControl as CustomButton;
                    ButtonSettingsMenu a = new ButtonSettingsMenu(button);
                    a.ShowDialog();
                    if(a.returnButton != null)
                    {
                        if (button.BorderSize != 0)
                            button.BorderColor = a.returnButton.FlatAppearance.BorderColor;
                        button.BackColor = a.returnButton.BackColor;
                        button.ForeColor = a.returnButton.ForeColor;
                        button.Size = a.returnButton.Size;

                        NodeOfTree x = null;
                        //x = openedNodes.Find(y => y.Text == n.Text);

                        TreeGraph currentGraph = null;
                        OpenedTreesDict.TryGetValue(tabControl1.SelectedTab, out currentGraph);
                        if (currentGraph != null)
                        {
                          x = currentGraph.listOfNodes.Find(y => y.buttonNode == button);
                            if (x != null)
                            {
                                DefaultTreesModel[tabControl1.SelectedTab].RemoveAll(elem => elem.Item1 == x);
                                DefaultTreesModel[tabControl1.SelectedTab].Add((x,button.Size));
                            }
                        }


                        if ((a.returnButton.Tag as string).ToLower() == "true")
                        {
                            int y = a.returnButton.Size.Width;
                            int borderRadius = (Int32)(y * 0.32);
                            button.BorderRadius = borderRadius;
                            button.Invalidate();
                        }
                        else
                        {
                            button.BorderRadius = 0;
                        }
                    }
                }
            }
            RefreshAfterPosChange();
        }

        private void addToTheTreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PictureBox pb = new PictureBox();
            foreach (Control c in tabControl1.SelectedTab.Controls)
            {
                if (c.Name.Contains("panel"))
                {
                    foreach(Control b in c.Controls)
                    {
                        if (b.Name.Contains("Box"))
                        {
                            pb = b as PictureBox;
                            break;
                        }
                    }
               }
            }
            Control[] arrayOfControls = tabControl1.SelectedTab.Controls.Find("pictureBox", true);
            if (arrayOfControls != null && arrayOfControls.Length != 0)
            {
                CustomButton btn = new CustomButton();
                btn.Name = treeView1.SelectedNode.Text;
                btn.Text = treeView1.SelectedNode.Text;
                btn.ContextMenuStrip = contextMenuStrip1;
                btn.Location = new Point(300, 50);
                btn.Size = new Size(200, 100);
                btn.MouseDown += Btn_MouseDown;
                btn.MouseUp += Btn_MouseUp;
                btn.Click += Btn_Click;

                string path = treeView1.SelectedNode.Tag as string;
                string name = treeView1.SelectedNode.Text;

                NodeOfTree node = new NodeOfTree(btn, name, path);

                TabPage ourPage = tabControl1.SelectedTab;
                TreeGraph thisGraph = null;
                if (Path.GetExtension(path) != ".txt")
                {
                    btn.BorderSize = 5;
                    btn.BorderColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                }
                


                if (OpenedTreesDict.TryGetValue(ourPage, out thisGraph))
                {
                    thisGraph.listOfNodes.Add(node);
                    DefaultTreesModel[ourPage].Add((node, btn.Size));
                    //thisGraph.listOfNodes.Find(x => x.Path == path);
                    NodeOfTree alreadyExistColorCheck = thisGraph.listOfNodes.Find(x => x.Path == path);
                    if (alreadyExistColorCheck != null)
                    {
                        while (true) 
                        { 
                            Color randomColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                            if (alreadyExistColorCheck.buttonNode.BackColor != randomColor && randomColor != btn.BorderColor)
                            {
                                btn.BackColor = randomColor;
                                break;
                            }
                        }
                    }
                    else
                        btn.BackColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256));
                }
                pb.Controls.Add(btn);
                //pb.Refresh();
            }
            else
            {
                MessageBox.Show("You must be at the Tree-type tab in order to add elements!");
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            string text = Path.GetExtension(e.Node.Text);
            if(e.Button == MouseButtons.Right)
            {
                if (!String.IsNullOrEmpty(text)) { 
                    string name = e.Node.Name;
                    if (e.Node.Tag != null && !e.Node.Tag.Equals(""))
                    {
                        string path = e.Node.Tag.ToString();
                        contextMenuStripRightClickTreeViewNode.Show(treeView1, new Point(e.Node.Bounds.X, e.Node.Bounds.Y));
                        treeView1.SelectedNode = e.Node;
                    }
                }
                else
                {
                    

                }
            }
        }

        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.Graphics.DrawString("x", e.Font, Brushes.Black, e.Bounds.Right - 15, e.Bounds.Top + 4);
            e.Graphics.DrawString(this.tabControl1.TabPages[e.Index].Text, e.Font, Brushes.Black, e.Bounds.Left + 12, e.Bounds.Top + 4);
            e.DrawFocusRectangle();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            targetOfSave = true;
            saveCurrentFileToolStripMenuItem_Click(null, null);
        }

        private void createLocalFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string Answer = Interaction.InputBox("Enter Folder Name", "Create Local Folder");
            string Path = null;
            if (Answer != null && Answer != "")
            {
                Path = Directory.GetCurrentDirectory() + "\\";
                Path += Answer;
                bool existance = Directory.Exists(Path);
                DirectoryInfo a = Directory.CreateDirectory(Path);
                if (!existance)
                {
                    loadFromDirectory(treeView1, Path, 1, 0);
                    addPathToDB(Path, true);
                }
                else
                {
                    MessageBox.Show("Directory with this name already exists!");
                }
            }
        }

        private void addFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "C:\\Users";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                string path = Path.GetDirectoryName(dialog.FileName);
                try
                {
                    int id = -1;
                    bool isOpen = false;
                    using (SqlCommand Sqlc = new SqlCommand($"SELECT * FROM [Folder]"))
                    {
                        Sqlc.Connection = sqlConnection;
                        if (sqlConnection.State == ConnectionState.Closed)
                            sqlConnection.Open();
                        SqlDataReader readerS = Sqlc.ExecuteReader();
                        while (readerS.Read())
                        {
                            if (readerS["Path"].ToString() == path)
                            {
                                isOpen = (bool)readerS["isOpen"];
                                id = (Int32)readerS["Id"];
                                break;
                            }
                        }
                        readerS.Close();
                        sqlConnection.Close();
                        if (id == -1)
                        {
                            using (SqlCommand com = new SqlCommand($"INSERT INTO [Folder] VALUES (@Path, @isOpen)"))
                            {
                                com.Connection = sqlConnection;
                                if (sqlConnection.State == ConnectionState.Closed)
                                    sqlConnection.Open();
                                com.Parameters.AddWithValue("@Path", path);
                                com.Parameters.AddWithValue("@isOpen", 1);
                                com.ExecuteNonQuery();
                                sqlConnection.Close();
                            }
                            try
                            {
                                loadFromDirectory(treeView1, path, 1, 0);
                            }
                            catch (Exception ex)
                            {
                                return;
                            }
                        }
                        else
                        {
                            if (isOpen == false)
                            {
                                using (SqlCommand sqlcom = new SqlCommand("UPDATE [Folder] SET [isOpen] = 1 WHERE [Id] = @Id"))
                                {
                                    sqlcom.Connection = sqlConnection;
                                    if (sqlConnection.State == ConnectionState.Closed)
                                        sqlConnection.Open();
                                    sqlcom.Parameters.AddWithValue("@Id", id);
                                    sqlcom.ExecuteNonQuery();
                                }
                                try
                                {
                                    loadFromDirectory(treeView1, path, 1, 0);
                                }
                                catch (Exception ex)
                                {
                                    return;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    sqlConnection.Close();
                }
            }
        }
        //###################################################################################################################################################
        /////////////////////////////////////////////   /// SCALING, SCROLLING , etc. ///////////////////////////////////////////////////////////////////////
        //###################################################################################################################################################
        private int counterPositive = 50;
        private int counterNegative = 0;
        public int defaultPictureBoxWidth = 3000;
        public int defaultPictureBoxHeigth = 3000;
        private Dictionary<TreeGraph, Tuple<int, int>> scaleStatus = new Dictionary<TreeGraph, Tuple<int, int>>();
        


        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            Thread.Sleep(50);
            Tuple<int, int> tempTuple = new Tuple<int,int>(0,0);
            //tabControl1.SelectedTab.Controls.Find("")
            TreeGraph tg = null;
            OpenedTreesDict.TryGetValue(tabControl1.SelectedTab, out tg);
            if(tg != null)
            {
                scaleStatus.TryGetValue(tg, out tempTuple);
                if(!(tempTuple.Item1 == 0 && tempTuple.Item2 == 0))
                {
                    counterPositive = tempTuple.Item1;
                    counterNegative = tempTuple.Item2;
                }
            }
          

            if(e.Delta > 0)
            {
                if (counterPositive < 50 && Control.ModifierKeys == Keys.Control)
                {
                    counterNegative--;
                    counterPositive++;

                    if (tabControl1.SelectedTab.Text.Contains("<"))
                    {
                        int pos = tabControl1.SelectedTab.Text.IndexOf(">");
                        tabControl1.SelectedTab.Text = "<" + counterPositive * 2 + "%>" + tabControl1.SelectedTab.Text.Substring(pos + 1);
                    }
                    else
                    {
                        tabControl1.SelectedTab.Text = "<" + counterPositive * 2 + "%>" + tabControl1.SelectedTab.Text;

                    }


                    foreach(Control c in (sender as PictureBox).Controls)
                    {
                        c.Scale(new SizeF(1.01f, 1.01f));
                        RefreshAfterPosChange();
                    }
                }
            }
            else
            {
                if(counterNegative < 50 && Control.ModifierKeys == Keys.Control)
                {
                    counterNegative++;
                    counterPositive--;

                    if (tabControl1.SelectedTab.Text.Contains("<"))
                    {
                        int pos = tabControl1.SelectedTab.Text.IndexOf(">");
                        tabControl1.SelectedTab.Text = "<" + counterPositive * 2 + "%>" + tabControl1.SelectedTab.Text.Substring(pos + 1);
                    }
                    else
                    {
                        tabControl1.SelectedTab.Text = "<" + counterPositive * 2 + "%>" + tabControl1.SelectedTab.Text;

                    }

                    foreach (Control c in (sender as PictureBox).Controls)
                    {
                        c.Scale(new SizeF(0.99f, 0.99f));
                        RefreshAfterPosChange();
                    }
                }

            }
            scaleStatus.Remove(tg);
            scaleStatus.Add(tg, new Tuple<int, int>(counterPositive, counterNegative));
            //scaleStatus[tg].Item1 = counterPositive;
            //scaleStatus[tg].Item2 = counterNegative;
        }






        //##################################TREEVIEW STATE SAVING##################################################################


        List<TreeNode> openedNodes = new List<TreeNode>();
        public void saveTheNodes()
        {
            openedNodes.Clear();
            foreach(TreeNode n in treeView1.Nodes)
            {
                //TreeNode x = null;
                //x = openedNodes.Find(y => y.Text == n.Text);
                if (/*x == null &&*/ n.IsExpanded)
                {
                    openedNodes.Add(n);
                }
            }
        }

        public void openNodes()
        {
            foreach(TreeNode tr in openedNodes)
            {
                foreach(TreeNode node in treeView1.Nodes)
                {
                    if(tr.Text == node.Text)
                    {
                        node.Expand();
                    }
                }
            }
            openedNodes.Clear();
        }


    }

    public class PanelWndProc: NativeWindow
    {
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x20a && Control.ModifierKeys == Keys.Control) return;
            base.WndProc(ref m);
        }
    }
    ///Richtextbox selected text
}

