using PingWatch.Models;
using PingWatch.Models.Enums;
using PingWatch.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace PingWatch.ViewModels
{
    public class MainViewModel
    {
        public ObservableCollection<DeviceModel> Devices { get; set; } = new();

        private readonly INetworkTesterService _networkTester;

        public MainViewModel(INetworkTesterService networkTester)
        {
            _networkTester = networkTester;
        }


        public async Task LoadDevicesFromConfigAsync()
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

                if (!File.Exists(path))
                {
                    MessageBox.Show("config.json dosyası bulunamadı.", "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string json = await File.ReadAllTextAsync(path);
                var config = JsonSerializer.Deserialize<ConfigModel>(json);

                Devices.Clear();

                if (config?.Devices != null)
                {
                    foreach (var device in config.Devices)
                    {
                        Devices.Add(device);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cihaz yapılandırma dosyası okunurken hata oluştu:\n" + ex.Message,
                                "Hata", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public async Task TestAllAsync()
        {
            //Max 10 ping at the same time
            using var semaphore = new SemaphoreSlim(10);

            var tasks = Devices.Select(async device =>
            {
                await semaphore.WaitAsync();
                try
                {
                    var (success, ms) = await _networkTester.PingAsync(device.IP);
                    device.Status = success ? ConnectionStatus.Online : ConnectionStatus.Offline;
                    device.PingMs = ms;
                }
                finally { semaphore.Release(); }
            });
            await Task.WhenAll(tasks);
        }

        public async Task<bool> TestFirewallUdpAsync()
        {
            return await _networkTester.TestUdpLoopBackAsync();
        }

        public async Task TestSingleDeviceAsync(DeviceModel device)
        {
            var (success, ms) = await _networkTester.PingAsync(device.IP);
            device.Status = success ? ConnectionStatus.Online : ConnectionStatus.Offline;
            device.PingMs = ms;
        }

        public async Task<List<string>> GetLocalIPAddressesAsync()
        {
            return await Task.Run(() =>
            {
                return _networkTester.GetLocalIPAddresses();
            });
        }
    }
}
