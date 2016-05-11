using UnityEngine;
using System.Collections;
using HappyFunTimes;

public class HFTDnsTest : MonoBehaviour
{

    // Use this for initialization
    void Start ()
    {
        Debug.Log("HFTDnsTest - start");
        dnsRunner_ = new HFTDnsRunner();
        dnsRunner_.Start();
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
