using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

namespace HappyFunTimes
{
    public class HFTLog
    {
        public delegate string PrintFunc();

        public HFTLog(string prefix = "")
        {
            prefix_ = prefix.Length > 0 ? (prefix + ": ") : "";
            HFTArgParser p = HFTArgParser.GetInstance();
            console_ = false;
            p.TryGetBool("hft-log", ref console_);
            if (console_ && s_out == null)
            {
                s_out = new System.IO.StreamWriter(System.Console.OpenStandardOutput());
                s_out.AutoFlush = true;
            }
        }

        public string prefix
        {
            get
            {
                return prefix_;
            }
            set
            {
                prefix_ = value;
            }
        }

        static public bool debug
        {
            get
            {
                return s_debug;
            }
            set
            {
                s_debug = value;
            }
        }

        private void DumpDictImpl(System.Text.StringBuilder sb, string indent, Dictionary<string, object> dict)
        {
            sb.Append("{\n");
            foreach (var pair in dict)
            {
                if (pair.Value is Dictionary<string, object>)
                {
                    Dictionary<string, object> child = (Dictionary<string, object>)pair.Value;
                    sb.Append(indent + pair.Key + ": ");
                    DumpDictImpl(sb, indent + "  ", child);
                    sb.Append(",\n");
                }
                else if (pair.Value is String)
                {
                    sb.Append(indent + pair.Key + ": \"" + pair.Value.ToString() + "\",\n");
                }
                else
                {
                    sb.Append(indent + pair.Key + ": " + pair.Value.ToString() + ",\n");
                }
            }
            sb.Append(indent + "}");
        }

        public void DumpDict(Dictionary<string, object> dict)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            DumpDictImpl(sb, "  ", dict);
            Debug.Log(sb.ToString());
        }


        /// <summary>Print message always</summary>
        ///
        /// <param name="fn">function to generaete message</param>
        public void Tell(PrintFunc fn)
        {
            Tell(fn());
        }

        /// <summary>Print message always</summary>
        ///
        /// <param name="msg">message</param>
        public void Tell(string msg)
        {
            Debug.Log(prefix_ + msg);

            if (console_)
            {
                WriteLine(prefix_ + msg);
            }
        }

        /// <summary>Print message if debugging</summary>
        ///
        /// <param name="fn">function to generaete message</param>
        public void Info(PrintFunc fn)
        {
            if (s_debug)
            {
                Info(fn());
            }
        }

        /// <summary>Print message if debugging</summary>
        ///
        /// <param name="msg">message</param>
        public void Info(string msg)
        {
            if (s_debug)
            {
                Debug.Log(prefix_ + msg);
            }
            if (console_)
            {
                WriteLine(prefix_ + msg);
            }
        }

        public void Warn(PrintFunc fn)
        {
            Warn(fn());
        }

        public void Warn(string msg)
        {
            Debug.Log(prefix_ + msg);
            if (console_)
            {
                WriteLine(prefix_ + "WARNING: " + msg);
            }
        }

        public void Error(PrintFunc fn)
        {
            Error(fn());
        }

        public void Error(string msg)
        {
            Debug.LogError(prefix_ + msg);
            if (console_)
            {
                WriteLine(prefix_ + "ERROR: " + msg);
            }
        }

        public void Error(System.Exception ex)
        {
            Debug.LogException(ex);
            if (console_)
            {
                WriteLine(prefix_ + "ERROR: " + ex.ToString());
            }
        }

        private void WriteLine(string msg)
        {
            s_out.WriteLine(msg);
        }

        static public HFTLog Global
        {
            get
            {
                return s_global;
            }
        }

        string prefix_;
        bool console_;

        static System.IO.StreamWriter s_out = null;
        static bool s_debug = false;
        static HFTLog s_global = new HFTLog("global");
    }

}  // namespace HappyFunTimes


