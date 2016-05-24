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
using UnityEngine;

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
                        Debug.Log("unknown key: " + fullKey);
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
                    Debug.Log("unknown key: " + fullKey);
                    return "%(" + fullKey + ")s";
                }
                return v;
            });
            return output;
        }

        public static Texture2D MakeColor(Color color)
        {
            Color[] pix = new Color[1];
            pix[0] = color;
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixels(pix);
            tex.Apply();
            return tex;
        }
    }

}  // namespace HappyFunTimes
