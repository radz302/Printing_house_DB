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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CursovaiUD
{
    public partial class FormForManager : Form
    {
        int EmployeeId;
        public FormForManager()
        {
            InitializeComponent();
        }
        public FormForManager(int EmployeeI)
        {
            EmployeeId = EmployeeI;
            InitializeComponent();
        }

        private void FormForTehnolog_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
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

        private void button_Print_Click(object sender, EventArgs e)
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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

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

        private void button2_Click(object sender, EventArgs e)
        {
            textBox6.Clear();
            textBox7.Clear();
            textBox8.Clear();
            textBox9.Clear();
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

        private void button5_Click(object sender, EventArgs e)
        {
            string[] validStatuses = { "Выполнен", "Принят в работу", "Ожидание подтверждения", "Отменен" };
            if (!validStatuses.Contains(comboBox1.Text))
            {
                MessageBox.Show("Некорректный статус выполнения.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (comboBox1.Text != "Ожидание подтверждения")
            {
                MessageBox.Show("Статус заказа должен быть 'Ожидание подтверждения'.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!DateTime.TryParse(textBox2.Text, out DateTime orderDate))
            {
                MessageBox.Show("Введите корректную дату заказа.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (orderDate > DateTime.Today)
            {
                MessageBox.Show("Дата заказа не может быть будущей. Введите сегодняшнюю или прошедшую дату.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string employeName = textBox11.Text.Trim();
            if (string.IsNullOrWhiteSpace(employeName))
            {
                MessageBox.Show("Введите ФИО сотрудника.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string clientName = textBox10.Text.Trim();
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
            employeeStatusCommand.Parameters.AddWithValue("@EmployeeName", textBox11.Text);
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
                insertCommand.Parameters.AddWithValue("@Cost", textBox4.Text);
                insertCommand.Parameters.AddWithValue("@Status", comboBox1.Text);

                insertCommand.ExecuteNonQuery();

                MessageBox.Show("Заказ успешно добавлен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                sqlConnection1.Close();
                textBox2.Clear();
                textBox11.Clear();
                textBox10.Clear();
                textBox4.Text = "0";
                textBox2.Text = DateTime.Now.ToString("dd.MM.yyyy");
                comboBox1.Text = "";

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

        private void button7_Click(object sender, EventArgs e)
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
                command.Parameters.AddWithValue("@EmployeeId", EmployeeId);
                SqlDataReader reader = command.ExecuteReader();
                dataGridView2.Rows.Clear();
                while (reader.Read())
                {
                    dataGridView2.Rows.Add(
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

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView2.Rows[e.RowIndex];

                textBox3.Text = row.Cells[0].Value.ToString();
                textBox2.Text = row.Cells[1].Value.ToString();
                textBox11.Text = row.Cells[2].Value.ToString();
                textBox10.Text = row.Cells[3].Value.ToString();
                textBox4.Text = row.Cells[4].Value.ToString();
                comboBox1.Text = row.Cells[5].Value.ToString();
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            textBox3.Clear();
            textBox2.Clear();
            textBox11.Clear();
            textBox10.Clear();
            textBox4.Clear();
            comboBox1.Text = "";
            textBox4.Text = "0";
            textBox2.Text = DateTime.Now.ToString("dd.MM.yyyy");
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
                getEmployeeIdCommand.Parameters.AddWithValue("@FIO", textBox5.Text.Trim());

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
                    dataGridView2.Rows.Clear();
                    while (reader.Read())
                    {
                        dataGridView2.Rows.Add(
                        reader["КодЗаказа"].ToString(),
                        Convert.ToDateTime(reader["ДатаЗаказа"]).ToShortDateString(),
                        reader["Сотрудник"].ToString(),
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

        private void FormForTehnolog_Load(object sender, EventArgs e)
        {
            textBox4.Text = "0";
            textBox2.Text = DateTime.Now.ToString("dd.MM.yyyy");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox3.Text) ||
                string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox11.Text) ||
                string.IsNullOrWhiteSpace(textBox10.Text) ||
                string.IsNullOrWhiteSpace(textBox4.Text) ||
                string.IsNullOrWhiteSpace(comboBox1.Text))
            {
                MessageBox.Show("Все поля должны быть заполнены.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string[] validStatuses = { "Выполнен", "Принят в работу", "Ожидание подтверждения", "Отменен" };
            if (!validStatuses.Contains(comboBox1.Text))
            {
                MessageBox.Show("Некорректный статус выполнения.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!DateTime.TryParse(textBox2.Text, out DateTime orderDate))
            {
                MessageBox.Show("Введите корректную дату заказа.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (orderDate > DateTime.Today)
            {
                MessageBox.Show("Дата заказа не может быть будущей. Введите сегодняшнюю или прошедшую дату.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string clientName = textBox10.Text.Trim();
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

            string employeName = textBox11.Text.Trim();
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
                    ДатаЗаказа = @OrderDate,
                    КодСотрудника = @EmployeeId,
                    КодКлиента = @ClientId,
                    Стоимость = @Cost,
                    СтатусВыполнения = @Status
                WHERE КодЗаказа = @OrderId";

                SqlCommand updateCommand = new SqlCommand(updateQuery, sqlConnection1);
                updateCommand.Parameters.AddWithValue("@OrderId", Convert.ToInt32(textBox3.Text));
                updateCommand.Parameters.AddWithValue("@OrderDate", orderDate);
                updateCommand.Parameters.AddWithValue("@EmployeeId", employId);
                updateCommand.Parameters.AddWithValue("@ClientId", clientId);
                updateCommand.Parameters.AddWithValue("@Cost", Convert.ToInt32(textBox4.Text));
                updateCommand.Parameters.AddWithValue("@Status", comboBox1.Text);

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

        private void button8_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("Заполните поле: Код заказа.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int id = Convert.ToInt32(textBox3.Text);

            try
            {
                ZakazUsluga zakazUsluga = new ZakazUsluga(id);
                zakazUsluga.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void button16_Click(object sender, EventArgs e)
        {
            try
            {
                ButtonSpisokSotrudnikov buttonSpisokSotrudnikov = new ButtonSpisokSotrudnikov();
                buttonSpisokSotrudnikov.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button10_Click(object sender, EventArgs e)
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

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
