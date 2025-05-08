using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace Fleetoria
{
    public class PageWithScaling : Page
    {
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            double baseWidth = 1280;
            double baseHeight = 800;

            double actualWidth = SystemParameters.PrimaryScreenWidth;
            double actualHeight = SystemParameters.PrimaryScreenHeight;

            double scaleX = actualWidth / baseWidth;
            double scaleY = actualHeight / baseHeight;
            double scale = Math.Min(scaleX, scaleY);

            if (Content is FrameworkElement rootElement)
            {
                rootElement.LayoutTransform = new ScaleTransform(scale, scale);
            }
        }
    }
}
