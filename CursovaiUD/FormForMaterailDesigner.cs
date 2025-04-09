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
    public partial class FormForMaterailDesigner : Form
    {
        int zakazId;
        public FormForMaterailDesigner()
        {
            InitializeComponent();
        }
        public FormForMaterailDesigner(int id)
        {
            InitializeComponent();
            zakazId = id;
        }
        private void FormForMaterailDesigner_Load(object sender, EventArgs e)
        {

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

        private void button3_Click(object sender, EventArgs e)
        {
            textBox5.Clear();
            textBox4.Clear();
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

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                textBox5.Text = row.Cells[0].Value.ToString();
                textBox4.Text = row.Cells[1].Value.ToString();
            }
        }
    }
}
