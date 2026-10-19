using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace LTMCB_BTTH2
{
    public partial class Bai01 : Form
    {
        public Bai01()
        {
            InitializeComponent();
            BindEvents();
        }

        private void BindEvents()
        {
            button1.Click += new EventHandler(button1_Click);
            button2.Click += new EventHandler(button2_Click);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string filePath = "input1.txt";

                if (!File.Exists(filePath))
                {
                    File.WriteAllText(filePath, "Nguyen Van A\nMSSV: 16520001\nClass: ANTT");
                }

                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                using (StreamReader sr = new StreamReader(fs))
                {
                    richTextBox1.Text = sr.ReadToEnd();
                }
                MessageBox.Show("Read input1.txt successfully!");
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("input1.txt not found. Make sure the file exists in your project's bin/Debug folder.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error reading file: " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                string upperContent = richTextBox1.Text.ToUpper();

                using (FileStream fs = new FileStream("output1.txt", FileMode.Create, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(upperContent);
                }
                MessageBox.Show("Content converted to UPPERCASE and written to output1.txt!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error writing file: " + ex.Message);
            }
        }
    }
}