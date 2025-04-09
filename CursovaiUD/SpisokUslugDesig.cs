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
    public partial class SpisokUslugDesig : Form
    {
        int zakazId;
        public SpisokUslugDesig()
        {
            InitializeComponent();
        }

        public SpisokUslugDesig(int id)
        {
            InitializeComponent();
            zakazId = id;
        }

        private void SpisokUslugDesig_Load(object sender, EventArgs e)
        {
            PrintUslugi();

        }
        private void PrintUslugi()
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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
