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

        public void UpdateKurse(ListDictionary Kurse)
        {
            lv_Kurse.Items.Clear();
            for (int i = 0; i < ((List<string>)Kurse["K_ID"]).Count; i++)
            {
                lv_Kurse.Items.Add(new HIDCheckBox {Content = ((List<string>)Kurse["K_Name"])[i], ItemID = Convert.ToInt16(((List<string>)Kurse["K_ID"])[i])});
            }
        }
        public List<string> GetChecked()
        {
            return (from HIDCheckBox k in lv_Kurse.Items where k.IsChecked ?? false select k.ItemID.ToString()).ToList();
        }
    }
}

namespace Test
{
    public class HIDCheckBox : CheckBox
    {
        public int ItemID;
    }

}
