using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;

namespace LTMCB_BTTH2
{
    public partial class Bai04 : Form
    {
        public class Student
        {
            public string Name { get; set; }
            public int ID { get; set; }
            public string Phone { get; set; }
            public float Course1 { get; set; }
            public float Course2 { get; set; }
            public float Course3 { get; set; }
            public float Average { get; set; }
        }

        private List<Student> studentsList = new List<Student>();
        private int currentIndex = 0;

        public Bai04()
        {
            InitializeComponent();
            BindEvents();
        }

        private void BindEvents()
        {
            button1.Click += new EventHandler(button1_Click); 
            button2.Click += new EventHandler(button2_Click); 
            button3.Click += new EventHandler(button3_Click); 
            button4.Click += new EventHandler(button4_Click); 
            button5.Click += new EventHandler(button5_Click); 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!Regex.IsMatch(textBox3.Text, @"^0\d{9}$"))
            {
                MessageBox.Show("Phone must be 10 digits and start with 0.");
                return;
            }

            if (!Regex.IsMatch(textBox2.Text, @"^\d{8}$"))
            {
                MessageBox.Show("ID must be exactly 8 digits.");
                return;
            }

            if (!float.TryParse(textBox4.Text, out float c1) || c1 < 0 || c1 > 10 ||
                !float.TryParse(textBox5.Text, out float c2) || c2 < 0 || c2 > 10 ||
                !float.TryParse(textBox12.Text, out float c3) || c3 < 0 || c3 > 10)
            {
                MessageBox.Show("Scores must be valid numbers between 0 and 10.");
                return;
            }

            Student newStudent = new Student
            {
                Name = textBox1.Text,
                ID = int.Parse(textBox2.Text),
                Phone = textBox3.Text,
                Course1 = c1,
                Course2 = c2,
                Course3 = c3,
                Average = 0 
            };

            studentsList.Add(newStudent);

            try
            {
                richTextBox1.AppendText($"{newStudent.Name}\n{newStudent.ID}\n{newStudent.Phone}\n{newStudent.Course1}\n{newStudent.Course2}\n{newStudent.Course3}\n0\n\n");
            }
            catch
            {

            }

            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
            textBox12.Clear();

            MessageBox.Show("Student added to memory.");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (studentsList.Count == 0)
            {
                MessageBox.Show("Please add at least one student before saving!");
                return;
            }

            string json = JsonSerializer.Serialize(studentsList, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("input4.txt", json);
            MessageBox.Show("Saved to input4.txt successfully!");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!File.Exists("input4.txt"))
            {
                File.WriteAllText("input4.txt", "[]");
                MessageBox.Show("input4.txt was not found. An empty input4.txt file has been created.");
                return;
            }

            string json = File.ReadAllText("input4.txt");

            if (string.IsNullOrWhiteSpace(json))
            {
                MessageBox.Show("input4.txt is empty! Add students and click Write first.");
                return;
            }

            try
            {
                studentsList = JsonSerializer.Deserialize<List<Student>>(json) ?? new List<Student>();
            }
            catch (JsonException)
            {
                MessageBox.Show("input4.txt contains invalid JSON format.");
                return;
            }

            if (studentsList.Count == 0)
            {
                MessageBox.Show("input4.txt has no student records! Add students and click Write first.");
                return;
            }

            foreach (var student in studentsList)
            {
                student.Average = (student.Course1 + student.Course2 + student.Course3) / 3.0f;
            }

            string outputJson = JsonSerializer.Serialize(studentsList, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("output4.txt", outputJson);

            currentIndex = 0;
            DisplayStudent();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (studentsList.Count > 0 && currentIndex > 0)
            {
                currentIndex--;
                DisplayStudent();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (studentsList.Count > 0 && currentIndex < studentsList.Count - 1)
            {
                currentIndex++;
                DisplayStudent();
            }
        }

        private void DisplayStudent()
        {
            if (studentsList.Count > 0)
            {
                var s = studentsList[currentIndex];
                textBox6.Text = s.Name;
                textBox7.Text = s.ID.ToString();
                textBox8.Text = s.Phone;
                textBox9.Text = s.Course1.ToString();
                textBox10.Text = s.Course2.ToString();
                textBox11.Text = s.Course3.ToString();
                textBox13.Text = s.Average.ToString("0.00");
                label14.Text = $"{currentIndex + 1} / {studentsList.Count}";
            }
        }

        private void label14_Click(object sender, EventArgs e)
        {

        }
    }
}