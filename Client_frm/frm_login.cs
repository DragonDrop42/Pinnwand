using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//Eigene Bibliotheken
using ServerData;
using ClientClassLib;
using System.Collections.Specialized;

namespace Client_frm
{
    public partial class frm_login : Form
    {
        private Client client;
        private frm_register register;

        public frm_login(object c, frm_register _register)
        {
            InitializeComponent();
            client = (Client)c;
            register = _register;
        }

        //Login+++++++++++++++++++++++++++++++++++++++++++++++++++++++
        private void cmd_login_Click(object sender, EventArgs e)
        {
            ListDictionary list = new ListDictionary();
            list.Add("email", txt_email.Text);
            list.Add("password", txt_pass.Text);

        }

        private void cmd_register_Click(object sender, EventArgs e)
        {
            register.Show();
            this.Close();
        }
    }
}
