using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PingWatch.Services
{
    public interface INetworkTesterService
    {
        Task<(bool success, long? pingMs)> PingAsync(string ip);
        Task<bool> TestUdpLoopBackAsync(string message ="udp_test");

        List<string> GetLocalIPAddresses();
    }
}
