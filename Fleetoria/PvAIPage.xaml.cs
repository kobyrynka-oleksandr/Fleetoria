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
    /// Interaction logic for PvAIPage.xaml
    /// </summary>
    public partial class PvAIPage : Page
    {
        private readonly char[] Letters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' };
        private readonly int[] Numbers = Enumerable.Range(1, 10).ToArray();

        public PvAIPage()
        {
            InitializeComponent();
            CreateBattlefield();
        }
        private void CreateBattlefield()
        {
            // Тепер можна використовувати Letters і Numbers тут
            for (int row = 0; row < Numbers.Length; row++)
            {
                for (int col = 0; col < Letters.Length; col++)
                {
                    // Наприклад, створення клітинки
                    var cell = new Border
                    {
                        Width = 40,
                        Height = 40,
                        BorderThickness = new Thickness(1),
                        BorderBrush = Brushes.Black,
                        Background = Brushes.LightBlue,
                        Tag = $"{Letters[col]}{Numbers[row]}"
                    };

                    // Додати до Grid (припустимо, ви маєте поле з ім'ям BattlefieldGrid)
                    Grid.SetRow(cell, row + 1); // +1 якщо перший ряд — заголовок
                    Grid.SetColumn(cell, col + 1);
                    PlayField.Children.Add(cell);
                }
            }
        }
    }
}
