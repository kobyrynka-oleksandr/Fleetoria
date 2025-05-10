using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Fleetoria
{
    public partial class PvAIPage : PageWithScaling
    {
        private readonly char[] Letters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' };
        private readonly int[] Numbers = Enumerable.Range(1, 10).ToArray();

        Player human = new Player();

        public PvAIPage()
        {
            InitializeComponent();

            DataContext = this;

            CreateBattlefield();

            AddShipsToPanel();
        }

        private void CreateBattlefield()
        {
            int gridSize = 10;

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

        private void AddShipsToPanel()
        {
            var deckCounts = new List<int>
            {
                4,
                3, 3,
                2, 2, 2,
                1, 1, 1, 1
            };

            foreach (var count in deckCounts)
            {
                var ship = new Ship(count);

                ship.MouseLeftButtonDown += (s, e) =>
                {
                    if (ship.isPlaced)
                    {
                        int oldRow = Grid.GetRow(ship);
                        int oldCol = Grid.GetColumn(ship);
                        human.ClearMatrixWhenShipMovedOnGrid(oldRow - 1, oldCol - 1, ship.DeckCount, ship.isRotated);
                        ship.isPlaced = false;
                    }
                };

                ShipPanel.Children.Add(ship);
            }
        }

        private void Ship_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(Ship)))
                return;

            var ship = e.Data.GetData(typeof(Ship)) as Ship;
            if (ship == null)
                return;

            Point dropPosition = e.GetPosition(LabeledBattleGrid);

            int totalRows = 10;
            int totalColumns = 10;

            double cellHeight = LabeledBattleGrid.ActualHeight / (totalRows + 1);
            double cellWidth = LabeledBattleGrid.ActualWidth / (totalColumns + 1);

            int row = (int)(dropPosition.Y / cellHeight);
            int col = (int)(dropPosition.X / cellWidth);

            if (row == 0 || col == 0)
                return;

            if (row + ship.DeckCount - 1 > totalRows)
            {
                if (!ShipPanel.Children.Contains(ship))
                {
                    ShipPanel.Children.Add(ship);
                }
                return;
            }

            if (human.isCanBeAdded(row - 1, col - 1, ship.DeckCount, ship.isRotated))
            {
                if (ship.Parent is Panel panel)
                {
                    panel.Children.Remove(ship);
                }

                Grid.SetColumn(ship, col);
                Grid.SetRow(ship, row);
                Grid.SetRowSpan(ship, ship.DeckCount);

                if (!LabeledBattleGrid.Children.Contains(ship))
                {
                    LabeledBattleGrid.Children.Add(ship);
                }

                human.AddShipToMatrix(row - 1, col - 1, ship.DeckCount, ship.isRotated);
                ship.isPlaced = true;

                MessageBox.Show(human.MBMatrix());
            }
        }
    }
}
