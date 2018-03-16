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
using ServerData;

namespace Pinnwand
{
    /// <summary>
    /// Interaction logic for Kursliste.xaml
    /// </summary>
    public partial class Kursliste : UserControl
    {
        private MainWindow mw;
        public Kursliste()
        {
            InitializeComponent();
            mw = (MainWindow)Application.Current.MainWindow;
            mw._kursliste = this;
        }

        public void Reload_Kurse()
        {
            try
            {
                mw = (MainWindow)Application.Current.MainWindow;
                Packet GetKurse = mw.client.SendAndWaitForResponse(PacketType.GetGewählteKurse);
                if (GetKurse.Success)
                {
                    foreach (var Link in mt_Kurse.Links.Where(L => L.Source.OriginalString != "Pages/Home.xaml").ToList())
                        mt_Kurse.Links.Remove(Link);

                    if (((List<string>)GetKurse.Data["K_Name"]).Count != 0)
                    {
                        foreach (string Kurs in (List<string>)GetKurse.Data["K_Name"])
                        {
                            mt_Kurse.Links.Add(new Link
                            {
                                DisplayName = Kurs,
                                Source = new Uri("Pages/Home.xaml?Kurs=" + Kurs, UriKind.Relative)
                            });
                        }
                    }
                }
                else
                {
                    throw new Exception(GetKurse.MessageString);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Load Kurs fehler " + ex.Message);
            }
        }
    }
}
