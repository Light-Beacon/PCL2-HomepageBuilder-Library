using System;
using System.Collections.Generic;

namespace PageBuilder
{
    public static class RandomName
    {
        static List<string> namelist = new List<string>();
        public static string GiveOne()
        {
            string str;
            do
            {
                Random r = new Random();
                str = r.Next(0x1000000, 0xFFFFFFF).ToString("X");
            }
            while (namelist.Contains(str));
            namelist.Add(str);
            return str;
        }
    }
}