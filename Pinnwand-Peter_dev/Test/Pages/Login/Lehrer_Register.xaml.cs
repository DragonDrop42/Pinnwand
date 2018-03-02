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
    public partial class Lehrer_Register : UserControl
    {
        public Lehrer_Register()
        {
            InitializeComponent();

            comboxenFuellen();
        }

        public void comboxenFuellen()
        {
            //Titel
            cbB_Titel.Items.Add("");
            cbB_Titel.Items.Add("Dr.");
            cbB_Titel.Items.Add("Prof.");

            // Anrede
            cbB_Anrede.Items.Add("Herr");
            cbB_Anrede.Items.Add("Frau");
            
        }
    }
}
