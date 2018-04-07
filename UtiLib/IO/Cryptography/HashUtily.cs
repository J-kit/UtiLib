using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;

namespace UtiLib.IO.Cryptography
{
    public static class HashUtily
    {
        private static readonly ConcurrentDictionary<Type, MethodInfo> MethodCache = new ConcurrentDictionary<Type, MethodInfo>();

        public static byte[] Calculate<T>(object input, HashFlag flag = 0) where T : HashAlgorithm
        {
            using (var fs = DecodeStream(input, flag))
                return GetHashAlgo<T>()?.ComputeHash(fs);
        }

        private static HashAlgorithm GetHashAlgo<T>() where T : HashAlgorithm
        {
            var typeVal = typeof(T);

            if (!MethodCache.TryGetValue(typeVal, out var cMeth))
            {
                cMeth = typeVal.GetMethodsFromType(new Type[0]).FirstOrDefault(m => m.Name == "Create");
                MethodCache[typeVal] = cMeth;
            }

            if (cMeth != null)
                return cMeth.Invoke(null, null) as HashAlgorithm;

            return default(T);
        }

        private static Stream DecodeStream(object input, HashFlag flag)
        {
            switch (input)
            {
                case string ip2:
                    if (flag == HashFlag.HashFile)
                        if (File.Exists(ip2))
                            return File.Open(ip2, FileMode.Open, FileAccess.Read, FileShare.Read);
                        else
                            throw new Exception("File does not exist");
                    else
                        return new MemoryStream(Settings.DefaultEncoding.GetBytes(ip2));

                case byte[] ip3:
                    return new MemoryStream(ip3);

                case IEnumerable<byte> ip4:
                    return new MemoryStream(ip4.ToArray());

                case FileInfo ip5 when Exists(ip5):
                    return ip5.Open(FileMode.Open, FileAccess.Read, FileShare.Read);

                case Stream str:
                    return str;
            }

            throw new Exception("Can't decode object");
        }

        private static bool Exists(this FileInfo fi)
        {
            fi.Refresh();
            return fi.Exists;
        }
    }
}