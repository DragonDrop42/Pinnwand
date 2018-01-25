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

namespace Test.Pages.Settings
{
    /// <summary>
    /// Interaction logic for KursAuswahl.xaml
    /// </summary>
    public partial class Kurswahl : UserControl
    {
        public Kurswahl()
        {
            InitializeComponent();
        }

        public void UpdateKurse(List<string> Kurse)
        {
            lv_Kurse.Items.Clear();
            foreach (string Kurs in Kurse)
            {
                lv_Kurse.Items.Add(new CheckBox {Content = Kurs});
            }
        }
        public List<string> GetChecked()
        {
            return (from CheckBox k in lv_Kurse.Items where k.IsEnabled select k.Content.ToString()).ToList();
        }
    }
}
