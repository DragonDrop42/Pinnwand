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

namespace Pinnwand.Pages.Settings
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : UserControl
    {
        public About()
        {
            InitializeComponent();
            AboutTextBlock.Text = "Diese Software wurde im Rahmen des IS unterrichts der TIG15 am BSZ-et entwickelt.\n " +
                                  "Dieses Programm ist lediglich eine Übung und nicht für den einsatz in einer echten Schule gedacht.\n " +
                                  "Wir sind keine Sicherheitsexperten und garantieren unter keinen Umständen für die Sicherheit ihrer daten. \n " +
                                  "\n " +
                                  "Unser Team: \n " +
                                  "Arndt Wagner - Design, Teamleitung\n " +
                                  "Fritjof Kulawik - Dokumentation\n " +
                                  "Maximillian Klose - Doku\n " +
                                  "Kevin Tuchel - Datenbankentwurf\n " +
                                  "Moritz Queißer - Programmierung\n " +
                                  "Peter Wacker - Programmierung\n ";
        }
    }
}
