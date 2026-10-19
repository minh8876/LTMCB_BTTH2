using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace LTMCB_BTTH2
{
    public partial class Bai07 : Form
    {
        public Bai07()
        {
            InitializeComponent();
            BindEvents();
            PopulateDrives();
        }

        private void BindEvents()
        {
            treeView1.BeforeExpand += treeView1_BeforeExpand;
            treeView1.NodeMouseClick += treeView1_NodeMouseClick;
        }

        private void PopulateDrives()
        {
            treeView1.Nodes.Clear();
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                {
                    TreeNode rootNode = new TreeNode(drive.Name);
                    rootNode.Tag = drive.RootDirectory;
                    rootNode.Nodes.Add(new TreeNode());
                    treeView1.Nodes.Add(rootNode);
                }
            }
        }

        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode expandingNode = e.Node;
            expandingNode.Nodes.Clear();

            if (expandingNode.Tag is DirectoryInfo dir)
            {
                try
                {
                    foreach (DirectoryInfo subDir in dir.GetDirectories())
                    {
                        TreeNode node = new TreeNode(subDir.Name);
                        node.Tag = subDir;
                        node.Nodes.Add(new TreeNode());
                        expandingNode.Nodes.Add(node);
                    }

                    foreach (FileInfo file in dir.GetFiles())
                    {
                        TreeNode node = new TreeNode(file.Name);
                        node.Tag = file;
                        expandingNode.Nodes.Add(node);
                    }
                }
                catch (UnauthorizedAccessException)
                {
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            treeView1.SelectedNode = e.Node;

            if (e.Node.Tag is FileInfo file)
            {
                try
                {
                    string ext = file.Extension.ToLower();

                    if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".bmp" || ext == ".gif")
                    {
                        richTextBox1.Visible = false;
                        pictureBox1.Visible = true;
                        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                        pictureBox1.ImageLocation = file.FullName;
                    }
                    else if (ext == ".txt" || ext == ".log" || ext == ".json" || ext == ".cs" || ext == ".xml")
                    {
                        pictureBox1.Visible = false;
                        richTextBox1.Visible = true;
                        richTextBox1.Text = File.ReadAllText(file.FullName);
                    }
                    else
                    {
                        pictureBox1.Visible = false;
                        richTextBox1.Visible = true;
                        richTextBox1.Text = "Không thể xem trước định dạng file này.";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi đọc file: " + ex.Message);
                }
            }
        }
    }
}
