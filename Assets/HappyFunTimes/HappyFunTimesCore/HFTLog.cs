using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;

namespace HappyFunTimes
{
    public class HFTLog
    {
        public delegate string PrintFunc();

        public HFTLog(string prefix = "")
        {
            prefix_ = prefix.Length > 0 ? (prefix + ": ") : "unprefixed: ";
            RemoveDeadLoggers();
            if (s_loggers != null)
            {
                s_loggers.Add(new WeakReference(this));
            }
            SetDebug();
        }

        static private bool IsPrefixInDebugString(string prefix)
        {
            if (String.IsNullOrEmpty(s_debug) || s_debugRE == null)
            {
                return false;
            }
            return s_debugRE.IsMatch(prefix);
        }

        private void SetDebug()
        {
            debug_ = IsPrefixInDebugString(prefix_);
        }

        private static void RemoveDeadLoggers()
        {
            if (s_loggers != null)
            {
                s_loggers.RemoveAll(x => !x.IsAlive);
            }
        }

        private static void SetAllDebug()
        {
            if (s_loggers != null)
            {
                RemoveDeadLoggers();
                s_loggers.ForEach((w) => {
                    (w.Target as HFTLog).SetDebug();
                });
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
                SetDebug();
            }
        }

        static public string debug
        {
            get
            {
                return s_debug;
            }
            set
            {
                s_debug = value;
                string[] patterns = s_debug.Replace("\n", ",").Replace(" ", ",").Split(',').Where(s => !String.IsNullOrEmpty(s)).ToArray();
                for (int i = 0; i < patterns.Length; ++i)
                {
                    patterns[i] = patterns[i].Replace("*", ".*?");
                }
                string pattern = "^(" + String.Join("|", patterns) + "): $";
                s_debugRE = new Regex(pattern);
                SetAllDebug();
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
            HFTLogger.Log(sb.ToString());
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
            HFTLogger.Log(prefix_ + msg);
        }

        /// <summary>Print message if debugging</summary>
        ///
        /// <param name="fn">function to generaete message</param>
        public void Info(PrintFunc fn)
        {
            if (debug_)
            {
                Info(fn());
            }
        }

        /// <summary>Print message if debugging</summary>
        ///
        /// <param name="msg">message</param>
        public void Info(string msg)
        {
            if (debug_)
            {
                HFTLogger.Log(prefix_ + msg);
            }
        }

        public void Warn(PrintFunc fn)
        {
            Warn(fn());
        }

        public void Warn(string msg)
        {
            HFTLogger.Log(prefix_ + msg);
        }

        public void Error(PrintFunc fn)
        {
            Error(fn());
        }

        public void Error(string msg)
        {
            HFTLogger.Error(prefix_ + msg);
        }

        public void Error(System.Exception ex)
        {
            HFTLogger.Error(ex);
        }

        static public HFTLog Global
        {
            get
            {
                return s_global;
            }
        }

        bool debug_;
        string prefix_;

        static string s_debug = "";
        static Regex s_debugRE;
        static HFTLog s_global = new HFTLog("global");
        static List<WeakReference> s_loggers = new List<WeakReference>();
    }

}  // namespace HappyFunTimes


