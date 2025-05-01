using PingWatch.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PingWatch.Models
{
    public class DeviceModel : INotifyPropertyChanged
    {
        private string name;

        private string ip;
        private ConnectionStatus status;
        private long? pingMs;

        public string Name
        {
            get => name;
            set => SetField(ref name, value);
        }

        public string IP
        {
            get => ip;
            set => SetField(ref ip, value);
        }

        public ConnectionStatus Status
        {
            get => status;
            set => SetField(ref status, value);
        }

        public long? PingMs
        {
            get => pingMs;
            set => SetField(ref pingMs, value);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

    }
}
