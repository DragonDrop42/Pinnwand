using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ServerData;

namespace Pinnwand.Pages.ExtraPage
{
    /// <summary>
    /// Interaction logic for TerminErstellen.xaml
    /// </summary>
    public partial class TerminErstellen : UserControl
    {
        private ClientClassLib.Client client;
        int K_ID;
        
        private CultureInfo ci = new CultureInfo("de-DE");

        public TerminErstellen(ClientClassLib.Client client,int K_ID)
        {
            InitializeComponent();
            datenInit();
            this.client = client;
            this.K_ID = K_ID;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string terminname;
            DateTime termindatum;
            string terminbeschreibung;

            terminname = txt_terminName.Text;
            termindatum = Convert.ToDateTime(cb_terminDatumTag.SelectedValue.ToString() + "." + cb_terminDatumMonat.SelectedValue.ToString() + "." + cb_terminDatumJahr.SelectedValue.ToString());
            terminbeschreibung = txt_terminBeschreibung.Text;

            ListDictionary data = new ListDictionary{
                {"K_ID",K_ID},
                {"E_Art", terminname},
                {"E_Autor",null},
                {"E_Fälligkeitsdatum", termindatum},
                {"E_Beschreibung", terminbeschreibung}
            };
            ///Senden+++++++++++++++++++++++++++++++++++++++++++++

            Packet args = client.SendAndWaitForResponse(PacketType.SendEreigniss, data);
            MessageBox.Show(args.Success + " " + args.MessageString);
        }

        private void datenInit()
        {
            for (int i = 0; i < 31; i++)
            {
                cb_terminDatumTag.Items.Add(i + 1);
            }
            for (int i = 0; i < 12; i++)
            {
                cb_terminDatumMonat.Items.Add(ci.DateTimeFormat.MonthNames[i]);
            }
            for (int i = DateTime.Now.Year; i < DateTime.Now.Year + 60; i++)
            {
                cb_terminDatumJahr.Items.Add(i);
            }
            cb_terminDatumTag.SelectedIndex = 0;
            cb_terminDatumMonat.SelectedIndex = 0;
            cb_terminDatumJahr.SelectedIndex = 0;
        }
    }
}
