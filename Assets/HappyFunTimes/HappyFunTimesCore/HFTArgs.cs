using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HappyFunTimes {

    public interface IHFTArg {
        void Init(string name, HFTArgParser p);
    }
    public class HFTArgHelper<T> {
        public T Value
        {
            get
            {
                return value_;
            }
        }
        public bool IsSet
        {
            get
            {
                return isSet_;
            }
        }
        public bool GetIfSet(ref T variable)
        {
            if (IsSet)
            {
                variable = Value;
            }
            return IsSet;
        }

        protected T value_;
        protected bool isSet_ = false;
    }

    public class HFTArg<T> : HFTArgHelper<T>
    {
        public void Init(string name, HFTArgParser p)
        {
            isSet_ = p.TryGet<T>(name, ref value_);
        }
    }

    public class HFTArgBool : HFTArgHelper<bool>
    {
        public void Init(string name, HFTArgParser p)
        {
            isSet_ = p.TryGetBool(name, ref value_);
        }
    }

    // Is this a good thing? No idea. It's D.R.Y. It's also auto-completeable in the
    // editor where as using a dict or somethig is not. And it checks for bad arguments so uses
    // could possibly get help.
    public class HFTArgsBase {
        public HFTArgsBase(string prefix)
        {
            prefix_ = prefix + "-";
            dashPrefix_ = "--" + prefix;
            HFTLog log = new HFTLog("HFTArgs");
            HashSet<string> validArgs = new HashSet<string>();

            HFTArgParser p = new HFTArgParser();
            System.Reflection.FieldInfo[] fields = this.GetType().GetFields();
            foreach (System.Reflection.FieldInfo info in fields) {
                object field = System.Activator.CreateInstance(info.FieldType);
                string dashName = GetHFTDashName(info.Name);
                validArgs.Add(dashName);
                info.FieldType.GetMethod("Init").Invoke(field, new object[]{ dashName, p });
                info.SetValue(this, field);
            }

            #if (!UNITY_IOS)
            string[] args = System.Environment.GetCommandLineArgs();
            foreach (string arg in args)
            {
                if (arg.StartsWith(dashPrefix_))
                {
                    if (!validArgs.Contains(arg.Substring(dashPrefix_.Length)))
                    {
                        log.Warn("unknown option: " + arg);
                    }
                }
            }
            #endif

            foreach (string evar in System.Environment.GetEnvironmentVariables().Keys)
            {
                string opt = evar.ToLowerInvariant().Replace("_", "-");
                if (opt.StartsWith(prefix_))
                {
                    if (!validArgs.Contains(opt))
                    {
                        log.Warn("unknown environment variable: " + evar);
                    }
                }
            }
        }

        public string GetHFTDashName(string camelCase)
        {
            return prefix_ + s_camelRE.Replace(camelCase, s_camelReplace).ToLower();
        }

        string prefix_;
        string dashPrefix_;
        static string s_camelReplace = @"-$1$2";
        static Regex s_camelRE = new Regex(@"([A-Z])([a-z0-9])", RegexOptions.Singleline);
    }

    public class HFTArgs : HFTArgsBase
    {
        public HFTArgs() : base("hft") {
        }
        public HFTArg<string> url;
        public HFTArg<string> id;
        public HFTArg<string> rendezvousUrl;
        public HFTArg<string> serverPort;
        public HFTArg<int> numGames;
        public HFTArgBool installationMode;
        public HFTArgBool master;
        public HFTArgBool showMessages;
        public HFTArgBool debug;

        public HFTArg<string> instructions;
        public HFTArgBool bottom;
    }

}
