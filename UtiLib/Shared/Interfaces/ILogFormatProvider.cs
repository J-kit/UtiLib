namespace UtiLib.Shared.Interfaces
{
    public interface ILogFormatProvider
    {
        /// <summary>
        /// Formats the logtext to a more convenient one before writing (Example: [Timestamp] Logtext)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        string Format(string input);
    }
}