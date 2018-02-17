using System.Collections.Generic;
using System.IO;
using System.Text;

// ReSharper disable once CheckNamespace
namespace System
{
    public static class FileInfoExtensions
    {
        #region Writing

        public static void WriteAllBytes(this FileInfo file, byte[] bytes) => File.WriteAllBytes(file.FullName, bytes);

        public static void WriteAllLines(this FileInfo file, string[] contents) => File.WriteAllLines(file.FullName, contents);

        public static void WriteAllLines(this FileInfo file, string[] contents, Encoding encoding) => File.WriteAllLines(file.FullName, contents, encoding);

        public static void WriteAllLines(this FileInfo file, IEnumerable<string> contents) => File.WriteAllLines(file.FullName, contents);

        public static void WriteAllLines(this FileInfo file, IEnumerable<string> contents, Encoding encoding) => File.WriteAllLines(file.FullName, contents, encoding);

        public static void WriteAllText(this FileInfo file, string contents) => File.WriteAllText(file.FullName, contents);

        public static void WriteAllText(this FileInfo file, string contents, Encoding encoding) => File.WriteAllText(file.FullName, contents, encoding);

        #endregion Writing

        #region Reading

        public static string ReadAllText(this FileInfo file) => File.ReadAllText(file.FullName);

        public static string ReadAllText(this FileInfo file, Encoding encoding) => File.ReadAllText(file.FullName, encoding);

        public static byte[] ReadAllBytes(this FileInfo file) => File.ReadAllBytes(file.FullName);

        public static string[] ReadAllLines(this FileInfo file) => File.ReadAllLines(file.FullName);

        public static string[] ReadAllLines(this FileInfo file, Encoding encoding) => File.ReadAllLines(file.FullName, encoding);

        public static IEnumerable<string> ReadLines(this FileInfo file) => File.ReadAllLines(file.FullName);

        public static IEnumerable<string> ReadLines(this FileInfo file, Encoding encoding) => File.ReadAllLines(file.FullName, encoding);

        #endregion Reading

        /// <summary>
        ///     Checks wether file exists or not
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool Exists(this FileInfo file)
        {
            if (file == null)
                return false;

            if (file.Exists)
                return true;

            file.Refresh();
            return file.Exists;
        }
    }
}