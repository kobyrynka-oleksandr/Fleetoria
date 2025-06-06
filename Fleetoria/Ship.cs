﻿using System;
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

        private string baseImageName;
        public bool IsRotatedDuringDrag { get; set; } = false;
        protected virtual string SkinFolder => "Ship_skin_1";

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
                _ => throw new ArgumentException("Invalid number of decks")
            };

            LoadShipImage();
        }

        private void LoadShipImage()
        {
            string imageName = isRotated ? $"{baseImageName}_rotated.png" : $"{baseImageName}.png";
            var bitmap = new BitmapImage(new Uri($"pack://application:,,,/Resources/Ship_skins/{SkinFolder}/{imageName}"));

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

        public void Rotate()
        {
            if (!isRotated)
                isRotated = true;
            else
                isRotated = false;
            LoadShipImage();
        }
        public void Rotate(bool isRotated)
        {
            this.isRotated = isRotated;
            LoadShipImage();
        }
        public void ResetSpan()
        {
            Grid.SetRowSpan(this, 1);
            Grid.SetColumnSpan(this, 1);
        }
    }
}
