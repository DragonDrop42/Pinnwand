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

namespace Test.Pages.ExtraPage
{
    /// <summary>
    /// Interaction logic for TerminErstellen.xaml
    /// </summary>
    public partial class TerminErstellen : UserControl
    {

        
        private CultureInfo ci = new CultureInfo("de-DE");

        public TerminErstellen()
        {
            InitializeComponent();
            datenInit();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string terminname;
            string termindatum;
            string terminbeschreibung;

            terminname = txt_terminName.Text;
            termindatum = (string)cb_terminDatumTag.SelectedValue + "." + (string)cb_terminDatumMonat.SelectedValue + "." + (string)cb_terminDatumJahr.SelectedValue;
            terminbeschreibung = txt_terminBeschreibung.Text;

            ListDictionary data = new ListDictionary{
                {"E_Art", terminname},
                {"E_Fälligkeitsdatum", termindatum},
                {"E_Beschreibung", terminbeschreibung}
            };
            ///Senden+++++++++++++++++++++++++++++++++++++++++++++
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
