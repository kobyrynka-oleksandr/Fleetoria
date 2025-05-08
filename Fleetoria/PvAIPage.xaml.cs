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
    public partial class PvAIPage : PageWithScaling
    {
        private readonly char[] Letters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' };
        private readonly int[] Numbers = Enumerable.Range(1, 10).ToArray();

        public PvAIPage()
        {
            InitializeComponent();

            DataContext = this;

            CreateBattlefield();
        }
        private void CreateBattlefield()
        {
            int gridSize = 10;


            // Створюємо структуру: 11 рядків і 11 колонок (1 заголовкова + 10 ігрових)
            for (int i = 0; i <= gridSize; i++)
            {
                LabeledBattleGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                LabeledBattleGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }

            var topLeftCell = new Border
            {
                Background = Brushes.White,
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.Black
            };

            Grid.SetRow(topLeftCell, 0);
            Grid.SetColumn(topLeftCell, 0);
            LabeledBattleGrid.Children.Add(topLeftCell);

            // Додаємо заголовки колонок (A–J)
            for (int col = 1; col <= gridSize; col++)
            {
                var headerCell = new Border
                {
                    Background = Brushes.White,
                    BorderThickness = new Thickness(1),
                    BorderBrush = Brushes.Black,
                    Child = new TextBlock
                    {
                        Text = Letters[col - 1].ToString(),
                        FontWeight = FontWeights.Bold,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    }
                };
                Grid.SetRow(headerCell, 0);
                Grid.SetColumn(headerCell, col);
                LabeledBattleGrid.Children.Add(headerCell);
            }

            // Додаємо заголовки рядків (1–10)
            for (int row = 1; row <= gridSize; row++)
            {
                var headerCell = new Border
                {
                    Background = Brushes.White,
                    BorderThickness = new Thickness(1),
                    BorderBrush = Brushes.Black,
                    Child = new TextBlock
                    {
                        Text = Numbers[row - 1].ToString(),
                        FontWeight = FontWeights.Bold,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    }
                };
                Grid.SetRow(headerCell, row);
                Grid.SetColumn(headerCell, 0);
                LabeledBattleGrid.Children.Add(headerCell);
            }

            // Додаємо самі клітинки поля бою
            for (int row = 1; row <= gridSize; row++)
            {
                for (int col = 1; col <= gridSize; col++)
                {
                    var cell = new Border
                    {
                        BorderThickness = new Thickness(1),
                        BorderBrush = Brushes.Black,
                        Background = Brushes.White,
                        Tag = $"{Letters[col - 1]}{Numbers[row - 1]}"
                    };

                    Grid.SetRow(cell, row);
                    Grid.SetColumn(cell, col);
                    LabeledBattleGrid.Children.Add(cell);
                }
            }
        }
    }
}
