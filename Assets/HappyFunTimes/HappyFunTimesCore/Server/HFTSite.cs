using DeJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

namespace HappyFunTimes
{

    /// <summary>
    /// A class to manage talking to the rendezvous site (eg:
    /// happyfuntimes.net
    /// </summary>
    /// <param name=""></param>
    /// <returns></returns>
    [AddComponentMenu("")]
    public class HFTSite : MonoBehaviour
    {
        public class Options
        {
            public Options()
            {
                rendezvousUrl = System.Environment.GetEnvironmentVariable("HFT_RENDEZVOUS_URL");
                if (rendezvousUrl == null)
                {
                    rendezvousUrl = "http://happyfuntimes.net/api/inform2";
                }
            }

            public string rendezvousUrl;
            public string port = "18679";
        }

        class SharedState {
            public bool success = false;
        }

        class Informer {
            const int kMaxTries = 60;

            string url_;
            string domain_;
            string addressesStr_;
            byte[] addressesBytes_;
            SharedState sharedState_;
            IEnumerator informCoroutine_;
            int tryCount_ = 0;
            bool done_ = false;
            bool haveNewAddresses_ = false;
            WWW www_;
            HFTLog log_;

            public Informer(HFTLog log, SharedState sharedState, string url, string domain)
            {
                sharedState_ = sharedState;
                log_ = log;
                url_ = url;
                domain_ = domain;
            }

            public void Inform(bool haveNewAddresses, MonoBehaviour monoBehaviour, byte[] addressBytes, string addressesStr)
            {
                addressesBytes_ = addressBytes;
                addressesStr_ = addressesStr;
                haveNewAddresses_ |= haveNewAddresses;

                if (www_ == null) // no request in progress
                {
                    if (haveNewAddresses)
                    {
                        haveNewAddresses = false;
                        tryCount_ = 0;
                        done_ = false;
                        sharedState_.success = false;
                    }
                    if (!done_)
                    {
                        informCoroutine_ = InformCoroutine();
                        monoBehaviour.StartCoroutine(informCoroutine_);
                    }
                }
            }

            public void Stop(MonoBehaviour monoBehaviour)
            {
                if (informCoroutine_ != null)
                {
                    monoBehaviour.StopCoroutine(informCoroutine_);
                    informCoroutine_ = null;
                }
            }


            IEnumerator InformCoroutine()
            {
                var headers = new Dictionary<string, string>();
                headers["Content-Type"] = "application/json";
                headers["Host"] = domain_;

                www_ = new WWW(url_, addressesBytes_, headers);

                yield return www_;

                string err = www_.error;
                string result = www_.text;
                www_ = null;

                // Was it successful?
                if (String.IsNullOrEmpty(err))
                {
                    // Yes
                    log_.Info("registered: " + addressesStr_ + " with " + domain_ + " for: " + result);
                    done_ = true;
                    sharedState_.success = true;
                }
                else
                {
                    ++tryCount_;
                    if (tryCount_ > 2 && !sharedState_.success)
                    {
                        log_.Info("Try " + tryCount_ + " of " + kMaxTries + ": Could not contact: " + domain_ +
                                  "[" + url_ + "]\nSTATUS: " + result);
                    }
                    if (tryCount_ >= kMaxTries || tryCount_ > 1 && sharedState_.success)
                    {
                        done_ = true;
                    }
                }
                informCoroutine_ = null;
            }
        }

        class InformData
        {
            public InformData(string[] _addresses, string _port)
            {
                addresses = _addresses;
                port = _port;
            }
            public string[] addresses;
            public string port;
        }

        string MakeDirectUrl(string address)
        {
            return rendezvousUri_.Scheme + "://" + address + ":" + rendezvousUri_.Port + rendezvousUri_.PathAndQuery;
        }

        string[] GetIPAddresses(DNS.Protocol.RecordType type)
        {
            // This part sucks. Basically I don't know how to make ipv6 and ipv4 requests in .NET
            // so my hacky workaround is to look up the addresses directly
            // BUT, if the user specifices an address URL or a local URL I'm SOL basically
            // I just pass them through if I can't find them
            if (rendezvousUri_.HostNameType != System.UriHostNameType.IPv4 &&
                rendezvousUri_.HostNameType != System.UriHostNameType.IPv6)
            {
                string[] addresses = HFTDnsUtils.GetIPAddresses(rendezvousUri_.DnsSafeHost, type);
                if (addresses != null && addresses.Length > 0)
                {
                    return addresses;
                }
            }
            return new string[] {
                rendezvousUri_.Host,
            };
        }

        public void Init(Options options)
        {
            log_ = new HFTLog("hftsite");
            options_ = options;
            rendezvousUri_ = new System.Uri(options_.rendezvousUrl);

            string[] ipv4Addresses = GetIPAddresses(DNS.Protocol.RecordType.A);
            string[] ipv6Addresses = GetIPAddresses(DNS.Protocol.RecordType.AAAA);
            informers_[0] = new Informer(log_, sharedState_, ipv4Addresses.Length > 0 ? MakeDirectUrl(ipv4Addresses[0]) : null, rendezvousUri_.Host);
            informers_[1] = new Informer(log_, sharedState_, ipv6Addresses.Length > 0 ? MakeDirectUrl(ipv6Addresses[0]) : null, rendezvousUri_.Host);

            checkCoroutine_ = CheckAddresses();
            StartCoroutine(checkCoroutine_);
        }

        public void Stop()
        {
            if (checkCoroutine_ != null)
            {
                StopCoroutine(checkCoroutine_);
                checkCoroutine_ = null;
            }

            foreach (Informer informer in informers_)
            {
                informer.Stop(this);
            }
        }


        // This runs continously, every second it checks for new addresses
        // If they're new it starts the informers. In pumps the informers
        // until they're finished (success or failure)
        IEnumerator CheckAddresses()
        {
            for (;;)
            {
                string[] addresses = GetLocalIPAddresses();
                string newAddressesStr = String.Join(", ", addresses);
                bool haveNewAddresses = false;
                if (!newAddressesStr.Equals(oldAddressesStr_))
                {
                    oldAddressesStr_ = newAddressesStr;
                    var data = new InformData(addresses, options_.port);
                    var json = Serializer.Serialize(data);
                    addressBytes_ = System.Text.Encoding.UTF8.GetBytes(json);
                    haveNewAddresses = true;
                }

                foreach(Informer informer in informers_)
                {
                    informer.Inform(haveNewAddresses, this, addressBytes_, oldAddressesStr_);
                }

                yield return new WaitForSeconds(1.0f);
            }
        }

        bool IsIpv4AddressWeCareAbout(string ipv4)
        {
            return true;
        }

        bool IsIpv6AddressWeCareAbout(string ipv6)
        {
            bool weCare = !(ipv6.StartsWith("fe80") ||
                            ipv6.StartsWith("ff00") ||
                            ipv6.StartsWith("fd00") ||
                            ipv6.StartsWith("fec0"));
            return weCare;
        }

        string[] GetLocalIPAddresses()
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

        SharedState sharedState_ = new SharedState();
        IEnumerator checkCoroutine_;
        Informer[] informers_ = new Informer[2];
        string oldAddressesStr_ = "";
        byte[] addressBytes_;
        System.Uri rendezvousUri_;
        Options options_;
        HFTLog log_;
    }

}  // namespace HappyFunTimes

