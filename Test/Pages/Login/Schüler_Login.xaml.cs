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
using ServerData;

namespace Pinnwand.Pages.Login
{
    /// <summary>
    /// Interaction logic for Schüler_Login.xaml
    /// </summary>
    public partial class Schüler_Login : UserControl
    {
        private MainWindow mw= (MainWindow)Application.Current.MainWindow;
        public Schüler_Login()
        {
            InitializeComponent();
        }

        private void cmd_SchülerLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ListDictionary dataLogin = new ListDictionary{
                    {"email", txt_Email.Text},
                    {"passwort", txt_Passwort.Password}
                };

                Packet loginResponse = mw.client.Login(dataLogin, true);
                if (loginResponse.Success)

                {
                    mw.LoginFrm.Closing -= mw.LoginFrmOnClosing; 
                    mw.LoginFrm.Close();
                    mw.IsEnabled = true;
                    mw.Reload_Kurse();
                }
                else
                {
                    throw new Exception(loginResponse.MessageString);

                }
            }
            catch (Exception ex)
            {
                lbl_SchülerLoginError.Text = ex.Message;
            }
        }

        private void cmd_Abbruch_Click(object sender, RoutedEventArgs e)
        {
            mw.Fehler_Ausgabe("Bitte wenden Sie sich an den Administrator!");
            lbl_SchülerLoginError.Text = "Bitte wenden Sie sich an den Administrator!";
        }
    }
}
