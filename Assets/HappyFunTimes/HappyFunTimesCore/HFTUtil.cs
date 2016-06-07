/*
 * Copyright 2014, Gregg Tavares.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are
 * met:
 *
 *     * Redistributions of source code must retain the above copyright
 * notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above
 * copyright notice, this list of conditions and the following disclaimer
 * in the documentation and/or other materials provided with the
 * distribution.
 *     * Neither the name of Gregg Tavares. nor the names of its
 * contributors may be used to endorse or promote products derived from
 * this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
 * OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
 * LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 * THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HappyFunTimes
{

    public class HFTUtil
    {
        public static void SetIfNotNullOrEmpty(string value, ref string dst)
        {
            if (!String.IsNullOrEmpty(value))
            {
                dst = value;
            }
        }

        static readonly Regex re = new Regex(@"%\(([^\)]+)\)s");
        public static string ReplaceParams(string s, Dictionary<string, object> subs)
        {
            string output = re.Replace(s, match =>
            {
                Dictionary<string, object>dict = subs;
                object obj = dict;
                string fullKey = match.Groups[1].Value;
                string[] keys = fullKey.Split('.');
                foreach (string key in keys)
                {
                    object v = null;
                    if (dict == null || !dict.TryGetValue(key, out v))
                    {
                        HFTLog.Global.Tell("unknown key: " + fullKey);
                        return "%(" + fullKey + ")s";
                    }
                    else
                    {
                        obj = v;
                        if (obj.GetType() == typeof(Dictionary<string, object>))
                        {
                            dict = (Dictionary<string, object>)obj;
                        }
                        else
                        {
                            dict = null;
                        }
                    }
                }
                return obj.ToString();
            });
            return output;
        }

        public static string ReplaceParamsFlat(string s, Dictionary<string, string> subs)
        {
            string output = re.Replace(s, match =>
            {
                string fullKey = match.Groups[1].Value;
                string v = null;
                if (!subs.TryGetValue(fullKey, out v))
                {
                    HFTLog.Global.Tell("unknown key: " + fullKey);
                    return "%(" + fullKey + ")s";
                }
                return v;
            });
            return output;
        }

        public static string ReadText(string filename)
        {
            return System.Text.Encoding.UTF8.GetString(ReadBytes(filename));
        }

        public static byte[] ReadBytes(string filename)
        {
            byte[] bytes;
            using(System.IO.FileStream fileStream = new System.IO.FileStream(filename, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                int length = (int)fileStream.Length;  // get file length
                bytes = new byte[length];            // create buffer
                int count;                            // actual number of bytes read
                int sum = 0;                          // total number of bytes read

                // read until Read method returns 0 (end of the stream has been reached)
                while ((count = fileStream.Read(bytes, sum, length - sum)) > 0)
                {
                  sum += count;  // sum is a buffer offset for next reading
                }
            }
            return bytes;
        }

        public static bool WriteBytes(string filename, byte[] bytes)
        {
            using (System.IO.BinaryWriter writer = new System.IO.BinaryWriter(System.IO.File.Open(filename, System.IO.FileMode.Create)))
            {
                writer.Write(bytes);
            }
            return true;
        }

        public static bool WriteText(string filename, string text)
        {
            return WriteBytes(filename, System.Text.Encoding.UTF8.GetBytes(text));
        }

        public static string SafeName(string s) {
            return s_safeRE.Replace(s, "-");
        }

        ///<summary>
        /// Returns the current JS time in Ticks. JS time starts at
        /// Januray 1st 1970 where as C# / .NET time Starts at January
        /// 1st 1900
        ///</summary>
        public static long GetJSTimeInTicks()
        {
            return System.DateTime.UtcNow.Ticks - s_javaScriptEpoch;
        }

        ///<summary>
        /// Returns the current JS time in milliseconds. JS time starts
        /// at Januray 1st 1970 where as C# / .NET time Starts at
        /// January 1st 1900
        ///</summary>
        public static long GetJSTimeInMs()
        {
            return GetJSTimeInTicks() / System.TimeSpan.TicksPerMillisecond;
        }

        ///<summary>
        /// Returns the current JS time in seconds. JS time starts at
        /// Januray 1st 1970 where as C# / .NET time Starts at January
        /// 1st 1900
        ///</summary>
        public static double GetJSTimeInSeconds()
        {
            return (double)GetJSTimeInMs() / 1000.0;
        }

        static long s_javaScriptEpoch = (new System.DateTime(1970, 1, 1)).Ticks;
        static System.Text.RegularExpressions.Regex s_safeRE = new System.Text.RegularExpressions.Regex("[^a-zA-Z0-9_]");
    }

}  // namespace HappyFunTimes
