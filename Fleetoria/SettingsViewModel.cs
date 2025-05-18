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
        private int _interactionVolume;
        private string _selectedSkin;
        public ObservableCollection<string> SkinImages { get; }

        public int MusicVolume
        {
            get => _musicVolume;
            set { _musicVolume = value; OnPropertyChanged(); }
        }

        public int InteractionVolume
        {
            get => _interactionVolume;
            set { _interactionVolume = value; OnPropertyChanged(); }
        }

        public string SelectedSkin
        {
            get => _selectedSkin;
            set { _selectedSkin = value; OnPropertyChanged(); }
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
            InteractionVolume = settings.InteractionVolume;
            SelectedSkin = $"{basePath}{settings.SelectedSkinFolder}_logo.png";
        }

        public SettingsData ToSettingsData()
        {
            string folderName = System.IO.Path.GetFileNameWithoutExtension(SelectedSkin).Replace("_logo", "");
            return new SettingsData
            {
                MusicVolume = this.MusicVolume,
                InteractionVolume = this.InteractionVolume,
                SelectedSkinFolder = folderName
            };
        }

        private void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
