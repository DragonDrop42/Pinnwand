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
    public partial class TerminErstellen
    {
        int K_ID;
        
        private CultureInfo ci = new CultureInfo("de-DE");
        private MainWindow mw = (MainWindow)Application.Current.MainWindow;
        private int E_ID = -1;

        public TerminErstellen(int K_ID)
        {
            InitializeComponent();
            datenInit();
            this.K_ID = K_ID;
        }

        public TerminErstellen(int K_ID,int E_ID, string E_Art, DateTime E_Fälligkeitsdatum,string E_Beschreibung) : this(K_ID)
        {
            this.E_ID = E_ID;
            txt_terminName.Text = E_Art;
            cb_terminDatumTag.SelectedValue = E_Fälligkeitsdatum.Day;
            cb_terminDatumMonat.SelectedValue = E_Fälligkeitsdatum.Month;
            cb_terminDatumJahr.SelectedValue = E_Fälligkeitsdatum.Year;
            txt_terminBeschreibung.Text = E_Beschreibung;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ListDictionary data = new ListDictionary{
                {"K_ID",K_ID},
                {"E_Art", txt_terminName.Text},
                {"E_Fälligkeitsdatum", Convert.ToDateTime(cb_terminDatumTag.SelectedValue + "." + 
                                                          cb_terminDatumMonat.SelectedValue + "." + 
                                                          cb_terminDatumJahr.SelectedValue)},
                {"E_Beschreibung", txt_terminBeschreibung.Text},
                {"E_ID",E_ID}
            };
            ///Senden+++++++++++++++++++++++++++++++++++++++++++++

            Packet args;

            if (E_ID == -1)
            {
                args = mw.client.SendAndWaitForResponse(PacketType.SendEreigniss, data);
            }
            else
            {
                args = mw.client.SendAndWaitForResponse(PacketType.EditEreigniss, data);
            }

            Debug.Content = args.MessageString;
            mw.CurrentKurs.reload_Ereignisse();
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
