/*
     * Class: SqlRunner.cs
     * Description: It is used to run sql query , specially used by developer not for normal users.
     * Created By: Prabin Siwakoti
     * Created Date: Oct 16, 2016
     * ---------------
 */
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

namespace DatabaseCreator
{
    public partial class SqlRunner : Form
    {
        public SqlRunner()
        {
            InitializeComponent();
        }

        string query = "";
        public string db_name = "";

        public string connectionString = "";
       

        private void SqlRunner_Load(object sender, EventArgs e)
        {
            connectionString += ";Initial Catalog = " + db_name + "";
            dataGridView1.ReadOnly = true;
        }

        private void btnGetAllTable_Click(object sender, EventArgs e)
        {
            if (tbQuery.Text.Trim() == "")
                fillDatagridView();
            else
                fillDatagridView(sql: tbQuery.SelectedText.Trim());
        }

        private void fillDatagridView(string table_name = "", string sql = "")
        {
            try
            {
                query = "SELECT distinct * FROM "+db_name+".INFORMATION_SCHEMA.Tables WHERE TABLE_TYPE != 'VIEW';";
                if (table_name != "")
                    query = "SELECT distinct * FROM "+db_name+".INFORMATION_SCHEMA.Tables WHERE TABLE_NAME LIKE '%" + table_name + "%'";

                if (sql != "")
                    query = sql;

                var conn = new SqlConnection(connectionString);
                var command = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    DataTable dtt = new DataTable();
                    da.Fill(dtt);
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = dtt;
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    if ((conn.State == ConnectionState.Open))
                    {
                        conn.Close();
                    }
                }

               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            fillDatagridView(textBox1.Text.Trim());
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F5)
            {
                if (tbQuery.Text.Trim() == "")
                    fillDatagridView();
                else
                    fillDatagridView(sql: tbQuery.SelectedText.Trim());
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
