using UnityEngine;
using System.Collections;
using HappyFunTimes;

public class HFTDnsTest : MonoBehaviour
{

    // Use this for initialization
    void Start ()
    {
        HFTLog.debug = HFTHappyFunTimesSettings.debug;
        Debug.Log("HFTDnsTest - start");
        string ipv4Address = HFTIpUtils.GetLocalIPv4Address();
        string ipv6Address = HFTIpUtils.GetLocalIPv6Address();
        dnsRunner_ = new HFTDnsRunner();
        dnsRunner_.Start(ipv4Address, ipv6Address, 4444);
    }

    // Update is called once per frame
    void Update ()
    {

    }

    void Cleanup()
    {
        if (dnsRunner_ != null)
        {
            Debug.Log("HFTDnsTest - stop");
            dnsRunner_.Stop();
            dnsRunner_ = null;
        }
    }

    void OnDestroy()
    {
        Cleanup();
    }

    void OnApplicationExit()
    {
        Cleanup();
    }

    HFTDnsRunner dnsRunner_;
}
