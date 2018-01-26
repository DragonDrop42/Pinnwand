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
using System.Collections.Specialized;
using System.ComponentModel;
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

            TCP_connection.ErrorMessageCallback ErrorCallback = new TCP_connection.ErrorMessageCallback(Fehler_Ausgabe);
            client = new Client(ErrorCallback);

            client.Connect(PacketHandler.GetIPAddress(), 4444);

            LoginFrm = new Login();
            LoginFrm.Show();
            LoginFrm.Loaded += LoginFrm_Loaded;
            LoginFrm.GotFocus += LoginFrm_GotFocus;
            LoginFrm.Closing += LoginFrmOnClosing;
            IsEnabled = false;
            GotFocus += OnGotFocus;

        }

        private void LoginFrmOnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            Close();
        }

        private void OnGotFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            Pages.Settings.Kurswahl kurswahl =
                UIHelper.FindVisualChildByName<Pages.Settings.Kurswahl>(this, "pg_Kurswahl");
            if (kurswahl != null && !SubscribedEvents.Contains("KurswahlOnInitialized"))
            {
                kurswahl.Initialized += KurswahlOnInitialized;
                SubscribedEvents.Add("KurswahlOnInitialized");
            }
        }

        private void KurswahlOnInitialized(object sender, EventArgs eventArgs)
        {
            Pages.Settings.Kurswahl kw = (Pages.Settings.Kurswahl)sender;
            client.Kurswahl();
        }

        //private void WPFInvoke(Packet p) { Application.Current.Dispatcher.Invoke(new Action(() => { DataManager(p); })); }

        //private void DataManager(Packet packet)
        //{
        //    try
        //    {
        //        switch (packet.packetType)
        //        {
        //            //Type Authentication-----------------------------
        //            case PacketType.Authentication:
        //                switch (packet.authState_SERVER)
        //                {
        //                    case AuthenticationState_SERVER_Events.SERVER_Register_ID:

        //                        client.ID = (packet.auth_keyList["id"].ToString());
        //                        break;

        //                    case AuthenticationState_SERVER_Events.SERVER_Klassenwahl_Response:
        //                        ComboBox cb = UIHelper.FindVisualChildByName<ComboBox>(LoginFrm, "cbB_Klasse");

        //                        List<string> lst_data = (List<string>)packet.auth_keyList["Kl_Name"];
        //                        cb.Items.Clear();
                                
        //                        foreach (string s in lst_data)
        //                        {
        //                            cb.Items.Add(s);    
        //                        }
        //                        break;

        //                    case AuthenticationState_SERVER_Events.SERVER_Login_Failed:
        //                        break;

        //                    case AuthenticationState_SERVER_Events.SERVER_Registraition_Accepted:
        //                        Pages.Login.Schüler_Register sr =
        //                            UIHelper.FindVisualChildByName<Pages.Login.Schüler_Register>(LoginFrm,
        //                                "pg_Schüler_Regi");
        //                        sr.lbl_Schüler_Registrations_Error.Text = "Erfolgreich Registriert";
        //                        sr.IsEnabled = false;
        //                        break;
        //                    case AuthenticationState_SERVER_Events.SERVER_Registraition_Failed:
        //                        UIHelper.FindVisualChildByName<TextBlock>(LoginFrm, "lbl_Schüler_Registrations_Error").Text = packet.informationString;
        //                        break;
        //                }
        //                return;
        //            //------------------------------
        //            case PacketType.DataTable:
        //                switch (packet.tableType_SERVER)
        //                {
        //                    case DataTableType_SERVER_Events.SERVER_Kurswahl_Response:
        //                        //frm_Kurswahl kurswahl = new frm_Kurswahl(client, packet.lst_TableDictionary);
        //                        //kurswahl.Show();
        //                        break;
        //                }
        //                break;

        //            case PacketType.System_Error:
        //                Fehler_Ausgabe("Server Error: " + packet.informationString);
        //                break;


        //            default:
        //                Fehler_Ausgabe("Unbekanntes Packet");
        //                break;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Fehler_Ausgabe(ex.Message);
        //    }
        //}

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

            Button cmd_LehrerLogin = UIHelper.FindVisualChildByName<Button>(LoginFrm, "cmd_LehrerLogin");
            if (cmd_LehrerLogin != null && !SubscribedEvents.Contains("cmd_LehrerLogin"))
            {
                cmd_LehrerLogin.Click += cmd_LehrerLogin_Click;
                SubscribedEvents.Add("cmd_LehrerLogin");
            }
            
            Button cmd_LehrerRegi = UIHelper.FindVisualChildByName<Button>(LoginFrm, "cmd_AbsendenRegistrierungLehrer");
            if (cmd_LehrerRegi != null && !SubscribedEvents.Contains("cmd_LehrerRegi"))
            {
                cmd_LehrerRegi.Click += cmd_LehrerRegi_Click;
                SubscribedEvents.Add("cmd_LehrerRegi");
            }

            ModernTab tab_Schüler = UIHelper.FindVisualChildByName<ModernTab>(LoginFrm, "tab_Schüler");
            if (tab_Schüler != null && !SubscribedEvents.Contains("tab_Schüler"))
            {
                tab_Schüler.SelectedSourceChanged += tab_Schüler_SelectedSourceChanged;
                SubscribedEvents.Add("tab_Schüler");
            }
        }

        private void cmd_LehrerLogin_Click(object sender, RoutedEventArgs e)
        {
            Pages.Login.Lehrer_Login lehrer_login =
                UIHelper.FindVisualParent<Pages.Login.Lehrer_Login>((Button) sender);
            try
            {
                Packet login = client.Login(lehrer_login.txt_Email.Text, lehrer_login.txt_Passwort.Password);
                if (login.success)
                {
                    LoginFrm.Closing -= LoginFrmOnClosing; 
                    LoginFrm.Close();
                    IsEnabled = true;
                }
                else
                {
                    lehrer_login.lbl_LehrerLoginError.Text = login.informationString;

                }
            }
            catch (Exception ex)
            {
                lehrer_login.lbl_LehrerLoginError.Text = ex.Message;
            }
        }

        private void cmd_LehrerRegi_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            Pages.Login.Lehrer_Register lehrer_regi = UIHelper.FindVisualParent<Pages.Login.Lehrer_Register>((Button)sender);
            try
            {
                string Name = lehrer_regi.txt_Name.Text;
                string Vname = lehrer_regi.txt_Vorname.Text;
                string lehrerCode = lehrer_regi.txt_LehrerCode.Text;
                string Email = lehrer_regi.txt_Email.Text;
                string Passwort = lehrer_regi.txt_Passwort.Password;
                //client.Register_Lehrer(Name, Vname, Phone,Klasse, Email, Passwort);
                string Anrede = (string)lehrer_regi.cbB_Anrede.SelectedValue;
                string Titel = (string) lehrer_regi.cbB_Titel.SelectedValue;
                MessageBox.Show(Vname + Name + Anrede + Email + Passwort + Titel);
            }
            catch (Exception ex)
            {
                lehrer_regi.lbl_Lehrer_Registrations_Error.Text = ex.Message;
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
                Packet klassen = client.GetKlassen();
                if (klassen.success)
                {
                    ComboBox cb = UIHelper.FindVisualChildByName<ComboBox>(LoginFrm, "cbB_Klasse");

                    List<string> lst_data = (List<string>) klassen.lst_Dir_Auth["Kl_Name"];
                    cb.Items.Clear();

                    foreach (string s in lst_data)
                    {
                        cb.Items.Add(s);
                    }
                }
                else
                {
                    MessageBox.Show(
                        "Klassen konnten nicht geladen werden:\n" + klassen.informationString);
                }
            }
        }

        void cmd_SchülerLogin_Click(object sender, RoutedEventArgs e)
        {
            Pages.Login.Schüler_Login schüler_login = UIHelper.FindVisualParent<Pages.Login.Schüler_Login>((Button)sender);
            try
            {
                Packet login = client.Login(schüler_login.txt_Email.Text, schüler_login.txt_Passwort.Password);
                if (login.success)
                {
                    LoginFrm.Closing -= LoginFrmOnClosing; 
                    LoginFrm.Close();
                    IsEnabled = true;
                }
                else
                {
                    schüler_login.lbl_SchülerLoginError.Text = login.informationString;

                }
            }
            catch (Exception ex)
            {
                schüler_login.lbl_SchülerLoginError.Text = ex.Message;
            }
        }
    }
}
