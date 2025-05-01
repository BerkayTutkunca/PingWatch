# PingWatch

**PingWatch** is a lightweight WPF desktop application that monitors the health of Raspberry Pi, BeagleBone, and other embedded IoT devices over a local network.

This tool performs parallel ICMP (ping) tests and local UDP loopback checks to help identify unreachable or firewall-blocked devices, especially in closed, internet-isolated environments.

---

## 🔧 Features

- ✅ Parallel ICMP Ping test for all configured devices  
- ✅ Local UDP loopback test to detect Windows Firewall blocking  
- ✅ Configuration via editable `config.json` file  
- ✅ Built-in editor to manage devices with IP validation  
- ✅ Tooltip showing local network adapter IP addresses  
- ✅ MVVM architecture and unit test support  
- ✅ Publish-ready build for end users

---

## 📂 Configuration (`config.json`)

Define the devices you want to monitor:

```json
{
  "Devices": [
    { "Name": "RaspberryPi", "IP": "192.168.1.10" },
    { "Name": "BeagleBone", "IP": "192.168.1.11" }
  ]
}