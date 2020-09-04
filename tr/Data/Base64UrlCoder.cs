// Base64UrlCoder.cs copyright tomjones - derived from this site https://raw.githubusercontent.com/neosmart/UrlBase64/master/UrlBase64/UrlBase64.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace tr.Data
{
    public enum PaddingPolicy
    {
        Discard,
        Preserve,
    }

    public static class Base64UrlCoder
    {
        private static readonly char[] TwoPads = { '=', '=' };

        public static string Encode(string str)
        {
            byte[] array = Encoding.Default.GetBytes(str);  //  TODO change to UF8
            return Encode(array);
        }

        public static string Encode(byte[] bytes, PaddingPolicy padding = PaddingPolicy.Discard)
        {
            var encoded = Convert.ToBase64String(bytes);
            if (padding == PaddingPolicy.Discard)
            {
                encoded = encoded.TrimEnd('=');
            }
            string test = Encoding.UTF8.GetString(Decode(encoded));
            return encoded.Replace('+', '-').Replace('/', '_');
        }
        /// <summary>
        /// Convert from a character string of encoded bytes to a clear text string
        /// </summary>
        /// <param name="encoded"></param>
        /// <returns></returns>
        public static string StrDecode (string encoded)
        {
            byte[] decoded = Decode(encoded);
            return Encoding.UTF8.GetString(decoded, 0, decoded.Length);
        }
        public static byte[] Decode(string encoded)
        {
      //      if (encoded.Contains(" ")) { encoded = new string(encoded.Where(c => !isWhiteSpace(c).))}
            var chars = new List<char>(encoded.Where(c => !isWhiteSapce(c)));

            for (int i = 0; i < chars.Count; ++i)
            {
                if (chars[i] == '_')
                {
                    chars[i] = '/';
                }
                else if (chars[i] == '-')
                {
                    chars[i] = '+';
                }
            }

            switch (chars.Count % 4)
            {
                case 2:
                    chars.AddRange(TwoPads);
                    break;
                case 3:
                    chars.Add('=');
                    break;
            }

            var array = chars.ToArray();

            return Convert.FromBase64CharArray(array, 0, array.Length);
        }
        /// <summary>
        /// test if character is considered to be whitespace
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private static bool isWhiteSapce(char c)
        {
            if (c == ' ') return true;
            if (c == '\r') return true;
            if (c == '\n') return true;
            return false;
        }

        /// <summary>
        /// Converts a dictionary to a uri fragment
        /// </summary>
        /// <param name="fragIn"></param>
        /// <returns>fragment as a string to be converted to URI safe coding</returns>
        public static string Fragment (Dictionary<string, string> fragIn, string sep)
        {
            StringBuilder fragOut = new StringBuilder();
            string equ = "=";
            foreach (KeyValuePair<string, string> entry in fragIn)
            {
                fragOut.Append(sep+entry.Key+equ+entry.Value);
                sep = "&";
            }
            return fragOut.ToString();
        }
    }
}