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

namespace Kolory
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Color rectangleColor
        {
            get
            {
                return (rectangle.Fill as SolidColorBrush).Color;
            }
            set
            {
                (rectangle.Fill as SolidColorBrush).Color = value;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            Settings.LoadWindow(this);
            Color color = Settings.LoadColor();
            rectangle.Fill = new SolidColorBrush(color);
            sliderR.Value = color.R;
            sliderG.Value = color.G;
            sliderB.Value = color.B;
        }

        private void sliderR_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Color color = Color.FromRgb((byte)sliderR.Value, (byte)sliderG.Value, (byte)sliderB.Value);
            rectangleColor = color;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) 
            { 
                Close();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
                Settings.SaveColor(rectangleColor);
                Settings.SaveWindow(this);
        }
    }
}
