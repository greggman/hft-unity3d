using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using UnityEngine;

namespace HappyFunTimes
{
    public class HFTWebFileDB
    {
        static private string HFT_SRC_PATH = "WebPlayerTemplates/HappyFunTimes";
        static HFTWebFileDB s_instance;

        HFTWebFileDB()
        {
            HFTWebFileLoader.LoadFiles(this);
        }

        static public HFTWebFileDB GetInstance()
        {
            if (s_instance == null)
            {
                s_instance = new HFTWebFileDB();
                s_instance.Init();
            }
            return s_instance;
        }

        public bool GetFile(string path, out byte[] content)
        {
            path = path.Replace('\\', '/');
            if (path.StartsWith("/"))
            {
                path = path.Substring(1);
            }
            if (isEditor_)
            {
                string livePath = Path.Combine(dataPath_, path);
                try
                {
                    content = System.IO.File.ReadAllBytes(livePath);
                    return true;
                }
                catch (System.Exception)
                {
                }
            }
            return files_.TryGetValue(path, out content);
        }

        public void AddFile(string path, byte[] content)
        {
            path = path.Replace('\\', '/');
            files_[path] = content;
        }

        public string[] GetFiles(string path, string glob)
        {
            path = path.Replace('\\', '/');
            if (path.StartsWith("/"))
            {
                path = path.Substring(1);
            }
            Regex globRE = new Regex(
                "^" + Regex.Escape(glob).Replace(@"\*", ".*").Replace(@"\?", ".") + "$",
                RegexOptions.IgnoreCase | RegexOptions.Singleline);
            List<string> filenames = new List<string>();
            foreach (var pair in files_)
            {
                string filename = pair.Key;
                if (filename.StartsWith(path))
                {
                    string remain = filename.Substring(path.Length + 1);
                    // No directory
                    if (String.IsNullOrEmpty(path) || String.IsNullOrEmpty(Path.GetDirectoryName(path)))
                    {
                        if (globRE.IsMatch(remain))
                        {
                            filenames.Add(filename);
                        }
                    }
                }
            }
            return filenames.ToArray();
        }

        void Init()
        {
            isEditor_ = Application.isEditor;
            dataPath_ = Path.Combine(Application.dataPath, HFT_SRC_PATH);
        }

        bool isEditor_;
        string dataPath_;
        private Dictionary<string, byte[] > files_ = new Dictionary<string, byte[] >();
    }

}  // namespace HappyFunTimes

