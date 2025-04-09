using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CursovaiUD
{
    public partial class FormForAdministrator : Form
    {
        public FormForAdministrator()
        {
            InitializeComponent();
        }

        private void FormForAdministrator_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void checkedListBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button_Print_Click(object sender, EventArgs e)
        {
            PrintClient();
        }

        private void PrintClient()
        {
            string query = "SELECT КодКлиента, ФИО, НомерТелефона, ДатаРождения FROM Клиент";

            try
            {
                sqlConnection1.Open();
                SqlCommand command = new SqlCommand(query, sqlConnection1);
                SqlDataReader reader = command.ExecuteReader();
                dataGridView1.Rows.Clear();
                while (reader.Read())
                {
                    dataGridView1.Rows.Add(
                        reader["КодКлиента"].ToString(),
                        reader["ФИО"].ToString(),
                        reader["НомерТелефона"].ToString(),
                        Convert.ToDateTime(reader["ДатаРождения"]).ToShortDateString()
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении данных: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                sqlConnection1.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox6.Clear();
            textBox7.Clear();
            textBox8.Clear();
            textBox9.Clear();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                textBox6.Text = row.Cells[0].Value.ToString();
                textBox7.Text = row.Cells[1].Value.ToString();
                textBox8.Text = row.Cells[2].Value.ToString();
                textBox9.Text = row.Cells[3].Value.ToString();
            }
        }

        private void button_Add_Click(object sender, EventArgs e)
        {
            string date_r = textBox9.Text;
            string phoneNumber = textBox8.Text;

            if (string.IsNullOrEmpty(textBox7.Text) || string.IsNullOrEmpty(textBox8.Text) || string.IsNullOrEmpty(textBox9.Text))
            {

                MessageBox.Show("Заполните поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }

            if (!Regex.IsMatch(date_r, @"^\d{2}\.\d{2}\.\d{4}$"))
            {
                MessageBox.Show("Дата рождения должна быть в формате ДД.ММ.ГГГГ", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!string.IsNullOrEmpty(textBox6.Text))
            {

                MessageBox.Show("Код клиента уже существует! Очистите поле!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;

            }

            if (phoneNumber.Length != 11 || !Regex.IsMatch(phoneNumber, @"^\d{11}$"))
            {
                MessageBox.Show("Номер телефона должен состоять из 11 цифр!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                sqlConnection1.Open();
                string getMaxID = "SELECT ISNULL(MAX(КодКлиента), 0) + 1 FROM Клиент";
                SqlCommand getMaxIDC = new SqlCommand(getMaxID, sqlConnection1);
                int newID = (int)getMaxIDC.ExecuteScalar();

                string insertQuery = @"
                INSERT INTO Клиент (КодКлиента, ФИО, НомерТелефона, ДатаРождения)
                VALUES (@КодКлиента, @ФИО, @НомерТелефона, @ДатаРождения)";

                SqlCommand insertCommand = new SqlCommand(insertQuery, sqlConnection1);
                insertCommand.Parameters.AddWithValue("@КодКлиента", newID);
                insertCommand.Parameters.AddWithValue("@ФИО", textBox7.Text);
                insertCommand.Parameters.AddWithValue("@НомерТелефона", textBox8.Text);
                insertCommand.Parameters.AddWithValue("@ДатаРождения", DateTime.ParseExact(date_r, "dd.MM.yyyy", null));

                insertCommand.ExecuteNonQuery();

                dataGridView1.Rows.Add(newID, textBox7.Text, phoneNumber, date_r);

                MessageBox.Show("Клиент успешно добавлен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox7.Clear();
                textBox8.Clear();
                textBox9.Clear();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547)
                {
                    MessageBox.Show("Клиенту должно быть больше 18 лет.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Ошибка при добавлении клиента: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении клиента: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                sqlConnection1.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox6.Text) ||
                string.IsNullOrWhiteSpace(textBox7.Text) ||
                string.IsNullOrWhiteSpace(textBox8.Text) ||
                string.IsNullOrWhiteSpace(textBox9.Text))
            {
                MessageBox.Show("Все поля должны быть заполнены!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string date_r = textBox9.Text;
            string phoneNumber = textBox8.Text;
            if (!Regex.IsMatch(date_r, @"^\d{2}\.\d{2}\.\d{4}$"))
            {
                MessageBox.Show("Дата рождения должна быть в формате ДД.ММ.ГГГГ", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (phoneNumber.Length != 11 || !Regex.IsMatch(phoneNumber, @"^\d{11}$"))
            {
                MessageBox.Show("Номер телефона должен состоять из 11 цифр!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                sqlConnection1.Open();

                string checkIDq = "SELECT COUNT(1) FROM Клиент WHERE КодКлиента = @КодКлиента";
                SqlCommand checkID = new SqlCommand(checkIDq, sqlConnection1);
                checkID.Parameters.AddWithValue("@КодКлиента", textBox6.Text);

                int count = (int)checkID.ExecuteScalar();
                if (count == 0)
                {
                    MessageBox.Show("Клиент с указанным кодом не найден!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string updateq = @"
                    UPDATE Клиент
                    SET ФИО = @ФИО, НомерТелефона = @НомерТелефона, ДатаРождения = @ДатаРождения
                    WHERE КодКлиента = @КодКлиента";

                SqlCommand updateCommand = new SqlCommand(updateq, sqlConnection1);
                updateCommand.Parameters.AddWithValue("@КодКлиента", textBox6.Text);
                updateCommand.Parameters.AddWithValue("@ФИО", textBox7.Text);
                updateCommand.Parameters.AddWithValue("@НомерТелефона", textBox8.Text);
                updateCommand.Parameters.AddWithValue("@ДатаРождения", DateTime.ParseExact(date_r, "dd.MM.yyyy", null));

                updateCommand.ExecuteNonQuery();

                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.Cells[0].Value != null &&
                        row.Cells[0].Value.ToString() == textBox6.Text)
                    {
                        row.Cells[1].Value = textBox7.Text;
                        row.Cells[2].Value = textBox8.Text;
                        row.Cells[3].Value = date_r;
                        break;
                    }
                }

                textBox6.Clear();
                textBox7.Clear();
                textBox8.Clear();
                textBox9.Clear();
                MessageBox.Show("Данные клиента были успешно изменены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (SqlException ex)
            {      
                if (ex.Number == 547)
                {
                    MessageBox.Show("Клиенту должно быть больше 18 лет.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Ошибка при добавлении клиента: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обновлении клиента: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                sqlConnection1.Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem == null || string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Выберите поле для поиска и введите значение.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string searchField = comboBox2.SelectedItem.ToString();
            string searchValue = textBox1.Text;

            if (searchField == "Код клиента")
            {
                searchField = "КодКлиента";
            }
            else if (searchField == "Номер телефона")
            {
                searchField = "НомерТелефона";
            }

            try
            {
                sqlConnection1.Open();
                string query = "";
                if (searchField == "КодКлиента")
                {
                    query = $@"
                    SELECT TOP 1 КодКлиента, ФИО, НомерТелефона, ДатаРождения 
                    FROM Клиент 
                    WHERE {searchField} LIKE @SearchValue";
                }
                else
                {
                    query = $@"
                    SELECT КодКлиента, ФИО, НомерТелефона, ДатаРождения 
                    FROM Клиент 
                    WHERE {searchField} LIKE @SearchValue";
                }

                SqlCommand command = new SqlCommand(query, sqlConnection1);
                command.Parameters.AddWithValue("@SearchValue", $"%{searchValue}%");

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable resultsTable = new DataTable();

                adapter.Fill(resultsTable);

                if (resultsTable.Rows.Count > 0)
                {
                    dataGridView1.Rows.Clear();

                    foreach (DataRow row in resultsTable.Rows)
                    {
                        dataGridView1.Rows.Add(
                            row["КодКлиента"],
                            row["ФИО"],
                            row["НомерТелефона"],
                            ((DateTime)row["ДатаРождения"]).ToString("dd.MM.yyyy")
                        );
                    }
                    textBox1.Clear();
                    comboBox2.Text = "";
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

        private void button4_Click_1(object sender, EventArgs e)
        {
            textBox10.Clear();
            textBox11.Clear();
            textBox12.Clear();
            comboBox5.Text = "";
            textBox14.Clear();
            textBox15.Clear();
            comboBox6.Text = "";
        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            PrintEmployees();
        }

        private void PrintEmployees()
        {
            string query = "SELECT * FROM Сотрудник";

            try
            {
                sqlConnection1.Open();
                SqlCommand command = new SqlCommand(query, sqlConnection1);
                SqlDataReader reader = command.ExecuteReader();
                dataGridView2.Rows.Clear();
                while (reader.Read())
                {
                    dataGridView2.Rows.Add(
                         reader.GetValue(0),
                         reader.GetValue(1),
                         reader.GetValue(2),
                         reader.GetValue(3),
                         Convert.ToDateTime(reader.GetValue(4)).ToShortDateString(),
                         reader.GetValue(5), 
                         reader.GetValue(6)
                     );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении данных: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                sqlConnection1.Close();
            }
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView2.Rows[e.RowIndex];

                textBox10.Text = row.Cells[0].Value.ToString();
                textBox11.Text = row.Cells[1].Value.ToString();
                textBox12.Text = row.Cells[2].Value.ToString();
                comboBox5.Text = row.Cells[3].Value.ToString();
                textBox14.Text = row.Cells[4].Value.ToString();
                textBox15.Text = row.Cells[5].Value.ToString();
                comboBox6.Text = row.Cells[6].Value.ToString();
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            string date_r = textBox14.Text;
            string phoneNumber = textBox12.Text;
            string statusWork = comboBox6.Text;

            if (!string.IsNullOrEmpty(textBox10.Text))
            {
                MessageBox.Show("Код сотрудника уже существует! Очистите поле!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(textBox11.Text) || string.IsNullOrEmpty(textBox12.Text) ||
                string.IsNullOrEmpty(comboBox5.Text) || string.IsNullOrEmpty(textBox14.Text) ||
                string.IsNullOrEmpty(textBox15.Text) || string.IsNullOrEmpty(comboBox6.Text))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string[] validStatuses = { "Управляющий", "Технолог", "Макетчик", "Дизайнер", "Менеджер" };
            if (!validStatuses.Contains(comboBox5.Text))
            {
                MessageBox.Show("Выбранная должность не существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string[] validWorkStatuses = { "Уволен", "Работает" };
            if (!validWorkStatuses.Contains(statusWork))
            {
                MessageBox.Show("Статус работы должен быть либо 'Уволен', либо 'Работает'.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!Regex.IsMatch(date_r, @"^\d{2}\.\d{2}\.\d{4}$"))
            {
                MessageBox.Show("Дата рождения должна быть в формате ДД.ММ.ГГГГ", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (phoneNumber.Length != 11 || !Regex.IsMatch(phoneNumber, @"^\d{11}$"))
            {
                MessageBox.Show("Номер телефона должен состоять из 11 цифр!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                sqlConnection1.Open();
                string checkFIOQuery = "SELECT COUNT(*) FROM Сотрудник WHERE ФИО = @ФИО";
                SqlCommand checkFIOCommand = new SqlCommand(checkFIOQuery, sqlConnection1);
                checkFIOCommand.Parameters.AddWithValue("@ФИО", textBox11.Text);
                int fioCount = (int)checkFIOCommand.ExecuteScalar();

                if (fioCount > 0)
                {
                    MessageBox.Show("Сотрудник с таким ФИО уже существует!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string getMaxID = "SELECT ISNULL(MAX(КодСотрудника), 0) + 1 FROM Сотрудник";
                SqlCommand getMaxIDC = new SqlCommand(getMaxID, sqlConnection1);
                int newID = (int)getMaxIDC.ExecuteScalar();

                string insertQuery = @"
                INSERT INTO Сотрудник (КодСотрудника, ФИО, НомерТелефона, Должность, ДатаРождения, Пароль, СтатусРаботы)
                VALUES (@КодСотрудника, @ФИО, @НомерТелефона, @Должность, @ДатаРождения, @Пароль, @СтатусРаботы)";

                SqlCommand insertCommand = new SqlCommand(insertQuery, sqlConnection1);
                insertCommand.Parameters.AddWithValue("@КодСотрудника", newID);
                insertCommand.Parameters.AddWithValue("@ФИО", textBox11.Text);
                insertCommand.Parameters.AddWithValue("@НомерТелефона", textBox12.Text);
                insertCommand.Parameters.AddWithValue("@Должность", comboBox5.Text);
                insertCommand.Parameters.AddWithValue("@ДатаРождения", DateTime.ParseExact(date_r, "dd.MM.yyyy", null));
                insertCommand.Parameters.AddWithValue("@Пароль", textBox15.Text);
                insertCommand.Parameters.AddWithValue("@СтатусРаботы", statusWork);

                insertCommand.ExecuteNonQuery();

                dataGridView2.Rows.Add(newID, textBox11.Text, phoneNumber, comboBox5.Text, date_r, textBox15.Text, statusWork);

                MessageBox.Show("Сотрудник успешно добавлен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox10.Clear();
                textBox11.Clear();
                textBox12.Clear();
                comboBox5.Text = "";
                textBox14.Clear();
                textBox15.Clear();
                comboBox6.Text = "";
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show("Номер телефона уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (ex.Number == 547)
                {
                    MessageBox.Show("Сотруднику должно быть больше 18 лет.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Ошибка при добавлении сотрудника: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении клиента: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                sqlConnection1.Close();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox10.Text) || string.IsNullOrEmpty(textBox11.Text) || string.IsNullOrEmpty(textBox12.Text) ||
                string.IsNullOrEmpty(comboBox5.Text) || string.IsNullOrEmpty(textBox14.Text) || string.IsNullOrEmpty(textBox15.Text) ||
                string.IsNullOrEmpty(comboBox6.Text))
            {
                MessageBox.Show("Все поля должны быть заполнены!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string date_r = textBox14.Text;
            string phoneNumber = textBox12.Text;
            string workStatus = comboBox6.Text;

            if (!Regex.IsMatch(date_r, @"^\d{2}\.\d{2}\.\d{4}$"))
            {
                MessageBox.Show("Дата рождения должна быть в формате ДД.ММ.ГГГГ", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (phoneNumber.Length != 11 || !Regex.IsMatch(phoneNumber, @"^\d{11}$"))
            {
                MessageBox.Show("Номер телефона должен состоять из 11 цифр!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string[] validStatuses = { "Управляющий", "Технолог", "Макетчик", "Дизайнер", "Менеджер" };
            if (!validStatuses.Contains(comboBox5.Text))
            {
                MessageBox.Show("Выбранная должность не существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string[] validWorkStatuses = { "Работает", "Уволен" };
            if (!validWorkStatuses.Contains(workStatus))
            {
                MessageBox.Show("Статус работы должен быть либо 'Работает', либо 'Уволен'.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                sqlConnection1.Open();

                string checkIDq = "SELECT COUNT(1) FROM Сотрудник WHERE КодСотрудника = @КодСотрудника";
                SqlCommand checkID = new SqlCommand(checkIDq, sqlConnection1);
                checkID.Parameters.AddWithValue("@КодСотрудника", textBox10.Text);

                int count = (int)checkID.ExecuteScalar();
                if (count == 0)
                {
                    MessageBox.Show("Сотрудник с указанным кодом не найден!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string checkFIOQuery = "SELECT COUNT(*) FROM Сотрудник WHERE ФИО = @ФИО";
                SqlCommand checkFIOCommand = new SqlCommand(checkFIOQuery, sqlConnection1);
                checkFIOCommand.Parameters.AddWithValue("@ФИО", textBox11.Text);
                int fioCount = (int)checkFIOCommand.ExecuteScalar();

                string checkIDQuery = "SELECT КодСотрудника FROM Сотрудник WHERE ФИО = @ФИО";
                SqlCommand checkIDCommand = new SqlCommand(checkIDQuery, sqlConnection1);
                checkIDCommand.Parameters.AddWithValue("@ФИО", textBox11.Text);
                int IDCount = (int)checkIDCommand.ExecuteScalar();

                if (fioCount > 0 && IDCount != Convert.ToInt32(textBox10.Text))
                {
                    MessageBox.Show("Сотрудник с таким ФИО уже существует!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string updateq = @"
                UPDATE Сотрудник
                SET ФИО = @ФИО, НомерТелефона = @НомерТелефона, Должность = @Должность, ДатаРождения = @ДатаРождения, Пароль = @Пароль, СтатусРаботы = @СтатусРаботы
                WHERE КодСотрудника = @КодСотрудника";

                SqlCommand updateCommand = new SqlCommand(updateq, sqlConnection1);
                updateCommand.Parameters.AddWithValue("@КодСотрудника", textBox10.Text);
                updateCommand.Parameters.AddWithValue("@ФИО", textBox11.Text);
                updateCommand.Parameters.AddWithValue("@НомерТелефона", textBox12.Text);
                updateCommand.Parameters.AddWithValue("@Должность", comboBox5.Text);
                updateCommand.Parameters.AddWithValue("@ДатаРождения", DateTime.ParseExact(date_r, "dd.MM.yyyy", null));
                updateCommand.Parameters.AddWithValue("@Пароль", textBox15.Text);
                updateCommand.Parameters.AddWithValue("@СтатусРаботы", workStatus);

                updateCommand.ExecuteNonQuery();

                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    if (row.Cells[0].Value != null &&
                        row.Cells[0].Value.ToString() == textBox10.Text)
                    {
                        row.Cells[1].Value = textBox11.Text;
                        row.Cells[2].Value = textBox12.Text;
                        row.Cells[3].Value = comboBox5.Text;
                        row.Cells[4].Value = date_r;
                        row.Cells[5].Value = textBox15.Text;
                        row.Cells[6].Value = workStatus;
                        break;
                    }
                }
                MessageBox.Show("Данные о сотруднике успешно обновлены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox10.Clear();
                textBox11.Clear();
                textBox12.Clear();
                comboBox5.Text = "";
                textBox14.Clear();
                textBox15.Clear();
                comboBox6.Text = "";
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show("Номер телефона уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (ex.Number == 547)
                {
                    MessageBox.Show("Сотруднику должно быть больше 18 лет.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Ошибка при обновлении сотрудника: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обновлении сотрудника: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                sqlConnection1.Close();
            }
        }


        private void button5_Click(object sender, EventArgs e)
        {
            if (comboBox3.SelectedItem == null || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("Выберите поле для поиска и введите значение.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string searchField = comboBox3.SelectedItem.ToString();
            string searchValue = textBox2.Text;

            if (searchField == "Код сотрудника")
            {
                searchField = "КодСотрудника";
            }
            else if (searchField == "Номер телефона")
            {
                searchField = "НомерТелефона";
            }

            try
            {
                
                sqlConnection1.Open();
                string query = "";
                if (searchField == "КодСотрудника")
                {
                    query = $@"
                    SELECT TOP 1 *
                    FROM Сотрудник 
                    WHERE {searchField} LIKE @SearchValue";
                }
                else
                {
                    query = $@"
                    SELECT *
                    FROM Сотрудник 
                    WHERE {searchField} LIKE @SearchValue";
                }
                

                SqlCommand command = new SqlCommand(query, sqlConnection1);
                command.Parameters.AddWithValue("@SearchValue", $"%{searchValue}%");

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
                             reader.GetValue(0),
                             reader.GetValue(1),
                             reader.GetValue(2),
                             reader.GetValue(3),
                             Convert.ToDateTime(reader.GetValue(4)).ToShortDateString(),
                             reader.GetValue(5)
                         );
                    }
                    textBox2.Clear();
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

        private void button10_Click(object sender, EventArgs e)
        {
            LoadOrders();
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
                    с.ФИО AS Сотрудник,
                    к.ФИО AS Клиент,
                    Стоимость,
                    СтатусВыполнения
                FROM ЗаказНаПечать з
                JOIN Сотрудник с ON с.КодСотрудника = з.КодСотрудника
                JOIN Клиент к ON к.КодКлиента = з.КодКлиента";

                SqlCommand command = new SqlCommand(query, sqlConnection1);
                SqlDataReader reader = command.ExecuteReader();
                dataGridView3.Rows.Clear();
                while (reader.Read())
                {
                    dataGridView3.Rows.Add(
                        reader["КодЗаказа"].ToString(),
                        Convert.ToDateTime(reader["ДатаЗаказа"]).ToShortDateString(),
                        reader["Сотрудник"].ToString(),
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

        private void dataGridView3_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView3.Rows[e.RowIndex];

                textBox20.Text = row.Cells[0].Value.ToString();
                textBox21.Text = row.Cells[1].Value.ToString();
                textBox22.Text = row.Cells[2].Value.ToString();
                textBox23.Text = row.Cells[3].Value.ToString();
                textBox24.Text = row.Cells[4].Value.ToString();
                comboBox4.Text = row.Cells[5].Value.ToString();
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            textBox20.Clear();
            textBox21.Clear();
            textBox22.Clear();
            textBox23.Clear();
            textBox24.Clear();
            comboBox4.Text = "";
            textBox24.Text = "0";
            textBox21.Text = DateTime.Now.ToString("dd.MM.yyyy");
        }

        private void button14_Click(object sender, EventArgs e)
        {
            string[] validStatuses = { "Выполнен", "Принят в работу", "Ожидание подтверждения", "Отменен" };
            if (!validStatuses.Contains(comboBox4.Text))
            {
                MessageBox.Show("Некорректный статус выполнения.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (comboBox4.Text != "Ожидание подтверждения")
            {
                MessageBox.Show("Статус заказа должен быть 'Ожидание подтверждения'.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!DateTime.TryParse(textBox21.Text, out DateTime orderDate))
            {
                MessageBox.Show("Введите корректную дату заказа.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (orderDate > DateTime.Today)
            {
                MessageBox.Show("Дата заказа не может быть будущей. Введите сегодняшнюю или прошедшую дату.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string employeName = textBox22.Text.Trim();
            if (string.IsNullOrWhiteSpace(employeName))
            {
                MessageBox.Show("Введите ФИО сотрудника.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string clientName = textBox23.Text.Trim();
            if (string.IsNullOrWhiteSpace(clientName))
            {
                MessageBox.Show("Введите ФИО клиента.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int clientId;
            sqlConnection1.Open();
            string clientQuery = "SELECT КодКлиента FROM Клиент WHERE ФИО = @ClientName";
            SqlCommand clientCommand = new SqlCommand(clientQuery, sqlConnection1);
            clientCommand.Parameters.AddWithValue("@ClientName", clientName);


            object result = clientCommand.ExecuteScalar();
            sqlConnection1.Close();
            if (result == null)
            {
                MessageBox.Show("Клиент с указанным ФИО не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            clientId = Convert.ToInt32(result);

            int employId;
            sqlConnection1.Open();
            string employQuery = "SELECT КодСотрудника FROM Сотрудник WHERE ФИО = @EmployeName";
            SqlCommand employCommand = new SqlCommand(employQuery, sqlConnection1);
            employCommand.Parameters.AddWithValue("@EmployeName", employeName);


            object result1 = employCommand.ExecuteScalar();
            sqlConnection1.Close();
            if (result1 == null)
            {
                MessageBox.Show("Сотрудник с указанным ФИО не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            employId = Convert.ToInt32(result1);

            sqlConnection1.Open();
            string employeeStatusQuery = "SELECT СтатусРаботы FROM Сотрудник WHERE ФИО = @EmployeeName";
            SqlCommand employeeStatusCommand = new SqlCommand(employeeStatusQuery, sqlConnection1);
            employeeStatusCommand.Parameters.AddWithValue("@EmployeeName", textBox22.Text);
            object employeeStatusObj = employeeStatusCommand.ExecuteScalar();
            sqlConnection1.Close();

            if (employeeStatusObj != null && employeeStatusObj.ToString() == "Уволен")
            {
                MessageBox.Show("Нельзя назначить заказ на уволенного сотрудника.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                sqlConnection1.Open();
                string orderIdQuery = "SELECT ISNULL(MAX(КодЗаказа), 0) + 1 FROM ЗаказНаПечать";
                SqlCommand orderIdCommand = new SqlCommand(orderIdQuery, sqlConnection1);
                int newOrderId = Convert.ToInt32(orderIdCommand.ExecuteScalar());
                sqlConnection1.Close();

                sqlConnection1.Open();
                string insertQuery = @"
                INSERT INTO ЗаказНаПечать (КодЗаказа, ДатаЗаказа, КодСотрудника, КодКлиента, Стоимость, СтатусВыполнения)
                VALUES (@OrderId, @OrderDate, @EmployeeId, @ClientId, @Cost, @Status)";
                SqlCommand insertCommand = new SqlCommand(insertQuery, sqlConnection1);
                insertCommand.Parameters.AddWithValue("@OrderId", newOrderId);
                insertCommand.Parameters.AddWithValue("@OrderDate", orderDate);
                insertCommand.Parameters.AddWithValue("@EmployeeId", employId);
                insertCommand.Parameters.AddWithValue("@ClientId", clientId);
                insertCommand.Parameters.AddWithValue("@Cost", textBox24.Text);
                insertCommand.Parameters.AddWithValue("@Status", comboBox4.Text);

                insertCommand.ExecuteNonQuery();

                MessageBox.Show("Заказ успешно добавлен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                sqlConnection1.Close();
                textBox21.Clear();
                textBox22.Clear();
                textBox23.Clear();
                textBox24.Text = "0";
                comboBox4.Text = "";
                textBox21.Text = DateTime.Now.ToString("dd.MM.yyyy");

                LoadOrders();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении заказа: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                sqlConnection1.Close();
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox20.Text) ||
                string.IsNullOrWhiteSpace(textBox21.Text) ||
                string.IsNullOrWhiteSpace(textBox22.Text) ||
                string.IsNullOrWhiteSpace(textBox23.Text) ||
                string.IsNullOrWhiteSpace(textBox24.Text) ||
                string.IsNullOrWhiteSpace(comboBox4.Text))
            {
                MessageBox.Show("Все поля должны быть заполнены.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string[] validStatuses = { "Выполнен", "Принят в работу", "Ожидание подтверждения", "Отменен" };
            if (!validStatuses.Contains(comboBox4.Text))
            {
                MessageBox.Show("Некорректный статус выполнения.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!DateTime.TryParse(textBox21.Text, out DateTime orderDate))
            {
                MessageBox.Show("Введите корректную дату заказа.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (orderDate > DateTime.Today)
            {
                MessageBox.Show("Дата заказа не может быть будущей. Введите сегодняшнюю или прошедшую дату.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string clientName = textBox23.Text.Trim();
            if (string.IsNullOrWhiteSpace(clientName))
            {
                MessageBox.Show("Введите ФИО клиента.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int clientId;
            sqlConnection1.Open();
            string clientQuery = "SELECT КодКлиента FROM Клиент WHERE ФИО = @ClientName";
            SqlCommand clientCommand = new SqlCommand(clientQuery, sqlConnection1);
            clientCommand.Parameters.AddWithValue("@ClientName", clientName);


            object result = clientCommand.ExecuteScalar();
            sqlConnection1.Close();
            if (result == null)
            {
                MessageBox.Show("Клиент с указанным ФИО не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            clientId = Convert.ToInt32(result);

            string employeName = textBox22.Text.Trim();
            int employId;
            sqlConnection1.Open();
            string employQuery = "SELECT КодСотрудника FROM Сотрудник WHERE ФИО = @EmployeName";
            SqlCommand employCommand = new SqlCommand(employQuery, sqlConnection1);
            employCommand.Parameters.AddWithValue("@EmployeName", employeName);


            object result1 = employCommand.ExecuteScalar();
            sqlConnection1.Close();
            if (result1 == null)
            {
                MessageBox.Show("Сотрудник с указанным ФИО не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            employId = Convert.ToInt32(result1);


            sqlConnection1.Open();
            string statusQuery = "SELECT СтатусВыполнения FROM ЗаказНаПечать WHERE КодЗаказа = @OrderId";
            SqlCommand statusCommand = new SqlCommand(statusQuery, sqlConnection1);
            statusCommand.Parameters.AddWithValue("@OrderId", Convert.ToInt32(textBox20.Text));
            object currentStatusObj = statusCommand.ExecuteScalar();
            sqlConnection1.Close();

            string currentStatus = currentStatusObj.ToString();
            if ((currentStatusObj != null) && (currentStatus != comboBox4.Text))
            {

                if (currentStatus == "Выполнен" || currentStatus == "Отменен")
                {
                    MessageBox.Show("Нельзя изменить заказ, если он уже выполнен или отменен.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (currentStatus == "Ожидание подтверждения" && comboBox4.Text != "Принят в работу" && comboBox4.Text != "Отменен")
                {
                    MessageBox.Show("Статус 'Ожидание подтверждения' можно изменить только на 'Принят в работу' или 'Отменен'.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (currentStatus == "Принят в работу" && comboBox4.Text != "Выполнен" && comboBox4.Text != "Отменен")
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
                    ДатаЗаказа = @OrderDate,
                    КодСотрудника = @EmployeeId,
                    КодКлиента = @ClientId,
                    Стоимость = @Cost,
                    СтатусВыполнения = @Status
                WHERE КодЗаказа = @OrderId";

                SqlCommand updateCommand = new SqlCommand(updateQuery, sqlConnection1);
                updateCommand.Parameters.AddWithValue("@OrderId", Convert.ToInt32(textBox20.Text));
                updateCommand.Parameters.AddWithValue("@OrderDate", orderDate);
                updateCommand.Parameters.AddWithValue("@EmployeeId", employId);
                updateCommand.Parameters.AddWithValue("@ClientId", clientId);
                updateCommand.Parameters.AddWithValue("@Cost", Convert.ToInt32(textBox24.Text));
                updateCommand.Parameters.AddWithValue("@Status", comboBox4.Text);

                int rowsAffected = updateCommand.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Заказ успешно обновлен.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    sqlConnection1.Close();
                    LoadOrders();
                }
                else
                {
                    MessageBox.Show("Заказ с указанным кодом не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (SqlException ex) when (ex.Number == 50000)
            {
                MessageBox.Show("Невозможно изменить дату заказа, сотрудника или клиента", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void button12_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null || string.IsNullOrWhiteSpace(textBox16.Text))
            {
                MessageBox.Show("Выберите поле для поиска и введите значение.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string searchField = comboBox1.SelectedItem.ToString();
            string searchValue = textBox16.Text;
            int employeID = -1;
            int clientID = -1;

            if (searchField == "Код заказа")
            {
                searchField = "КодЗаказа";
            }
            else if (searchField == "Сотрудник")
            {
                searchField = "з.КодСотрудника";

                sqlConnection1.Open();
                string getEmployeeIdQuery = "SELECT КодСотрудника FROM Сотрудник WHERE ФИО LIKE @FIO";
                SqlCommand getEmployeeIdCommand = new SqlCommand(getEmployeeIdQuery, sqlConnection1);
                getEmployeeIdCommand.Parameters.AddWithValue("@FIO", textBox16.Text.Trim());

                var result = getEmployeeIdCommand.ExecuteScalar();
                if (result == null)
                {
                    MessageBox.Show("Клиент не найден!", "Результаты поиска", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                sqlConnection1.Close();
                employeID = (int)result;

            }
            else if (searchField == "Клиент")
            {
                searchField = "з.КодКлиента";

                sqlConnection1.Open();
                string getEmployeeIdQuery = "SELECT КодКлиента FROM Клиент WHERE ФИО LIKE @FIO";
                SqlCommand getEmployeeIdCommand = new SqlCommand(getEmployeeIdQuery, sqlConnection1);
                getEmployeeIdCommand.Parameters.AddWithValue("@FIO", textBox16.Text.Trim());

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
                    с.ФИО AS Сотрудник,
                    к.ФИО AS Клиент,
                    з.Стоимость,
                    з.СтатусВыполнения
                FROM ЗаказНаПечать з
                JOIN Сотрудник с ON с.КодСотрудника = з.КодСотрудника
                JOIN Клиент к ON к.КодКлиента = з.КодКлиента
                WHERE {searchField} LIKE @SearchValue";

                SqlCommand command = new SqlCommand(query, sqlConnection1);

                if (searchField == "КодЗаказа" || searchField == "Стоимость" || searchField == "з.КодСотрудника" || searchField == "з.КодКлиента")
                {
                    if (int.TryParse(searchValue, out int numericValue))
                    {
                        command.Parameters.AddWithValue("@SearchValue", numericValue);
                    }
                    else if (searchField == "з.КодКлиента")
                    {
                        command.Parameters.AddWithValue("@SearchValue", clientID);
                    }
                    else if (searchField == "з.КодСотрудника")
                    {
                        command.Parameters.AddWithValue("@SearchValue", employeID);
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
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable resultsTable = new DataTable();

                adapter.Fill(resultsTable);

                if (resultsTable.Rows.Count > 0)
                {
                    SqlDataReader reader = command.ExecuteReader();
                    dataGridView3.Rows.Clear();
                    while (reader.Read())
                    {
                        dataGridView3.Rows.Add(
                        reader["КодЗаказа"].ToString(),
                        Convert.ToDateTime(reader["ДатаЗаказа"]).ToShortDateString(),
                        reader["Сотрудник"].ToString(),
                        reader["Клиент"].ToString(),
                        reader["Стоимость"].ToString(),
                        reader["СтатусВыполнения"].ToString()
                    );
                    }

                    textBox16.Clear();
                    comboBox1.Text = "";
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
            if (string.IsNullOrWhiteSpace(textBox20.Text))
            {
                MessageBox.Show("Заполните поле: Код заказа.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int id = Convert.ToInt32(textBox20.Text);

            try
            {
                ZakazMaterialUsluga uslugaMaterial = new ZakazMaterialUsluga(id);
                uslugaMaterial.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void button17_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox3.Text) || string.IsNullOrEmpty(textBox4.Text))
            {
                MessageBox.Show("Заполните поля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!DateTime.TryParse(textBox3.Text, out DateTime startDate))
            {
                MessageBox.Show("Введите корректную начальную дату.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!DateTime.TryParse(textBox4.Text, out DateTime endDate))
            {
                MessageBox.Show("Введите корректную конечную дату.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SqlCommand command = null;
            SqlDataReader reader = null;

            try
            {
                sqlConnection1.Open();

                string query = @"
                SELECT 
                    Услуга.Наименование AS Услуга,
                    SUM(УслугаЗаказа.КоличествоУслуг) AS Количество
                FROM 
                    ЗаказНаПечать
                INNER JOIN 
                    УслугаЗаказа ON ЗаказНаПечать.КодЗаказа = УслугаЗаказа.КодЗаказа
                INNER JOIN 
                    Услуга ON УслугаЗаказа.КодУслуги = Услуга.КодУслуги
                WHERE 
                    ЗаказНаПечать.ДатаЗаказа BETWEEN @StartDate AND @EndDate
                GROUP BY 
                    Услуга.Наименование
                ORDER BY 
                    Количество DESC";

                command = new SqlCommand(query, sqlConnection1);

                command.Parameters.AddWithValue("@StartDate", startDate);
                command.Parameters.AddWithValue("@EndDate", endDate);

                reader = command.ExecuteReader();

                chart1.Series.Clear();
                chart1.Titles.Clear();
                chart1.Titles.Add("Популярность услуг за период");

                Series series = new Series
                {
                    Name = "Услуги",
                    ChartType = SeriesChartType.Pie,
                    IsValueShownAsLabel = true,
                };
                chart1.Series.Add(series);

                int totalCount = 0;
                Dictionary<string, int> servicesData = new Dictionary<string, int>();

                while (reader.Read())
                {
                    string serviceName = reader["Услуга"].ToString();
                    int count = Convert.ToInt32(reader["Количество"]);
                    servicesData[serviceName] = count;
                    totalCount += count;
                }

                foreach (var item in servicesData)
                {
                    double percentage = (double)item.Value / totalCount * 100;
                    DataPoint point = new DataPoint();
                    point.SetValueXY(item.Key, item.Value);
                    point.Label = $"{percentage:F2}%"; 
                    point.LegendText = item.Key;  
                    series.Points.Add(point);
                }

                var existingLegend = chart1.Legends.FindByName("Default");
                if (existingLegend != null)
                {
                    chart1.Legends.Remove(existingLegend); 
                }

                Legend legend = new Legend("Default");
                chart1.Legends.Add(legend); 

                chart1.ChartAreas[0].Area3DStyle.Enable3D = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                sqlConnection1.Close();
            }

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void FormForAdministrator_Load(object sender, EventArgs e)
        {
            textBox24.Text = "0";

            textBox21.Text = DateTime.Now.ToString("dd.MM.yyyy");
        }

        private void button19_Click(object sender, EventArgs e)
        {
            textBox31.Clear();
            textBox32.Clear();
            textBox33.Clear();
            textBox34.Clear();
        }

        private void button20_Click(object sender, EventArgs e)
        {
            PrintMaterial();
        }
        private void PrintMaterial()
        {
            try
            {
                string query = @"SELECT * FROM ПечатныеМатериалы";
                sqlConnection1.Open();
                SqlCommand command = new SqlCommand(query, sqlConnection1);

                SqlDataReader reader = command.ExecuteReader();
                dataGridView4.Rows.Clear();
                while (reader.Read())
                {
                    dataGridView4.Rows.Add(
                       reader.GetValue(0),
                       reader.GetValue(1),
                       reader.GetValue(2),
                       reader.GetValue(3)
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

        private void dataGridView4_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView4.Rows[e.RowIndex];

                textBox31.Text = row.Cells[0].Value.ToString();
                textBox32.Text = row.Cells[1].Value.ToString();
                textBox33.Text = row.Cells[2].Value.ToString();
                textBox34.Text = row.Cells[3].Value.ToString();
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox31.Text) || string.IsNullOrEmpty(textBox32.Text) || string.IsNullOrEmpty(textBox33.Text) || string.IsNullOrEmpty(textBox34.Text))
            {
                MessageBox.Show("Все поля должны быть заполнены!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(textBox33.Text, out int quantity) || quantity < 0)
            {
                MessageBox.Show("Поле 'Количество' должно быть числом большим или равным нулю!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(textBox34.Text, out int quantity1) || quantity1 <= 0)
            {
                MessageBox.Show("Поле 'Стоимость' должно быть числом большим нуля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                sqlConnection1.Open();

                string checkIDq = "SELECT COUNT(1) FROM ПечатныеМатериалы WHERE КодМатериала = @КодМатериала";
                SqlCommand checkID = new SqlCommand(checkIDq, sqlConnection1);
                checkID.Parameters.AddWithValue("@КодМатериала", textBox31.Text);

                int count = (int)checkID.ExecuteScalar();
                if (count == 0)
                {
                    MessageBox.Show("Материал с указанным кодом не найден!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string updateq = @"
                    UPDATE ПечатныеМатериалы
                    SET Название = @Название, Количество = @Количество, Цена = @Цена
                    WHERE КодМатериала = @КодМатериала";

                SqlCommand updateCommand = new SqlCommand(updateq, sqlConnection1);
                updateCommand.Parameters.AddWithValue("@КодМатериала", textBox31.Text);
                updateCommand.Parameters.AddWithValue("@Название", textBox32.Text);
                updateCommand.Parameters.AddWithValue("@Количество", textBox33.Text);
                updateCommand.Parameters.AddWithValue("@Цена", textBox34.Text);

                updateCommand.ExecuteNonQuery();

                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    if (row.Cells[0].Value != null &&
                        row.Cells[0].Value.ToString() == textBox31.Text)
                    {
                        row.Cells[1].Value = textBox32.Text;
                        row.Cells[2].Value = textBox33.Text;
                        row.Cells[3].Value = textBox34.Text;
                        break;
                    }
                }
                MessageBox.Show("Материал успешно изменен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox31.Clear();
                textBox32.Clear();
                textBox33.Clear();
                textBox34.Clear();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show("Материал уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Ошибка при добавлении материала: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обновлении материала: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                sqlConnection1.Close();
            }
        }

        private void button21_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox32.Text) || string.IsNullOrEmpty(textBox33.Text) || string.IsNullOrEmpty(textBox34.Text))
            {
                MessageBox.Show("Все поля должны быть заполнены!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(textBox33.Text, out int quantity) || quantity < 0)
            {
                MessageBox.Show("Поле 'Количество' должно быть числом большим или равным нулю!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(textBox34.Text, out int quantity1) || quantity1 <= 0)
            {
                MessageBox.Show("Поле 'Стоимость' должно быть числом большим нуля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                sqlConnection1.Open();
                string getMaxID = "SELECT ISNULL(MAX(КодМатериала), 0) + 1 FROM ПечатныеМатериалы";
                SqlCommand getMaxIDC = new SqlCommand(getMaxID, sqlConnection1);
                int newID = (int)getMaxIDC.ExecuteScalar();

                string insertQuery = @"
                INSERT INTO ПечатныеМатериалы (КодМатериала, Название, Количество, Цена)
                VALUES (@КодМатериала, @Название, @Количество, @Цена)";

                SqlCommand insertCommand = new SqlCommand(insertQuery, sqlConnection1);
                insertCommand.Parameters.AddWithValue("@КодМатериала", newID);
                insertCommand.Parameters.AddWithValue("@Название", textBox32.Text);
                insertCommand.Parameters.AddWithValue("@Количество", textBox33.Text);
                insertCommand.Parameters.AddWithValue("@Цена", textBox34.Text);

                insertCommand.ExecuteNonQuery();

                dataGridView2.Rows.Add(newID, textBox32.Text, textBox33.Text, textBox34.Text);

                MessageBox.Show("Материал успешно добавлен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox32.Clear();
                textBox33.Clear();
                textBox34.Clear();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show("Материал уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Ошибка при добавлении материала: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении материала: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                sqlConnection1.Close();
            }
        }

        private void dataGridView5_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView5.Rows[e.RowIndex];

                textBox41.Text = row.Cells[0].Value.ToString();
                textBox42.Text = row.Cells[1].Value.ToString();
                textBox43.Text = row.Cells[2].Value.ToString();
                textBox44.Text = row.Cells[3].Value.ToString();
            }
        }

        private void button23_Click(object sender, EventArgs e)
        {
            textBox41.Clear();
            textBox42.Clear();
            textBox43.Clear();
            textBox44.Clear();
        }

        private void button24_Click(object sender, EventArgs e)
        {
            PrintUsluga();
        }

        private void PrintUsluga()
        {
            try
            {
                string query = @"SELECT * FROM Услуга";
                sqlConnection1.Open();
                SqlCommand command = new SqlCommand(query, sqlConnection1);

                SqlDataReader reader = command.ExecuteReader();
                dataGridView5.Rows.Clear();
                while (reader.Read())
                {
                    dataGridView5.Rows.Add(
                       reader.GetValue(0),
                       reader.GetValue(1),
                       reader.GetValue(2),
                       reader.GetValue(3)
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

        private void button22_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox41.Text) || string.IsNullOrEmpty(textBox42.Text) || string.IsNullOrEmpty(textBox43.Text) || string.IsNullOrEmpty(textBox44.Text))
            {
                MessageBox.Show("Все поля должны быть заполнены!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(textBox44.Text, out int quantity1) || quantity1 <= 0)
            {
                MessageBox.Show("Поле 'Стоимость' должно быть числом больше нуля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                sqlConnection1.Open();

                string checkIDq = "SELECT COUNT(1) FROM Услуга WHERE КодУслуги = @КодУслуги";
                SqlCommand checkID = new SqlCommand(checkIDq, sqlConnection1);
                checkID.Parameters.AddWithValue("@КодУслуги", textBox41.Text);

                int count = (int)checkID.ExecuteScalar();
                if (count == 0)
                {
                    MessageBox.Show("Услуга с указанным кодом не найден!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string updateq = @"
            UPDATE Услуга
            SET Наименование = @Наименование, Описание = @Описание, Цена = @Цена
            WHERE КодУслуги = @КодУслуги";

                SqlCommand updateCommand = new SqlCommand(updateq, sqlConnection1);
                updateCommand.Parameters.AddWithValue("@КодУслуги", textBox41.Text);
                updateCommand.Parameters.AddWithValue("@Наименование", textBox42.Text);
                updateCommand.Parameters.AddWithValue("@Описание", textBox43.Text);
                updateCommand.Parameters.AddWithValue("@Цена", textBox44.Text);

                updateCommand.ExecuteNonQuery();

                foreach (DataGridViewRow row in dataGridView2.Rows)
                {
                    if (row.Cells[0].Value != null &&
                        row.Cells[0].Value.ToString() == textBox41.Text)
                    {
                        row.Cells[1].Value = textBox42.Text;
                        row.Cells[2].Value = textBox43.Text;
                        row.Cells[3].Value = textBox44.Text;
                        break;
                    }
                }
                MessageBox.Show("Данные об услуге успешно изменены!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox41.Clear();
                textBox42.Clear();
                textBox43.Clear();
                textBox44.Clear();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show("Услуга уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Ошибка при добавлении услуги: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обновлении услуги: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                sqlConnection1.Close();
            }
        }

        private void button25_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox42.Text) || string.IsNullOrEmpty(textBox43.Text) || string.IsNullOrEmpty(textBox44.Text))
            {
                MessageBox.Show("Все поля должны быть заполнены!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(textBox44.Text, out int quantity1) || quantity1 <= 0)
            {
                MessageBox.Show("Поле 'Стоимость' должно быть числом больше нуля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                sqlConnection1.Open();
                string getMaxID = "SELECT ISNULL(MAX(КодУслуги), 0) + 1 FROM Услуга";
                SqlCommand getMaxIDC = new SqlCommand(getMaxID, sqlConnection1);
                int newID = (int)getMaxIDC.ExecuteScalar();

                string insertQuery = @"
        INSERT INTO Услуга (КодУслуги, Наименование, Описание, Цена)
        VALUES (@КодУслуги, @Наименование, @Описание, @Цена)";

                SqlCommand insertCommand = new SqlCommand(insertQuery, sqlConnection1);
                insertCommand.Parameters.AddWithValue("@КодУслуги", newID);
                insertCommand.Parameters.AddWithValue("@Наименование", textBox42.Text);
                insertCommand.Parameters.AddWithValue("@Описание", textBox43.Text);
                insertCommand.Parameters.AddWithValue("@Цена", textBox44.Text);

                insertCommand.ExecuteNonQuery();

                dataGridView2.Rows.Add(newID, textBox42.Text, textBox43.Text, textBox44.Text);

                MessageBox.Show("Услуга успешно добавлена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                textBox42.Clear();
                textBox43.Clear();
                textBox44.Clear();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    MessageBox.Show("Услуга уже существует.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Ошибка при добавлении услуги: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при добавлении услуги: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                sqlConnection1.Close();
            }
        }

    }
}
