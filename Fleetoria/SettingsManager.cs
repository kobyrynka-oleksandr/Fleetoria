using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace Fleetoria
{
    public static class SettingsManager
    {
        private static string settingsPath = "settings.json";

        public static SettingsData LoadSettings()
        {
            if (!File.Exists(settingsPath))
                return new SettingsData { MusicVolume = 50, InteractionVolume = 50, SelectedSkinFolder = "Ship_skin_1" };

            string json = File.ReadAllText(settingsPath);
            return JsonConvert.DeserializeObject<SettingsData>(json);
        }

        public static void SaveSettings(SettingsData settings)
        {
            string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(settingsPath, json);
        }
    }
}
