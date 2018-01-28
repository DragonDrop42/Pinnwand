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
using FirstFloor.ModernUI.Windows.Controls;

namespace Test.Pages
{
    /// <summary>
    /// Interaction logic for KursÜbersicht.xaml
    /// </summary>
    public partial class KursÜbersicht : UserControl
    {
        private ModernTab KListe;
        private string kurs;
        
        public KursÜbersicht()
        {
            InitializeComponent();
            this.Loaded += OnLoaded;
            //kurs = KListe.mt_Kurse.SelectedSource.Query.;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            KListe = UIHelper.FindVisualChildByName<ModernTab>(Application.Current.MainWindow,"mt_Kurse");
            Test.Text = KListe.SelectedSource.OriginalString.Split(Char.Parse("=")).Last();
        }
    }
}
