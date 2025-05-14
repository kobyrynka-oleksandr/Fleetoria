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

            this.Focusable = true;
            this.Focus(); 
            Keyboard.Focus(this);
            
            DataContext = this;

            CreateBattlefield(LabeledBattleGridHuman);

            AddShipsToPanel(ShipPanel, human, LabeledBattleGridHuman);

            CreateBattlefield(LabeledBattleGridBot);
        }

        private void CreateBattlefield(Grid grid)
        {
            int gridSize = 10;

            for (int i = 0; i <= gridSize; i++)
            {
                grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }

            var topLeftCell = new Border
            {
                Background = Brushes.White,
                BorderThickness = new Thickness(1),
                BorderBrush = Brushes.Black
            };
            Grid.SetRow(topLeftCell, 0);
            Grid.SetColumn(topLeftCell, 0);
            grid.Children.Add(topLeftCell);

            for (int col = 1; col <= gridSize; col++)
            {
                var headerCell = CreateHeaderCell(Letters[col - 1].ToString());
                Grid.SetRow(headerCell, 0);
                Grid.SetColumn(headerCell, col);
                grid.Children.Add(headerCell);
            }

            for (int row = 1; row <= gridSize; row++)
            {
                var headerCell = CreateHeaderCell(Numbers[row - 1].ToString());
                Grid.SetRow(headerCell, row);
                Grid.SetColumn(headerCell, 0);
                grid.Children.Add(headerCell);
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
                    grid.Children.Add(cell);
                }
            }
        }

        private Border CreateHeaderCell(string text) => new Border
        {
            Background = Brushes.White,
            BorderThickness = new Thickness(1),
            BorderBrush = Brushes.Black,
            Child = new TextBlock
            {
                Text = text,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            }
        };

        private void AddShipsToPanel(Panel panel, Player player, Grid grid)
        {
            panel.Children.Clear();
            var deckCounts = new List<int> { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 };

            foreach (var count in deckCounts)
            {
                var ship = new Ship(count);

                ship.MouseLeftButtonDown += (s, e) =>
                {
                    if (ship.isPlaced)
                    {
                        int oldRow = Grid.GetRow(ship);
                        int oldCol = Grid.GetColumn(ship);
                        player.ClearMatrixWhenShipMovedOnGrid(oldRow - 1, oldCol - 1, ship.DeckCount, ship.isRotated);
                        ship.isPlaced = false;
                    }

                    DragDrop.DoDragDrop(ship, ship, DragDropEffects.Move);

                    if (!ship.isPlaced && grid.Children.Contains(ship))
                    {
                        int newRow = Grid.GetRow(ship) - 1;
                        int newCol = Grid.GetColumn(ship) - 1;

                        player.AddShipToMatrix(newRow, newCol, ship.DeckCount, ship.isRotated);
                        ship.isPlaced = true;

                        MessageBox.Show(player.MBMatrix());
                    }
                };

                panel.Children.Add(ship);
            }
        }
        private void Ship_DragOver_Wrapper(object sender, DragEventArgs e)
        {
            if (sender is Grid grid)
            {
                Ship_DragOver(sender, e, grid, human);
            }
        }

        private void Ship_DragOver(object sender, DragEventArgs e, Grid grid, Player player)
        {
            if (!e.Data.GetDataPresent(typeof(Ship))) return;

            var ship = e.Data.GetData(typeof(Ship)) as Ship;
            if (ship == null) return;

            Point dropPosition = e.GetPosition(grid);
            int totalRows = 10;
            int totalColumns = 10;

            double cellHeight = grid.ActualHeight / (totalRows + 1);
            double cellWidth = grid.ActualWidth / (totalColumns + 1);

            int row = (int)(dropPosition.Y / cellHeight);
            int col = (int)(dropPosition.X / cellWidth);

            if (row == 0 || col == 0) return;

            int matrixRow = row - 1;
            int matrixCol = col - 1;

            HandleRotationDuringDrag(ship, e, matrixRow, matrixCol, totalRows, totalColumns, player);

            if (!CanPlaceShipAt(matrixRow, matrixCol, ship, totalRows, totalColumns, player))
                return;

            PlaceShipOnGrid(ship, row, col, grid);
        }

        private bool CanPlaceShipAt(int row, int col, Ship ship, int totalRows, int totalColumns, Player player)
        {
            if ((!ship.isRotated && row + ship.DeckCount > totalRows) ||
                (ship.isRotated && col + ship.DeckCount > totalColumns))
                return false;

            return player.isCanBeAdded(row, col, ship.DeckCount, ship.isRotated);
        }

        private void HandleRotationDuringDrag(Ship ship, DragEventArgs e, int row, int col, int totalRows, int totalColumns, Player player)
        {
            bool isAltPressed = e.KeyStates.HasFlag(DragDropKeyStates.AltKey);

            if (isAltPressed && !ship.IsRotatedDuringDrag)
            {
                ship.Rotate();
                ship.ResetSpan();
                ship.IsRotatedDuringDrag = true;

                if (!CanPlaceShipAt(row, col, ship, totalRows, totalColumns, player))
                {
                    ship.Rotate();
                    ship.ResetSpan();
                    ship.IsRotatedDuringDrag = false;
                }
            }
            else if (!isAltPressed)
            {
                ship.IsRotatedDuringDrag = false;
            }
        }

        private void PlaceShipOnGrid(Ship ship, int row, int col, Grid grid)
        {
            if (ship.Parent is Panel panel)
                panel.Children.Remove(ship);

            Grid.SetRow(ship, row);
            Grid.SetColumn(ship, col);

            if (!ship.isRotated)
                Grid.SetRowSpan(ship, ship.DeckCount);
            else
                Grid.SetColumnSpan(ship, ship.DeckCount);

            if (!grid.Children.Contains(ship))
                grid.Children.Add(ship);
        }

        private void ResetBattleGrid(Grid grid, Player player, Panel panel)
        {
            var shipsToRemove = grid.Children.OfType<UIElement>().Where(el => el is Ship).ToList();
            player.ClearData();

            foreach (var ship in shipsToRemove)
                grid.Children.Remove(ship);

            AddShipsToPanel(panel, player, grid);
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetBattleGrid(LabeledBattleGridHuman, human, ShipPanel);
        }

        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            ResetBattleGrid(LabeledBattleGridHuman, human, ShipPanel);
            var random = new Random();

            foreach (Ship ship in ShipPanel.Children.OfType<Ship>().ToList())
            {
                if (!TryPlaceShipRandomly(LabeledBattleGridHuman, ShipPanel, human, ship, random))
                {
                    MessageBox.Show("Не вдалося розмістити всі кораблі. Спробуйте ще раз.");
                    return;
                }
            }
        }
        private bool TryPlaceShipRandomly(Grid grid, Panel panel, Player player, Ship ship, Random random)
        {
            int attempts = 0;
            ship.ResetSpan();

            while (attempts < 1000)
            {
                bool isRotated = random.Next(2) == 0;
                ship.SetRotation(isRotated);

                int maxRow = isRotated ? 9 : 10 - ship.DeckCount;
                int maxCol = isRotated ? 10 - ship.DeckCount : 9;

                int row = random.Next(0, maxRow + 1);
                int col = random.Next(0, maxCol + 1);

                if (player.isCanBeAdded(row, col, ship.DeckCount, isRotated))
                {
                    if (ship.Parent is Panel parentPanel)
                        parentPanel.Children.Remove(ship);

                    grid.Children.Add(ship);
                    Grid.SetRow(ship, row + 1);
                    Grid.SetColumn(ship, col + 1);

                    if (isRotated)
                    {
                        Grid.SetRowSpan(ship, 1);
                        Grid.SetColumnSpan(ship, ship.DeckCount);
                    }
                    else
                    {
                        Grid.SetRowSpan(ship, ship.DeckCount);
                        Grid.SetColumnSpan(ship, 1);
                    }

                    player.AddShipToMatrix(row, col, ship.DeckCount, isRotated);
                    ship.isPlaced = true;

                    return true;
                }

                attempts++;
            }

            return false;
        }
    }
}
