using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
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

namespace Pinnwand.Pages.ExtraPage
{
    /// <summary>
    /// Interaction logic for TerminInformation.xaml
    /// </summary>
    public partial class TerminInformation : UserControl
    {
        private readonly Home _home;
        private Termin _sourceTermin;
        private MainWindow mw = (MainWindow) Application.Current.MainWindow;

        public TerminInformation(Termin sender, Home home) //string E_Art, DateTime E_Datum, string E_Beschreibung, string E_Autor, bool has_rights,Home home)
        {
            _home = home;
            _sourceTermin = sender;
            InitializeComponent();
            lbl_E_Art.Content = sender.Art;
            lbl_E_Datum.Content = sender.Datum.ToShortDateString();
            lbl_E_Beschreibung.Content = sender.Inhalt;
            lbl_E_Autor.Content = sender.Autor;

            if (!mw.hasRights)
            {
                stack_button.Children.Clear();
            }
        }

        private void Cmd_delete_OnClick(object sender, RoutedEventArgs e)
        {
            Packet deletePacket = mw.client.SendAndWaitForResponse(PacketType.DeleteEreignis, new ListDictionary{
                {"E_ID",_sourceTermin.eId}
            });
            lbl_error.Content = deletePacket.Success ? "Ereigniss erfolgreich gelöscht" : deletePacket.MessageString;
            _home.reload_Ereignisse();
        }

        private void Cmd_edit_OnClick(object sender, RoutedEventArgs e)
        {
            _home.frame_informationsausgabe.Navigate(new TerminErstellen(_home.K_ID, _sourceTermin.eId,
                _sourceTermin.Art, _sourceTermin.Datum, _sourceTermin.Inhalt));
        }
    }
}
