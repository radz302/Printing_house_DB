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
    public partial class ButtonUslugi : Form
    {
        public ButtonUslugi()
        {
            InitializeComponent();
        }

        private void ButtonUslugi_Load(object sender, EventArgs e)
        {
            PrintUslugi();
        }

        private void PrintUslugi()
        {
            sqlConnection1.Open();
            var temp = new DataTable();
            temp.Load(myCommand.ExecuteReader());
            dataGridView1.DataSource = temp;
            sqlConnection1.Close();
        }
    }
}
