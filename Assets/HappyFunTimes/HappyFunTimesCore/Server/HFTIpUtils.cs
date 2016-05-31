using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace HappyFunTimes
{
    public class HFTIpUtils {

        static bool IsIpv4AddressWeCareAbout(string ipv4)
        {
            return true;
        }

        static bool IsIpv6AddressWeCareAbout(string ipv6)
        {
            bool weCare = !(ipv6.StartsWith("fe80") ||
                            ipv6.StartsWith("ff00") ||
                            ipv6.StartsWith("fd00") ||
                            ipv6.StartsWith("fec0"));
            return weCare;
        }

        public static string[] GetLocalIPAddresses()
        {
            List<string> ipaddresses = new List<string>();
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && IsIpv4AddressWeCareAbout(ip.ToString()))
                {
                    ipaddresses.Add(ip.ToString());
                }
                else if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6 && IsIpv6AddressWeCareAbout(ip.ToString()))
                {
                    ipaddresses.Add("[" + ip.ToString() + "]");
                }
            }
            return ipaddresses.ToArray();
        }
    }

}
