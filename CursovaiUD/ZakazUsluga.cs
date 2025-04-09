using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CursovaiUD
{
    public partial class ZakazUsluga : Form
    {
        int zakazId;
        public ZakazUsluga()
        {
            InitializeComponent();
        }

        public ZakazUsluga(int id)
        {
            InitializeComponent();
            zakazId = id;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            PrintUsluga();
        }
        private void PrintUsluga()
        {
            try
            {
                string query = @"
                SELECT 
                    y.Наименование as Услуга,
                    cd.КоличествоУслуг
                FROM 
                    dbo.УслугаЗаказа cd
                JOIN 
                    dbo.Услуга y ON cd.КодУслуги = y.КодУслуги
                WHERE 
                    cd.КодЗаказа = @ZakazId";
                sqlConnection1.Open();
                SqlCommand command = new SqlCommand(query, sqlConnection1);
                command.Parameters.AddWithValue("@ZakazId", zakazId);

                SqlDataReader reader = command.ExecuteReader();
                dataGridView2.Rows.Clear();
                while (reader.Read())
                {
                    dataGridView2.Rows.Add(
                       reader.GetValue(0),
                       reader.GetValue(1)
                   );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки: " + ex.Message);
            }
            finally
            {
                sqlConnection1.Close();
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            textBox2.Clear();
            textBox10.Clear();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                ButtonUslugi buttonUslugi = new ButtonUslugi();
                buttonUslugi.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView2.Rows[e.RowIndex];
                textBox2.Text = row.Cells[0].Value.ToString();
                textBox10.Text = row.Cells[1].Value.ToString();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Введите название услуги.");
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox10.Text))
            {
                MessageBox.Show("Введите количество.");
                return;
            }

            if (!int.TryParse(textBox10.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Количество должно быть целым числом больше нуля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string getUslugaQuery = @"
                SELECT КодУслуги 
                FROM dbo.Услуга 
                WHERE Наименование = @UslugaName";

                string insertQuery = @"
                INSERT INTO dbo.УслугаЗаказа (КодЗаказа, КодУслуги, КоличествоУслуг)
                VALUES (@ZakazId, @СodUslugi, @Сolichestvo)";

                sqlConnection1.Open();
                SqlCommand getUslugaCommand = new SqlCommand(getUslugaQuery, sqlConnection1);
                getUslugaCommand.Parameters.AddWithValue("@UslugaName", textBox2.Text);

                object result = getUslugaCommand.ExecuteScalar();

                if (result == null)
                {
                    MessageBox.Show("Услуга с таким названием не найдена.");
                    return;
                }

                int сodUslugi = Convert.ToInt32(result);

                SqlCommand insertCommand = new SqlCommand(insertQuery, sqlConnection1);
                insertCommand.Parameters.AddWithValue("@ZakazId", zakazId);
                insertCommand.Parameters.AddWithValue("@СodUslugi", сodUslugi);
                insertCommand.Parameters.AddWithValue("@Сolichestvo", int.Parse(textBox10.Text));

                insertCommand.ExecuteNonQuery();
                sqlConnection1.Close();
                PrintUsluga();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627)
                {
                    MessageBox.Show("Такая услуга уже добавлена в заказ.");
                }
                else
                {
                    MessageBox.Show("Ошибка добавления: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
            finally
            {
                sqlConnection1.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Введите название услуги для удаления.");
                return;
            }

            try
            {
                string getUslugaQuery = @"
                SELECT КодУслуги 
                FROM dbo.Услуга 
                WHERE Наименование = @UslugaName";

                string deleteQuery = @"
                DELETE FROM dbo.УслугаЗаказа 
                WHERE КодЗаказа = @ZakazId AND КодУслуги = @СodUslugi";

                sqlConnection1.Open();

                SqlCommand getUslugaCommand = new SqlCommand(getUslugaQuery, sqlConnection1);
                getUslugaCommand.Parameters.AddWithValue("@UslugaName", textBox2.Text);

                object result = getUslugaCommand.ExecuteScalar();

                if (result == null)
                {
                    MessageBox.Show("Услуга с таким названием не найдена.");
                    return;
                }

                int сodUslugi = Convert.ToInt32(result);

                SqlCommand deleteCommand = new SqlCommand(deleteQuery, sqlConnection1);
                deleteCommand.Parameters.AddWithValue("@ZakazId", zakazId);
                deleteCommand.Parameters.AddWithValue("@СodUslugi", сodUslugi);

                int rowsAffected = deleteCommand.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    sqlConnection1.Close();
                    PrintUsluga();
                }
                else
                {
                    MessageBox.Show("Услуга не найдена в заказе.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка удаления: " + ex.Message);
            }
            finally
            {
                sqlConnection1.Close();
            }
        }
    }
}
