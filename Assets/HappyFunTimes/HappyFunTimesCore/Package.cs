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

using DeJson;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace HappyFunTimes
{
    public enum HFTInstructionsPosition
    {
        top,
        bottom,
    }

    [Serializable]
    public class HFTString
    {
        public HFTString(string name, string value)
        {
            m_name = name;
            m_value = value;
        }

        [SerializeField]
        public string m_name;
        [SerializeField]
        public string m_value;
    }

    [Serializable]
    public class HFTInt
    {
        public HFTInt(string name, int value)
        {
            m_name = name;
            m_value = value;
        }

        [SerializeField]
        public string m_name;
        [SerializeField]
        public int m_value;
    }

    [Serializable]
    public class HFTPackage
    {
        [SerializeField]
        public List<HFTString> m_strings;

        [SerializeField]
        public List<HFTInt> m_ints;

        private static System.Random s_random = new System.Random((int)DateTime.Now.Ticks);

        private Dictionary<string, object>GetSection(string loc, Dictionary<string, object>section)
        {
            if (String.IsNullOrEmpty(loc)) {
                return section;
            }

            string first;
            string remaining;

            int period = loc.IndexOf(".");
            if (period < 0) {
                first = loc;
                remaining = "";
            } else {
                first = loc.Substring(0, period);
                remaining = loc.Substring(period + 1);
            }

            object child = null;
            if (section.TryGetValue(first, out child)) {
                return GetSection(remaining, (Dictionary<string, object>)child);
            }
            return null;
        }

        private object GetObject(string loc, string id, Dictionary<string, object> package)
        {
            Dictionary<string, object> section = GetSection(loc, package);
            if (section != null) {
                object o = null;
                if (section.TryGetValue(id, out o)) {
                    return o;
                }
            }
            return null;
        }

        private void AddString(string loc, string id, Dictionary<string, object> package, string defaultValue = "")
        {
            object o = GetObject(loc, id, package);
            m_strings.Add(new HFTString(id, o != null ? Convert.ToString(o) : defaultValue));
        }

        private void SetString(string loc, string id, Dictionary<string, object> package)
        {
            Dictionary<string, object> section = GetSection(loc, package);
            section[id] = FindString(id).m_value;
        }

        private void AddInt(string loc, string id, Dictionary<string, object> package)
        {
            object o = GetObject(loc, id, package);
            m_ints.Add(new HFTInt(id, Convert.ToInt32(o)));
        }

        private void SetInt(string loc, string id, Dictionary<string, object> package)
        {
            Dictionary<string, object> section = GetSection(loc, package);
            section[id] = FindInt(id).m_value;
        }

        private HFTString FindString(string id)
        {
            return m_strings.Find(delegate(HFTString s) {
                return s.m_name == id;
            });
        }

        private HFTInt FindInt(string id)
        {
            return m_ints.Find(delegate(HFTInt s) {
                return s.m_name == id;
            });
        }

        public string GetString(string id)
        {
            HFTString h = FindString(id);
            return h.m_value;
        }

        public void SetString(string id, string s)
        {
            HFTString h = FindString(id);
            h.m_value = s;
        }

        public int GetInt(string id)
        {
            HFTInt h = FindInt(id);
            return h.m_value;
        }

        public void SetInt(string id, int i)
        {
            HFTInt h = FindInt(id);
            h.m_value = i;
        }

        private string RandomString(int size)
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * s_random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        private string GetPackagePath()
        {
            // If it exists do the "Assets/../package.json" (legacy)
            // If it does not exist use "Assets/WebPlayerTemplates/HappyFunTimes"
            string path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.dataPath), "package.json");
            if (System.IO.File.Exists(path)) {
                return path;
            }
            path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.dataPath), "HappyFunTimes");
            path = System.IO.Path.Combine(path, "package.json");
            if (System.IO.File.Exists(path)) {
                return path;
            }
            path = System.IO.Path.Combine(Application.dataPath, "WebPlayerTemplates");
            path = System.IO.Path.Combine(path, "HappyFunTimes");
            path = System.IO.Path.Combine(path, "package.json");
            return path;
        }

        // TODO! If package changes, reload it and kill undo

        private Dictionary<string, object> ReadPackage()
        {
            string packagePath = GetPackagePath();
            string json;
            if (System.IO.File.Exists(packagePath)) {
                json = System.IO.File.ReadAllText(packagePath, System.Text.Encoding.UTF8);
            } else {
                json = @"{
                    ""name"": ""Unnamed Game"",
                    ""description"": ""My unnamed and undescribed game"",
                    ""version"": ""0.0.0"",
                    ""private"": false,
                    ""dependencies"": {},
                    ""happyFunTimes"": {
                      ""gameId"": ""???id???"",
                      ""category"": ""game"",
                      ""apiVersion"": ""1.14.0"",
                      ""gameType"": ""Unity3D"",
                      ""minPlayers"": 1,
                      ""instructionsPosition"", ""top""
                    }
                }";
                json = json.Replace("???id???", RandomString(32));
            }

            Deserializer deserializer = new Deserializer();
            Dictionary<string, object> package = deserializer.Deserialize<Dictionary<string, object> >(json);

            return package;
        }

        private void WritePackage()
        {
            Dictionary<string, object> package = ReadPackage();
            string old = Serializer.Serialize(package, false, true);

            SetString("", "name", package);
            SetString("", "description", package);
            SetString("", "version", package);

            SetString("happyFunTimes", "gameId", package);
            SetString("happyFunTimes", "category", package);
            SetString("happyFunTimes", "apiVersion", package);
            SetString("happyFunTimes", "gameType", package);
            SetString("happyFunTimes", "instructionsPosition", package);
            SetInt("happyFunTimes", "minPlayers", package);

            string json = Serializer.Serialize(package, false, true);
            if (old != json) {
                string packagePath = GetPackagePath();
                if (packagePath.Contains("WebPlayerTemplates")) {
                    // Check folder exists
                    string dirPath = System.IO.Path.GetDirectoryName(packagePath);
                    if (!System.IO.Directory.Exists(dirPath)) {
                        System.IO.Directory.CreateDirectory(dirPath);
                    }
                }
                System.IO.File.WriteAllText(packagePath, json);
            }
        }

        public void Init()
        {
            m_strings = new List<HFTString>();
            m_ints = new List<HFTInt>();

            Dictionary<string, object> package = ReadPackage();

            AddString("", "name", package);
            AddString("", "description", package);
            AddString("", "version", package);

            AddString("happyFunTimes", "gameId", package);
            AddString("happyFunTimes", "category", package);
            AddString("happyFunTimes", "apiVersion", package);
            AddString("happyFunTimes", "gameType", package);
            AddString("happyFunTimes", "instructionsPosition", package);
            AddInt("happyFunTimes", "minPlayers", package);
        }

        public void Persist()
        {
            WritePackage();
        }
    }

}   // namespace HappyFunTimes

