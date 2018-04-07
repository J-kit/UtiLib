using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtiLib.Common
{
    public class RandomExtended : Random
    {
        public const string DefaultSet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

        public string NextString(int length, string set = DefaultSet)
        {
            var sb = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                sb.Append(set[Next(set.Length)]);
            }

            return sb.ToString();
        }
    }
}