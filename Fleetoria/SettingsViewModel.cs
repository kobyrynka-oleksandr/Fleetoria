using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Fleetoria
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int _musicVolume;
        private string _player1Skin;
        private string _player2Skin;

        public ObservableCollection<string> SkinImages { get; }

        public int MusicVolume
        {
            get => _musicVolume;
            set { _musicVolume = value; OnPropertyChanged(); }
        }

        public string Player1Skin
        {
            get => _player1Skin;
            set { _player1Skin = value; OnPropertyChanged(); }
        }

        public string Player2Skin
        {
            get => _player2Skin;
            set { _player2Skin = value; OnPropertyChanged(); }
        }

        public SettingsViewModel()
        {
            string basePath = "/Resources/Ship_skins/Ship_skin_logos/";
            SkinImages = new ObservableCollection<string>
        {
            $"{basePath}Ship_skin_1_logo.png",
            $"{basePath}Ship_skin_2_logo.png"
        };

            var settings = SettingsManager.LoadSettings();

            MusicVolume = settings.MusicVolume;
            Player1Skin = $"{basePath}{settings.Player1Skin}_logo.png";
            Player2Skin = $"{basePath}{settings.Player2Skin}_logo.png";
        }

        public SettingsData ToSettingsData()
        {
            string skin1 = System.IO.Path.GetFileNameWithoutExtension(Player1Skin).Replace("_logo", "");
            string skin2 = System.IO.Path.GetFileNameWithoutExtension(Player2Skin).Replace("_logo", "");
            return new SettingsData
            {
                MusicVolume = this.MusicVolume,
                Player1Skin = skin1,
                Player2Skin = skin2
            };
        }

        private void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
