using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DatabaseCreator
{
    public partial class Authencate : Form
    {
        public Authencate()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string admin_password = "@Prabin86501";
            if (textBox1.Text.Trim() == admin_password)
            {
                this.Hide();
                DBADMIN obj_dbadmin = new DBADMIN();
                obj_dbadmin.ShowDialog();
            }
        }
    }
}
