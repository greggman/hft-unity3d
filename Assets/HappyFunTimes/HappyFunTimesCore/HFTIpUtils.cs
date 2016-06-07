using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;

namespace HappyFunTimes
{
    public class HFTIpUtils {

        static bool IsIpv4AddressWeCareAbout(IPAddress ip)
        {
            return true;
        }

        static bool IsIpv6AddressWeCareAbout(IPAddress ip)
        {
            string ipv6 = ip.ToString();
            bool weCare = !(ipv6.StartsWith("fe80") ||
                            ipv6.StartsWith("ff00") ||
                            ipv6.StartsWith("fd00") ||
                            ipv6.StartsWith("fec0"));
            return weCare;
        }

        static string GetLocalIPAddress(IPAddress address) {
            try
            {
                HFTLog.Global.Info("Trying: " + address.ToString());
                string localIP;
                using (System.Net.Sockets.Socket socket = new System.Net.Sockets.Socket(address.AddressFamily, System.Net.Sockets.SocketType.Dgram, 0))
                {
                    socket.Connect(address, 65530);
                    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                    string s = endPoint.Address.ToString();
                    localIP = System.Text.RegularExpressions.Regex.Replace(s, "%.+", "");
                }
                return localIP;
            }
            catch (System.Exception ex)
            {
                HFTLog.Global.Info(ex.ToString());
                return null;
            }
        }

        static void AddLocalIPAddress(string address, List<string> addresses)
        {
            string ip = GetLocalIPAddress(IPAddress.Parse(address));
            if (!String.IsNullOrEmpty(ip))
            {
                if (ip.IndexOf(":") >= 0)
                {
                    addresses.Add("[" + ip + "]");
                }
                else
                {
                    addresses.Add(ip);
                }
            }
        }

        public static string[] GetLocalIPAddresses()
        {
            List<string> addresses = new List<string>();
            AddLocalIPAddress("10.253.253.253", addresses);
            AddLocalIPAddress("3d97:8cfc:3440:1234:4567:5678:1234:123E", addresses);
            AddLocalIPAddress("::FFFF:10.253.253.253", addresses);
            return addresses.ToArray();
        }

        public static string GetLocalIPv4Address()
        {
            return GetLocalIPAddress(IPAddress.Parse("10.253.253.253"));
        }

        public static string GetLocalIPv6Address()
        {
            return GetLocalIPAddress(IPAddress.Parse("3d97:8cfc:3440:1234:4567:5678:1234:123E"));
        }

        // This one does let me check en0 vs vnet0 etc but how do I know
        // which one to use?
        #if FALSE
        public static string[] GetLocalIPAddresses()
        {
            List<string> ipAddrList = new List<string>();
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                var _type = NetworkInterfaceType.Ethernet;
                HFTLog.Global.Tell(
                    "----" +
                "\n  item.Description +          :" +  item.Description +
                "\n  item.Id +                   :" +  item.Id +
                "\n  item.IsReceiveOnly +        :" +  item.IsReceiveOnly +
                "\n  item.Name +                 :" +  item.Name +
                "\n  item.NetworkInterfaceType + :" +  item.NetworkInterfaceType +
                "\n  item.OperationalStatus +    :" +  item.OperationalStatus +
                "\n  item.Speed +                :" +  item.Speed +
                "\n  item.SupportsMulticast +    :" +  item.SupportsMulticast +
                "");
                if (item.NetworkInterfaceType == _type/* && item.OperationalStatus == OperationalStatus.Up*/)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        HFTLog.Global.Tell("    ----" +
                        "\n      ip.Address +        : " + ip.Address +
                        "\n      ip.IsDnsEligible +  : " + ip.IsDnsEligible +
                        "\n      ip.IsTransient +    : " + ip.IsTransient +
                        "");
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            ipAddrList.Add(ip.Address.ToString());
                        }
                    }
                }
            }
            return ipAddrList.ToArray();
        }
        #endif

        // This one doesn't let me distinguish between en0 and vnet0
        #if FALSE
        public static string[] GetLocalIPAddresses()
        {
            List<string> ipaddresses = new List<string>();
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && IsIpv4AddressWeCareAbout(ip))
                {
                    ipaddresses.Add(ip.ToString());
                }
                else if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6 && IsIpv6AddressWeCareAbout(ip))
                {
                    ipaddresses.Add("[" + ip.ToString() + "]");
                }
            }
            return ipaddresses.ToArray();
        }
        #endif
    }

}
