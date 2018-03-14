using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using FirstFloor.ModernUI.Windows.Controls;
using ServerData;

namespace Pinnwand.Pages.Login
{
    /// <summary>
    /// Interaction logic for Schüler_Login.xaml
    /// </summary>
    public partial class Lehrer_Login : UserControl
    {
        private MainWindow mw = (MainWindow)Application.Current.MainWindow;

        public Lehrer_Login()
        {
            InitializeComponent();
        }

        private void CmdLehrerLoginOnClick(object o, RoutedEventArgs routedEventArgs)
        {
            try
            {
                ListDictionary dataLogin = new ListDictionary{
                    {"email", txt_Email.Text},
                    {"passwort", txt_Passwort.Password}
                };
                Packet loginResponse = mw.client.Login(dataLogin, false);

                if (loginResponse.Success)
                {
                    mw.LoginFrm.Closing -= mw.LoginFrmOnClosing; 
                    mw.IsEnabled = true;
                    mw.hasRights = true;
                    mw.Reload_Kurse();
                    mw.LoginFrm.Close();
                }
                else
                {
                    lbl_LehrerLoginError.Text = loginResponse.MessageString;
                }
            }
            catch (Exception ex)
            {
                lbl_LehrerLoginError.Text = ex.Message;
            }
        }

        private void cmd_LeherPasswortReset_Click(object sender, RoutedEventArgs e)
        {
            mw.Fehler_Ausgabe("Bitte wenden Sie sich an den Administrator!");
        }
    }
}
