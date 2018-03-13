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
using ClientClassLib;
using ServerData;

namespace Pinnwand.Pages.Settings
{
    /// <summary>
    /// Interaction logic for KursAuswahl.xaml
    /// </summary>
    public partial class Kurswahl : UserControl
    {
        private ListDictionary Kurse;
        private MainWindow mw;
        private ListDictionary Lehrer;
        private ListDictionary Klassen;
        
        public Kurswahl()
        {
            mw = (MainWindow)Application.Current.MainWindow;
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (!((MainWindow)Application.Current.MainWindow).hasRights)
            {
                 Lehrerptions.Children.Clear();   
            }
            reload_kurse();
            cmd_save.Click += cmd_save_Click;
            cb_klassen.DropDownOpened += (o, a) =>
            {
                Packet klassen = mw.client.SendAndWaitForResponse(PacketType.Klassenwahl);
                if (klassen.Success)
                {
                    Klassen = klassen.Data;
                    List<string> lst_data = (List<string>) klassen.Data["Kl_Name"];
                    List<string> ids = (List<string>) klassen.Data["Kl_ID"];
                    cb_klassen.Items.Clear();

                    for (var i = 0; i < lst_data.Count; i++)
                    {
                        cb_klassen.Items.Add(lst_data[i]+"#"+Math.Abs(Convert.ToInt32(ids[i])));
                    }
                }
                else
                {
                    MessageBox.Show(
                        "Klassen konnten nicht geladen werden:\n" + klassen.MessageString);
                }
            };
            cb_lehrer.DropDownOpened += (o, a) => { 
                Packet lehrer = mw.client.SendAndWaitForResponse(PacketType.GetLehrer);
                if (lehrer.Success)
                {
                    Lehrer = lehrer.Data;
                    List<string> lst_data = (List<string>) lehrer.Data["L_Name"];
                    List<string> ids = (List<string>) lehrer.Data["L_ID"];
                    cb_lehrer.Items.Clear();

                    for (var i = 0; i < lst_data.Count; i++)
                    {
                        cb_lehrer.Items.Add(lst_data[i]+"#"+ids[i]);
                    }
                }
                else
                {
                    MessageBox.Show(
                        "Klassen konnten nicht geladen werden:\n" + lehrer.MessageString);
                }};
            cmd_addKlasse.Click += (o, a) =>
            {
                Packet KlasseErstellen = mw.client.SendAndWaitForResponse(PacketType.KlasseErstellen, new ListDictionary
                {
                    {"Kl_Name", txt_Klassenname.Text},
                    {"Kl_Abschlussdatum", dp_Abschlussdatum.SelectedDate}
                });
                MessageBox.Show(KlasseErstellen.Success
                    ? "Klasse Erfolgreich hinzugefügt!"
                    : KlasseErstellen.MessageString);
            };
            cmd_addKurs.Click += (o, a) =>
            {
                Packet KursErstellen = mw.client.SendAndWaitForResponse(PacketType.CreateKurs, new ListDictionary
                {
                    {"K_Name", txt_Kursname.Text},
                    {"L_ID", Convert.ToInt32(cb_lehrer.SelectedValue.ToString().Split('#').Last())},
                    {"Kl_ID", Convert.ToInt32(cb_klassen.SelectedValue.ToString().Split('#').Last())}
                });
                mw.Fehler_Ausgabe(KursErstellen.Success
                    ? "Kurs Erfolgreich hinzugefügt!"
                    : KursErstellen.MessageString);
                lbl_bestätigung.Content = KursErstellen.Success
                    ? "Kurs Erfolgreich hinzugefügt!"
                    : KursErstellen.MessageString;
                reload_kurse();
            };
        }

        private void reload_kurse()
        {
            try
            {
                Packet kurse = mw.client.SendAndWaitForResponse(PacketType.GetAlleKurse);
                Packet getKurse = mw.client.SendAndWaitForResponse(PacketType.GetGewählteKurse);
                if (kurse.Success && getKurse.Success)
                {
                    UpdateKurse(kurse.Data,getKurse.Data);
                }
                else
                {
                    throw new Exception (kurse.MessageString+" "+getKurse.MessageString);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void cmd_save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<string> k = GetChecked();
                if (k.Count == 0) throw new Exception("Bitte mindestens einen Kurs auswählen.");
                ListDictionary data = new ListDictionary
                {
                    {"K_ID",k}
                };
                Packet kursUpdate = mw.client.SendAndWaitForResponse(PacketType.KursUpdate, data);
                if (!kursUpdate.Success) throw new Exception(kursUpdate.MessageString);
                mw.Reload_Kurse();
                throw new Exception("Erfolgreich gespeichert");
            }
            catch (Exception ex)
            {
                lbl_Kurswahl_Error.Text = ex.Message;
            }
        }

        public void UpdateKurse(ListDictionary Kurse,ListDictionary CheckedKurse)
        {
            this.Kurse = Kurse;
            lv_Kurse.Items.Clear();
            for (int i = 0; i < ((List<string>)Kurse["K_ID"]).Count; i++)
            {
                lv_Kurse.Items.Add(new CheckBox
                {
                    Content = ((List<string>) Kurse["K_Name"])[i],
                    IsChecked = ((List<string>) CheckedKurse["K_Name"]).Contains(((List<string>) Kurse["K_Name"])[i])
                });
            }
        }
        public List<string> GetChecked()
        {
            List<string> IDS = new List<string>();
            for (int i = 0; i < ((List<string>) Kurse["K_ID"]).Count; i++)
            {
                IDS.AddRange(from CheckBox hcb in lv_Kurse.Items where (bool) hcb.IsChecked && hcb.Content == ((List<string>) Kurse["K_Name"])[i] select ((List<string>) Kurse["K_ID"])[i]);
            }
            return IDS;
        }
    }
}
