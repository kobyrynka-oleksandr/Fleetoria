using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows;

namespace Fleetoria
{
    public class Ship : UserControl
    {
        public int DeckCount { get; private set; }
        public Image ShipImage { get; private set; }
        public bool isRotated { get; private set; }
        public bool isPlaced { get; set; }

        //public event Action<Ship> OnDragStart;

        private string baseImageName;
        public bool IsRotatedDuringDrag { get; set; } = false;

        public Ship(int deckCount)
        {
            DeckCount = deckCount;
            isRotated = true;
            isPlaced = false;
            InitializeShipImage();
        }

        private void InitializeShipImage()
        {
            baseImageName = DeckCount switch
            {
                1 => "1Deck_Ship",
                2 => "2Deck_Ship",
                3 => "3Deck_Ship",
                4 => "4Deck_Ship",
                _ => throw new ArgumentException("Неприпустима кількість палуб")
            };

            LoadShipImage();
            //MouseMove += Ship_MouseMove;
        }

        private void LoadShipImage()
        {
            string imageName = isRotated ? $"{baseImageName}_rotated.png" : $"{baseImageName}.png";
            var bitmap = new BitmapImage(new Uri($"pack://application:,,,/Resources/{imageName}"));

            ShipImage = new Image
            {
                Source = bitmap,
                Stretch = Stretch.Uniform,
                Width = bitmap.PixelWidth,
                Height = bitmap.PixelHeight,
                RenderTransform = new TranslateTransform(),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            Content = ShipImage;
        }

        //private void Ship_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (e.LeftButton == MouseButtonState.Pressed)
        //    {
        //        OnDragStart?.Invoke(this);
        //        DragDrop.DoDragDrop(this, this, DragDropEffects.Move);
        //    }
        //}

        public void Rotate()
        {
            if (!isRotated)
                isRotated = true;
            else
                isRotated = false;
            LoadShipImage();
        }
        public void ResetSpan()
        {
            Grid.SetRowSpan(this, 1);
            Grid.SetColumnSpan(this, 1);
        }
    }

}
