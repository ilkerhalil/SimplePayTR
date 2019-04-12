using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SimplePayTR.Helper
{
    public class HashHelper
    {
        public static string GetSha1(string text)
        {
            var ue = Encoding.GetEncoding("ISO-8859-9");
            var message = ue.GetBytes(text);
            var hashString = new SHA1Managed();
            var hashValue = hashString.ComputeHash(message);
            return GetHexaDecimal(hashValue);
        }

        private static string GetHexaDecimal(IReadOnlyList<byte> bytes)
        {
            var s = new StringBuilder();
            var length = bytes.Count;
            for (var n = 0; n <= length - 1; n++)
            {
                s.Append($"{bytes[n],2:x}".Replace(" ", "0"));
            }
            return s.ToString();
        }
    }



}
