using PingWatch.Models;
using PingWatch.Models.Enums;
using PingWatch.Services;
using PingWatch.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PingWatch
{
    public partial class MainWindow : Window
    {
        private MainViewModel viewModel = new (new NetworkTesterService());
        public MainWindow()
        {
            InitializeComponent();

            DataContext = viewModel;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await viewModel.TestAllAsync();
        }

        private async void deviceGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (deviceGrid.SelectedItem is DeviceModel selectedDevice)
            {
                await viewModel.TestSingleDeviceAsync(selectedDevice);
            }

        }
        private async void Firewall_Button_Click(object sender, RoutedEventArgs e)
        {
            bool result = await viewModel.TestFirewallUdpAsync();

            string msg = result ? "UDP bağlantısı çalışıyor. Firewall engellemiyor" : "UDP paketi alınamadı. Firewall engelliyor olabilir...";
            MessageBox.Show(msg, "UDP Test Sonucu", MessageBoxButton.OK, MessageBoxImage.Information);

        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await viewModel.LoadDevicesFromConfigAsync();
            var ipList = await viewModel.GetLocalIPAddressesAsync();
            string tooltipText = string.Join("\n", ipList);

            ipInfoLabel.ToolTip = tooltipText;

        }

        private async void EditConfig_Click(object sender, RoutedEventArgs e)
        {
            var editor = new ConfigEditorWindow();
            editor.Owner = this;
            editor.ShowDialog();

            await viewModel.LoadDevicesFromConfigAsync();

        }
    }
}
