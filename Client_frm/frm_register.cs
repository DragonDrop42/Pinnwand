using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//
using ClientClassLib;
using System.Collections.Specialized;

namespace Client_frm
{
    public partial class frm_register : Form
    {
        Client client;
        public frm_register(object o)
        {
            InitializeComponent();
            client = (Client)o;

            client.GetKlassen();
        }

        public void LoadKlassen(ListDictionary klassen)
        {
            cKlassen.Items.Clear();
            List<string> K_name = (List<string>)klassen["Kl_Name"];

            MessageBox.Show("" + K_name.Count());

            for (int i = 0; i < K_name.Count; i++)
            {
                cKlassen.Items.Add(K_name[i]);
            }
        }

        private void cmd_register_Click(object sender, EventArgs e)
        {
            ListDictionary list = new ListDictionary();
            list.Add("name", txt_name.Text);
            list.Add("vname", txt_vName.Text);
            list.Add("phone", txt_tel.Text);
            list.Add("email", txt_newEmail.Text);
            list.Add("password", txt_newPass.Text);


        }
    }
}
