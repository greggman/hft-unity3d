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

    class HFTDnsServer : DNS.Server.DnsServer {
        public HFTDnsServer(string ipAddressToReturnForAllDomains) : base (new DNSAll(ipAddressToReturnForAllDomains))
        {
        }

        override protected IResponse ResolveRemote(Request request)
        {
            #pragma warning disable
                throw new System.InvalidOperationException("should never get here!");
                return null;
            #pragma warning restore
        }
    }

    class HFTDnsRunnerHelper
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
                dnsServer_ = new HFTDnsServer("5.6.7.10");
                dnsServer_.Listen(port);
                log_.Info("DNS DoWork -end-");
            }
            catch (System.Exception ex)
            {
                eventProcessor_.QueueEvent(() => {

                  HFTDialog.MessageBox(
                    "ERROR",
                    "Could not start DNS Server on port:" + port + "\n" +
                    "Did you run from the command line with sudo?\n\n" + ex.ToString());
                });
            }
        }

        public void Close()
        {
            dnsServer_.Close();
            log_.Info("DNS DoWork -close-");
        }

        DNS.Server.DnsServer dnsServer_;
        DNSAll dnsAll_;
        HFTEventProcessor eventProcessor_;
        HFTLog log_ = new HFTLog("HFTDnsRunnerHelper");
    }

    class DNSAll : IQuestionAnswerer
    {
        public DNSAll(string ipAddress)
        {
            address_ = IPAddress.Parse(ipAddress);
        }

        public IList<IResourceRecord> Get(Question question)
        {
            List<IResourceRecord> entries = new List<IResourceRecord>();
            entries.Add(new IPAddressResourceRecord(question.Name, address_));
            return entries;
        }

        IPAddress address_;
    }
}
