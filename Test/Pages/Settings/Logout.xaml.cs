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
    /// Interaction logic for Logout.xaml
    /// </summary>
    public partial class Logout : UserControl
    {
        private MainWindow mw = (MainWindow) Application.Current.MainWindow;

        public Logout()
        {
            InitializeComponent();
        }

        private void Cmd_logout_OnClick(object sender, RoutedEventArgs e)
        {
            mw.client.Disconnect();
            mw.Login();
        }

        private void Cmd_cancel_OnClick(object sender, RoutedEventArgs e)
        {
            NavigationCommands.BrowseBack.Execute(null, null);
        }
    }
}
