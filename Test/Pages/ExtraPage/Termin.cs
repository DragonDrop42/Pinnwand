using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using FirstFloor.ModernUI.Windows.Controls;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace Test.Pages.ExtraPage
{
    class Termin : Button
    {
        public string Art;
        public string Inhalt;
        public DateTime Datum;
        public string Autor;
        
        public Termin(string Art,DateTime Datum,string Inhalt,string Autor)
        {
            LinearGradientBrush gradientBrush = new LinearGradientBrush();
            gradientBrush.StartPoint = new Point(0.5, 0);
            gradientBrush.EndPoint = new Point(0.5, 1);
            gradientBrush.GradientStops.Add(new GradientStop(Color.FromArgb(255, 178, 178, 178), 0));
            gradientBrush.GradientStops.Add(new GradientStop(Color.FromArgb(255, 38, 139, 185), 1));

            this.Art = Art;
            this.Datum = Datum;
            this.Inhalt = Inhalt;
            this.Autor = Autor;
            
            Width = 150;
            Height = 70;
            Background = gradientBrush;

            StackPanel stack_ereignis = new StackPanel();

            Label lbl_namenEreignis = new Label { HorizontalContentAlignment = HorizontalAlignment.Center, FontSize = 17, Margin = new Thickness(0, 10, 0, 5), Foreground = Brushes.Black };
            lbl_namenEreignis.Content = Art;    // Name des neuen Ereignisses

            Label lbl_datumEreignis = new Label { HorizontalContentAlignment = HorizontalAlignment.Center, FontSize = 17, Foreground = Brushes.Black };
            lbl_datumEreignis.Content = Datum.ToShortDateString();   // Datum des neuen Ereignisses

            stack_ereignis.Children.Add(lbl_namenEreignis);
            stack_ereignis.Children.Add(lbl_datumEreignis);
            //AddChild(stack_ereignis);
            Content = stack_ereignis;

        }
    }
}
