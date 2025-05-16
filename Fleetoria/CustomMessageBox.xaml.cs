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
using System.Windows.Shapes;
using System.Xml;

namespace Fleetoria
{
    public partial class CustomMessageBox : Window
    {
        public enum MessageBoxResult
        {
            None, OK, Yes, No
        }

        public MessageBoxResult Result { get; private set; } = MessageBoxResult.None;

        public CustomMessageBox(string message, params string[] buttons)
        {
            InitializeComponent();
            MessageText.Text = message;

            foreach (var btnText in buttons)
            {
                var button = new Button
                {
                    Style = (Style)FindResource("FlatButtonStyle"),
                    FontSize = 16,
                    FontWeight = FontWeights.Normal,
                    Content = btnText,
                    Margin = new Thickness(10, 0, 10, 0),
                    Padding = new Thickness(15, 5, 15, 5),
                    MinWidth = 60
                };

                button.Click += (s, e) =>
                {
                    switch (btnText.ToLower())
                    {
                        case "ok":
                            Result = MessageBoxResult.OK;
                            break;
                        case "yes":
                            Result = MessageBoxResult.Yes;
                            break;
                        case "no":
                            Result = MessageBoxResult.No;
                            break;
                        default:
                            Result = MessageBoxResult.None;
                            break;
                    }

                    DialogResult = true;
                    Close();
                };

                ButtonPanel.Children.Add(button);
            }
        }

        public static MessageBoxResult Show(string message, params string[] buttons)
        {
            var box = new CustomMessageBox(message, buttons);
            box.Owner = Application.Current.MainWindow;
            box.ShowDialog();
            return box.Result;
        }
    }
}
