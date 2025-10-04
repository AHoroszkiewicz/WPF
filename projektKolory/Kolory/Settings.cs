using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Kolory
{
    internal class Settings
    {
        public static Color LoadColor()
        {
            Properties.Settings settings = Properties.Settings.Default;
            Color color = new Color()
            {
                R = settings.R,
                G = settings.G,
                B = settings.B,
            };
            return color;
        }

        public static void SaveColor(Color color)
        {
            Properties.Settings settings = Properties.Settings.Default;
            settings.R = color.R;
            settings.G = color.G;
            settings.B = color.B;
            settings.Save();
        }

        public static void LoadWindow(Window window)
        {
            Properties.Settings settings = Properties.Settings.Default;
            window.Left = settings.x;
            window.Top = settings.y;
            window.Width = settings.width;
            window.Height = settings.height;
        }

        public static void SaveWindow(Window window)
        {
            Properties.Settings settings = Properties.Settings.Default;
            settings.x = window.Left;
            settings.y = window.Top;
            settings.width = window.Width;
            settings.height = window.Height;
            settings.Save();
        }
    }
}
