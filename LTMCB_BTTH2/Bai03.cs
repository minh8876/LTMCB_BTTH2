using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace LTMCB_BTTH2
{
    public partial class Bai03 : Form
    {
        public Bai03()
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
                string filePath = "input3.txt";

                if (!File.Exists(filePath))
                {
                    string sampleExpressions = "1+2+3+4\r\n12-7-5+2-3\r\n2024-1-2+3\r\n222+333-444*2+1";
                    File.WriteAllText(filePath, sampleExpressions);
                }

                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                using (StreamReader sr = new StreamReader(fs))
                {
                    richTextBox1.Text = sr.ReadToEnd();
                }

                MessageBox.Show("Read input3.txt successfully!");
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
                string[] lines = richTextBox1.Lines;
                List<string> evaluatedResults = new List<string>();

                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string expression = line.Trim();
                    double result = EvaluateExpression(expression);

                    evaluatedResults.Add($"{expression} = {result}");
                }

                using (FileStream fs = new FileStream("output3.txt", FileMode.Create, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    foreach (string resultLine in evaluatedResults)
                    {
                        sw.WriteLine(resultLine);
                    }
                }

                MessageBox.Show("Successfully calculated results and saved to output3.txt!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error processing calculations: " + ex.Message);
            }
        }

        private double EvaluateExpression(string expression)
        {
            char[] tokens = expression.ToCharArray();
            Stack<double> values = new Stack<double>();
            Stack<char> ops = new Stack<char>();

            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i] == ' ') continue;

                if ((tokens[i] >= '0' && tokens[i] <= '9') || tokens[i] == '.')
                {
                    string sb = "";
                    while (i < tokens.Length && ((tokens[i] >= '0' && tokens[i] <= '9') || tokens[i] == '.'))
                    {
                        sb += tokens[i++];
                    }
                    i--;
                    values.Push(double.Parse(sb, CultureInfo.InvariantCulture));
                }
                else if (tokens[i] == '(')
                {
                    ops.Push(tokens[i]);
                }
                else if (tokens[i] == ')')
                {
                    while (ops.Peek() != '(')
                    {
                        values.Push(ApplyOp(ops.Pop(), values.Pop(), values.Pop()));
                    }
                    ops.Pop();
                }
                else if (tokens[i] == '+' || tokens[i] == '-' || tokens[i] == '*' || tokens[i] == '/')
                {
                    while (ops.Count > 0 && HasPrecedence(tokens[i], ops.Peek()))
                    {
                        values.Push(ApplyOp(ops.Pop(), values.Pop(), values.Pop()));
                    }
                    ops.Push(tokens[i]);
                }
            }

            while (ops.Count > 0)
            {
                values.Push(ApplyOp(ops.Pop(), values.Pop(), values.Pop()));
            }

            return values.Pop();
        }

        private bool HasPrecedence(char op1, char op2)
        {
            if (op2 == '(' || op2 == ')') return false;
            if ((op1 == '*' || op1 == '/') && (op2 == '+' || op2 == '-')) return false;
            return true;
        }

        private double ApplyOp(char op, double b, double a)
        {
            switch (op)
            {
                case '+': return a + b;
                case '-': return a - b;
                case '*': return a * b;
                case '/':
                    if (b == 0) throw new DivideByZeroException("Division by zero");
                    return a / b;
            }
            return 0;
        }
    }
}
