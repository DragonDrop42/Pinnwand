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
using FirstFloor.ModernUI.Presentation;

namespace Test
{
    /// <summary>
    /// Interaction logic for Kursliste.xaml
    /// </summary>
    public partial class Kursliste : UserControl
    {
        public Kursliste()
        {
            InitializeComponent();
        }

        public void KursUpdate(List<string> Kurslist)
        {
            mt_Kurse.Links.Clear();

            foreach (string kurs in Kurslist)
                mt_Kurse.Links.Add(
                    new Link
                    {
                        DisplayName = kurs,
                        Source = new Uri("/Pages/Home.xaml?Kurs=" + kurs)
                    });
        }
    }
}
