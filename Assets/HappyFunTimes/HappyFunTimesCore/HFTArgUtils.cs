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

    public class HFTArgChecker
    {
        public HFTArgChecker(HFTLog log, string prefix)
        {
            log_ = log;
            prefix_ = prefix + "-";
            dashPrefix_ = "--" + prefix;
            noPrefix_ = "--no-" + prefix;
        }

        public void AddArg(string arg)
        {
            validArgs_.Add(arg);
        }

        public bool CheckArgs()
        {
            bool good = true;
            #if (!UNITY_IOS)
            string[] args = System.Environment.GetCommandLineArgs();
            foreach (string arg in args)
            {
                if (arg.StartsWith(dashPrefix_))
                {
                    int equalsNdx = arg.IndexOf('=');
                    string checkArg = equalsNdx >= 0 ? arg.Substring(0, equalsNdx) : arg;
                    if (!validArgs_.Contains(checkArg.Substring(2)))
                    {
                        good = false;
                        log_.Warn("unknown option: " + arg);
                    }
                }
                else if (arg.StartsWith(noPrefix_))
                {
                    int equalsNdx = arg.IndexOf('=');
                    string checkArg = equalsNdx >= 0 ? arg.Substring(0, equalsNdx) : arg;
                    if (!validArgs_.Contains(checkArg.Substring(5)))
                    {
                        good = false;
                        log_.Warn("unknown option: " + arg);
                    }
                }

            }
            #endif

            foreach (string evar in System.Environment.GetEnvironmentVariables().Keys)
            {
                string opt = evar.ToLowerInvariant().Replace("_", "-");
                if (opt.StartsWith(prefix_))
                {
                    if (!validArgs_.Contains(opt))
                    {
                        good = false;
                        log_.Warn("unknown environment variable: " + evar);
                    }
                }
            }
            return good;
        }

        public string GetHFTDashName(string camelCase)
        {
            return prefix_ + s_camelRE.Replace(camelCase, s_camelReplace).ToLower();
        }

        HFTLog log_;
        string prefix_;
        string dashPrefix_;
        string noPrefix_;
        HashSet<string> validArgs_ = new HashSet<string>();

        static string s_camelReplace = @"-$1$2";
        static Regex s_camelRE = new Regex(@"([A-Z])([a-z0-9])", RegexOptions.Singleline);
    }

    // Is this a good thing? No idea. It's D.R.Y. It's also auto-completeable in the
    // editor where as using a dict or somethig is not. And it checks for bad arguments so uses
    // could possibly get help.
    public class HFTArgsBase {
        public HFTArgsBase(string prefix)
        {
            HFTLog log = new HFTLog("HFTArgs");
            HFTArgChecker checker = new HFTArgChecker(log, prefix);

            HFTArgParser p = HFTArgParser.GetInstance();
            System.Reflection.FieldInfo[] fields = this.GetType().GetFields();
            foreach (System.Reflection.FieldInfo info in fields) {
                object field = System.Activator.CreateInstance(info.FieldType);
                string dashName = checker.GetHFTDashName(info.Name);
                checker.AddArg(dashName);
                info.FieldType.GetMethod("Init").Invoke(field, new object[]{ dashName, p });
                info.SetValue(this, field);
            }

            checker.CheckArgs();
        }
    }

    // This class fills in the args directly from public fields of
    // sub classes. Where as HFTArgsBase above gets the fields
    // but also records if they were set or not. Not sure that's a good thing
    public class HFTArgsToFields {
        public static bool Apply(string prefix, object obj)
        {
            HFTLog log = new HFTLog("HFTArgsDirect");
            HFTArgChecker checker = new HFTArgChecker(log, prefix);

            HFTArgParser p = HFTArgParser.GetInstance();
            System.Reflection.FieldInfo[] fields = obj.GetType().GetFields();
            foreach (System.Reflection.FieldInfo info in fields) {
                //object field = System.Activator.CreateInstance(info.FieldType);
                object value = null;
                System.Type fieldType = info.FieldType;
                string dashName = checker.GetHFTDashName(info.Name);
                checker.AddArg(dashName);
                // Should do this with reflection but ...
                if (fieldType == typeof(string))
                {
                    string strValue = "";
                    if (p.TryGet<string>(dashName, ref strValue))
                    {
                        value = strValue;
                    }
                }
                else if (fieldType == typeof(int))
                {
                    int intValue = 0;
                    if (p.TryGet<int>(dashName, ref intValue))
                    {
                        value = intValue;
                    }
                }
                else if (fieldType == typeof(bool))
                {
                    bool boolValue = false;
                    if (p.TryGetBool(dashName, ref boolValue))
                    {
                        value = boolValue;
                    }
                }
                else if (fieldType == typeof(float))
                {
                    float floatValue = 0;
                    if (p.TryGet<float>(dashName, ref floatValue))
                    {
                        value = floatValue;
                    }
                }
                else
                {
                    throw new System.InvalidOperationException("no support for type: " + fieldType.Name);
                }

                if (value != null)
                {
                    info.SetValue(obj, value);
                }
            }

            return checker.CheckArgs();
        }
    }
}
