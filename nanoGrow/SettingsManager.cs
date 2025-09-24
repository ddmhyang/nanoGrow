using Newtonsoft.Json;
using System;
using System.IO;
// using System.Xml; // 이 줄을 삭제하거나 주석 처리합니다.

namespace nanoGrow
{
    public class SettingsManager
    {
        private readonly string _settingsFilePath;

        public SettingsManager()
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            _settingsFilePath = Path.Combine(documentsPath, "WpfPetGameSettings.json");
        }

        public void SaveSettings(AnimationData data)
        {
            string json = JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented); // 명확하게 지정
            File.WriteAllText(_settingsFilePath, json);
        }

        public AnimationData LoadSettings()
        {
            if (!File.Exists(_settingsFilePath))
            {
                return new AnimationData();
            }

            string json = File.ReadAllText(_settingsFilePath);
            return JsonConvert.DeserializeObject<AnimationData>(json);
        }
    }
}