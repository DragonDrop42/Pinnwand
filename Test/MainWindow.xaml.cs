using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Collections.Specialized;
using ClientClassLib;
using ServerData;

namespace Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        Client client;
        List<string> SubscribedEvents = new List<string>();
        public Login LoginFrm;

        public MainWindow()
        {
            InitializeComponent();

            CenterWindowOnScreen();

            TCP_connection.DataManagerCallback PacketCallback = new TCP_connection.DataManagerCallback(WPFInvoke);
            TCP_connection.ErrorMessageCallback ErrorCallback = new TCP_connection.ErrorMessageCallback(Fehler_Ausgabe);
            client = new Client(ErrorCallback,PacketCallback);

            client.Connect(PacketHandler.GetIPAddress(), 4444);

            LoginFrm = new Login();
            LoginFrm.Show();
            LoginFrm.Loaded += LoginFrm_Loaded;
            LoginFrm.GotFocus += LoginFrm_GotFocus;
           // this.IsEnabled = false;

        }

        private void WPFInvoke(Packet p) { Application.Current.Dispatcher.Invoke(new Action(() => { DataManager(p); })); }

        private void DataManager(Packet packet)
        {
            try
            {
                switch (packet.packetType)
                {
                    //Type Authentication-----------------------------
                    case PacketType.Authentication:
                        switch (packet.authState_SERVER)
                        {
                            case AuthenticationState_SERVER_Events.SERVER_Register_ID:

                                client.ID = (packet.auth_keyList["id"].ToString());
                                break;

                            case AuthenticationState_SERVER_Events.SERVER_Klassenwahl_Response:
                                ComboBox cb = UIHelper.FindVisualChildByName<ComboBox>(LoginFrm, "cbB_Klasse");

                                List<string> lst_data = (List<string>)packet.auth_keyList["Kl_Name"];

                                foreach (string s in lst_data)
                                {
                                    cb.Items.Add(s);    
                                }
                                break;

                            case AuthenticationState_SERVER_Events.SERVER_Login_Accepted:
                                LoginFrm.Close();
                                this.IsEnabled = true;
                                break;

                            case AuthenticationState_SERVER_Events.SERVER_Login_Failed:
                                UIHelper.FindVisualChildByName<TextBlock>(LoginFrm, "lbl_SchülerLoginError").Text = packet.informationString;
                                break;

                            case AuthenticationState_SERVER_Events.SERVER_Registraition_Accepted:
                                NavigationService ns = NavigationService.GetNavigationService(LoginFrm);
                                ns.GoBack();
                                break;
                            case AuthenticationState_SERVER_Events.SERVER_Registraition_Failed:
                                Fehler_Ausgabe(packet.informationString);
                                break;
                        }
                        return;
                    //------------------------------
                    case PacketType.DataTable:
                        switch (packet.tableType_SERVER)
                        {
                            case DataTableType_SERVER_Events.SERVER_Kurswahl_Response:
                                //frm_Kurswahl kurswahl = new frm_Kurswahl(client, packet.lst_TableDictionary);
                                //kurswahl.Show();
                                break;
                        }
                        break;

                    case PacketType.System_Error:
                        Fehler_Ausgabe("Server Error: " + packet.informationString);
                        break;


                    default:
                        Fehler_Ausgabe("Unbekanntes Packet");
                        break;
                }
            }
            catch (Exception ex)
            {
                Fehler_Ausgabe(ex.Message);
            }
        }

        private void Fehler_Ausgabe(string s)
        {
            MessageBox.Show(s);
        }

        void LoginFrm_Loaded(object sender, RoutedEventArgs e)
        {
            LoginFrm.Focus();
        }

        void LoginFrm_GotFocus(object sender, RoutedEventArgs e)
        {

            Button cmd_SchülerLogin = UIHelper.FindVisualChildByName<Button>(LoginFrm, "cmd_SchülerLogin");
            if (cmd_SchülerLogin != null && !SubscribedEvents.Contains("cmd_SchülerLogin_Click"))
            {
                cmd_SchülerLogin.Click += cmd_SchülerLogin_Click;
                SubscribedEvents.Add("cmd_SchülerLogin_Click");
            }

            Button cmd_SchülerRegi = UIHelper.FindVisualChildByName<Button>(LoginFrm, "cmd_AbsendenRegistrierungSchueler");
            if (cmd_SchülerRegi != null && !SubscribedEvents.Contains("cmd_SchülerRegi"))
            {
                cmd_SchülerRegi.Click += cmd_SchülerRegi_Click;
                SubscribedEvents.Add("cmd_SchülerRegi");
            }

            ModernTab tab_Lehrer = UIHelper.FindVisualChildByName<ModernTab>(LoginFrm, "tab_Lehrer");
            if (tab_Lehrer != null && !SubscribedEvents.Contains("tab_Lehrer"))
            {
                tab_Lehrer.SelectedSourceChanged += tab_Lehrer_SelectedSourceChanged;
                SubscribedEvents.Add("tab_Lehrer");
            }

            ModernTab tab_Schüler = UIHelper.FindVisualChildByName<ModernTab>(LoginFrm, "tab_Schüler");
            if (tab_Schüler != null && !SubscribedEvents.Contains("tab_Schüler"))
            {
                tab_Schüler.SelectedSourceChanged += tab_Schüler_SelectedSourceChanged;
                SubscribedEvents.Add("tab_Schüler");
            }
        }

        void cmd_SchülerRegi_Click(object sender, RoutedEventArgs e)
        {
            Pages.Login.Schüler_Register schüler_regi = UIHelper.FindVisualParent<Pages.Login.Schüler_Register>((Button)sender);
            try
            {
                string Name = schüler_regi.txt_Name.Text;
                string Vname = schüler_regi.txt_Vorname.Text;
                string Phone = schüler_regi.txt_Telefonnummer.Text;
                string Email = schüler_regi.txt_Email.Text;
                string Passwort = schüler_regi.txt_Passwort.Password;
                string Klasse = Convert.ToString(schüler_regi.cbB_Klasse.SelectedValue);
                client.Register_User(Name, Vname, Phone,Klasse, Email, Passwort);
            }
            catch (Exception ex)
            {
                schüler_regi.lbl_Schüler_Registrations_Error.Text = ex.Message;
            }
        }

        void tab_Schüler_SelectedSourceChanged(object sender, SourceEventArgs e)
        {
            if (e.Source.ToString() == "Pages/Login/Schüler_Register.xaml")
            {
                client.GetKlassen();
            }
        }

        void tab_Lehrer_SelectedSourceChanged(object sender, SourceEventArgs e)
        {
            if (e.Source.ToString() == "Pages/Login/Lehrer_Register.xaml")
            {

            }
        }

        void cmd_SchülerLogin_Click(object sender, RoutedEventArgs e)
        {
            Pages.Login.Schüler_Login schüler_login = UIHelper.FindVisualParent<Pages.Login.Schüler_Login>((Button)sender);
            try
            {
                client.Login(schüler_login.txt_Email.Text, schüler_login.txt_Passwort.Password);
            }
            catch (Exception ex)
            {
                schüler_login.lbl_SchülerLoginError.Text = ex.Message;
            }
        }

        private void Reload()
        {
            //TODO: Content wird von server geladen
        }


        private void CenterWindowOnScreen()
        {
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
            this.Left = (screenWidth / 2) - (windowWidth / 2);
            this.Top = (screenHeight / 2) - (windowHeight / 2);
        }
    }
}
