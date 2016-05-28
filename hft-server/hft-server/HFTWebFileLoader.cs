using System;

namespace HappyFunTimes
{
    public class HFTWebFileLoader
    {
        public delegate void AddFunc(string filename, byte[] bytes);

        static public void LoadFiles(AddFunc addFunc)
        {
        }
    }
}

