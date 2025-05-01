using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PingWatch.Services
{
    public class NetworkTesterService : INetworkTesterService
    {
        private const int PingTimeoutMilliseconds = 1000;
        private const int UdpReceiveTimeoutMilliseconds = 2000;
        private const int MinUdpPort = 40000;
        private const int MaxUdpPort = 60000;
        public async Task<(bool success, long? pingMs)> PingAsync(string IP)
        {
            try
            {
                using Ping ping = new Ping();
                var reply = await ping.SendPingAsync(IP, PingTimeoutMilliseconds);
                return (reply.Status == IPStatus.Success, reply.RoundtripTime);
            }
            catch
            {
                return (false, null);
            }
        }

        public async Task<bool> TestUdpLoopBackAsync(string message = "udp_test")
        {

            int port = new Random().Next(MinUdpPort, MaxUdpPort);
            var endpoint = new IPEndPoint(IPAddress.Loopback, port);
            var cts = new CancellationTokenSource();


            var receiveTask = Task.Run(() =>
            {
                try
                {
                    using var receiver = new UdpClient(port);
                    receiver.Client.ReceiveTimeout = UdpReceiveTimeoutMilliseconds;

                    var remoteEp = new IPEndPoint(IPAddress.Any, 0);
                    var receivedData = receiver.Receive(ref remoteEp);
                    string receivedMessage = Encoding.UTF8.GetString(receivedData);
                    return receivedMessage == message;

                }
                catch
                {
                    return false;
                }

            },cts.Token);

            try
            {
                using var sender = new UdpClient();
                byte[] data = Encoding.UTF8.GetBytes(message);
                await sender.SendAsync(data, data.Length, "127.0.0.1", port);
            }
            catch
            {
                return false;
            }

            return await receiveTask;
        }

        public List<string> GetLocalIPAddresses()
        {
            var result = new List<string>();

            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus != OperationalStatus.Up)
                    continue;

                var ipProps = ni.GetIPProperties();

                foreach (var addr in ipProps.UnicastAddresses)
                {
                    if (addr.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        string name = ni.Name;
                        string desc = ni.Description;
                        string ip = addr.Address.ToString();
                        string label = ni.NetworkInterfaceType switch
                        {
                            NetworkInterfaceType.Wireless80211 => "Wi-Fi",
                            NetworkInterfaceType.Ethernet => "Ethernet",
                            _ => ni.NetworkInterfaceType.ToString()
                        };

                        result.Add($"{label} ({name}): {ip}");
                    }
                }
            }

            return result;
        }

    }
}
