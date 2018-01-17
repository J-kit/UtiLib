using System;
using UtiLib.Shared.Interfaces;

namespace UtiLib.Logging.FormatProvider
{
    /// <summary>
    ///
    /// </summary>
    public class DefaultFormatProvider : ILogFormatProvider
    {
        /// <summary>
        ///  Formats to [hh:mm:ss] Text <see cref="input"/>
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public string Format(string input) => $"[{Time}] {input}";

        private string Time => DateTime.Now.ToString("h:mm:ss");
    }
}