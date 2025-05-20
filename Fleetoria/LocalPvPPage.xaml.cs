using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Fleetoria
{
    public partial class LocalPvPPage : PageWithScaling
    {
        private readonly char[] Letters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' };
        private readonly int[] Numbers = Enumerable.Range(1, 10).ToArray();

        PlayerHuman human1 = new PlayerHuman();
        PlayerHuman human2 = new PlayerHuman();

        string human1Skin = SettingsManager.LoadSettings().Player1Skin;
        string human2Skin = SettingsManager.LoadSettings().Player2Skin;

        private bool isSecondHumanTurn = false;

        private enum AttackResult { Miss, Hit }

        BattleGridOperations bgOps = new BattleGridOperations();

        private bool isShowedPlayer1 = false;
        private bool isShowedPlayer2 = false;

        public LocalPvPPage()
        {
            InitializeComponent();

            this.Focusable = true;
            this.Focus();
            Keyboard.Focus(this);

            DataContext = this;

            bgOps.CreateBattlefield(LabeledBattleGridHuman1);

            bgOps.AddShipsToPanel(ShipPanel1, human1, LabeledBattleGridHuman1, human1Skin);

            bgOps.CreateBattlefield(LabeledBattleGridHuman2);

            bgOps.AddShipsToPanel(ShipPanel2, human2, LabeledBattleGridHuman2, human2Skin);
        }

        private void ResetButton1_Click(object sender, RoutedEventArgs e)
        {
            ResetShips(LabeledBattleGridHuman1, human1, ShipPanel1, human1Skin);
        }

        private void ResetButton2_Click(object sender, RoutedEventArgs e)
        {
            ResetShips(LabeledBattleGridHuman2, human2, ShipPanel2, human2Skin);
        }

        private void ShuffleButton1_Click(object sender, RoutedEventArgs e)
        {
            ShuffleShips(LabeledBattleGridHuman1, human1, ShipPanel1, human1Skin);
        }

        private void ShuffleButton2_Click(object sender, RoutedEventArgs e)
        {
            ShuffleShips(LabeledBattleGridHuman2, human2, ShipPanel2, human2Skin);
        }

        private void ResetShips(Grid grid, PlayerHuman human, Panel panel, string skinName)
        {
            bgOps.ResetBattleGrid(grid, human, panel, skinName);
        }
        private void Ship_DragOver_Wrapper1(object sender, DragEventArgs e)
        {
            if (sender is Grid grid)
            {
                Ship_DragOver(sender, e, grid, human1);
            }
        }
        private void Ship_DragOver_Wrapper2(object sender, DragEventArgs e)
        {
            if (sender is Grid grid)
            {
                Ship_DragOver(sender, e, grid, human2);
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

        private void ShuffleShips(Grid grid, PlayerHuman human, Panel panel, string skinName)
        {
            bgOps.ResetBattleGrid(grid, human, panel, skinName);
            var random = new Random();

            foreach (Ship ship in panel.Children.OfType<Ship>().ToList())
            {
                if (!bgOps.TryPlaceShipRandomly(grid, panel, human, ship, random))
                {
                    CustomMessageBox.Show("Unable to place all ships. Please try again.");
                    return;
                }
            }
        }
        private void ToSecondPlayerButton_Click(object sender, RoutedEventArgs e)
        {
            if (human1.GetHealth() != 20)
            {
                CustomMessageBox.Show("Place all ships!", "OK");
                return;
            }
            LabeledBattleGridHuman1.AllowDrop = false;
            LabeledBattleGridHuman2.AllowDrop = true;

            ShipBorder1.Visibility = Visibility.Collapsed;
            ToSecondPlayerButton.Visibility = Visibility.Collapsed;
            ResetButton1.Visibility = Visibility.Collapsed;
            ShuffleButton1.Visibility = Visibility.Collapsed;

            ShipBorder2.Visibility = Visibility.Visible;
            StartButton.Visibility = Visibility.Visible;
            ResetButton2.Visibility = Visibility.Visible;
            ShuffleButton2.Visibility = Visibility.Visible;

            bgOps.CreateOverlapGrid(LabeledBattleGridOverlap1, OverlapCell1_MouseLeftButtonDown);
        }
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (human2.GetHealth() != 20)
            {
                CustomMessageBox.Show("Place all ships!", "OK");
                return;
            }
            LabeledBattleGridHuman2.AllowDrop = false;

            ShipBorder2.Visibility = Visibility.Collapsed;
            StartButton.Visibility = Visibility.Collapsed;
            ResetButton2.Visibility = Visibility.Collapsed;
            ShuffleButton2.Visibility = Visibility.Collapsed;

            HumanHealthBorder1.Visibility = Visibility.Visible;
            HumanHealthBorder2.Visibility = Visibility.Visible;

            TurnArrowCanvas.Visibility = Visibility.Visible;

            bgOps.CreateOverlapGrid(LabeledBattleGridOverlap2, OverlapCell2_MouseLeftButtonDown);

            PlacementButton1.Visibility = Visibility.Visible;

            UpdateHealthDisplays();
        }
        private void OverlapCell1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!isSecondHumanTurn) return;

            if (sender is Border cell)
            {
                int row = Grid.GetRow(cell);
                int column = Grid.GetColumn(cell);

                bool cellOccupied = LabeledBattleGridOverlap1.Children
                    .OfType<Image>()
                    .Any(img => Grid.GetRow(img) == row && Grid.GetColumn(img) == column);

                if (cellOccupied) return;

                AttackResult playerResult = AttackCell(human1, cell, row, column, LabeledBattleGridOverlap1);

                if (human1.GetHealth() == 0)
                {
                    EndGame("Player 2 win!");
                    return;
                }

                if (playerResult == AttackResult.Miss)
                {
                    PlacementButton2.Visibility = Visibility.Collapsed;
                    isShowedPlayer2 = false;
                    ShowAndHidePlacement(isShowedPlayer2, LabeledBattleGridOverlap2, PlacementButton2);

                    isSecondHumanTurn = false;

                    PlacementButton1.Visibility = Visibility.Visible;

                    UpdateArrowDirection();
                }
            }
        }
        private void OverlapCell2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isSecondHumanTurn) return;

            if (sender is Border cell)
            {
                int row = Grid.GetRow(cell);
                int column = Grid.GetColumn(cell);

                bool cellOccupied = LabeledBattleGridOverlap2.Children
                    .OfType<Image>()
                    .Any(img => Grid.GetRow(img) == row && Grid.GetColumn(img) == column);

                if (cellOccupied) return;

                AttackResult playerResult = AttackCell(human2, cell, row, column, LabeledBattleGridOverlap2);

                if (human2.GetHealth() == 0)
                {
                    EndGame("Player 1 win!");
                    return;
                }

                if (playerResult == AttackResult.Miss)
                {
                    PlacementButton1.Visibility = Visibility.Collapsed;
                    isShowedPlayer1 = false;
                    ShowAndHidePlacement(isShowedPlayer1, LabeledBattleGridOverlap1, PlacementButton1);

                    isSecondHumanTurn = true;

                    PlacementButton2.Visibility = Visibility.Visible;

                    UpdateArrowDirection();
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

                    bgOps.MarkDestroyedShip(destroyedShip, grid);
                }

                return AttackResult.Hit;
            }

            return AttackResult.Miss;
        }
        private void EndGame(string message)
        {
            CustomMessageBox.Show($"{message}", "OK");

            bgOps.ResetBattleGrid(LabeledBattleGridHuman1, human1, ShipPanel1, human1Skin);
            bgOps.ResetBattleGrid(LabeledBattleGridHuman2, human2, ShipPanel2, human2Skin);

            LabeledBattleGridOverlap1.Children.Clear();
            LabeledBattleGridOverlap1.RowDefinitions.Clear();
            LabeledBattleGridOverlap1.ColumnDefinitions.Clear();

            LabeledBattleGridOverlap2.Children.Clear();
            LabeledBattleGridOverlap2.RowDefinitions.Clear();
            LabeledBattleGridOverlap2.ColumnDefinitions.Clear();

            ShipBorder1.Visibility = Visibility.Visible;
            ToSecondPlayerButton.Visibility = Visibility.Visible;
            ResetButton1.Visibility = Visibility.Visible;
            ShuffleButton1.Visibility = Visibility.Visible;

            TurnArrowCanvas.Visibility = Visibility.Collapsed;

            HumanHealthBorder1.Visibility = Visibility.Collapsed;
            HumanHealthBorder2.Visibility = Visibility.Collapsed;

            PlacementButton1.Visibility = Visibility.Collapsed;
            PlacementButton2.Visibility = Visibility.Collapsed;
            isShowedPlayer1 = false;
            isShowedPlayer2 = false;
            ShowAndHidePlacement(isShowedPlayer1, LabeledBattleGridOverlap1, PlacementButton1);
            ShowAndHidePlacement(isShowedPlayer2, LabeledBattleGridOverlap2, PlacementButton2);

            LabeledBattleGridHuman1.AllowDrop = true;
            LabeledBattleGridHuman2.AllowDrop = false;

            isSecondHumanTurn = false;
            UpdateArrowDirection();
        }
        private void UpdateHealthDisplays()
        {
            HumanHealthText1.Text = $"Player 1 Health: {human1.GetHealth()}";
            HumanHealthText2.Text = $"Player 2 Health: {human2.GetHealth()}";
        }
        private void UpdateArrowDirection()
        {
            var rotate = (RotateTransform)TurnArrowImage.RenderTransform;
            rotate.Angle = isSecondHumanTurn ? 180 : 0;
        }

        private void PlacementButton1_Click(object sender, RoutedEventArgs e)
        {
            isShowedPlayer1 = !isShowedPlayer1;
            ShowAndHidePlacement(isShowedPlayer1, LabeledBattleGridOverlap1, PlacementButton1);
        }
        private void PlacementButton2_Click(object sender, RoutedEventArgs e)
        {
            isShowedPlayer2 = !isShowedPlayer2;
            ShowAndHidePlacement(isShowedPlayer2, LabeledBattleGridOverlap2, PlacementButton2);
        }
        private void ShowAndHidePlacement(bool isShowed, Grid grid, Button placementButton)
        {
            if (isShowed)
            {
                grid.Visibility = Visibility.Collapsed;
                placementButton.Content = "Hide placement";
            }
            else
            {
                grid.Visibility = Visibility.Visible;
                placementButton.Content = "Show placement";
            }
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
