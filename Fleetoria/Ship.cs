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

        public event Action<Ship> OnDragStart;

        public Ship(int deckCount)
        {
            DeckCount = deckCount;
            isRotated = false;
            isPlaced = false;
            InitializeShipImage();
        }

        private void InitializeShipImage()
        {
            string imageName = DeckCount switch
            {
                1 => "1Deck_Ship.png",
                2 => "2Deck_Ship.png",
                3 => "3Deck_Ship.png",
                4 => "4Deck_Ship.png",
                _ => throw new ArgumentException("Неприпустима кількість палуб")
            };

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

            MouseMove += Ship_MouseMove;
        }
        private void Ship_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                OnDragStart?.Invoke(this);
                DragDrop.DoDragDrop(this, this, DragDropEffects.Move);
            }
        }
    }
}
