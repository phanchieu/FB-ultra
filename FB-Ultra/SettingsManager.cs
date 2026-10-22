using System;
using System.IO;
using Newtonsoft.Json;

namespace FB_Ultra
{
    public static class SettingsManager
    {
        private static readonly string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");

        public static SettingsModel LoadSettings()
        {
            try
            {
                // Nếu file chưa tồn tại, tự động tạo mới với giá trị mặc định luôn
                if (!File.Exists(filePath))
                {
                    SettingsModel defaultSettings = new SettingsModel();
                    SaveSettings(defaultSettings); // Gọi hàm lưu để sinh ra file json mặc định
                    return defaultSettings;
                }

                // Nếu đã có file thì đọc lên
                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<SettingsModel>(json) ?? new SettingsModel();
            }
            catch (Exception)
            {
                // Nếu lỗi đọc file hoặc deserialize thì trả về mặc định
            }
            return new SettingsModel();
        }

        public static void SaveSettings(SettingsModel settings)
        {
            try
            {
                string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
            catch (Exception) { }
        }
    }
}