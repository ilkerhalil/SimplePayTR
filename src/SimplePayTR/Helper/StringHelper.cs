using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SimplePayTR.Helper
{
    public static class StringHelpers
    {
        private const string Letters = "abcdefghijklmnopqrstuvwxyz";
        private const string Digits = "0123456789";
        
        private const string Special = "-()+*~";

        /// <summary>
        /// Iterates thru a string, and removes anything set to clean.
        /// Except---Does NOT remove anything in exceptionsToAllow
        /// </summary>
        /// <param name="stream">
        /// The string to clean</param>
        /// <param name="cleanWhiteSpace"></param>
        /// <param name="cleanDigits"></param>
        /// <param name="cleanLetters"></param>
        /// <param name="exceptionsToAllow"></param>
        /// <param name="cleanSpecial"></param>
        /// <returns>
        /// The same string, missing all elements that were set to clean
        /// (except when a character was listed in exceptionsToAllow)
        /// </returns>
        public static string Clean(this string stream, bool cleanWhiteSpace,
            bool cleanDigits, bool cleanLetters, string exceptionsToAllow, bool cleanSpecial)
        {
            try
            {
                var newString = string.Empty;
                var blessed = string.Empty;
                if (!cleanDigits)
                    blessed += Digits;

                if (!cleanLetters)
                    blessed += Letters;

                if (!cleanSpecial)
                    blessed += Special;

                blessed += exceptionsToAllow;
                //we set the comparison string to lower
                //and will compare each character's lower case version
                //against the comparison string, without
                //altering the original case of the character
                blessed = blessed.ToLower();
                for (var i = 0; i < stream.Length; i++)
                {
                    var character = stream.Substring(i, 1);
                    if (blessed.Contains(character.ToLower()))
                        //add the altered character to the new string:
                        newString += character;
                    else if (character.Trim() == string.Empty &&
                    !cleanWhiteSpace)
                        newString += character;
                }
                return newString;
            }
            catch (Exception)
            {
                //ErrorTool.ProcessError(ex);
                return null;
            }
        }

        #region StringLocators

        #endregion

        private static readonly Dictionary<string, Regex> Cache = new Dictionary<string, Regex>();

        private static Regex CacheRegex(string r)
        {

            if (!Cache.ContainsKey(r))
                Cache[r] = new Regex(r, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            return Cache[r];
        }

        public static Match Match(this string s, string regex)
        {
            var r = CacheRegex(regex);
            return r.Match(s);
        }
    }
}