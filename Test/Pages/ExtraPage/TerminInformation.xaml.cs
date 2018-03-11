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

namespace Test.Pages.ExtraPage
{
    /// <summary>
    /// Interaction logic for TerminInformation.xaml
    /// </summary>
    public partial class TerminInformation : UserControl
    {
        public TerminInformation(string E_Art, DateTime E_Datum, string E_Beschreibung,string E_Autor,bool has_rights)
        {
            InitializeComponent();
            lbl_E_Art.Content = E_Art;
            lbl_E_Datum.Content = E_Datum.ToShortDateString();
            lbl_E_Beschreibung.Content = E_Beschreibung;
            lbl_E_Autor.Content = E_Autor;

            if (!has_rights)
            {
                stack_button.Children.Clear();
            }
        }

    }
}
