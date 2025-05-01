using PingWatch.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Net;

namespace PingWatch
{
    public partial class ConfigEditorWindow : Window
    {
        private readonly string configPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
        public ObservableCollection<DeviceModel> Devices { get; set; } = new();

        public ConfigEditorWindow()
        {
            InitializeComponent();
            LoadConfig();
            deviceGrid.ItemsSource = Devices;
        }
        private void LoadConfig()
        {
            if (!File.Exists(configPath))
                return;

            string json = File.ReadAllText(configPath);
            var config = JsonSerializer.Deserialize<ConfigModel>(json);
            if (config?.Devices != null)
            {
                foreach (var device in config.Devices)
                    Devices.Add(device);
            }
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            foreach (var device in Devices)
            {
                if (string.IsNullOrWhiteSpace(device.Name) || string.IsNullOrWhiteSpace(device.IP))
                {
                    MessageBox.Show("Tüm cihazlar için Ad ve IP girilmelidir.", "Eksik Bilgi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!IsValidIp(device.IP))
                {
                    MessageBox.Show($"Geçersiz IP adresi: {device.IP}", "Hatalı IP", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
            
            var newConfig = new ConfigModel { Devices = Devices.ToList() };
            string json = JsonSerializer.Serialize(newConfig, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(configPath, json);
            this.Close();
        }
        private bool IsValidIp(string ip)
        {
            return IPAddress.TryParse(ip, out _);
        }

    }
}
