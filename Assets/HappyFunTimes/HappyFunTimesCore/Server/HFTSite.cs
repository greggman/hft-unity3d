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

        public void Init(Options options)
        {
            log_ = new HFTLog("hftsite");
            options_ = options;
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

            if (informCoroutine_ != null)
            {
                StopCoroutine(informCoroutine_);
                informCoroutine_ = null;
            }
        }


        IEnumerator CheckAddresses()
        {
            for (;;)
            {
                string[] addresses = GetLocalIPAddresses();
                string addressesStr = String.Join(", ", addresses);
                if (!addressesStr.Equals(addressesStr_))
                {
                    inform_ = true;
                    tryCount_ = 0;
                }

                if (www_ == null && inform_)
                {
                    informCoroutine_ = Inform();
                    StartCoroutine(informCoroutine_);
                }

                yield return new WaitForSeconds(1.0f);
            }
        }

        string[] GetLocalIPAddresses()
        {
            List<string> ipaddresses = new List<string>();
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    ipaddresses.Add(ip.ToString());
                }
            }
            return ipaddresses.ToArray();
        }

        IEnumerator Inform()
        {
            inform_ = false;
            var uri = new System.Uri(options_.rendezvousUrl);
            addresses_ = GetLocalIPAddresses();
            addressesStr_ = String.Join(", ", addresses_);
            var data = new InformData(addresses_, options_.port);
            var json = Serializer.Serialize(data);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
            var headers = new Dictionary<string, string>();
            headers["Content-Type"] = "application/json";

            www_ = new WWW(options_.rendezvousUrl, bytes, headers);

            yield return www_;

            string err = www_.error;
            string result = www_.text;
            www_ = null;

            // Was it successful?
            if (String.IsNullOrEmpty(err))
            {
                // Yes
                log_.Info("registered: " + addressesStr_ + " with " + uri.Host + " for: " + result);
            }
            else
            {
                ++tryCount_;
                // No
                log_.Info("Try " + tryCount_ + " of " + maxTries_ + ": Could not contact: " + uri.Host +
                          "\nSTATUS: " + result);
                if (tryCount_ < maxTries_)
                {
                    inform_ = true;
                }
            }
            informCoroutine_ = null;

//            var req = new HFTHttpRequest.Options();
//            req.content = json;
//            req.url = options_.rendezvousUrl;
//            string result = "";
//            var code = HFTHttpRequest.SyncRequest(req, out result);
//            if (code == System.Net.HttpStatusCode.OK)
//            {
//                log_.Info("registered: " + String.Join(", ", addresses) + " with " + uri.Host + " for: " + result);
//            }
//            else
//            {
//                int tries = 1;
//                int maxTries = 1;
//                log_.Info("Try " + tries + " of " + maxTries + ": Could not contact: " + uri.Host +
//                          "\nSTATUS: " + code.ToString() + ": " + result);
//            }
        }

        IEnumerator checkCoroutine_;
        IEnumerator informCoroutine_;
        string[] addresses_;
        string addressesStr_ = "";
        WWW www_;
        Options options_;
        HFTLog log_;
        int maxTries_ = 60;
        int tryCount_ = 0;
        bool inform_ = true;
    }

}  // namespace HappyFunTimes

