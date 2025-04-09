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
    public partial class ButtonMaterial : Form
    {
        public ButtonMaterial()
        {
            InitializeComponent();
        }

        private void ButtonMaterial_Load(object sender, EventArgs e)
        {
            sqlConnection1.Open();
            var temp = new DataTable();
            temp.Load(myCommand.ExecuteReader());
            dataGridView1.DataSource = temp;
            sqlConnection1.Close();
        }
    }
}
