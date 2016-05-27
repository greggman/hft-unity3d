using DNS.Protocol;
using DNS.Protocol.ResourceRecords;
using DNS.Server;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace HappyFunTimes {

    public class HFTDnsRunnerHelper
    {
        public HFTDnsRunnerHelper(HFTEventProcessor ep)
        {
            eventProcessor_ = ep;
        }

        public void DoWork()
        {
            int port = 53;

            try
            {
                log_.Info("DNS DoWork -start-");
                log_.Tell("FIX: hard coded dns");
                dnsServer_ = new HFTDnsServer("192.168.2.9");
                dnsServer_.Listen(port);
                log_.Info("DNS DoWork -end-");
            }
            catch (System.Exception ex)
            {
                string msg =
                    "Could not start DNS Server on port:" + port + "\n" +
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
            dnsServer_.Close();
            log_.Info("DNS DoWork -close-");
        }

        HFTDnsServer dnsServer_;
        HFTEventProcessor eventProcessor_;
        HFTLog log_ = new HFTLog("HFTDnsRunnerHelper");
    }
}
