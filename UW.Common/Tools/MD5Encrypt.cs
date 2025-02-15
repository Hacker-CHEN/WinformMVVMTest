using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace UW.Common
{
    public class MD5Encrypt
    {
        public static string DoEncrypt(string pwd)
        {
            MD5 md5 = (MD5)new MD5CryptoServiceProvider();
            byte[] bytes = Encoding.Default.GetBytes(pwd);
            byte[] hash = md5.ComputeHash(bytes);
            md5.Clear();
            string str = "";
            for (int index = 0; index < hash.Length; ++index)
                str += hash[index].ToString("X").PadLeft(2, '0');
            return str;
        }
    }
}
