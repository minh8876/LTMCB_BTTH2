using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LTMCB_BTTH2
{
    public partial class Bai02 : Form
    {
        public Bai02()
        {
            InitializeComponent();
            BindEvents();
        }

        private void BindEvents()
        {
            button1.Click += new EventHandler(button1_Click);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                FileInfo fileInfo = new FileInfo(ofd.FileName);

                string content = File.ReadAllText(ofd.FileName);

                int charCount = content.Length;
                int wordCount = content.Split(new char[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
                int lineCount = File.ReadLines(ofd.FileName).Count();

                textBox1.Text = content;
                textBox2.Text = fileInfo.Name;
                textBox3.Text = fileInfo.Length.ToString() + " bytes";
                textBox4.Text = fileInfo.FullName;
                textBox5.Text = lineCount.ToString();
                textBox6.Text = wordCount.ToString();
                textBox7.Text = charCount.ToString();
            }
        }
    }
}
