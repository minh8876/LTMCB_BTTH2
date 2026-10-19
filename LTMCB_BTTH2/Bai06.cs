using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LTMCB_BTTH2
{
    public partial class Bai06 : Form
    {
        private string connectionString = "Data Source=food.db;Version=3;";

        public Bai06()
        {
            InitializeComponent();
            SetupListViewHeaders();
            BindEvents();
            LoadDataToListView();
        }

        private void BindEvents()
        {
            button1.Click += button1_Click;
            button2.Click += button2_Click;
            button3.Click += button3_Click;
        }

        private void SetupListViewHeaders()
        {
            listView1.View = View.Details;
            listView1.FullRowSelect = true;
            listView1.Columns.Clear();
            listView1.Columns.Add("ID", 50);
            listView1.Columns.Add("Tên Món Ăn", 150);
            listView1.Columns.Add("Hình Ảnh", 150);
            listView1.Columns.Add("Người Đóng Góp", 150);
        }

        private void LoadDataToListView()
        {
            listView1.Items.Clear();

            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"
                        SELECT m.IDMA, m.TenMonAn, m.HinhAnh, n.HoVaTen 
                        FROM MonAn m 
                        LEFT JOIN NguoiDung n ON m.IDNCC = n.IDNCC";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ListViewItem item = new ListViewItem(reader["IDMA"].ToString());
                            item.SubItems.Add(reader["TenMonAn"].ToString());
                            item.SubItems.Add(reader["HinhAnh"].ToString());
                            item.SubItems.Add(reader["HoVaTen"] != DBNull.Value ? reader["HoVaTen"].ToString() : "N/A");
                            listView1.Items.Add(item);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi kết nối CSDL: " + ex.Message);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Vui lòng nhập tên món ăn!");
                return;
            }

            object idnccValue = DBNull.Value;
            if (!string.IsNullOrWhiteSpace(textBox3.Text))
            {
                if (int.TryParse(textBox3.Text.Trim(), out int parsedID))
                {
                    idnccValue = parsedID;
                }
                else
                {
                    MessageBox.Show("ID Nhà cung cấp (textBox3) phải là số nguyên!");
                    return;
                }
            }

            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "INSERT INTO MonAn (TenMonAn, HinhAnh, IDNCC) VALUES (@TenMonAn, @HinhAnh, @IDNCC)";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TenMonAn", textBox1.Text.Trim());
                        cmd.Parameters.AddWithValue("@HinhAnh", textBox2.Text.Trim());
                        cmd.Parameters.AddWithValue("@IDNCC", idnccValue);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Thêm món ăn thành công!");
                    LoadDataToListView();
                    button3_Click(sender, e);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi thêm món ăn: " + ex.Message);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"
                SELECT m.TenMonAn, m.HinhAnh, n.HoVaTen 
                FROM MonAn m 
                LEFT JOIN NguoiDung n ON m.IDNCC = n.IDNCC 
                ORDER BY RANDOM() LIMIT 1";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string tenMon = reader["TenMonAn"].ToString();

                            string hinhAnh = reader["HinhAnh"].ToString().Trim('"', ' ');
                            string nguoiDongGop = reader["HoVaTen"] != DBNull.Value ? reader["HoVaTen"].ToString() : "Chưa rõ";

                            textBox4.Text = tenMon;
                            textBox5.Text = nguoiDongGop;

                            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

                            if (File.Exists(hinhAnh) || hinhAnh.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || hinhAnh.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                            {
                                pictureBox1.ImageLocation = hinhAnh;
                            }
                            else
                            {
                                pictureBox1.Image = null;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Cơ sở dữ liệu chưa có món ăn nào!");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi trích xuất dữ liệu: " + ex.Message);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // CASE 1: An item IS selected in ListView -> Perform DELETE from SQLite
            if (listView1.SelectedItems.Count > 0)
            {
                string selectedID = listView1.SelectedItems[0].SubItems[0].Text;
                string selectedName = listView1.SelectedItems[0].SubItems[1].Text;

                DialogResult dialogResult = MessageBox.Show(
                    $"Bạn có chắc chắn muốn XÓA món: \"{selectedName}\" (ID: {selectedID}) khỏi CSDL không?",
                    "Xác nhận xóa món ăn",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (dialogResult == DialogResult.Yes)
                {
                    using (SQLiteConnection conn = new SQLiteConnection(connectionString))
                    {
                        try
                        {
                            conn.Open();
                            string query = "DELETE FROM MonAn WHERE IDMA = @IDMA";

                            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@IDMA", selectedID);
                                cmd.ExecuteNonQuery();
                            }

                            MessageBox.Show("Đã xóa món ăn khỏi CSDL thành công!");
                            LoadDataToListView(); 

                            textBox1.Clear();
                            textBox2.Clear();
                            textBox3.Clear();
                            textBox4.Clear();
                            textBox5.Clear();
                            pictureBox1.Image = null;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Lỗi khi xóa món ăn: " + ex.Message);
                        }
                    }
                }
            }
        }
    }
}
