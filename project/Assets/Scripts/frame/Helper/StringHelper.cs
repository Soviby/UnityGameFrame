using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
 
    public static class StringHelper
    {
        public static string Repeat(this string s, int n)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < n; i++)
            {
                sb.Append(s);
            }
            return sb.ToString();
        }

        static Regex _substVariablesRE;

        static Regex substVariablesRE
        {
            get
            {
                if (_substVariablesRE == null)
                {
                    _substVariablesRE = new Regex(@"\{(.+?)\}", RegexOptions.Compiled);
                }

                return _substVariablesRE;
            }
        }

        public static int FindByPredicate(string str, Predicate<char> pred)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (pred(str[i]))
                    return i;
            }

            return -1;
        }

        public static string Fmt(this string text, params object[] args)
        {
            return string.Format(text, args);
        }

        public static byte[] HexToBytes(this string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture,
                                                          "The binary key cannot have an odd number of digits: {0}",
                                                          hexString));
            }

            var hexAsBytes = new byte[hexString.Length / 2];
            for (int index = 0; index < hexAsBytes.Length; index++)
            {
                string byteValue = "";
                byteValue += hexString[index * 2];
                byteValue += hexString[index * 2 + 1];
                hexAsBytes[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return hexAsBytes;
        }

        public static string ListToString<T>(this List<T> list, string split = ",")
        {
            StringBuilder sb = new StringBuilder();
            if (list != null)
            {
                foreach (T t in list)
                {
                    sb.Append(t);
                    sb.Append(split);
                }
            }
            return sb.ToString();
        }

        public static string Subst(this string src, object args)
        {
            return SubstVars(src, args);
        }

        public static string Subst(this string src, IDictionary<string, string> args)
        {
            return SubstVars(src, args);
        }

        public static string SubstVars(string src, object args)
        {
            var t = args.GetType();
            return substVariablesRE.Replace(src, m =>
            {
                string key = m.Groups[1].Value;
                var prop = t.GetProperty(key);
                if (prop == null)
                {
                    return "##" + key + "##";
                }

                var value = prop.GetValue(args, null);
                if (value == null)
                {
                    return "$$" + key + "$$";
                }

                return value.ToString();
            });
        }

        public static string SubstVars(string src, IDictionary<string, string> args)
        {
            return substVariablesRE.Replace(src, m =>
            {
                string key = m.Groups[1].Value;
                if (args.TryGetValue(key, out var value))
                {
                    return value;
                }
                return "##" + key + "##";
            });
        }

        public static byte[] ToByteArray(this string str)
        {
            byte[] byteArray = Encoding.Default.GetBytes(str);
            return byteArray;
        }

        public static IEnumerable<byte> ToBytes(this string str)
        {
            byte[] byteArray = Encoding.Default.GetBytes(str);
            return byteArray;
        }

        public static byte[] ToUtf8(this string str)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(str);
            return byteArray;
        }

        private static readonly string no_breaking_space = "\u00A0";
        public static string ChangeStringSpace(this string str)
        {
            if (str.Contains(no_breaking_space))
            {
                str = str.Replace(no_breaking_space, " ");
            }
            return str;
        }

        public static string FirstSymbolToLower(this string str)
        {
            StringBuilder sb = new StringBuilder(str);
            sb.Replace(str[0], Char.ToLowerInvariant(str[0]), 0, 1);
            return sb.ToString();
        }

        public static string MessageToStr(object message)
        {
            return JsonConvert.SerializeObject(message);
        }
    }

