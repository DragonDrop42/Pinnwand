using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
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
        private ListDictionary Kurse;
        
        public Kurswahl()
        {
            InitializeComponent();
        }

        public void UpdateKurse(ListDictionary Kurse,ListDictionary CheckedKurse)
        {
            this.Kurse = Kurse;
            lv_Kurse.Items.Clear();
            for (int i = 0; i < ((List<string>)Kurse["K_ID"]).Count; i++)
            {
                lv_Kurse.Items.Add(new CheckBox
                {
                    Content = ((List<string>) Kurse["K_Name"])[i],
                    IsChecked = ((List<string>) CheckedKurse["K_Name"]).Contains(((List<string>) Kurse["K_Name"])[i])
                });
            }
        }
        public List<string> GetChecked()
        {
            List<string> IDS = new List<string>();
            for (int i = 0; i < ((List<string>) Kurse["K_ID"]).Count; i++)
            {
                IDS.AddRange(from CheckBox hcb in lv_Kurse.Items where (bool) hcb.IsChecked && hcb.Content == ((List<string>) Kurse["K_Name"])[i] select ((List<string>) Kurse["K_ID"])[i]);
            }
            return IDS;
        }
    }
}
