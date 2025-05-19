using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
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

        PlayerHuman Human = new PlayerHuman();
        PlayerBot Bot;

        private bool isBotTurn = false;
        private enum AttackResult { Miss, Hit }

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
                    CustomMessageBox.Show("Unable to place all ships. Please try again.");
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
            if (Human.GetHealth() != 20)
            {
                CustomMessageBox.Show("Place all ships!", "OK");
                return;
            }
            if (selectedDifficultyButton == null)
            {
                CustomMessageBox.Show("Choose difficulty!", "OK");
                return;
            }
            LabeledBattleGridHuman.AllowDrop = false;
            if (selectedDifficultyButton == EasyButton)
            {
                Bot = new PlayerEasyBot();
            }
            else if (selectedDifficultyButton == HardButton)
            {
                Bot = new PlayerHardBot(Human);
            }

            bgOps.AddShipsToBotGrid(LabeledBattleGridBot, Bot);

            LabeledBattleGridOverlap.Children.Clear();
            LabeledBattleGridOverlap.RowDefinitions.Clear();
            LabeledBattleGridOverlap.ColumnDefinitions.Clear();

            bgOps.CreateOverlapGrid(LabeledBattleGridOverlap, OverlapCell_MouseLeftButtonDown);

            StartButton.Visibility = Visibility.Collapsed;
            DifficultyButtons.Visibility = Visibility.Collapsed;
            ResetButton.Visibility = Visibility.Collapsed;
            ShuffleButton.Visibility = Visibility.Collapsed;
            ShipBorder.Visibility = Visibility.Collapsed;

            TurnArrowCanvas.Visibility = Visibility.Visible;

            HumanHealthBorder.Visibility = Visibility.Visible;
            BotHealthBorder.Visibility = Visibility.Visible;
            UpdateHealthDisplays();
        }

        private async void OverlapCell_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isBotTurn) return;

            if (sender is Border cell)
            {
                int row = Grid.GetRow(cell);
                int column = Grid.GetColumn(cell);

                bool cellOccupied = LabeledBattleGridOverlap.Children
                    .OfType<Image>()
                    .Any(img => Grid.GetRow(img) == row && Grid.GetColumn(img) == column);

                if (cellOccupied) return;

                AttackResult playerResult = AttackCell(Bot, cell, row, column, LabeledBattleGridOverlap);

                if (Bot.GetHealth() == 0)
                {
                    EndGame("Human win!");
                    return;
                }

                if (playerResult == AttackResult.Miss)
                {
                    UpdateArrowDirection();
                    await BotTurn();
                }
            }
        }

        private AttackResult AttackCell(Player target, Border cell, int row, int col, Grid grid)
        {
            Image mark = bgOps.CreateMark();
            Grid.SetRow(mark, row);
            Grid.SetColumn(mark, col);
            grid.Children.Add(mark);

            if (target.IsShipPresent(row, col))
            {
                cell.Background = new SolidColorBrush(Color.FromArgb(255, 247, 118, 106));
                target.DestroyDeck(row, col);
                UpdateHealthDisplays();

                if (target.IsShipDestroyed(row, col))
                {
                    var destroyedShip = target.GetDestroyedShipCells(row, col);

                    if (target == Bot)
                    {
                        MarkDestroyedShip(destroyedShip);
                    }
                    else
                    {
                        MarkDestroyedShip(destroyedShip, grid);
                    }
                }

                return AttackResult.Hit;
            }

            return AttackResult.Miss;
        }

        private async Task BotTurn()
        {
            isBotTurn = true;
            UpdateArrowDirection();

            await Task.Delay(700);

            AttackResult botResult;
            do
            {
                var cellForAttack = Bot.GetCellForAttack();

                Image mark = bgOps.CreateMark();

                Border markContainer = new Border
                {
                    Width = 40,
                    Height = 40,
                    Background = Brushes.Transparent,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    BorderThickness = new Thickness(0),
                    Child = mark
                };

                int row = cellForAttack.row + 1;
                int col = cellForAttack.col + 1;

                Grid.SetRow(markContainer, row);
                Grid.SetColumn(markContainer, col);
                LabeledBattleGridHuman.Children.Add(markContainer);

                if (Human.IsShipPresent(row, col))
                {
                    markContainer.Background = new SolidColorBrush(Color.FromArgb(125, 247, 118, 106));
                    Human.DestroyDeck(row, col);
                    UpdateHealthDisplays();

                    if (Human.IsShipDestroyed(row, col))
                    {
                        var destroyed = Human.GetDestroyedShipCells(row, col);
                        MarkDestroyedShip(destroyed, LabeledBattleGridHuman);
                    }

                    botResult = AttackResult.Hit;
                }
                else
                {
                    botResult = AttackResult.Miss;
                }

                await Task.Delay(500);

            } while (botResult == AttackResult.Hit && Human.GetHealth() > 0);

            isBotTurn = false;
            UpdateArrowDirection();

            if (Human.GetHealth() == 0)
            {
                EndGame("Bot win!");
            }
        }

        private void EndGame(string message)
        {
            CustomMessageBox.Show($"{message}", "OK");

            StartButton.Visibility = Visibility.Visible;
            DifficultyButtons.Visibility = Visibility.Visible;
            ResetButton.Visibility = Visibility.Visible;
            ShuffleButton.Visibility = Visibility.Visible;
            ShipBorder.Visibility = Visibility.Visible;

            bgOps.ResetBattleGrid(LabeledBattleGridHuman, Human, ShipPanel);
            bgOps.ResetBattleGrid(LabeledBattleGridBot, Bot);
            LabeledBattleGridOverlap.Children.Clear();
            LabeledBattleGridOverlap.RowDefinitions.Clear();
            LabeledBattleGridOverlap.ColumnDefinitions.Clear();

            TurnArrowCanvas.Visibility = Visibility.Collapsed;

            HumanHealthBorder.Visibility = Visibility.Collapsed;
            BotHealthBorder.Visibility = Visibility.Collapsed;

            LabeledBattleGridHuman.AllowDrop = true;

            isBotTurn = false;
            UpdateArrowDirection();
        }
        public Border GetCellFromGrid(Grid grid, int row, int column)
        {
            foreach (UIElement element in grid.Children)
            {
                if (Grid.GetRow(element) == row && Grid.GetColumn(element) == column && element is Border)
                    return (Border)element;
            }
            return null;
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
                    Image mark = bgOps.CreateMark();

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
        private void MarkDestroyedShip(List<(int row, int col)> destroyedShipCells, Grid grid)
        {
            int size = destroyedShipCells.Count - 1;

            int startI = destroyedShipCells[0].row - 1 > 0 ? destroyedShipCells[0].row - 1 : destroyedShipCells[0].row;
            int stopI = destroyedShipCells[size].row + 1 < 11 ? destroyedShipCells[size].row + 1 : destroyedShipCells[size].row;

            int startJ = destroyedShipCells[0].col - 1 > 0 ? destroyedShipCells[0].col - 1 : destroyedShipCells[0].col;
            int stopJ = destroyedShipCells[size].col + 1 < 11 ? destroyedShipCells[size].col + 1 : destroyedShipCells[size].col;

            for (int i = startI; i <= stopI; i++)
            {
                for (int j = startJ; j <= stopJ; j++)
                {
                    Image mark = bgOps.CreateMark();

                    bool cellOccupied = grid.Children
                                    .OfType<Image>()
                                    .Any(img => Grid.GetRow(img) == i && Grid.GetColumn(img) == j);

                    if (!cellOccupied)
                    {
                        Grid.SetRow(mark, i);
                        Grid.SetColumn(mark, j);
                        grid.Children.Add(mark);

                        Bot.RemoveMarkedCell(i - 1, j - 1);
                    }
                }
            }
        }
        private void UpdateArrowDirection()
        {
            var rotate = (RotateTransform)TurnArrowImage.RenderTransform;
            rotate.Angle = isBotTurn ? 180 : 0;
        }
        private void UpdateHealthDisplays()
        {
            HumanHealthText.Text = $"Human Health: {Human.GetHealth()}";
            BotHealthText.Text = $"Bot Health: {Bot.GetHealth()}";
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            var result = CustomMessageBox.Show("The current state will not be saved. Are you sure?", "Yes", "No");
            if (result == CustomMessageBox.MessageBoxResult.Yes)
            {
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
            }
            else
            {
                return;
            }
        }
    }
}
