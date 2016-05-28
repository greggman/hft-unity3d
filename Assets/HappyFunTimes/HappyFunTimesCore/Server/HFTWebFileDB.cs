using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

namespace HappyFunTimes
{
    public class HFTWebFileDB
    {
        static HFTWebFileDB s_instance;

        HFTWebFileDB()
        {
            HFTWebFileLoader.LoadFiles((filename, bytes) => {
                AddFile(filename, bytes);
            });
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

        public bool FileExists(string path)
        {
            // hacky
            byte[] data = null;
            return GetFile(path, out data);
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
                    content = HFTUtil.ReadBytes(livePath);
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
            log_.Info("Add File: " + path + ", size: " + content.Length);
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
            #if UNITY_EDITOR
            string HFT_SRC_PATH = "WebPlayerTemplates/HappyFunTimes";
            isEditor_ = UnityEngine.Application.isEditor;
            dataPath_ = Path.Combine(UnityEngine.Application.dataPath, HFT_SRC_PATH);
            #endif
        }

        public string DataPath
        {
            set
            {
                dataPath_ = value;
                isEditor_ = true;
            }
        }

        HFTLog log_ = new HFTLog("HFTWebFileDB");
        bool isEditor_;
        string dataPath_;
        private Dictionary<string, byte[] > files_ = new Dictionary<string, byte[] >();
    }

}  // namespace HappyFunTimes

