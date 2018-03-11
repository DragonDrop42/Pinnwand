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

namespace Test.Pages.Settings
{
    /// <summary>
    /// Interaction logic for KursAuswahl.xaml
    /// </summary>
    public partial class Kurswahl : UserControl
    {
        private ListDictionary Kurse;
        
        public Kurswahl()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (!UIHelper.FindVisualParent<MainWindow>(this).hasRights)
            {
                 Lehrerptions.Children.Clear();   
            }
            
            cmd_refresh.Click += Cmd_Refresh_Click;
            cmd_save.Click += cmd_save_Click;
        }
        private void cmd_save_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = UIHelper.FindVisualParent<MainWindow>(this);
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
        private void Cmd_Refresh_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            Client client = UIHelper.FindVisualParent<MainWindow>(this).client;
            try
            {
                Packet kurse = client.SendAndWaitForResponse(PacketType.GetAlleKurse);
                Packet getKurse = client.SendAndWaitForResponse(PacketType.GetGewählteKurse);
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
