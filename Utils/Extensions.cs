using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace VkBotApi.Utils
{
    public static class Extensions
    {
        public static string ConverFromUnicode(this string str)
        {
            return Regex.Replace(str.Replace(@"\u200e", ""), @"\\u([\da-f]{4})", m => ((char)Convert.ToInt32(m.Groups[1].Value, 16)).ToString());
        }
        public static string UrlEncode(this string str)
        {
            return HttpUtility.UrlEncode(str);
        }
        public static long GetTimeStampLong(this DateTime dateTime)
        {
            return (dateTime.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        }
        public static int GetTimeStampInt(this DateTime dateTime)
        {
            return (int)(GetTimeStampLong(dateTime) / 1000);
        }
        public static DateTime GetDateTime(this string timeStamp)
        {
            if (string.IsNullOrWhiteSpace(timeStamp))
            {
                return DateTime.MinValue;
            }
            var num = long.Parse(timeStamp);
            DateTime dtStart = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            if (num > 9466560000)
            {
                TimeSpan toNow = new TimeSpan(num * 10000);
                return dtStart.Add(toNow);
            }
            else
            {
                TimeSpan toNow = new TimeSpan(num * 1000 * 10000);
                return dtStart.Add(toNow);
            }
        }
        public static string EncryptAES(this string plainText, string key, string salt)
        {
            var bsalt = Encoding.ASCII.GetBytes(salt);
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException("invalid plain text");
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("invalid key");
            string outStr;
            RijndaelManaged aesAlg = null;
            try
            {
                var bkey = new Rfc2898DeriveBytes(key, bsalt);
                aesAlg = new RijndaelManaged();
                aesAlg.Key = bkey.GetBytes(aesAlg.KeySize / 8);
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using (var msEncrypt = new MemoryStream())
                {
                    msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }
                    outStr = Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
            finally
            {
                if (aesAlg != null)
                    aesAlg.Clear();
            }
            return outStr;
        }
        public static string DecryptAES(this string cipherText, string key, string salt)
        {
            var bsalt = Encoding.ASCII.GetBytes(salt);
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentNullException("invalid cipher text");
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("invalid key");
            RijndaelManaged aesAlg = null;
            string plaintext;
            try
            {
                var bkey = new Rfc2898DeriveBytes(key, bsalt);
                byte[] bytes = Convert.FromBase64String(cipherText);
                using (var msDecrypt = new MemoryStream(bytes))
                {
                    aesAlg = new RijndaelManaged();
                    aesAlg.Key = bkey.GetBytes(aesAlg.KeySize / 8);
                    aesAlg.IV = ReadByteArray(msDecrypt);
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                            plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
            finally
            {
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            return plaintext;
        }
        private static byte[] ReadByteArray(Stream s)
        {
            var rawLength = new byte[sizeof(int)];
            if (s.Read(rawLength, 0, rawLength.Length) == rawLength.Length)
            {
                var buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
                if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
                {
                    throw new SystemException("Did not read byte array properly");
                }
                return buffer;
            }
            throw new SystemException("Stream did not contain properly formatted byte array");
        }
    }
}
