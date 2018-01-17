using System;
using UtiLib.Shared.Interfaces;

namespace UtiLib.Logging.FormatProvider
{
    public class DefaultFormatProvider : ILogFormatProvider
    {
        public string Format(string input) => $"[{Time}] {input}";

        private string Time => DateTime.Now.ToString("h:mm:ss");
    }
}