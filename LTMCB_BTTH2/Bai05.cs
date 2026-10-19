using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LTMCB_BTTH2
{
    public partial class Bai05 : Form
    {
        public class Movie
        {
            public string Name { get; set; }
            public decimal BasePrice { get; set; }
            public List<int> Rooms { get; set; } = new List<int>();
        }

        private List<Movie> movieList = new List<Movie>();
        private Dictionary<string, HashSet<string>> soldSeatsMap = new Dictionary<string, HashSet<string>>();

        private readonly string[] allSeats = new string[]
        {
            "A1", "A2", "A3", "A4", "A5",
            "B1", "B2", "B3", "B4", "B5",
            "C1", "C2", "C3", "C4", "C5"
        };

        public Bai05()
        {
            InitializeComponent();
            BindEvents();
            InitSeatList();
        }

        private void BindEvents()
        {
            button1.Click += button1_Click;
            button2.Click += button2_Click;
            button3.Click += button3_Click;
            button4.Click += button4_Click;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            comboBox2.SelectedIndexChanged += comboBox2_SelectedIndexChanged;
        }

        private void InitSeatList()
        {
            checkedListBox1.Items.Clear();
            foreach (string seat in allSeats)
            {
                checkedListBox1.Items.Add(seat);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                !decimal.TryParse(textBox2.Text, out decimal price) ||
                string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("Please enter valid movie details (Name, Price, and Rooms like '1, 2, 3').");
                return;
            }

            using (StreamWriter sw = new StreamWriter("input5.txt", append: true))
            {
                sw.WriteLine(textBox1.Text.Trim());
                sw.WriteLine(price.ToString());
                sw.WriteLine(textBox3.Text.Trim());
            }

            MessageBox.Show($"Movie '{textBox1.Text}' saved to input5.txt!");
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!File.Exists("input5.txt"))
            {
                File.WriteAllText("input5.txt", "");
                MessageBox.Show("input5.txt not found. An empty file was created.");
                return;
            }

            string[] lines = File.ReadAllLines("input5.txt");
            movieList.Clear();

            for (int i = 0; i < lines.Length; i += 3)
            {
                if (i + 2 < lines.Length)
                {
                    string name = lines[i].Trim();
                    if (decimal.TryParse(lines[i + 1], out decimal price))
                    {
                        var roomList = lines[i + 2]
                            .Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(r => int.Parse(r))
                            .ToList();

                        movieList.Add(new Movie { Name = name, BasePrice = price, Rooms = roomList });
                    }
                }
            }

            comboBox1.DataSource = null;
            comboBox1.DataSource = movieList.Select(m => m.Name).ToList();

            if (movieList.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
                MessageBox.Show($"Loaded {movieList.Count} movies from input5.txt!");
            }
            else
            {
                MessageBox.Show("input5.txt is empty. Add some movies first!");
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex < 0 || comboBox1.SelectedIndex >= movieList.Count) return;

            Movie selectedMovie = movieList[comboBox1.SelectedIndex];
            comboBox2.DataSource = null;
            comboBox2.DataSource = selectedMovie.Rooms;
            UpdateSeatStatuses();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateSeatStatuses();
        }

        private void UpdateSeatStatuses()
        {
            if (comboBox1.SelectedItem == null || comboBox2.SelectedItem == null) return;

            string key = $"{comboBox1.SelectedItem}_{comboBox2.SelectedItem}";
            soldSeatsMap.TryGetValue(key, out HashSet<string> soldSeats);

            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemChecked(i, false);
            }
        }

        private decimal CalculateSeatPrice(string seat, decimal basePrice)
        {
            if (seat == "A1" || seat == "A5" || seat == "C1" || seat == "C5")
                return basePrice * 0.25m;

            if (seat == "B2" || seat == "B3" || seat == "B4")
                return basePrice * 2.0m;

            return basePrice * 1.0m;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox4.Text))
            {
                MessageBox.Show("Please enter the customer's name!");
                return;
            }
            if (comboBox1.SelectedItem == null || comboBox2.SelectedItem == null)
            {
                MessageBox.Show("Please select a movie and a room!");
                return;
            }
            if (checkedListBox1.CheckedItems.Count == 0)
            {
                MessageBox.Show("Please select at least one seat!");
                return;
            }

            string movieName = comboBox1.SelectedItem.ToString();
            int room = (int)comboBox2.SelectedItem;
            string key = $"{movieName}_{room}";

            if (!soldSeatsMap.ContainsKey(key))
            {
                soldSeatsMap[key] = new HashSet<string>();
            }

            foreach (var item in checkedListBox1.CheckedItems)
            {
                string seat = item.ToString();
                if (soldSeatsMap[key].Contains(seat))
                {
                    MessageBox.Show($"Seat {seat} in Room {room} has already been sold!");
                    return;
                }
            }

            Movie currentMovie = movieList.First(m => m.Name == movieName);
            decimal totalBill = 0;
            List<string> bookedSeatsList = new List<string>();

            foreach (var item in checkedListBox1.CheckedItems)
            {
                string seat = item.ToString();
                soldSeatsMap[key].Add(seat);
                bookedSeatsList.Add(seat);
                totalBill += CalculateSeatPrice(seat, currentMovie.BasePrice);
            }

            string receipt = $"--- BILL ---\n" +
                             $"Customer: {textBox4.Text}\n" +
                             $"Movie: {movieName}\n" +
                             $"Room: {room}\n" +
                             $"Seats: {string.Join(", ", bookedSeatsList)}\n" +
                             $"Total: {totalBill:N0} VND\n";

            richTextBox1.Text = receipt;
            MessageBox.Show("Booking successful!");
            UpdateSeatStatuses();
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            if (!File.Exists("input5.txt"))
            {
                File.WriteAllText("input5.txt", "");
                MessageBox.Show("input5.txt not found. An empty file was created.");
                return;
            }

            if (movieList.Count == 0)
            {
                MessageBox.Show("No movie data loaded to export!");
                return;
            }

            var statsList = movieList.Select(m =>
            {
                int totalCapacity = m.Rooms.Count * 15;
                int totalSold = 0;
                decimal totalRevenue = 0;

                foreach (int r in m.Rooms)
                {
                    string key = $"{m.Name}_{r}";
                    if (soldSeatsMap.TryGetValue(key, out HashSet<string> soldSeats))
                    {
                        totalSold += soldSeats.Count;
                        foreach (string seat in soldSeats)
                        {
                            totalRevenue += CalculateSeatPrice(seat, m.BasePrice);
                        }
                    }
                }

                int remaining = totalCapacity - totalSold;
                double sellRate = totalCapacity > 0 ? ((double)totalSold / totalCapacity) * 100 : 0;

                return new
                {
                    MovieName = m.Name,
                    Sold = totalSold,
                    Remaining = remaining,
                    SellRate = sellRate,
                    Revenue = totalRevenue
                };
            })
            .OrderByDescending(s => s.Revenue)
            .ToList();

            progressBar1.Minimum = 0;
            progressBar1.Maximum = statsList.Count;
            progressBar1.Value = 0;

            richTextBox1.Clear();

            using (StreamWriter sw = new StreamWriter("output5.txt"))
            {
                for (int i = 0; i < statsList.Count; i++)
                {
                    var stat = statsList[i];
                    string entry = $"Rank #{i + 1}\n" +
                                   $"Movie: {stat.MovieName}\n" +
                                   $"Tickets Sold: {stat.Sold}\n" +
                                   $"Tickets Remaining: {stat.Remaining}\n" +
                                   $"Sales Rate: {stat.SellRate:0.00}%\n" +
                                   $"Revenue: {stat.Revenue:N0} VND\n" +
                                   new string('-', 25);

                    sw.WriteLine(entry);
                    richTextBox1.AppendText(entry + "\n");

                    progressBar1.Value = i + 1;
                    await Task.Delay(200);
                }
            }

            MessageBox.Show("Statistics exported to output5.txt successfully!");
            progressBar1.Value = 0;
        }
    }
}
