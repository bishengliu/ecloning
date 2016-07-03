using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace ecloning.Models
{
    public class genCode
    {
        public string HashStringCode(int byteNum)
        {
            //generate the invitation code in the group
            byte[] bytes = new byte[byteNum];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            HashAlgorithm algorithm = SHA256.Create();
            var Hash = algorithm.ComputeHash(bytes);

            StringBuilder sb = new StringBuilder();
            foreach (byte b in Hash) { sb.Append(b.ToString("X2")); }
            var code = sb.ToString();
            return code;
        }

        public byte[] HashByteCode(int byteNum)
        {
            //generate the invitation code in the group
            byte[] bytes = new byte[byteNum];
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            HashAlgorithm algorithm = SHA256.Create();
            var Hash = algorithm.ComputeHash(bytes);            
            return Hash;
        }
    }
}