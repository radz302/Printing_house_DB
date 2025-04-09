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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CursovaiUD
{
    public partial class FormForDesigner : Form
    {
        int EmployeeId;
        public FormForDesigner()
        {
            InitializeComponent();
        }

        public FormForDesigner(int EmployeeI)
        {
            EmployeeId = EmployeeI;
            InitializeComponent();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox3.Text) ||
                string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox10.Text) ||
                string.IsNullOrWhiteSpace(textBox4.Text) ||
                string.IsNullOrWhiteSpace(comboBox1.Text))
            {
                MessageBox.Show("Выберите заказ из списка!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string[] validStatuses = { "Выполнен", "Принят в работу", "Ожидание подтверждения", "Отменен" };
            if (!validStatuses.Contains(comboBox1.Text))
            {
                MessageBox.Show("Некорректный статус выполнения.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            sqlConnection1.Open();
            string statusQuery = "SELECT СтатусВыполнения FROM ЗаказНаПечать WHERE КодЗаказа = @OrderId";
            SqlCommand statusCommand = new SqlCommand(statusQuery, sqlConnection1);
            statusCommand.Parameters.AddWithValue("@OrderId", Convert.ToInt32(textBox3.Text));
            object currentStatusObj = statusCommand.ExecuteScalar();
            sqlConnection1.Close();

            string currentStatus = currentStatusObj.ToString();
            if ((currentStatusObj != null) && (currentStatus != comboBox1.Text))
            {

                if (currentStatus == "Выполнен" || currentStatus == "Отменен")
                {
                    MessageBox.Show("Нельзя изменить заказ, если он уже выполнен или отменен.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (currentStatus == "Ожидание подтверждения" && comboBox1.Text != "Принят в работу" && comboBox1.Text != "Отменен")
                {
                    MessageBox.Show("Статус 'Ожидание подтверждения' можно изменить только на 'Принят в работу' или 'Отменен'.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (currentStatus == "Принят в работу" && comboBox1.Text != "Выполнен" && comboBox1.Text != "Отменен")
                {
                    MessageBox.Show("Статус 'Принят в работу' можно изменить только на 'Выполнен' или 'Отменен'.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            try
            {
                sqlConnection1.Open();
                string updateQuery = @"
            UPDATE ЗаказНаПечать
            SET 
                СтатусВыполнения = @Status
            WHERE КодЗаказа = @OrderId";

                SqlCommand updateCommand = new SqlCommand(updateQuery, sqlConnection1);
                updateCommand.Parameters.AddWithValue("@OrderId", Convert.ToInt32(textBox3.Text));
                updateCommand.Parameters.AddWithValue("@Status", comboBox1.Text);

                int rowsAffected = updateCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Статус успешно обновлен.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    sqlConnection1.Close();
                    LoadOrders();
                }
                else
                {
                    MessageBox.Show("Заказ с указанным кодом не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обновлении заказа: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                sqlConnection1.Close();
            }
        }
        private void LoadOrders()
        {
            try
            {
                sqlConnection1.Open();
                string query = @"
                SELECT 
                    КодЗаказа, 
                    ДатаЗаказа, 
                    к.ФИО AS Клиент,
                    Стоимость,
                    СтатусВыполнения
                FROM ЗаказНаПечать з
                JOIN Сотрудник с ON с.КодСотрудника = з.КодСотрудника
                JOIN Клиент к ON к.КодКлиента = з.КодКлиента
                WHERE з.КодСотрудника = @EmployeeId";

                SqlCommand command = new SqlCommand(query, sqlConnection1);
                command.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                SqlDataReader reader = command.ExecuteReader();
                dataGridView2.Rows.Clear();
                while (reader.Read())
                {
                    dataGridView2.Rows.Add(
                        reader["КодЗаказа"].ToString(),
                        Convert.ToDateTime(reader["ДатаЗаказа"]).ToShortDateString(),
                        reader["Клиент"].ToString(),
                        reader["Стоимость"].ToString(),
                        reader["СтатусВыполнения"].ToString()
                    );
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки заказов: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                sqlConnection1.Close();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            LoadOrders();
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView2.Rows[e.RowIndex];

                textBox3.Text = row.Cells[0].Value.ToString();
                textBox2.Text = row.Cells[1].Value.ToString();
                textBox10.Text = row.Cells[2].Value.ToString();
                textBox4.Text = row.Cells[3].Value.ToString();
                comboBox1.Text = row.Cells[4].Value.ToString();
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            textBox3.Clear();
            textBox2.Clear();
            textBox10.Clear();
            textBox4.Clear();
            comboBox1.Text = "";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (comboBox3.SelectedItem == null || string.IsNullOrWhiteSpace(textBox5.Text))
            {
                MessageBox.Show("Выберите поле для поиска и введите значение.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string searchField = comboBox3.SelectedItem.ToString();
            string searchValue = textBox5.Text;
            int clientID = -1;

            if (searchField == "Код заказа")
            {
                searchField = "КодЗаказа";
            }
            else if (searchField == "Клиент")
            {
                searchField = "з.КодКлиента";

                sqlConnection1.Open();
                string getEmployeeIdQuery = "SELECT КодКлиента FROM Клиент WHERE ФИО LIKE @FIO";
                SqlCommand getEmployeeIdCommand = new SqlCommand(getEmployeeIdQuery, sqlConnection1);
                getEmployeeIdCommand.Parameters.AddWithValue("@FIO", textBox5.Text.Trim());

                var result = getEmployeeIdCommand.ExecuteScalar();
                if (result == null)
                {
                    MessageBox.Show("Клиент не найден!", "Результаты поиска", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                sqlConnection1.Close();
                clientID = (int)result;

            }
            else if (searchField == "Стоимость")
            {
                searchField = "Стоимость";
            }
            else if (searchField == "Статус выполнения")
            {
                searchField = "СтатусВыполнения";
            }
            else
            {
                MessageBox.Show("Выбранное поле поиска недопустимо.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                sqlConnection1.Open();

                string query = $@"
                SELECT 
                    з.КодЗаказа, 
                    з.ДатаЗаказа, 
                    к.ФИО AS Клиент,
                    з.Стоимость,
                    з.СтатусВыполнения
                FROM ЗаказНаПечать з
                JOIN Сотрудник с ON с.КодСотрудника = з.КодСотрудника
                JOIN Клиент к ON к.КодКлиента = з.КодКлиента
                WHERE {searchField} LIKE @SearchValue AND з.КодСотрудника = @EmployeeId";

                SqlCommand command = new SqlCommand(query, sqlConnection1);

                if (searchField == "КодЗаказа" || searchField == "Стоимость" || searchField == "з.КодКлиента")
                {
                    if (int.TryParse(searchValue, out int numericValue))
                    {
                        command.Parameters.AddWithValue("@SearchValue", numericValue);
                    }
                    else if (searchField == "з.КодКлиента")
                    {
                        command.Parameters.AddWithValue("@SearchValue", clientID);
                    }
                    else
                    {
                        MessageBox.Show("Для выбранного поля поиска необходимо ввести числовое значение.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                else
                {
                    command.Parameters.AddWithValue("@SearchValue", $"%{searchValue}%");
                }
                command.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable resultsTable = new DataTable();

                adapter.Fill(resultsTable);

                if (resultsTable.Rows.Count > 0)
                {
                    SqlDataReader reader = command.ExecuteReader();
                    dataGridView2.Rows.Clear();
                    while (reader.Read())
                    {
                        dataGridView2.Rows.Add(
                            reader["КодЗаказа"].ToString(),
                            Convert.ToDateTime(reader["ДатаЗаказа"]).ToShortDateString(),
                            reader["Клиент"].ToString(),
                            reader["Стоимость"].ToString(),
                            reader["СтатусВыполнения"].ToString()
                        );
                    }

                    textBox5.Clear();
                    comboBox3.Text = "";
                }
                else
                {
                    MessageBox.Show("Записи не найдены.", "Результат поиска", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при выполнении поиска: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                sqlConnection1.Close();
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("Выберите заказ из списка!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int id = Convert.ToInt32(textBox3.Text);

            try
            {
                FormForMaterailDesigner forMaterailDesigner = new FormForMaterailDesigner(id);
                forMaterailDesigner.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("Заполните поле: Код заказа.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int id = Convert.ToInt32(textBox3.Text);

            try
            {
                SpisokUslugDesig usluga = new SpisokUslugDesig(id);
                usluga.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
