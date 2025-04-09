using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CursovaiUD
{
    public partial class ZakazMaterialUsluga : Form
    {
        int zakazId;
        public ZakazMaterialUsluga()
        {
            InitializeComponent();
        }

        public ZakazMaterialUsluga(int id)
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

        private void ZakazMaterialUsluga_Load(object sender, EventArgs e)
        {
            PrintUsluga();
            PrintMaterial();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            textBox2.Clear();
            textBox10.Clear();
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

        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                ButtonMaterial buttonMaterial = new ButtonMaterial();
                buttonMaterial.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                textBox5.Text = row.Cells[0].Value.ToString();
                textBox4.Text = row.Cells[1].Value.ToString();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            PrintMaterial();
        }
        private void PrintMaterial()
        {
            try
            {
                string query = @"
                SELECT 
                    y.Название as Материал,
                    cd.Количество
                FROM 
                    dbo.МатериалВЗаказе cd
                JOIN 
                    dbo.ПечатныеМатериалы y ON cd.КодМатериала = y.КодМатериала
                WHERE 
                    cd.КодЗаказа = @ZakazId";
                sqlConnection1.Open();
                SqlCommand command = new SqlCommand(query, sqlConnection1);
                command.Parameters.AddWithValue("@ZakazId", zakazId);

                SqlDataReader reader = command.ExecuteReader();
                dataGridView1.Rows.Clear();
                while (reader.Read())
                {
                    dataGridView1.Rows.Add(
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

        private void button3_Click(object sender, EventArgs e)
        {
            textBox5.Clear();
            textBox4.Clear();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox5.Text))
            {
                MessageBox.Show("Введите название материала.");
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox4.Text))
            {
                MessageBox.Show("Введите количество.");
                return;
            }

            if (!int.TryParse(textBox4.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Количество должно быть целым числом больше нуля.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string getMaterialQuery = @"
                SELECT КодМатериала 
                FROM dbo.ПечатныеМатериалы 
                WHERE Название = @MaterialName";

                string insertQuery = @"
                INSERT INTO dbo.МатериалВЗаказе (КодЗаказа, КодМатериала, Количество)
                VALUES (@ZakazId, @CodMateriala, @Colichestvo)";

                sqlConnection1.Open();

                SqlCommand getMaterialCommand = new SqlCommand(getMaterialQuery, sqlConnection1);
                getMaterialCommand.Parameters.AddWithValue("@MaterialName", textBox5.Text);

                object result = getMaterialCommand.ExecuteScalar();

                if (result == null)
                {
                    MessageBox.Show("Материал с таким названием не найден.");
                    return;
                }

                int codMateriala = Convert.ToInt32(result);

                SqlCommand insertCommand = new SqlCommand(insertQuery, sqlConnection1);
                insertCommand.Parameters.AddWithValue("@ZakazId", zakazId);
                insertCommand.Parameters.AddWithValue("@CodMateriala", codMateriala);
                insertCommand.Parameters.AddWithValue("@Colichestvo", int.Parse(textBox4.Text));

                insertCommand.ExecuteNonQuery();
                sqlConnection1.Close();
                PrintMaterial();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627) 
                {
                    MessageBox.Show("Такой материал уже добавлен в заказ.");
                }
                else if (ex.Message.Contains("Недостаточно материала на складе")) 
                {
                    MessageBox.Show("Недостаточно материала на складе.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox5.Text))
            {
                MessageBox.Show("Введите название материала.");
                return;
            }

            try
            {
                string getMaterialQuery = @"
                SELECT КодМатериала 
                FROM dbo.ПечатныеМатериалы 
                WHERE Название = @MaterialName";

                string deleteQuery = @"
                DELETE FROM dbo.МатериалВЗаказе
                WHERE КодЗаказа = @ZakazId AND КодМатериала = @CodMateriala";

                sqlConnection1.Open();

                SqlCommand getMaterialCommand = new SqlCommand(getMaterialQuery, sqlConnection1);
                getMaterialCommand.Parameters.AddWithValue("@MaterialName", textBox5.Text);

                object result = getMaterialCommand.ExecuteScalar();

                if (result == null)
                {
                    MessageBox.Show("Материал с таким названием не найден.");
                    return;
                }

                int codMateriala = Convert.ToInt32(result);

                SqlCommand deleteCommand = new SqlCommand(deleteQuery, sqlConnection1);
                deleteCommand.Parameters.AddWithValue("@ZakazId", zakazId);
                deleteCommand.Parameters.AddWithValue("@CodMateriala", codMateriala);

                int rowsAffected = deleteCommand.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    sqlConnection1.Close();
                    PrintMaterial();
                }
                else
                {
                    MessageBox.Show("Материал не найден в заказе.");
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
    }
}
