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
using Pinnwand.Pages.ExtraPage;
using System.Threading;
using System.Windows.Threading;

namespace Pinnwand.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        private ModernTab KListe;
        private string kurs;
        private ListDictionary Schüler;
        private int K_ID;
        private Button letzterAktiverButton;
        private MainWindow mw = (MainWindow) Application.Current.MainWindow;

        public Home()
        {
            InitializeComponent();
        }

        public void reload_Liste()
        {
            Packet SchülerPacket;
            if (kurs != "all")
            {
                ((MainWindow) Application.Current.MainWindow).client.ChatUpdate += (sender, args) => InvokeIfNecessary(args); ;
                lbl_Schülerliste.Content = "Schülerliste " + kurs;
                SchülerPacket = mw.client.SendAndWaitForResponse(
                    PacketType.GetSchülerInKurs,
                    new ListDictionary
                    {
                        {"K_ID", Math.Abs(K_ID)}
                    }
                );
            }
            else
            {
                if(mw.hasRights) return;
                Packet Klassenpacket = mw.client.SendAndWaitForResponse(PacketType.GetKlasse);
                if (Klassenpacket.Success)
                {
                    lbl_Schülerliste.Content = "Schülerliste " + ((List<string>) Klassenpacket.Data["Kl_Name"])[0];
                }
                else
                {
                    MessageBox.Show(Klassenpacket.MessageString);
                }
                SchülerPacket = mw.client.SendAndWaitForResponse(
                    PacketType.GetSchülerInKlasse,
                    new ListDictionary
                    {
                        {"Kl_ID", Math.Abs(Convert.ToInt32(((List<string>)Klassenpacket.Data["Kl_ID"])[0]))}
                    }
                );
            }

            if (SchülerPacket.Success)
            {
                Mitschüler_box.Text = "";
                for (int i = 0; i < ((List<string>) SchülerPacket.Data["S_ID"]).Count; i++)
                {
                    Mitschüler_box.Text += ((List<string>) SchülerPacket.Data["S_Vorname"])[i] + " " +
                                           ((List<string>) SchülerPacket.Data["S_Name"])[i] + "\n";
                }
            }
            else { MessageBox.Show(SchülerPacket.MessageString); }
        }

        public void reload_Ereignisse()
        {
            Packet EreignissPacket = mw.client.SendAndWaitForResponse(PacketType.GetEreignisse);
                
            if (EreignissPacket.Success)
            {
                ug_terminÜbersicht.Children.Clear();
                for (int i = 0; i < ((List<string>)EreignissPacket.Data["K_ID"]).Count; i++)
                {
                    string t = ((List<string>) EreignissPacket.Data["K_ID"])[i];
                    if (((List<string>) EreignissPacket.Data["K_ID"])[i] == K_ID.ToString() || kurs == "all")
                    {
                        string E_Art = ((List<string>) EreignissPacket.Data["E_Art"])[i];
                        DateTime E_Fälligkeitsdatum =
                            Convert.ToDateTime(((List<string>) EreignissPacket.Data["E_Fälligkeitsdatum"])[i]);
                        string E_Inhalt = ((List<string>) EreignissPacket.Data["E_Beschreibung"])[i];
                        string E_Autor = ((List<string>) EreignissPacket.Data["E_Autor"])[i];

                        ereignisseErstellen(E_Art, E_Fälligkeitsdatum, E_Inhalt, E_Autor);
                    }
                }
            }
            else
            {
                MessageBox.Show(EreignissPacket.MessageString);
            }
        }
        //syncronise Threads
        public void InvokeIfNecessary(Packet packet)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                reload_Chat(packet);
            }
            else
            {
                Application.Current.Dispatcher.BeginInvoke(
                  DispatcherPriority.Background,
                  new Action(() => {
                      reload_Chat(packet);
                  }));
            }
        }

        public void reload_Chat(Packet ChatPacket)
        {
            if (ChatPacket.Success)
            {
                lbl_chatAusgabe.Content = "";
                for (int i = 0; i < ((List<string>) ChatPacket.Data["C_ID"]).Count; i++)
                {
                    if (((List<string>) ChatPacket.Data["K_ID"])[i] == K_ID.ToString())
                    {
                        lbl_chatAusgabe.Content += ((List<string>) ChatPacket.Data["C_Sendername"])[i] + ": " +
                                                   ((List<string>) ChatPacket.Data["C_Inhalt"])[i] + "\n";
                    }
                }
            }
            else
            {
                Console.WriteLine(ChatPacket.MessageString);
            }
        }
        
        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
                kurs = mw._kursliste.mt_Kurse.SelectedSource.OriginalString.Split(Char.Parse("=")).Last();
                mw.LoginFrm.Closed += (o, args) => { OnLoaded(o, new RoutedEventArgs()); };
                ((MainWindow)Application.Current.MainWindow).CurrentKurs = this;

                if (kurs == "Pages/Home.xaml")
                {
                    stack_Chat.Children.Clear();
                    //idontevencareanymore.Children.Clear();
                    kurs = "all";
                }
                else
                {
                    Packet kidp = mw.client.SendAndWaitForResponse(PacketType.GetGewählteKurse);
                    K_ID = Math.Abs(Convert.ToInt32(
                        ((List<string>) kidp.Data["K_ID"])[((List<string>) kidp.Data["K_Name"]).IndexOf(kurs)]));
                    reload_Chat(mw.client.SendAndWaitForResponse(PacketType.GetChat));
                }
            
            if (!mw.LoginFrm.IsVisible)
            {
                reload_Liste();
                reload_Ereignisse();
            }
        }

        private void txt_chatEingabe_GotFocus(object sender, RoutedEventArgs e) //leeren des Placeholders in der Leiste
        {
            txt_chatEingabe.Text = "";
        }

        private void cmd_senden_Click(object sender, RoutedEventArgs e) // Ausführen wenn der klick auf senden ausgeführt wird
        {
            //lbl_chatAusgabe.Content += txt_chatEingabe.Text + "\n";
            Packet chatsendpacket = mw.client.SendAndWaitForResponse(PacketType.SendChatNachricht,
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
            frame_informationsausgabe.Navigate(new TerminErstellen(mw.client, K_ID));
        }

        private void ereignisseErstellen(string Art, DateTime Datum, string Inhalt, string Autor)
        {
            Termin cmd_termin = new Termin(Art, Datum, Inhalt, Autor);
            cmd_termin.Click += aktiverButton;
            ug_terminÜbersicht.Children.Add(cmd_termin);
        }
        
        // Wird von jedem Button aufgerufen wenn er geklickt wird --> Übergabe des Names
        private void aktiverButton(object sender, RoutedEventArgs e)
        {
            // Aufrufen der Information über das Ereigniss identifizierne aus der DB mittels des einer ID?????
            foreach (Button cmd in ug_terminÜbersicht.Children)
            {
                cmd.Background = getFarbverlauf(1);
            }            

            Termin aktiverButton = (Termin)sender;
            aktiverButton.Background = getFarbverlauf(2);
            frame_informationsausgabe.Content = null;
            frame_informationsausgabe.Navigate( new TerminInformation(
                aktiverButton.Art,
                aktiverButton.Datum,
                aktiverButton.Inhalt,
                aktiverButton.Autor,
                mw.hasRights));

            letzterAktiverButton = (Button)sender;  // Zum speichern des letzten gedrückten Buttons
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
            }

            return gradientBrush;

        }

    }
}
