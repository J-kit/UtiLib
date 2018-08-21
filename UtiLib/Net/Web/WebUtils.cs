using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace UtiLib.Net.Web
{
    internal class WebUtils
    {
        internal static ByteArrayContent ToByteArrayContent(NameValueCollection collection)
        {
            string str = string.Empty;
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string allKey in collection.AllKeys)
            {
                stringBuilder.Append(str);
                stringBuilder.Append(WebUtils.UrlEncode(allKey));
                stringBuilder.Append("=");
                stringBuilder.Append(WebUtils.UrlEncode(collection[allKey]));
                str = "&";
            }

            var postBytes = Encoding.ASCII.GetBytes(stringBuilder.ToString());

            var postResult = new ByteArrayContent(postBytes);
            postResult.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

            return postResult;
        }

        internal static string UrlEncode(string str)
        {
            if (str == null)
                return (string)null;
            return UrlEncode(str, Encoding.UTF8);
        }

        private static string UrlEncode(string str, Encoding e)
        {
            if (str == null)
                return (string)null;
            return Encoding.ASCII.GetString(UrlEncodeToBytes(str, e));
        }

        internal static byte[] UrlEncodeToBytes(string str, Encoding e)
        {
            if (str == null)
                return (byte[])null;
            byte[] bytes = e.GetBytes(str);
            return UrlEncodeBytesToBytesInternal(bytes, 0, bytes.Length, false);
        }

        private static byte[] UrlEncodeBytesToBytesInternal(byte[] bytes, int offset, int count, bool alwaysCreateReturnValue)
        {
            int num1 = 0;
            int num2 = 0;
            for (int index = 0; index < count; ++index)
            {
                char ch = (char)bytes[offset + index];
                if (ch == ' ')
                    ++num1;
                else if (!IsSafe(ch))
                    ++num2;
            }
            if (!alwaysCreateReturnValue && num1 == 0 && num2 == 0)
                return bytes;
            byte[] numArray1 = new byte[count + num2 * 2];
            int num3 = 0;
            for (int index1 = 0; index1 < count; ++index1)
            {
                byte num4 = bytes[offset + index1];
                char ch = (char)num4;
                if (IsSafe(ch))
                    numArray1[num3++] = num4;
                else if (ch == ' ')
                {
                    numArray1[num3++] = (byte)43;
                }
                else
                {
                    byte[] numArray2 = numArray1;
                    int index2 = num3;
                    int num5 = index2 + 1;
                    int num6 = 37;
                    numArray2[index2] = (byte)num6;
                    byte[] numArray3 = numArray1;
                    int index3 = num5;
                    int num7 = index3 + 1;
                    int hex1 = (int)(byte)IntToHex((int)num4 >> 4 & 15);
                    numArray3[index3] = (byte)hex1;
                    byte[] numArray4 = numArray1;
                    int index4 = num7;
                    num3 = index4 + 1;
                    int hex2 = (int)(byte)IntToHex((int)num4 & 15);
                    numArray4[index4] = (byte)hex2;
                }
            }
            return numArray1;
        }

        private static char IntToHex(int n)
        {
            if (n <= 9)
                return (char)(n + 48);
            return (char)(n - 10 + 97);
        }

        private static bool IsSafe(char ch)
        {
            if (ch >= 'a' && ch <= 'z' || ch >= 'A' && ch <= 'Z' || (ch >= '0' && ch <= '9' || ch == '!'))
                return true;
            switch (ch)
            {
                case '\'':
                case '(':
                case ')':
                case '*':
                case '-':
                case '.':
                case '_':
                    return true;

                default:
                    return false;
            }
        }
    }
}
