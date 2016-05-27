using DeJson;
using System;
using System.Collections.Generic;
using HappyFunTimes;

namespace HappyFunTimes
{
    class MainClass
    {
        public static int Main (string[] args)
        {
            HFTRuntimeOptions m_options;
            HFTArgParser p = new HFTArgParser();
            string argStr = "";
            if (p.TryGet<string> ("hft-args", ref argStr))
            {
                Deserializer d = new Deserializer();
                m_options = d.Deserialize<HFTRuntimeOptions>(argStr);
            }
            else
            {
                m_options = new HFTRuntimeOptions ();
            }
            if (!HFTArgsToFields.Apply("hft", m_options))
            {
                System.Console.WriteLine("bad args!");
                return 1;
            }

            //using (System.IO.StreamWriter writer = new System.IO.StreamWriter(System.IO.File.Open("/Users/gregg/temp/hft-server.log", System.IO.FileMode.Create)))
            //{
            //    writer.WriteLine(System.DateTime.Now.ToString());
            //    writer.WriteLine(Serializer.Serialize(m_options, false, true));
            //}

            List<string> addresses = new List<string>();
            addresses.Add("http://[::0]:18679");
            //            addresses.Add("http://0.0.0.0:18679");

            if (m_options.installationMode)
            {
                addresses.Add("http://[::0]:80");
                //                addresses.Add("http://0.0.0.0:80");
            }

            // Do I want this option ever?
            // Good: Need from editor to test instalation mode in editor
            // Bad: If game is hacked and serve any folder. But if game
            // is hack you can probably own machine anyway.
            if (!String.IsNullOrEmpty(m_options.dataPath))
            {
                HFTWebFileDB.GetInstance().DataPath = m_options.dataPath;
            }

            HFTWebServer webServer = new HFTWebServer(m_options, addresses.ToArray());
            webServer.Start();

            if (m_options.dns || m_options.installationMode)
            {
                HFTDnsRunner dnsRunner = new HFTDnsRunner();
                dnsRunner.Start();
            }

            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);

            return 0;
        }
    }

    public class HFTDnsRunner
    {
        public void Start()
        {
            helper_ = new HFTDnsRunnerHelper();
            dnsThread_ = new System.Threading.Thread(helper_.DoWork);
            dnsThread_.Start();
        }

        public void Stop()
        {
            helper_.Close();
            dnsThread_.Join();
        }

        System.Threading.Thread dnsThread_;
        HFTDnsRunnerHelper helper_;
    }

    public class HFTDnsRunnerHelper
    {
        public HFTDnsRunnerHelper()
        {
        }

        public void DoWork()
        {
            int port = 53;

            try
            {
                log_.Info("DNS DoWork -start-");
                log_.Tell("FIX: hard coded dns");
                dnsServer_ = new HFTDnsServer("192.168.2.9");
                dnsServer_.Listen(port);
                log_.Info("DNS DoWork -end-");
            }
            catch (System.Exception ex)
            {
                log_.Error(
                    "Could not start DNS Server on port:" + port + "\n" +
                    "Did you run from the command line with sudo?\n\n" + ex.ToString());
            }
        }

        public void Close()
        {
            dnsServer_.Close();
            log_.Info("DNS DoWork -close-");
        }

        HFTDnsServer dnsServer_;
        HFTLog log_ = new HFTLog("HFTDnsRunnerHelper");
    }
}
