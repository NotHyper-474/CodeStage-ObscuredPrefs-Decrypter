using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace CodeStage_Decrypter
{
    internal static class Base64Utils
    {
        public static bool IsBase64(char[] b64)
        {
            string s = new string(b64).Trim();

            return (s.Length % 4 == 0) && Regex.IsMatch(s, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
            /*try
            {
                Convert.FromBase64CharArray(b64, 0, b64.Length);
                return true;
            }
            catch (Exception)
            {
                return false;
            }*/
        }

        public static bool IsBase64(string b64)
        {
            return IsBase64(b64.ToCharArray());
        }

        public static string CharArrayToBase64(char[] array)
        {
            return Convert.ToBase64String(CharArrayToBytes(array));
        }

        public static string StringToBase64(string str)
        {
            return CharArrayToBase64(str.ToCharArray());
        }

        public static char[] FromBase64ToCharArray(char[] b64)
        {
            return BytesToCharArray(Convert.FromBase64CharArray(b64, 0, b64.Length));
        }

        public static char[] FromBase64ToCharArray(string b64)
        {
            return BytesToCharArray(Convert.FromBase64String(b64));
        }

        public static string FromBase64ToString(string b64)
        {
            return new string(FromBase64ToCharArray(b64));
        }

        public static byte[] CharArrayToBytes(char[] array)
        {
            return Encoding.UTF8.GetBytes(array);
        }

        public static char[] BytesToCharArray(byte[] bytes)
        {
            return Encoding.UTF8.GetChars(bytes);
        }
    }
}
