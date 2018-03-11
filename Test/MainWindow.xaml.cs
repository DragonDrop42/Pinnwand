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
using FirstFloor.ModernUI.Windows.Navigation;
using ServerData;
using Test.Pages;
using Test.Pages.Settings;
using NavigationEventArgs = FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs;

namespace Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        public Client client;
        List<string> SubscribedEvents = new List<string>();
        public Login LoginFrm;
        private ModernTab Kurse;
        public bool hasRights = false;

        public MainWindow()
        {
            InitializeComponent();

            GlobalMethods.ErrorMessageCallback ErrorCallback = Fehler_Ausgabe;
            GlobalMethods.UpdateFormCallback UpdateFormCallback = UpdateChat;

            client = new Client(ErrorCallback, UpdateFormCallback);

            client.Connect(PacketHandler.GetIPAddress(), 4444); //Connect to Server on IP and POrt 4444

            LoginFrm = new Login(this);
            LoginFrm.Show();
            LoginFrm.Closing += LoginFrmOnClosing;
            Closing += OnClosing;
            IsEnabled = false;
        }

        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            client.Disconnect();
        }

        //Callback Delegates+++++++++++++++++++++++++++
        private void Fehler_Ausgabe(string s)
        {
            MessageBox.Show(s);
        }
        private void UpdateChat(Packet p)
        {
            //Chat im aktiven Fenster neu laden ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        }

        public void LoginFrmOnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            Close();
        }

        public void Reload_Kurse()
        {
            try
            {
                if (Kurse == null)
                {
                    Kurse = UIHelper.FindVisualChildByName<ModernTab>(this, "mt_Kurse");
                }
                Packet GetKurse = client.SendAndWaitForResponse(PacketType.GetGewählteKurse);
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
                                Source = new Uri("Pages/Home.xaml?Kurs=" + Kurs, UriKind.Relative)
                            });
                        }
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
