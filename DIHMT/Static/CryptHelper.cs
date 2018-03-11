using System;
using System.Text;
using System.Web.Configuration;
using CryptSharp.Utility;

namespace DIHMT.Static
{
    public class CryptHelper
    {
        private static string IpSalt => WebConfigurationManager.AppSettings["IpSalt"];

        public static string Hash(string secret)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secret);
            var saltBytes = Encoding.UTF8.GetBytes(IpSalt);
            const int cost = 262144;
            const int blockSize = 8;
            const int parallel = 1;
            const int derivedKeyLength = 128;

            var bytes = SCrypt.ComputeDerivedKey(keyBytes, saltBytes, cost, blockSize, parallel, null, derivedKeyLength);
            return Convert.ToBase64String(bytes);
        }
    }
}
