using DNS.Protocol;
using DNS.Protocol.ResourceRecords;
using DNS.Server;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace HappyFunTimes {

    public class HFTDnsServer : DNS.Server.DnsServer {
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
