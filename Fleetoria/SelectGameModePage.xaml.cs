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

namespace Fleetoria
{
    /// <summary>
    /// Interaction logic for SelectGameModePage.xaml
    /// </summary>
    public partial class SelectGameModePage : PageWithScaling
    {
        public SelectGameModePage()
        {
            InitializeComponent();
        }

        private void PvAIButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new PvAIPage());
        }

        private void LocalPVPButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
    }
}
