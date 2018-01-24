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
    }
}
