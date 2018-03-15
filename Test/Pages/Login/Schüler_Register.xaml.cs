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
    public partial class Schüler_Register : UserControl
    {
        private MainWindow mw = (MainWindow)Application.Current.MainWindow;
        
        public Schüler_Register()
        {
            InitializeComponent();
        }
        void cmd_SchülerRegi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ListDictionary dataRegister = new ListDictionary{
                    {"name", txt_Name.Text},
                    {"vname", txt_Vorname.Text},
                    {"phone", txt_Telefonnummer.Text},
                    {"klasse",Convert.ToString(cbB_Klasse.SelectedValue)},
                    {"email", txt_Email.Text},
                    {"passwort", txt_Passwort.Password}
                };

                Packet registerResponse = mw.client.Register_Schüler(dataRegister);
                if (registerResponse.Success)
                {
                    throw new Exception("Registrierung erfolgreich.");
                }

                throw new Exception(registerResponse.MessageString);
            }
            catch (Exception ex)
            {
                lbl_Schüler_Registrations_Error.Text = ex.Message;
            }
        }
        
        void CbB_Klasse_DropDownOpened(object sender, EventArgs e)
        {
            Packet klassen = mw.client.SendAndWaitForResponse(PacketType.Klassenwahl);
            if (klassen.Success)
            {
                List<string> lst_data = (List<string>) klassen.Data["Kl_Name"];
                cbB_Klasse.Items.Clear();

                foreach (string s in lst_data)
                {
                    cbB_Klasse.Items.Add(s);
                }
            }
            else
            {
                MessageBox.Show(
                    "Klassen konnten nicht geladen werden:\n" + klassen.MessageString);
            }
        }
    }
}
