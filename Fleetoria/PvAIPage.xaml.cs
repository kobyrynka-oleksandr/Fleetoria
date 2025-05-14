using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Fleetoria
{
    public partial class PvAIPage : PageWithScaling
    {
        private Button selectedDifficultyButton;

        private readonly char[] Letters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' };
        private readonly int[] Numbers = Enumerable.Range(1, 10).ToArray();

        Player Human = new Player();
        Player Bot = new Player();

        BattleGridOperations bgOps = new BattleGridOperations();

        public PvAIPage()
        {
            InitializeComponent();

            this.Focusable = true;
            this.Focus(); 
            Keyboard.Focus(this);
            
            DataContext = this;

            bgOps.CreateBattlefield(LabeledBattleGridHuman);

            bgOps.AddShipsToPanel(ShipPanel, Human, LabeledBattleGridHuman);

            bgOps.CreateBattlefield(LabeledBattleGridBot);
        }

        private void Ship_DragOver_Wrapper(object sender, DragEventArgs e)
        {
            if (sender is Grid grid)
            {
                Ship_DragOver(sender, e, grid, Human);
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

            bgOps.HandleRotationDuringDrag(ship, e, matrixRow, matrixCol, totalRows, totalColumns, player);

            if (!bgOps.CanPlaceShipAt(matrixRow, matrixCol, ship, totalRows, totalColumns, player))
                return;

            bgOps.PlaceShipOnGrid(ship, row, col, grid);
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            bgOps.ResetBattleGrid(LabeledBattleGridHuman, Human, ShipPanel);
        }

        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            bgOps.ResetBattleGrid(LabeledBattleGridHuman, Human, ShipPanel);
            var random = new Random();

            foreach (Ship ship in ShipPanel.Children.OfType<Ship>().ToList())
            {
                if (!bgOps.TryPlaceShipRandomly(LabeledBattleGridHuman, ShipPanel, Human, ship, random))
                {
                    MessageBox.Show("Не вдалося розмістити всі кораблі. Спробуйте ще раз.");
                    return;
                }
            }
        }

        private void DifficultyButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedButton = sender as Button;

            if (selectedDifficultyButton != null)
            {
                selectedDifficultyButton.Background = Brushes.White;
                selectedDifficultyButton.Foreground = Brushes.Black;
            }

            clickedButton.Background = Brushes.Black;
            clickedButton.Foreground = Brushes.White;

            selectedDifficultyButton = clickedButton;

            string difficulty = clickedButton.Content.ToString();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (Human.health != 20)
            {
                MessageBox.Show("Place all ships!");
                return;
            }
            if (selectedDifficultyButton == null)
            {
                MessageBox.Show("Choose difficulty!");
                return;
            }
            bgOps.AddShipsToBotGrid(LabeledBattleGridBot, Bot);

            LabeledBattleGridOverlap.Children.Clear();
            LabeledBattleGridOverlap.RowDefinitions.Clear();
            LabeledBattleGridOverlap.ColumnDefinitions.Clear();

            CreateOverlapGridForBot(LabeledBattleGridOverlap);
        }

        private void CreateOverlapGridForBot(Grid grid)
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
                var headerCell = bgOps.CreateHeaderCell(Letters[col - 1].ToString());
                Grid.SetRow(headerCell, 0);
                Grid.SetColumn(headerCell, col);
                grid.Children.Add(headerCell);
            }

            for (int row = 1; row <= gridSize; row++)
            {
                var headerCell = bgOps.CreateHeaderCell(Numbers[row - 1].ToString());
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
                        Tag = $"{Numbers[col - 1]} {Numbers[row - 1]}"
                    };

                    Grid.SetRow(cell, row);
                    Grid.SetColumn(cell, col);

                    cell.MouseLeftButtonDown += OverlapCell_MouseLeftButtonDown;

                    grid.Children.Add(cell);
                }
            }
        }
        private static Image CreateMark()
        {
            var bitmap = new BitmapImage(new Uri("pack://application:,,,/Resources/Mark.png"));

            Image mark = new Image
            {
                Source = bitmap,
                Stretch = Stretch.Uniform,
                Width = bitmap.PixelWidth,
                Height = bitmap.PixelHeight,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            return mark;
        }
        private void OverlapCell_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image mark = CreateMark();
            if (sender is Border cell)
            {
                int row = Grid.GetRow(cell);
                int column = Grid.GetColumn(cell);

                Grid.SetRow(mark, row);
                Grid.SetColumn(mark, column);

                bool cellOccupied = LabeledBattleGridOverlap.Children
                                    .OfType<Image>()
                                    .Any(img => Grid.GetRow(img) == row && Grid.GetColumn(img) == column);

                if (!cellOccupied)
                {
                    LabeledBattleGridOverlap.Children.Add(mark);
                }
                if (Bot.IsShipPresent(row, column))
                {
                    cell.Background = new SolidColorBrush(Color.FromArgb(255, 247, 118, 106));
                    Bot.DestroyDeck(row, column);

                    if (Bot.IsShipDestroyed(row, column))
                    {
                        MarkDestroyedShip(Bot.GetDestroyedShipCells(row, column));
                    }
                }
            }
        }
        private void MarkDestroyedShip(List<(int row, int col)> destroyedShipCells)
        {
            foreach (var (row, col) in destroyedShipCells)
            {
                var cell = LabeledBattleGridOverlap.Children
                             .OfType<Border>()
                             .FirstOrDefault(b => Grid.GetRow(b) == row && Grid.GetColumn(b) == col);
                if (cell != null)
                {
                    cell.Opacity = 0.5;
                }
            }

            int size = destroyedShipCells.Count - 1;

            int startI = destroyedShipCells[0].row - 1 > 0 ? destroyedShipCells[0].row - 1 : destroyedShipCells[0].row;
            int stopI = destroyedShipCells[size].row + 1 < 11 ? destroyedShipCells[size].row + 1 : destroyedShipCells[size].row;

            int startJ = destroyedShipCells[0].col - 1 > 0 ? destroyedShipCells[0].col - 1 : destroyedShipCells[0].col;
            int stopJ = destroyedShipCells[size].col + 1 < 11 ? destroyedShipCells[size].col + 1 : destroyedShipCells[size].col;

            for (int i = startI; i <= stopI; i++)
            {
                for (int j = startJ; j <= stopJ; j++)
                {
                    Image mark = CreateMark();

                    bool cellOccupied = LabeledBattleGridOverlap.Children
                                    .OfType<Image>()
                                    .Any(img => Grid.GetRow(img) == i && Grid.GetColumn(img) == j);

                    if (!cellOccupied)
                    {
                        Grid.SetRow(mark, i);
                        Grid.SetColumn(mark, j);

                        LabeledBattleGridOverlap.Children.Add(mark);
                    }
                }
            }
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
