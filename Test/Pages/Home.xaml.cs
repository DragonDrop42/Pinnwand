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
using System.Globalization;

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
        private int K_ID;
        private Button letzterAktiverButton;
        
        
        public Home()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            //kurs = KListe.mt_Kurse.SelectedSource.Query.;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            KListe = UIHelper.FindVisualChildByName<ModernTab>(Application.Current.MainWindow,"mt_Kurse");
            client = UIHelper.FindVisualParent<MainWindow>(this).client;
            kurs = KListe.SelectedSource.OriginalString.Split(Char.Parse("=")).Last();

            if (kurs != "Pages/Home.xaml") {
                Packet kidp = client.SendAndWaitForResponse(PacketType.GetKurseVonSchüler);
                K_ID = Convert.ToInt32( ((List<string>)kidp.Data["K_ID"])[((List<string>)kidp.Data["K_Name"]).IndexOf(kurs)]);
                Packet SchülerPacket = client.SendAndWaitForResponse(
                    PacketType.GetSchülerInKurs,
                    new ListDictionary
                    {
                        {"K_ID",Math.Abs(K_ID)}
                    }
                    );
                if (SchülerPacket.Success)
                {
                    Mitschüler_box.Text = "";
                    for (int i = 0; i < ((List<string>)SchülerPacket.Data["S_ID"]).Count; i++)
                    {
                        Mitschüler_box.Text += ((List<string>) SchülerPacket.Data["S_Vorname"])[i] + " " +
                                               ((List<string>) SchülerPacket.Data["S_Name"])[i] + "\n";
                    }
                }
                else { MessageBox.Show(SchülerPacket.MessageString); }

                Packet ChatPacket =
                    client.SendAndWaitForResponse(
                        PacketType.GetChat,
                        new ListDictionary
                        {
                            {"K_ID", K_ID}
                        });
                if (ChatPacket.Success)
                {
                    for (int i = 0; i < ((List<string>)ChatPacket.Data["C_ID"]).Count; i++)
                    {
                        lbl_chatAusgabe.Content += ((List<string>) ChatPacket.Data["C_Sendername"])[i] + ": " +
                                                   ((List<string>) ChatPacket.Data["C_Inhalt"])[i] + "\n";
                    }
                }
                else
                {
                    MessageBox.Show(ChatPacket.MessageString);
                }
            }
        }

        private void txt_chatEingabe_GotFocus(object sender, RoutedEventArgs e) //leeren des Placeholders in der Leiste
        {
            txt_chatEingabe.Text = "";
        }

        private void cmd_senden_Click(object sender, RoutedEventArgs e) // Ausführen wenn der klick auf senden ausgeführt wird
        {
            //lbl_chatAusgabe.Content += txt_chatEingabe.Text + "\n";
            Packet chatsendpacket = client.SendAndWaitForResponse(PacketType.SendChatNachricht,
                new ListDictionary
                {
                    {"K_ID",K_ID},
                    {"C_Sendername", "Mehritz"},
                    {"C_Inhalt", txt_chatEingabe.Text}
                });
            if (!chatsendpacket.Success)
            {
                MessageBox.Show("Senden Fehlgeschlagen: " + chatsendpacket.MessageString);
            }
            
        }


        

        private void terminHinzufügen_Click(object sender, RoutedEventArgs e)
        {
            frame_informationsausgabe.Navigate(new ExtraPage.TerminErstellen(client,K_ID));

            // Hinzufügen eines neuen Termins
            // Einfügen einer Page an die unten rechte Stelle !!!!!


            //Aktualisierung der Datenbank mit dem neuen Termin
        }


        #region Erstellen eines Ereignisses auf der eigenen Pinnwand
        private void ereignisseErstellen()
        {
            // Verlauf der Farbe erstellen
            LinearGradientBrush gradientBrush = new LinearGradientBrush();
            gradientBrush.StartPoint = new Point(0.5, 0);
            gradientBrush.EndPoint = new Point(0.5, 1);
            gradientBrush.GradientStops.Add(new GradientStop(Color.FromArgb(255, 178, 178, 178), 0));
            gradientBrush.GradientStops.Add(new GradientStop(Color.FromArgb(255, 38, 139, 185), 1));

            Button cmd_ereignis = new Button { Width = 150, Height = 70, Background = gradientBrush, Content="Hallo" };
            cmd_ereignis.Click += new RoutedEventHandler(aktiverButton);

            StackPanel stack_ereignis = new StackPanel();

            Label lbl_namenEreignis = new Label { HorizontalContentAlignment = HorizontalAlignment.Center, FontSize = 17, Margin = new Thickness(0, 10, 0, 5), Foreground = Brushes.Black };
            lbl_namenEreignis.Content = "Hallo";    // Name des neuen Ereignisses

            Label lbl_datumEreignis = new Label { HorizontalContentAlignment = HorizontalAlignment.Center, FontSize = 17, Foreground = Brushes.Black };
            lbl_datumEreignis.Content = "02.10.1998";   // Datum des neuen Ereignisses

            // Anzeigen auf der Pinnwand
            ug_terminÜbersicht.Children.Add(cmd_ereignis);
            
            stack_ereignis.Children.Add(lbl_namenEreignis);
            stack_ereignis.Children.Add(lbl_datumEreignis);

        }
        #endregion

        private void terminLöschen_Click(object sender, RoutedEventArgs e)
        {
            
        }

        // Wird von jedem Button aufgerufen wenn er geklickt wird --> Übergabe des Names
        private void aktiverButton(object sender, RoutedEventArgs e)
        {
            // Aufrufen der Information über das Ereigniss identifizierne aus der DB mittels des einer ID?????
            foreach (Button cmd in ug_terminÜbersicht.Children)
            {
                cmd.Background = getFarbverlauf(1);
            }            

            Button aktiverButton = (Button)sender;
            aktiverButton.Background = getFarbverlauf(2);
            frame_informationsausgabe.Content = null;
            frame_informationsausgabe.Navigate( new ExtraPage.TerminInformation());

            letzterAktiverButton = (Button)sender;  // Zum speichern des letzten gedrückten Buttons
        }

        private void buttonHinzufügen(object sender, RoutedEventArgs e)
        {
            ereignisseErstellen();
        }

        //Farben
        private GradientBrush getFarbverlauf(int zahl)
        {
            LinearGradientBrush gradientBrush = new LinearGradientBrush();
            switch (zahl)
            {
                case 1:             
                    gradientBrush.StartPoint = new Point(0.5, 0);
                    gradientBrush.EndPoint = new Point(0.5, 1);
                    gradientBrush.GradientStops.Add(new GradientStop(Color.FromArgb(255, 178, 178, 178), 0));
                    gradientBrush.GradientStops.Add(new GradientStop(Color.FromArgb(255, 38, 139, 185), 1));           
                    break;
                case 2:
                    gradientBrush.StartPoint = new Point(0.5, 0);
                    gradientBrush.EndPoint = new Point(0.5, 1);
                    gradientBrush.GradientStops.Add(new GradientStop(Color.FromArgb(200, 178, 178, 178), 0));
                    gradientBrush.GradientStops.Add(new GradientStop(Color.FromArgb(200, 38, 139, 185), 1)); 
                    break;
                default:
                    break;
            }

            return gradientBrush;

        }

    }
}
