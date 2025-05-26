using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Fleetoria
{
    public static class MusicPlayer
    {
        private static MediaPlayer _mediaPlayer;
        private static bool _isInitialized = false;

        public static void Initialize()
        {
            if (_isInitialized) return;

            _mediaPlayer = new MediaPlayer();
            _mediaPlayer.Open(new Uri("Resources/BackgroundMusic.mp3", UriKind.Relative));
            _mediaPlayer.Volume = SettingsManager.LoadSettings().MusicVolume / 100.0;
            _mediaPlayer.MediaEnded += (s, e) => _mediaPlayer.Position = TimeSpan.Zero;
            _mediaPlayer.Play();
            _isInitialized = true;
        }

        public static void SetVolume(double volume)
        {
            if (_mediaPlayer != null)
                _mediaPlayer.Volume = volume / 100.0;
        }

        public static void Stop()
        {
            _mediaPlayer?.Stop();
        }
    }
}
