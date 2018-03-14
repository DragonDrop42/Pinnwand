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
    public partial class Lehrer_Register : UserControl
    {
        private MainWindow mw= (MainWindow)Application.Current.MainWindow;
        public Lehrer_Register()
        {
            InitializeComponent();

            comboxenFuellen();
        }

        private void CmdAbsendenRegistrierungLehrerOnClick(object o, RoutedEventArgs routedEventArgs)
        {
            try
            {
                ListDictionary dataRegister = new ListDictionary
                {
                    {"name", txt_Name.Text},
                    {"vname", txt_Vorname.Text},
                    {"anrede", (string) cbB_Anrede.SelectedValue},
                    {"titel", (string) cbB_Titel.SelectedValue},
                    {"email", txt_Email.Text},
                    {"passwort", txt_Passwort.Password},
                    {
                        "lehrerPasswort", txt_LehrerCode.Text
                    } //Lehrer Passwort abfrage designen+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                };

                Packet registerResponse = mw.client.Register_Lehrer(dataRegister);
                if (registerResponse.Success)
                {
                    throw new Exception("Registrierung erfolgreich.");
                }

                throw new Exception(registerResponse.MessageString);
            }
            catch (Exception ex)
            {
                lbl_Lehrer_Registrations_Error.Text = ex.Message;
            }

        }

        public void comboxenFuellen()
        {
            //Titel
            cbB_Titel.Items.Add("");
            cbB_Titel.Items.Add("Dr.");
            cbB_Titel.Items.Add("Prof.");

            // Anrede
            cbB_Anrede.Items.Add("Herr");
            cbB_Anrede.Items.Add("Frau");
            
        }
    }
}
