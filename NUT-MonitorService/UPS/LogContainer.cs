using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace NUTMonitor.UPS
{
    public class LogContainer
    {
        public HashBlock Hashes { get; private set; } = new HashBlock();

        private string _logs;

        public string Logs
        {
            get { return _logs; }
            set
            {
                _logs = value;
                Hashes = new HashBlock();
            }
        }


        public HashBlock GetHashes()
        {
            HashBlock result = new HashBlock();

            byte[] payload = Encoding.UTF8.GetBytes(Logs);


            SHA1 sha1 = SHA1.Create();
            SHA256 sha256 = SHA256.Create();
            SHA384 sha384 = SHA384.Create();
            SHA512 sha512 = SHA512.Create();
            MD5 md5 = MD5.Create();

            byte[] hash1 = sha512.ComputeHash(payload);

            List<byte> hash2Payload = new List<byte>();
            hash2Payload.AddRange(sha256.ComputeHash(payload));
            hash2Payload.AddRange(sha1.ComputeHash(payload));
            hash2Payload.AddRange(md5.ComputeHash(payload));

            byte[] hash2PayloadAsArray = hash2Payload.ToArray();

            List<byte> hash2 = new List<byte>();
            hash2.AddRange(sha384.ComputeHash(hash2PayloadAsArray));
            hash2.AddRange(sha1.ComputeHash(hash2PayloadAsArray));


            result.Hash1 = string.Concat(hash1.Select(b => b.ToString("X2")));
            result.Hash2 = string.Concat(hash2.Select(b => b.ToString("X2")));
            result.Hash3 = string.Empty;

            return result;
        }

        public void RefreshHashes()
        {
            if (!(string.IsNullOrEmpty(Hashes.Hash1)
                && string.IsNullOrEmpty(Hashes.Hash2)
                && string.IsNullOrEmpty(Hashes.Hash3)))
                throw new InvalidOperationException("Hash refresh can be done only for fresh content");

            Hashes = GetHashes();
        }

        public bool CheckValid()
        {
            HashBlock ContentHashes = GetHashes();

            bool result = (Hashes.Hash1 == ContentHashes.Hash1) || (string.IsNullOrEmpty(Hashes.Hash1) && string.IsNullOrEmpty(ContentHashes.Hash1));

            result = result && ((Hashes.Hash2 == ContentHashes.Hash2) || (string.IsNullOrEmpty(Hashes.Hash2) && string.IsNullOrEmpty(ContentHashes.Hash2)));
            result = result && ((Hashes.Hash3 == ContentHashes.Hash3) || (string.IsNullOrEmpty(Hashes.Hash3) && string.IsNullOrEmpty(ContentHashes.Hash3)));

            return result;
        }
    }
}
