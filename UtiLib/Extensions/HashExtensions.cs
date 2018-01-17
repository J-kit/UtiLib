using System.IO;
using System.Security.Cryptography;
using UtiLib.IO.Cryptography;

// ReSharper disable once CheckNamespace
namespace System
{
    public static class HashExtensions
    {
        public static byte[] ToHash<T>(this FileInfo input) where T : HashAlgorithm
            => HashUtily.Calculate<T>(input, HashFlag.HashFile);

        public static byte[] ToHash<T>(this object input, bool isFile = false) where T : HashAlgorithm
            => HashUtily.Calculate<T>(input, isFile ? HashFlag.HashFile : HashFlag.HashString);

        public static byte[] ToHash<T>(this string input) where T : HashAlgorithm
            => HashUtily.Calculate<T>(input, HashFlag.HashString);

        public static byte[] ToHash(this string input)
            => HashUtily.Calculate<MD5>(input, HashFlag.HashString);
    }

    public enum HashFlag
    {
        HashString,
        HashFile,
    }
}