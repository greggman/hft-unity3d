using System;
using System.Collections.Generic;

namespace HappyFunTimes
{
    public class HFTLog
    {
        public delegate string PrintFunc();

        public HFTLog(string prefix = "")
        {
            prefix_ = prefix.Length > 0 ? (prefix + ": ") : "";
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
            Tell(sb.ToString());
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
            WriteLine(prefix_ + msg);
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
                WriteLine(prefix_ + msg);
            }
        }

        public void Warn(PrintFunc fn)
        {
            Warn(fn());
        }

        public void Warn(string msg)
        {
            WriteLine(prefix_ + "WARNING: " + msg);
        }

        public void Error(PrintFunc fn)
        {
            Error(fn());
        }

        public void Error(string msg)
        {
            WriteLine(prefix_ + "ERROR: " + msg);
        }

        public void Error(System.Exception ex)
        {
            WriteLine(prefix_ + "ERROR: " + ex.ToString());
        }

        private void WriteLine(string msg)
        {
            Console.WriteLine(msg);
        }

        static public HFTLog Global
        {
            get
            {
                return s_global;
            }
        }

        string prefix_;

        static bool s_debug = false;
        static HFTLog s_global = new HFTLog("global");
    }
}

