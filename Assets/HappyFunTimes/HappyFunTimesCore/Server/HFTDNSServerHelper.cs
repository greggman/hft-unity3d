using DNS.Protocol;
using DNS.Protocol.ResourceRecords;
using DNS.Server;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace HappyFunTimes {

    public class HFTDnsRunnerHelper
    {
        public HFTDnsRunnerHelper(HFTEventProcessor ep, string ipv4Address, string ipv6Address, int port)
        {
            eventProcessor_ = ep;
            ipv4Address_ = ipv4Address;
            ipv6Address_ = ipv6Address;
            port_ = port;
        }

        public void DoWork()
        {
            try
            {
                log_.Info("DNS DoWork -start- port:" + port_);
                dnsServer_ = new HFTDnsServer(ipv4Address_, ipv6Address_);
                dnsServer_.Listen(port_);
                log_.Info("DNS DoWork -end-");
            }
            catch (System.Exception ex)
            {
                string msg =
                    "Could not start DNS Server on port:" + port_ + "\n" +
                    "Did you run from the command line with sudo?\n\n" + ex.ToString();

                if (!HFTGlobalEventEmitter.GetInstance().QueueEvent(HFTGlobalEventType.Error, msg))
                {
                    // No one handled it. Let's do it ourselves.
                    eventProcessor_.QueueEvent(() => {
                        HFTDialog.MessageBox("ERROR", msg);
                    });
                }
            }
        }

        public void Close()
        {
            log_.Info("DNS DoWork -close-start");
            dnsServer_.Close();
            log_.Info("DNS DoWork -close-end");
        }

        int port_;
        string ipv4Address_;
        string ipv6Address_;
        HFTDnsServer dnsServer_;
        HFTEventProcessor eventProcessor_;
        HFTLog log_ = new HFTLog("HFTDnsRunnerHelper");
    }
}
