using System.IO;

namespace UtiLib.Logging.LogProvider
{
    public class FileLogger : StreamLogger
    {
        public FileLogger(string fileName)
        {
            Initiate(fileName);
        }

        public FileLogger(FileInfo file)
        {
            Initiate(file.FullName);
        }

        private void Initiate(string fileName)
        {
            var logStream = File.Open(fileName, File.Exists(fileName) ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.Read);
            base.UpdateBaseStream(logStream);
        }

        ~FileLogger()
        {
            base.Dispose();
        }
    }
}