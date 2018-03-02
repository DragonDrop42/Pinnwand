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

namespace Test.Pages.Login
{
    /// <summary>
    /// Interaction logic for Schüler_Login.xaml
    /// </summary>
    public partial class Schüler_Register : UserControl
    {
        public Schüler_Register()
        {
            InitializeComponent();
        }

        

        public void KlassenFüllen(List<string> Klassen)
        {
            foreach (string k in Klassen)
            {
                cbB_Klasse.Items.Add(k);
            }
        }
    }
}
