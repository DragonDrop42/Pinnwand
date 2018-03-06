using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.Specialized;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FirstFloor.ModernUI.Windows.Controls;
using ServerData;

namespace Test.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        private ModernTab KListe;
        private string kurs;
        private ListDictionary Schüler;
        private ClientClassLib.Client client;
        
        public Home()
        {
            InitializeComponent();
            this.Loaded += OnLoaded;
            //kurs = KListe.mt_Kurse.SelectedSource.Query.;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            KListe = UIHelper.FindVisualChildByName<ModernTab>(Application.Current.MainWindow,"mt_Kurse");
            client = UIHelper.FindVisualParent<MainWindow>(this).client;
            kurs = KListe.SelectedSource.OriginalString.Split(Char.Parse("=")).Last();
            lbl_chatAusgabe.Content += kurs +"\n";

            if (kurs != "Pages/Home.xaml") {
                lbl_chatAusgabe.Content += sender.ToString() + "\n";
                Packet kidp = client.SendAndWaitForResponse(PacketType.GetKurse);
                int K_ID = Convert.ToInt32( ((List<string>)kidp.Data["K_ID"])[((List<string>)kidp.Data["K_Name"]).IndexOf(kurs)]);
                ListDictionary data = new ListDictionary{{"K_ID",K_ID}};
                Packet p = client.SendAndWaitForResponse(PacketType.GetSchülerInKurs, data);
                if (p.Success)
                {
                    Schüler = p.Data;
                    schülerLaden((List<string>)Schüler["S_Vorname"]);
                }
                else { MessageBox.Show(p.MessageString); }
            }
        }

        #region Mitschüler liste erstellen
        public void schülerLaden( List<string> schüler)
        {
            foreach (string schülername in schüler)
            {
                StackPanel stack_schüler = new StackPanel { Margin = new Thickness(5) };
                Label lbl_schüler = new Label { FontSize = 17 };
                lbl_schüler.Content = schülername;


                stack_Mitschüler.Children.Add(stack_schüler);
                stack_schüler.Children.Add(lbl_schüler);
            }
        }
        #endregion

        public void chatLaden( List<string> chats)
        {
            lbl_chatAusgabe.Content = "";
            foreach (string nachricht in chats)
            {
                lbl_chatAusgabe.Content += nachricht + "\n";
            }
        }

        private void txt_chatEingabe_GotFocus(object sender, RoutedEventArgs e) //leeren des Placeholders in der Leiste
        {
            txt_chatEingabe.Text = "";
        }

        private void cmd_senden_Click(object sender, RoutedEventArgs e) // Ausführen wenn der klick auf senden ausgeführt wird
        {
            lbl_chatAusgabe.Content += txt_chatEingabe.Text + "\n";
            ereignisseErstellen();
        }


        private void ereignisseErstellen()
        {
            // Verlauf der Farbe erstellen
            LinearGradientBrush gradientBrush = new  LinearGradientBrush();
            gradientBrush.StartPoint = new Point(0.5, 0);
            gradientBrush.EndPoint = new Point(0.5, 1);
            gradientBrush.GradientStops.Add(new GradientStop(Color.FromArgb(255, 178, 178, 178), 0));
            gradientBrush.GradientStops.Add(new GradientStop(Color.FromArgb(255, 38, 139, 185), 1));

            StackPanel stack_ereignis = new StackPanel {Width = 150 , Height = 70, Background = gradientBrush };

            Label lbl_namenEreignis = new Label {HorizontalContentAlignment = HorizontalAlignment.Center ,  FontSize= 17 , Margin = new Thickness(0,10,0,5), Foreground = Brushes.Black };
            lbl_namenEreignis.Content = "Hallo";    // Name des neuen Ereignisses

            Label lbl_datumEreignis = new Label {HorizontalContentAlignment = HorizontalAlignment.Center, FontSize = 17, Foreground = Brushes.Black};
            lbl_datumEreignis.Content = "02.10.1998";   // Datum des neuen Ereignisses

            // Anzeigen auf der Pinnwand
            ug_terminÜbersicht.Children.Add(stack_ereignis);
            stack_ereignis.Children.Add(lbl_namenEreignis);
            stack_ereignis.Children.Add(lbl_datumEreignis);

            //Hinzufügen zur Datenbank

        }




    }
}
