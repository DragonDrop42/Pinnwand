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
using FirstFloor.ModernUI.Presentation;
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
        private ModernTab Kurse;

        public MainWindow()
        {
            InitializeComponent();

            TCP_connection.ErrorMessageCallback ErrorCallback = Fehler_Ausgabe;
            client = new Client(ErrorCallback);

            client.Connect(PacketHandler.GetIPAddress(), 4444);

            LoginFrm = new Login();
            LoginFrm.Show();
            LoginFrm.Loaded += LoginFrm_Loaded;
            LoginFrm.GotFocus += LoginFrm_GotFocus;
            //LoginFrm.Closing += LoginFrmOnClosing;
            //IsEnabled = false;
            GotFocus += OnGotFocus;
            
        }

        private void LoginFrmOnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            Close();
        }

        private void OnGotFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            Button cmd_refresh = UIHelper.FindVisualChildByName<Button>(this, "cmd_refresh");
            if (cmd_refresh != null && !SubscribedEvents.Contains("cmd_refresh"))
            {
                cmd_refresh.Click += Cmd_Refresh_Click;
                SubscribedEvents.Add("cmd_refresh");
            }

            Button cmd_save = UIHelper.FindVisualChildByName<Button>(this, "cmd_save");
            if (cmd_save != null && !SubscribedEvents.Contains("cmd_save"))
            {
                cmd_save.Click += cmd_save_Click;
                SubscribedEvents.Add("cmd_save");
            }
        
    }

    private void cmd_save_Click(object sender, RoutedEventArgs e)
        {
            Pages.Settings.Kurswahl kw = UIHelper.FindVisualParent<Pages.Settings.Kurswahl>((Button)sender);
            try
            {
                List<string> k = kw.GetChecked();
                if (k.Count == 0) throw new Exception("Bitte mindestens einen Kurs auswählen.");
                Packet kursUpdate = client.SendKursUpdatePacket(k);
                if (!kursUpdate.Success) throw new Exception(kursUpdate.MessageString);
                Reload_Kurse();
                throw new Exception("Erfolgreich gespeichert");
            }
            catch (Exception ex)
            {
                kw.lbl_Kurswahl_Error.Text = ex.Message;
            }
        }

        private void Cmd_Refresh_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            Pages.Settings.Kurswahl kw = UIHelper.FindVisualParent<Pages.Settings.Kurswahl>((Button)sender);
            try
            {
                Packet kurse = client.Kurswahl();
                Packet getKurse = client.SendGetKursePacket();
                if (kurse.Success && getKurse.Success)
                {
                    kw.UpdateKurse(kurse.Data,getKurse.Data);
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
            
            ComboBox cbB_Klasse = UIHelper.FindVisualChildByName<ComboBox>(LoginFrm, "cbB_Klasse");
            if (cbB_Klasse != null && !SubscribedEvents.Contains("cbB_Klasse"))
            {
                cbB_Klasse.DropDownOpened += CbB_Klasse_DropDownOpened;
                SubscribedEvents.Add("cbB_Klasse");
            }
            
        }

        private void cmd_LehrerLogin_Click(object sender, RoutedEventArgs e)
        {
            Pages.Login.Lehrer_Login lehrer_login =
                UIHelper.FindVisualParent<Pages.Login.Lehrer_Login>((Button) sender);
            try
            {
                Packet login = client.Login(lehrer_login.txt_Email.Text, lehrer_login.txt_Passwort.Password,false);
                if (login.Success)
                {
                    LoginFrm.Closing -= LoginFrmOnClosing; 
                    LoginFrm.Close();
                    IsEnabled = true;
                }
                else
                {
                    lehrer_login.lbl_LehrerLoginError.Text = login.MessageString;

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
                string Anrede = (string)lehrer_regi.cbB_Anrede.SelectedValue;
                string Titel = (string)lehrer_regi.cbB_Titel.SelectedValue;
                //MessageBox.Show(Vname + Name + Anrede + Email + Passwort + Titel);
                Packet register = client.Register_Lehrer(Name, Vname, Anrede, Email, Passwort, Titel);
                if (register.Success)
                {
                    throw new Exception("Registrierung erfolgreich.");
                }
                else
                {
                    throw new Exception(register.MessageString);
                }
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

                Packet register = client.Register_Schüler(Name, Vname, Phone,Klasse, Email, Passwort);
                if (register.Success)
                {
                    throw new Exception("Registrierung erfolgreich.");
                }
                else
                {
                    throw new Exception(register.MessageString);
                }
            }
            catch (Exception ex)
            {
                schüler_regi.lbl_Schüler_Registrations_Error.Text = ex.Message;
            }
        }

        void CbB_Klasse_DropDownOpened(object sender, EventArgs e)
        {
                Packet klassen = client.GetKlassen();
                if (klassen.Success)
                {
                    ComboBox cb = (ComboBox) sender;

                    List<string> lst_data = (List<string>) klassen.Data["Kl_Name"];
                    cb.Items.Clear();

                    foreach (string s in lst_data)
                    {
                        cb.Items.Add(s);
                    }
                }
                else
                {
                    MessageBox.Show(
                        "Klassen konnten nicht geladen werden:\n" + klassen.MessageString);
                }
            
        }

        void cmd_SchülerLogin_Click(object sender, RoutedEventArgs e)
        {
            Pages.Login.Schüler_Login schüler_login = UIHelper.FindVisualParent<Pages.Login.Schüler_Login>((Button)sender);
            try
            {
                Packet login = client.Login(schüler_login.txt_Email.Text, schüler_login.txt_Passwort.Password,true);
                if (login.Success)

                {
                    LoginFrm.Closing -= LoginFrmOnClosing; 
                    LoginFrm.Close();
                    IsEnabled = true;
                    Reload_Kurse();
                }
                else
                {
                    throw new Exception(login.MessageString);

                }
            }
            catch (Exception ex)
            {
                schüler_login.lbl_SchülerLoginError.Text = ex.Message;
            }
        }

        void Reload_Kurse()
        {
            try
            {
                if (Kurse == null)
                {
                    Kurse = UIHelper.FindVisualChildByName<ModernTab>(this, "mt_Kurse");
                }
                Packet GetKurse = client.SendGetKursePacket();
                if (GetKurse.Success)
                {
                    foreach (var Link in Kurse.Links.Where(L => L.Source.OriginalString != "Pages/Home.xaml").ToList()) Kurse.Links.Remove(Link);

                    if (((List<string>)GetKurse.Data["K_Name"]).Count != 0)
                    {
                        foreach (string Kurs in (List<string>)GetKurse.Data["K_Name"])
                        {
                            Kurse.Links.Add(new Link
                            {
                                DisplayName = Kurs,
                                Source = new Uri("Pages/KursUebersicht.xaml?Kurs=" + Kurs, UriKind.Relative)
                            });
                        }
                    }
                    else
                    {
                        Kurse.Links.Add(new Link
                        {
                            DisplayName = "Kurse Hinzufügen",
                            Source = new Uri("Pages/Settings/KursAuswahl.xaml", UriKind.Relative)
                        });
                    }
                }
                else
                {
                    throw new Exception(GetKurse.MessageString);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
