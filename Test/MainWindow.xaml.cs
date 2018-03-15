using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.IO;
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
            
            ListDictionary config = getConfigFromTxt();
            client.Connect((string)config["ip"], Convert.ToInt32(config["port"])); //Connect to Server on IP and POrt 4444


            client.Busy += OnClientOnBusy;
            client.Available += OnClientOnAvailable;
            Login();
        }

        public void Login()
        {
            client.Connect(PacketHandler.GetIPAddress(), 4444); //Connect to Server on IP and POrt 4444
            LoginFrm = new Login();
            LoginFrm.Show();
            LoginFrm.Closing += LoginFrmOnClosing;
            Closing += OnClosing;
            IsEnabled = false;
        }

        public void OnClientOnBusy(object sender, EventArgs args)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            IsEnabled = false;
        }

        public void OnClientOnAvailable(object sender, EventArgs args)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
            IsEnabled = true;
        }

        public void OnClosing(object sender, CancelEventArgs cancelEventArgs)
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

        private ListDictionary getConfigFromTxt()
        {
            string standard = "IP: " + PacketHandler.GetIPAddress() + "\n" + "PORT: 4444";

            ListDictionary config = new ListDictionary();

            if (!File.Exists("config.con"))
            {
                //File.Create("config.con");
                StreamWriter sw = new StreamWriter("config.con", false, Encoding.ASCII);
                sw.WriteLine(standard);
                sw.Dispose();
                //File.WriteAllText("config.con", standard);
            }

            string[] txt = File.ReadAllLines("config.con");

            config.Add("ip", txt[0].Split(' ')[1]);
            config.Add("port", txt[1].Split(' ')[1]);
            return config;
        }
    }
}
