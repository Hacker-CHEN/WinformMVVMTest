using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UW.Common
{
    public static class StringExtension
    {
        public static bool ToBoolean(this string str, bool def = false)
        {
            bool result;
            return !bool.TryParse(str, out result) ? def : result;
        }

        public static char ToChar(this string str, char def = '\0')
        {
            char result;
            return !char.TryParse(str, out result) ? def : result;
        }

        public static byte ToByte(this string str, byte def = 0)
        {
            byte result;
            return !byte.TryParse(str, out result) ? def : result;
        }

        public static sbyte ToSByte(this string str, sbyte def = 0)
        {
            sbyte result;
            return !sbyte.TryParse(str, out result) ? def : result;
        }

        public static short ToShort(this string str, short def = 0)
        {
            short result;
            return !short.TryParse(str, out result) ? def : result;
        }

        public static ushort ToUShort(this string str, ushort def = 0)
        {
            ushort result;
            return !ushort.TryParse(str, out result) ? def : result;
        }

        public static int ToInt(this string str, int def = 0)
        {
            int result;
            return !int.TryParse(str, out result) ? def : result;
        }

        public static uint ToUInt(this string str, uint def = 0)
        {
            uint result;
            return !uint.TryParse(str, out result) ? def : result;
        }

        public static long ToLong(this string str, long def = 0)
        {
            long result;
            return !long.TryParse(str, out result) ? def : result;
        }

        public static ulong ToULong(this string str, ulong def = 0)
        {
            ulong result;
            return !ulong.TryParse(str, out result) ? def : result;
        }

        public static Decimal ToDecimal(this string str, Decimal def = 0M)
        {
            Decimal result;
            return !Decimal.TryParse(str, out result) ? def : result;
        }

        public static double ToDouble(this string str, double def = 0.0)
        {
            double result;
            return !double.TryParse(str, out result) ? def : result;
        }

        public static float ToFloat(this string str, float def = 0.0f)
        {
            float result;
            return !float.TryParse(str, out result) ? def : result;
        }

        public static DateTime ToDateTime(this string str, DateTime def = default(DateTime))
        {
            DateTime result;
            return !DateTime.TryParse(str, out result) ? def : result;
        }

        public static Guid ToGuid(this string str, Guid def = default(Guid))
        {
            Guid result;
            return !Guid.TryParse(str, out result) ? def : result;
        }

        public static bool IsBoolean(this string str) => bool.TryParse(str, out bool _);

        public static bool IsChar(this string str) => char.TryParse(str, out char _);

        public static bool IsByte(this string str) => byte.TryParse(str, out byte _);

        public static bool IsSByte(this string str) => sbyte.TryParse(str, out sbyte _);

        public static bool IsDecimal(this string str) => Decimal.TryParse(str, out Decimal _);

        public static bool IsShort(this string str) => short.TryParse(str, out short _);

        public static bool IsUShort(this string str) => ushort.TryParse(str, out ushort _);

        public static bool IsInt(this string str) => int.TryParse(str, out int _);

        public static bool IsUInt(this string str) => uint.TryParse(str, out uint _);

        public static bool IsLong(this string str) => long.TryParse(str, out long _);

        public static bool IsULong(this string str) => ulong.TryParse(str, out ulong _);

        public static bool IsDouble(this string str) => double.TryParse(str, out double _);

        public static bool IsFloat(this string str) => float.TryParse(str, out float _);

        public static bool IsDateTime(this string str) => DateTime.TryParse(str, out DateTime _);

        public static bool IsGuid(this string str) => Guid.TryParse(str, out Guid _);

        public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);

        public static bool IsValid(this string str) => !string.IsNullOrEmpty(str);

        public static bool IsNullOrWhiteSpace(this string str) => string.IsNullOrWhiteSpace(str);

        public static bool IsNumeric(this string value) => Regex.IsMatch(value, "^[+-]?\\d*[.]?\\d*$");

        public static bool IsNumber(this string str) => new Regex("^[0-9]+$").Match(str).Success;

        public static bool IsNumberWithSign(this string str)
        {
            return new Regex("^[+-]?[0-9]+$").Match(str).Success;
        }

        public static bool IsIP4(this string str)
        {
            string[] strArray = str.Split(".");
            if (strArray.Length != 4)
                return false;
            foreach (string str1 in strArray)
            {
                if (!str1.Trim().IsNumber() || str1.Trim().ToInt() < 0 || str1.Trim().ToInt() > (int)byte.MaxValue)
                    return false;
            }
            return true;
        }

        public static bool IsValidEmail(this string str)
        {
            string str1 = str.Trim();
            if (str1 == "" || str1.Length == 0 || str.SplitSeparatorCount("@") != 1)
                return false;
            string[] strArray = str1.Split('@');
            if (strArray.Length != 2 || strArray[0] == "" || strArray[1] == "")
                return false;
            foreach (string str2 in strArray)
            {
                for (int startIndex = 0; startIndex < str2.Length; ++startIndex)
                {
                    if ("abcdefghijklmnopqrstuvwxyz_-.0123456789".SplitSeparatorCount(str2.Substring(startIndex, 1).ToLower()) == 0)
                        return false;
                }
            }
            return true;
        }

        public static bool IsPureEnglishChar(string str) => new Regex("^[a-zA-Z]+$").IsMatch(str);

        public static bool IsHasCnChar(this string str) => new Regex("[一-龥]").Match(str).Success;

        public static bool IsNameSpace(string str)
        {
            return new Regex("^[a-zA-Z.]+$").IsMatch(str) && !str.StartsWith(".") && !str.EndsWith(".") && str.SplitSeparatorCount(".") <= 3 && !str.Contains("..") && !str.Contains("...");
        }

        public static bool IsTel(this string str) => Regex.IsMatch(str, "\\d{3}-\\d{8}|\\d{4}-\\d{7}");

        public static string DealPath(this string dir)
        {
            return StringExtension.IsNullOrEmpty(dir) || (int)dir[dir.Length - 1] == (int)Path.DirectorySeparatorChar ? dir : dir + Path.DirectorySeparatorChar.ToString();
        }

        public static string FormatString(
          this string str,
          int stringLength,
          char placeholder = ' ',
          bool isFront = false)
        {
            int length1 = str.Length;
            for (int length2 = str.Length; length2 < stringLength; ++length2)
                str = !isFront ? str + placeholder.ToString() : placeholder.ToString() + str;
            return str;
        }

        public static bool EqualsIgnoreCase(this string value, params string[] strs)
        {
            foreach (string str in strs)
            {
                if (string.Equals(value, str, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        public static bool EqualsIgnoreCase(this string value, string comparestring)
        {
            return string.Equals(value, comparestring, StringComparison.OrdinalIgnoreCase);
        }

        public static string Repet(this string str, int count)
        {
            if (count <= 0)
                return string.Empty;
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("");
            for (int index = 0; index < count; ++index)
                stringBuilder.Append(str);
            return stringBuilder.ToString();
        }

        public static string Left(this string str, int length)
        {
            return StringExtension.IsNullOrEmpty(str) || length > str.Length ? str : str.Substring(0, length);
        }

        public static string Middle(this string str, int start, int length)
        {
            if (StringExtension.IsNullOrEmpty(str))
                return str;
            return length > str.Length - start ? str.Substring(start, str.Length - start) : str.Substring(start, length);
        }

        public static string TrimToMaxLength(this string value, int maxLength)
        {
            return value != null && value.Length > maxLength ? value.Substring(0, maxLength) : value;
        }

        public static string Right(this string s, int length)
        {
            return StringExtension.IsNullOrEmpty(s) || length >= s.Length ? s : s.Substring(s.Length - length, length);
        }

        public static string[] Split(this string str, string separator, bool ignoreEmpty = false)
        {
            string[] strArray = str.Split(new string[1]
            {
        separator
            }, StringSplitOptions.None);
            if (!ignoreEmpty)
                return strArray;
            List<string> stringList = new List<string>();
            foreach (string str1 in strArray)
            {
                if (!StringExtension.IsNullOrEmpty(str1))
                    stringList.Add(str1);
            }
            return stringList.ToArray();
        }

        public static int SplitSeparatorCount(this string str, string separator)
        {
            if (!str.Contains(separator))
                return 0;
            string str1 = str.Replace(separator, "");
            return (str.Length - str1.Length) / separator.Length;
        }

        public static string SplitLast(this string str, string separator, bool ignoreEmpty = false)
        {
            string[] strArray = str.Split(separator, ignoreEmpty);
            return strArray.Length != 0 ? strArray[strArray.Length - 1] : string.Empty;
        }

        public static string SplitFirst(this string str, string separator, bool ignoreEmpty = false)
        {
            string[] strArray = str.Split(separator, ignoreEmpty);
            return strArray.Length != 0 ? strArray[0] : string.Empty;
        }

        public static string SplitIndex(
          this string str,
          string separator,
          int index,
          bool ignoreEmpty = false)
        {
            string[] strArray = str.Split(separator, ignoreEmpty);
            return strArray.Length != 0 ? strArray[index] : string.Empty;
        }

        public static string SplitBeforeLast(this string str, string separator)
        {
            int length = str.LastIndexOf(separator, StringComparison.Ordinal);
            return length <= 0 ? str : str.Substring(0, length);
        }

        public static byte[] ToEnBytes(this string str, int len)
        {
            byte[] encodeBytes = str.ToEncodeBytes(Encoding.ASCII);
            byte[] destinationArray = new byte[len];
            Array.Copy((Array)encodeBytes, 0, (Array)destinationArray, 0, Math.Min(encodeBytes.Length, len));
            return destinationArray;
        }

        public static byte[] ToEncodeBytes(this string str, Encoding encode) => encode.GetBytes(str);

        public static string ToEnString(this byte[] s)
        {
            return s.ToEncodeString(Encoding.ASCII).Trim(new char[1]).Trim();
        }

        public static string ToEncodeString(this byte[] dealBytes, Encoding encode)
        {
            return encode.GetString(dealBytes);
        }

        public static string Before(this string value, string x)
        {
            int length = value.IndexOf(x, StringComparison.Ordinal);
            return length != -1 ? value.Substring(0, length) : string.Empty;
        }

        public static string Between(this string value, string x, string y)
        {
            int num1 = value.IndexOf(x, StringComparison.Ordinal);
            int num2 = value.LastIndexOf(y, StringComparison.Ordinal);
            if (num1 == -1 || num1 == -1)
                return string.Empty;
            int startIndex = num1 + x.Length;
            return startIndex < num2 ? value.Substring(startIndex, num2 - startIndex).Trim() : string.Empty;
        }

        public static string After(this string value, string x)
        {
            int num = value.LastIndexOf(x, StringComparison.Ordinal);
            if (num == -1)
                return string.Empty;
            int startIndex = num + x.Length;
            return startIndex < value.Length ? value.Substring(startIndex).Trim() : string.Empty;
        }

        public static string Md5String(this string str) => str.Md5String(Encoding.UTF8).ToUpper();

        private static string Md5String(this string str, Encoding encode)
        {
            MD5 md5 = MD5.Create();
            string str1;
            try
            {
                byte[] hash = md5.ComputeHash(encode.GetBytes(str));
                StringBuilder stringBuilder = new StringBuilder();
                foreach (byte num in hash)
                    stringBuilder.Append(num.ToString("x2"));
                str1 = stringBuilder.ToString();
            }
            finally
            {
                md5.Dispose();
            }
            return str1;
        }

        public static string DesEncrypt(this string str, string key = "")
        {
            string str1;
            try
            {
                byte[] bytes1 = Encoding.UTF8.GetBytes((key + "!@#$%^&*").Substring(0, 8));
                byte[] rgbIV = bytes1;
                byte[] bytes2 = Encoding.UTF8.GetBytes(str);
                DESCryptoServiceProvider cryptoServiceProvider = new DESCryptoServiceProvider();
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, cryptoServiceProvider.CreateEncryptor(bytes1, rgbIV), CryptoStreamMode.Write);
                cryptoStream.Write(bytes2, 0, bytes2.Length);
                cryptoStream.FlushFinalBlock();
                str1 = Convert.ToBase64String(memoryStream.ToArray());
            }
            catch
            {
                str1 = "";
            }
            return str1;
        }

        public static string DesDecrypt(this string str, string key = "")
        {
            string str1;
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes((key + "!@#$%^&*").Substring(0, 8));
                byte[] rgbIV = bytes;
                byte[] buffer = Convert.FromBase64String(str);
                DESCryptoServiceProvider cryptoServiceProvider = new DESCryptoServiceProvider();
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, cryptoServiceProvider.CreateDecryptor(bytes, rgbIV), CryptoStreamMode.Write);
                cryptoStream.Write(buffer, 0, buffer.Length);
                cryptoStream.FlushFinalBlock();
                str1 = Encoding.UTF8.GetString(memoryStream.ToArray());
            }
            catch
            {
                str1 = "";
            }
            return str1;
        }

        public static string MyEncrypt(this string str, string key = "")
        {
            string str1;
            try
            {
                byte[] bytes1 = Encoding.UTF8.GetBytes((key + "!@#$%^&*").Substring(0, 8));
                byte[] rgbIV = bytes1;
                byte[] bytes2 = Encoding.UTF8.GetBytes(str);
                DESCryptoServiceProvider cryptoServiceProvider = new DESCryptoServiceProvider();
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, cryptoServiceProvider.CreateEncryptor(bytes1, rgbIV), CryptoStreamMode.Write);
                cryptoStream.Write(bytes2, 0, bytes2.Length);
                cryptoStream.FlushFinalBlock();
                string base64String = Convert.ToBase64String(memoryStream.ToArray());
                int num = 0;
                for (int index = base64String.Length - 1; index >= 0 && base64String[index] == '='; --index)
                    ++num;
                str1 = base64String.Left(base64String.Length - num) + num.ToString();
            }
            catch
            {
                str1 = "";
            }
            return str1;
        }

        public static string MyDecrypt(this string str, string key = "")
        {
            string str1;
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes((key + "!@#$%^&*").Substring(0, 8));
                byte[] rgbIV = bytes;
                int num = str[0].ToString().ToInt();
                str = str.Left(str.Length - 1);
                for (int index = 0; index < num; ++index)
                    str += "=";
                byte[] buffer = Convert.FromBase64String(str);
                DESCryptoServiceProvider cryptoServiceProvider = new DESCryptoServiceProvider();
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, cryptoServiceProvider.CreateDecryptor(bytes, rgbIV), CryptoStreamMode.Write);
                cryptoStream.Write(buffer, 0, buffer.Length);
                cryptoStream.FlushFinalBlock();
                str1 = Encoding.UTF8.GetString(memoryStream.ToArray());
            }
            catch
            {
                str1 = "";
            }
            return str1;
        }

        public static string Base64Encode(this string value) => value.Base64Encode((Encoding)null);

        public static string Base64Encode(this string value, Encoding encoding)
        {
            encoding = encoding ?? Encoding.UTF8;
            return Convert.ToBase64String(encoding.GetBytes(value));
        }

        public static string Base64Decode(this string encodedValue)
        {
            return encodedValue.Base64Decode((Encoding)null);
        }

        public static string Base64Decode(this string encodedValue, Encoding encoding)
        {
            encoding = encoding ?? Encoding.UTF8;
            byte[] bytes = Convert.FromBase64String(encodedValue);
            return encoding.GetString(bytes);
        }

        public static string SHA1Hash(this string stringToHash)
        {
            return StringExtension.IsNullOrEmpty(stringToHash) ? (string)null : Convert.ToBase64String(new SHA1CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(stringToHash)));
        }

        public static string RemoveLeft(this string str, int number_of_characters)
        {
            return str.Length <= number_of_characters ? "" : str.Substring(number_of_characters);
        }

        public static string RemoveRight(this string str, int number_of_characters)
        {
            return str.Length <= number_of_characters ? "" : str.Substring(0, str.Length - number_of_characters);
        }

        public static string Reverse(this string text)
        {
            char[] charArray = text.ToCharArray();
            Array.Reverse((Array)charArray);
            return new string(charArray);
        }

        public static SecureString ToSecureString(this string u, bool makeReadOnly = true)
        {
            if (StringExtension.IsNullOrEmpty(u))
                return (SecureString)null;
            SecureString secureString = new SecureString();
            foreach (char c in u)
                secureString.AppendChar(c);
            if (makeReadOnly)
                secureString.MakeReadOnly();
            return secureString;
        }

        public static string ToUnSecureString(this SecureString s)
        {
            if (s == null)
                return (string)null;
            IntPtr num = IntPtr.Zero;
            string stringUni;
            try
            {
                num = Marshal.SecureStringToGlobalAllocUnicode(s);
                stringUni = Marshal.PtrToStringUni(num);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(num);
            }
            return stringUni;
        }

        public static string ToSBC(this string input)
        {
            char[] charArray = input.ToCharArray();
            for (int index = 0; index < charArray.Length; ++index)
            {
                if (charArray[index] == ' ')
                    charArray[index] = '　';
                else if (charArray[index] < '\u007F')
                    charArray[index] += 'ﻠ';
            }
            return new string(charArray);
        }

        public static string ToDBC(this string input)
        {
            char[] charArray = input.ToCharArray();
            for (int index = 0; index < charArray.Length; ++index)
            {
                if (charArray[index] == '　')
                    charArray[index] = ' ';
                else if (charArray[index] > '\uFF00' && charArray[index] < '｟')
                    charArray[index] -= 'ﻠ';
            }
            return new string(charArray);
        }

        public static string DormantChar(this string str, int begin, int end, char dormatChar = '*')
        {
            if (string.IsNullOrEmpty(str))
                return str;
            if (begin < 0)
                begin = 0;
            if (begin > str.Length || begin > end)
                return str;
            if (end > str.Length)
                end = str.Length;
            char[] charArray = str.ToCharArray();
            for (int index = begin; index < end; ++index)
                charArray[index] = dormatChar;
            return new string(charArray);
        }

        public static string DormantChar(this string str, int retentionCharLength, char dormantChar = '*')
        {
            if (string.IsNullOrEmpty(str))
                return str;
            if (retentionCharLength > str.Length)
                retentionCharLength = str.Length;
            char[] charArray = str.ToCharArray();
            for (int index = 0; index < str.Length - retentionCharLength; ++index)
                charArray[index] = dormantChar;
            return new string(charArray);
        }
    }
}
