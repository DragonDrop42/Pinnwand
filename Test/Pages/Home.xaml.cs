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
            frame_informationsausgabe.Navigate(new ExtraPage.TerminErstellen());

            // Hinzufügen eines neuen Termins
            // Einfügen einer Page an die unten rechte Stelle !!!!!


            //Aktualisierung der Datenbank mit dem neuen Termin
        }

        #region Ist nur noch wichtig wenn das mit dem Frame nicht gehen sollte
        //private void datenEingabefeldErstellen()
        //{
        //    //stack_informationsausgabe.Visibility = Visibility.Collapsed;
        //    StackPanel stack_main = new StackPanel { Width = 390, Margin = new Thickness(10, 0, 0, 0) , Name ="stack_main" };
        //    Label lbl_Überschrift = new Label { FontSize = 23, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 5, 0, 10), Content = "Neuen Termin erstellen:" };

        //    #region Termin Namenseingabe
        //    // Eingabe des Namens des Termins
        //    StackPanel stack_Name = new StackPanel { Orientation = Orientation.Horizontal };
        //    Label lbl_schrift1 = new Label { Content = "Terminname", Width = 80 };
        //    TextBox txt_terminName = new TextBox { Width = 250, Height = 20, Margin = new Thickness(5, 0, 0, 0) , Name = "txt_terminName" };

        //    stack_Name.Children.Add(lbl_schrift1);
        //    stack_Name.Children.Add(txt_terminName);
        //    #endregion

        //    #region Termin Datumseingabe
        //    // Eingabe des Datums des Termins
        //    StackPanel stack_Datum = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 10, 0, 0) };
        //    Label lbl_schrift2 = new Label { Content = "Datum", Width = 80 };
        //    //Combobox Tag
        //    ComboBox cb_terminDatumTag = new ComboBox { Width = 40, Height = 25, Margin = new Thickness(5, 0, 0, 0) , Name = "cb_terminDatumTag"};
        //    for (int i = 0; i < 31; i++)
        //    {
        //        cb_terminDatumTag.Items.Add(i + 1);
        //    }
        //    cb_terminDatumTag.SelectedIndex = 0;
        //    //Combobox Monat
        //    ComboBox cb_terminDatumMonat = new ComboBox { Width = 80, Height = 25, Margin = new Thickness(5, 0, 0, 0), Name = "cb_terminDatumMonat" };
        //    CultureInfo ci = new CultureInfo("de-DE");
        //    for (int i = 0; i < 12; i++)
        //    {
        //        cb_terminDatumMonat.Items.Add(ci.DateTimeFormat.MonthNames[i]);
        //    }
        //    cb_terminDatumMonat.SelectedIndex = 0;
        //    //Combobox Jahr
        //    ComboBox cb_terminDatumJahr = new ComboBox { Width = 80, Height = 25, Margin = new Thickness(5, 0, 0, 0), Name = "cb_terminDatumJahr" };
        //    for (int i = DateTime.Now.Year; i < DateTime.Now.Year + 60; i++)
        //    {
        //        cb_terminDatumJahr.Items.Add(i);
        //    }
        //    cb_terminDatumJahr.SelectedIndex = 0;
        //    //Hinzufügen zum Datums stack
        //    stack_Datum.Children.Add(lbl_schrift2);
        //    stack_Datum.Children.Add(cb_terminDatumTag);
        //    stack_Datum.Children.Add(cb_terminDatumMonat);
        //    stack_Datum.Children.Add(cb_terminDatumJahr);
        //    #endregion

        //    #region Terminbeschreibung 
        //    //Eingabe der Beschreibung des Termins
        //    StackPanel stack_terminBeschreibung = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 10, 0, 0) };
        //    Label lbl_schrift3 = new Label { Content = "Beschreibung", Width = 80 };
        //    TextBox txt_TerminBeschreibung = new TextBox { Width = 250, Height = 145, Margin = new Thickness(5, 0, 0, 0) };

        //    stack_terminBeschreibung.Children.Add(lbl_schrift3);
        //    stack_terminBeschreibung.Children.Add(txt_TerminBeschreibung);
        //    #endregion

        //    Button cmd_hinzufügen = new Button { Width = 100 , Height = 35  ,Content = "Abschicken" ,Margin = new Thickness(180,10,0,0)};
        //    cmd_hinzufügen.Click += new RoutedEventHandler(cmd_hinzufügen_Click);

        //    // Namen im Array speichern
        //    namensspeicher[0] = txt_terminName.Name;
        //    namensspeicher[1] = cb_terminDatumTag.Name;
        //    namensspeicher[2] = cb_terminDatumMonat.Name;
        //    namensspeicher[3] = cb_terminDatumJahr.Name;


        //    //Hinzufügen der verschiedenen stacks zum Main stack
        //    stack_chatUndInfoBereich.Children.Add(stack_main);
        //    stack_main.Children.Add(lbl_Überschrift);
        //    stack_main.Children.Add(stack_Name);
        //    stack_main.Children.Add(stack_Datum);
        //    stack_main.Children.Add(stack_terminBeschreibung);
        //    stack_main.Children.Add(cmd_hinzufügen);
        //}
        #endregion


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
            // Löschen eines Termins
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
