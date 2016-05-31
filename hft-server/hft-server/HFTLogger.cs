using System;
using System.Collections.Generic;

namespace HappyFunTimes
{
    public class HFTLogger
    {
        static public void Log(string s)
        {
            System.Console.WriteLine(s);
        }

        static public void Warn(string s)
        {
            System.Console.WriteLine(s);
        }

        static public void Error(string s)
        {
            System.Console.Error.WriteLine(s);
        }

        static public void Error(System.Exception ex)
        {
            System.Console.Error.WriteLine(ex.ToString());
        }
    }
}

