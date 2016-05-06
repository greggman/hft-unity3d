using DNS.Protocol;
using DNS.Protocol.ResourceRecords;
using DNS.Server;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace HappyFunTimes {

    public class HFTDnsRunner
    {
        public void Start()
        {
            helper_ = new HFTDnsRunnerHelper();
            dnsThread_ = new Thread(helper_.DoWork);
            dnsThread_.Start();
        }

        public void Stop()
        {
            helper_.Close();
            dnsThread_.Join();
        }

        Thread dnsThread_;
        HFTDnsRunnerHelper helper_;
    }

    class HFTDnsRunnerHelper
    {
        public void DoWork()
        {
            dnsAll_ = new DNSAll("5.6.7.10");
            dnsServer_ = new DNS.Server.DnsServer(dnsAll_, "9.9.9.9", 53);
            dnsServer_.Listen(4444);
        }

        public void Close()
        {
            dnsServer_.Close();
        }

        DNS.Server.DnsServer dnsServer_;
        DNSAll dnsAll_;
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
