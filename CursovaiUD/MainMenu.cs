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
    public partial class MainMenu : Form
    {
        public MainMenu()
        {
            InitializeComponent();
            textBox1.Text = "Кузнецова Мария Николаевна";
            textBox2.Text = "Password1";
        }

        private void MainMenu_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            myCommand.Parameters["@FIO"].Value = textBox1.Text;
            myCommand.Parameters["@Password"].Value = textBox2.Text;
            try
            {
                mySqlConnection.Open();
                myCommand.ExecuteNonQuery();
                string flag = myCommand.Parameters["@Res"].Value as string;
                if (flag == "0")
                {
                    MessageBox.Show("Ошибка: неверный логин или пароль!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (flag == "1")
                {
                    FormForAdministrator adminForm = new FormForAdministrator();
                    adminForm.Show();
                    this.Hide();
                }
                else if (flag == "2")
                {
                    FormForManager tehnologForm = new FormForManager();
                    tehnologForm.Show();
                    this.Hide();
                }
                else if (flag == "3")
                {
                    int EmployeeId = Convert.ToInt32(myCommand.Parameters["@EmployeeId"].Value);
                    FormForDesigner designerForm = new FormForDesigner(EmployeeId);
                    designerForm.Show();
                    this.Hide();
                }
                else if (flag == "4")
                {
                    MessageBox.Show("Вы уволены!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Неизвестный результат!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                mySqlConnection.Close();
            }
        }

        private void mySqlConnection_InfoMessage(object sender, SqlInfoMessageEventArgs e)
        {

        }
    }
}
