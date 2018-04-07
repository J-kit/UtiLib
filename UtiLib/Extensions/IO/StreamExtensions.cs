using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtiLib;

// ReSharper disable once CheckNamespace
namespace System.IO
{
    public static class StreamExtensions
    {
        public static MemoryStream ToMemoryStream(this byte[] input)
        {
            return new MemoryStream(input);
        }

        public static string GetString(this Stream input)
        {
            using (var iStream = new StreamReader(input))
            {
                return iStream.ReadToEnd();
            }
        }

        public static async Task<string> GetStringAsync(this Stream input)
        {
            using (var iStream = new StreamReader(input))
            {
                return await iStream.ReadToEndAsync();
            }
        }

        public static async Task<T> ParseStreamAsJsonAsync<T>(this Stream inputStream) where T : class
        {
            try
            {
                var rString = await inputStream.GetStringAsync();
                return Settings.JsonSerializer.DeserializeObject<T>(rString.GetBytes());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while parsing. Details{ex}");
            }

            return default;
        }

        public static void WriteToStream(this Stream outputStream, string text)
        {
            using (var oStream = new StreamWriter(outputStream))
            {
                oStream.WriteAsync(text);
            }
        }

        public static async Task WriteToStreamAsync(this Stream outputStream, string text)
        {
            using (var oStream = new StreamWriter(outputStream))
            {
                await oStream.WriteAsync(text);
            }
        }
    }
}