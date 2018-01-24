using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ClientClassLib;
using System.Collections.Specialized;

namespace Client_frm
{
    public partial class frm_Kurswahl : Form
    {
        Client client;
        ListDictionary list;

        public frm_Kurswahl(Client _client, ListDictionary _list)
        {
            InitializeComponent();

            client = _client;
            list = _list;

            LoadData();
        }

        private void LoadData()
        {
            String[] keys = new String[list.Count];
            list.Keys.CopyTo(keys, 0);

            cLst_kurse.Items.Add("amzahkl " + list.Count);

            List<string> K_name = (List<string>)list["K_Name"];
            List<string> L_name = (List<string>)list["L_ID"];

            for (int i = 0; i < K_name.Count; i++)
            {
                 cLst_kurse.Items.Add(K_name[i] + " L_ID " + L_name[i]);
            }
        }

        private void cmd_senden_Click(object sender, EventArgs e)
        {

        }
    }
}
