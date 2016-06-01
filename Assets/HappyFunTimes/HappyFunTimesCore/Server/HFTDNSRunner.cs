using DNS.Protocol;
using DNS.Protocol.ResourceRecords;
using DNS.Server;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using UnityEngine;

namespace HappyFunTimes {

    public class HFTDnsRunner
    {
        public void Start(string ipv4Address, string ipv6Address, int port)
        {
            GameObject go = new GameObject("HFTDnsEventProcessor");
            eventProcessor_ = go.AddComponent<HFTEventProcessor>();
            helper_ = new HFTDnsRunnerHelper(eventProcessor_, ipv4Address, ipv6Address, port);
            dnsThread_ = new Thread(helper_.DoWork);
            dnsThread_.Start();
        }

        public void Stop()
        {
            // Must close helper before joining because
            // helper makes it possible to join
            if (helper_ != null)
            {
                helper_.Close();
            }
            if (dnsThread_ != null)
            {
                dnsThread_.Join();
            }
            if (eventProcessor_ != null)
            {
                UnityEngine.Object.Destroy(eventProcessor_.gameObject);
            }
        }

        Thread dnsThread_;
        HFTDnsRunnerHelper helper_;
        HFTEventProcessor eventProcessor_;
    }
}
