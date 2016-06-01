using DNS.Protocol;
using DNS.Protocol.ResourceRecords;
using DNS.Server;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace HappyFunTimes {

    public class HFTDnsServer : DNS.Server.DnsServer {
        public HFTDnsServer(
            string ipv4AddressToReturnForAllDomains,
            string ipv6AddressToReturnForAllDomains) : base (new DNSAll(
                ipv4AddressToReturnForAllDomains,
                ipv6AddressToReturnForAllDomains))
        {
        }

        // called if no answer
        override protected IResponse ResolveRemote(Request request)
        {
            Response response = Response.FromRequest(request);
            return response;
        }
    }

    class DNSAll : IQuestionAnswerer
    {
        public DNSAll(string ipv4Address, string ipv6Address)
        {
            ipv4Address_ = System.String.IsNullOrEmpty(ipv4Address) ? null : IPAddress.Parse(ipv4Address);
            ipv6Address_ = System.String.IsNullOrEmpty(ipv6Address) ? null : IPAddress.Parse(ipv6Address);
        }

        public IList<IResourceRecord> Get(Question question)
        {
            List<IResourceRecord> entries = new List<IResourceRecord>();
            switch (question.Type)
            {
                case DNS.Protocol.RecordType.A:
                    if (ipv4Address_ == null)
                    {
                        log_.Info("No IPv4 address for A question");
                        break;
                    }
                    log_.Info("Answer[A]:" + question.Name + ":" + ipv4Address_);
                    entries.Add(new IPAddressResourceRecord(question.Name, ipv4Address_, ttl_));
                    break;
                case DNS.Protocol.RecordType.AAAA:
                    if (ipv6Address_ == null)
                    {
                        log_.Info("No IPv6 address for AAAA question");
                        break;
                    }
                    log_.Info("Answer[AAAA]:" + question.Name + ":" + ipv6Address_);
                    entries.Add(new IPAddressResourceRecord(question.Name, ipv6Address_, ttl_));
                    break;
                default:
                    if (ipv4Address_ == null)
                    {
                        log_.Info("No IPv4 address for " + question.Name + " question");
                        break;
                    }
                    log_.Info("Answer[" + question.Type.ToString() + "]:" + question.Name + ":" + ipv4Address_);
                    entries.Add(new IPAddressResourceRecord(question.Name, ipv4Address_, ttl_));
                    break;
            }
            return entries;
        }

        System.TimeSpan ttl_ = new System.TimeSpan(0, 5, 0);
        HFTLog log_ = new HFTLog("HFTDNSServer");
        IPAddress ipv4Address_;
        IPAddress ipv6Address_;
    }
}
