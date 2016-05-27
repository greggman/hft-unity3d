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
        public void Start()
        {
            GameObject go = new GameObject("HFTDnsEventProcessor");
            eventProcessor_ = go.AddComponent<HFTEventProcessor>();
            helper_ = new HFTDnsRunnerHelper(eventProcessor_);
            dnsThread_ = new Thread(helper_.DoWork);
            dnsThread_.Start();
        }

        public void Stop()
        {
            helper_.Close();
            dnsThread_.Join();
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
