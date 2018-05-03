using System;
using System.Text.RegularExpressions;

namespace HappyFunTimes {
    public class HFTBrowserVersion {
        static public bool IsIOS(string userAgent)
        {
            return m_iosRE.IsMatch(userAgent);
        }

        public class Version {
            public Version(int major = 0, int minor = 0)
            {
                Major = major;
                Minor = minor;
            }
            public int Major = 0;
            public int Minor = 0;
        };

        static public Version IOSVersion(string userAgent)
        {
            var m = m_versionRE.Match(userAgent);
            if (m.Success)
            {
                return new Version(
                    Int32.Parse(m.Groups[1].ToString()),
                    Int32.Parse(m.Groups[2].ToString()));
            }
            return new Version();
        }

        static public bool IsIOS10(string userAgent)
        {
            return IsIOS(userAgent) && IOSVersion(userAgent).Major == 10;
        }

        static private Regex m_iosRE = new Regex("iPad|iPhone");
        static private Regex m_versionRE = new Regex("OS (\\d+)_(\\d+)\\D");
    }
}
