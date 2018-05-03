using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace HappyFunTimes
{
    public class HFTDnsUtils {

        public static string[] GetIPAddresses(string domain, DNS.Protocol.RecordType type)
        {
            // This client is probably heavy? Maybe I should have just one
            // NOTE: This is hardcoded to Google's DNS servers. I'd prefer to use
            // the system DNS but ATM (a) I don't know how to find it and (b)
            // .NET/mono is broken
            DNS.Client.DnsClient client = new DNS.Client.DnsClient("8.8.8.8");
            List<string> addresses = new List<string>();
            try 
            {
              IList<System.Net.IPAddress> ips = client.Lookup(domain, type);
              foreach (var ip in ips)
              {
                  addresses.Add(ip.ToString());
              }
            }
            catch(DNS.Client.ResponseException ex)
            {
              var log = new HFTLog("HFTDnsUtils");
              log.Warn(String.Format("error getting DNS {0} record for {1}: {2}", type, domain, ex.ToString()));
            }
            return addresses.ToArray();
        }
    }

}
