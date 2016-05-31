using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using UnityEngine;

namespace HappyFunTimes
{
    public class HFTLogger
    {
        static public void Log(string s)
        {
            Debug.Log(s);
        }

        static public void Warn(string s)
        {
            Debug.Log(s);
        }

        static public void Error(string s)
        {
            Debug.LogError(s);
        }

        static public void Error(System.Exception ex)
        {
            Debug.LogException(ex);
        }
    }

}  // namespace HappyFunTimes


