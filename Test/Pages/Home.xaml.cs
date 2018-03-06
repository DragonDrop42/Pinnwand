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

namespace Test.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {

        public Home()
        {
            InitializeComponent();
            
        }

        #region Mitschüler liste erstellen
        public void schülerLaden( List<string> schüler)
        {
            foreach (string schülername in schüler)
            {
                StackPanel stack_schüler = new StackPanel { Margin = new Thickness(5) };
                Label lbl_schüler = new Label { FontSize = 17 };
                lbl_schüler.Content = schülername;


                stack_Mitschüler.Children.Add(stack_schüler);
                stack_schüler.Children.Add(lbl_schüler);
            }
        #endregion



        }

        public void chatLaden( List<string> chats)
        {
            lbl_chatAusgabe.Content = "";
            foreach (string nachricht in chats)
            {
                lbl_chatAusgabe.Content += nachricht + "\n";
            }
        }

        private void txt_chatEingabe_GotFocus(object sender, RoutedEventArgs e) //leeren des Placeholders in der Leiste
        {
            txt_chatEingabe.Text = "";
        }

        private void cmd_senden_Click(object sender, RoutedEventArgs e) // Ausführen wenn der klick auf senden ausgeführt wird
        {
            lbl_chatAusgabe.Content += txt_chatEingabe.Text + "\n";
        }


        private void ereignisseErstellen()
        {
            LinearGradientBrush gradientBrush = new  LinearGradientBrush( Color.FromArgb(0, 178, 178, 178),  Color.FromArgb(0, 38, 139, 185), new Point(0.5, 0), new Point(0.5, 1));

            StackPanel stack_ereignis = new StackPanel {Width = 150 , Height = 70, Background = gradientBrush };
            Label lbl_namenEreignis = new Label();
            Label lbl_datumEreignis = new Label();


        }




    }
}
