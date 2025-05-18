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
    public partial class SettingsPage : PageWithScaling
    {
        private SettingsViewModel viewModel;

        public SettingsPage()
        {
            InitializeComponent();
            viewModel = new SettingsViewModel();
            this.DataContext = viewModel;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var result = CustomMessageBox.Show("Save changes?", "Yes", "No");
            if (result == CustomMessageBox.MessageBoxResult.Yes)
            {
                SettingsManager.SaveSettings(viewModel.ToSettingsData());
            }
            else if (result == CustomMessageBox.MessageBoxResult.No)
            {
                return;
            }

            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }
    }
}
