using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;

namespace HappyFunTimes
{
    public class HFTLog
    {
        public HFTLog(string prefix = "")
        {
            prefix_ = prefix.Length > 0 ? (prefix + ": ") : "";
            debug_ = s_debug;
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

        public void Info(string msg)
        {
            if (debug_)
            {
                Debug.Log(prefix_ + msg);
            }
        }

        public void Warn(string msg)
        {
            Debug.Log(prefix_ + msg);
        }

        public void Error(string msg)
        {
            Debug.LogError(prefix_ + msg);
        }

        string prefix_;
        bool debug_;
//FIX
        static bool s_debug = true;
    }

}  // namespace HappyFunTimes


