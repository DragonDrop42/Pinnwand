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
using Pinnwand.Pages;
using Pinnwand.Pages.Settings;
using NavigationEventArgs = FirstFloor.ModernUI.Windows.Navigation.NavigationEventArgs;

namespace Pinnwand
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        public Client client;
        public Login LoginFrm;
        public bool hasRights = false;
        public Kursliste _kursliste;
        public GlobalMethods.ErrorMessageCallback ErrorCallback;
        public Home CurrentKurs;

        public MainWindow()
        {
            InitializeComponent();

            ErrorCallback += Fehler_Ausgabe;

            client = new Client(ErrorCallback);
    
            client.Connect(PacketHandler.GetIPAddress(), 4444); //Connect to Server on IP and POrt 4444

            LoginFrm = new Login();
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
        public void Fehler_Ausgabe(string s)
        {
            Console.WriteLine(s);
        }

        public void LoginFrmOnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            Close();
        }

        public void Reload_Kurse()
        {
            if (_kursliste != null)
            {
                _kursliste.Reload_Kurse();
            }
        }
    }
}
