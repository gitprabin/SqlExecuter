/*
 *@author : Prabin Siwakoti 
 *@email  : developer.prabin@gmail.com
 *@license : own
 *@description : Used to create, drop, view , backup and restored database for sql server
 *@date : 2016-10-16
 *@copyright : @author
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DatabaseCreator
{
    public partial class DBADMIN : Form
    {
        string connectionString = "";

        public DBADMIN()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // your connection string
            //string connectionString = "Server=PRABIN\\PRABIN;Database=master;Integrated Security=True";

            setConnectionString();

            // your query:
            var query = GetDbCreationQuery(textBox1.Text.Trim());
            if (query.Equals(""))
            {
                MessageBox.Show("Database name is not valid."); return;
            }

            var conn = new SqlConnection(connectionString);
            var command = new SqlCommand(query, conn);

            try
            {
                conn.Open();
                command.ExecuteNonQuery();

                LoadDatabase();

                MessageBox.Show("Database (" + textBox1.Text + ") is created successfully", "MyProgram",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        static string GetDbCreationQuery(string dbName, string query_for = "create")
        {
            if (dbName.Equals("master") || dbName.Equals(""))
                return "";
            // db creation query
            string query = "CREATE DATABASE " + dbName + ";";

            //drop
            if (query_for.Equals("drop"))
            {
                query = "ALTER Database " + dbName + " SET SINGLE_USER WITH ROLLBACK IMMEDIATE;";
                query += "IF EXISTS(select * from sys.databases where name='" + dbName + "') DROP DATABASE " + dbName + "";
            }

            return query;
        }


        //drop
        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure to delete?", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
                return;
            // your connection string
            //string connectionString = "Server=PRABIN\\PRABIN;Database=master;Integrated Security=True";
            setConnectionString();

            // your query:
            var query = GetDbCreationQuery(textBox1.Text.Trim(), "drop");
            if (query.Equals(""))
            {
                MessageBox.Show("Database name is not valid."); return;
            }

            var conn = new SqlConnection(connectionString);
            var command = new SqlCommand(query, conn);

            try
            {
                conn.Open();
                command.ExecuteNonQuery();

                LoadDatabase();

                MessageBox.Show("Database(" + textBox1.Text + ") is drop successfully", "MyProgram",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void Form1_Load(object sender, EventArgs e)
        {
            //LoadDatabase();
        }

        private void LoadDatabase()
        {
            // your connection string
            //string connectionString = "Server=PRABIN\\PRABIN;Database=master;Integrated Security=True";
            setConnectionString();

            // your query:
           // var query = "select * from sys.databases";
            var query = "SELECT * FROM sys.databases d WHERE d.database_id>4;";

            var conn = new SqlConnection(connectionString);
            var command = new SqlCommand(query, conn);

            try
            {
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter(command);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();
                dataGridView1.Columns.Add("name", "Name");
                foreach (DataRow dr in dt.Rows)
                {
                    dataGridView1.Rows.Add(dr["name"].ToString());
                }


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

        private void button3_Click(object sender, EventArgs e)
        {
            LoadDatabase();
        }

        private void btnBackup_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string path = dlg.SelectedPath.ToString();
                try
                {
                    if (textBox1.Text.Equals(""))
                    {
                        MessageBox.Show("Please select a Database.");
                        return;
                    }

                    setConnectionString();

                    var conn = new SqlConnection(connectionString);
                    conn.Open();
                    string sql = "BACKUP DATABASE " + textBox1.Text.Trim() + " TO DISK ='" + path + "\\" + textBox1.Text.Trim() + "-" + DateTime.Now.Ticks.ToString() + ".bak'";
                    SqlCommand command = new SqlCommand(sql, conn);
                    command.ExecuteNonQuery();
                    conn.Close();
                    conn.Dispose();
                    MessageBox.Show("Successfully Database Backup Completed.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
           // dlg.Filter = "Backup Filed(*.bak)|*.bak|All Files(*.*)|*.* ";
           // dlg.FilterIndex = 0;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string file_location = dlg.FileName.ToString();
                //restore
                try
                {
                    if (textBox1.Text.Trim().Equals(""))
                    {
                        MessageBox.Show("Please select a Database.");
                        return;
                    }

                    setConnectionString();

                    SqlConnection conn = new SqlConnection(connectionString);
                    conn.Open();

                    //string sql = "ALTER Database " + textBox1.Text.Trim() + " SET SINGLE_USER WITH ROLLBACK IMMEDIATE;";
                    string sql = "Restore Database " + textBox1.Text.Trim() + " FROM DISK ='" + file_location + "' WITH REPLACE;";

                    SqlCommand command = new SqlCommand(sql, conn);
                    command.ExecuteNonQuery();
                    conn.Close();
                    conn.Dispose();

                    MessageBox.Show("Successfully Restore Database.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SqlRunner srun = new SqlRunner();
            srun.db_name = textBox1.Text.Trim();
            setConnectionString();
            srun.connectionString = connectionString;
            srun.ShowDialog();
        }

        public void setConnectionString()
        {
            connectionString = connectionString = "Server=" + txtConnectionString.Text.Trim() + ";Database=master;Integrated Security=True";
        }
    }
}
