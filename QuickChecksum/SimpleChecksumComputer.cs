using System;
using System.IO;
using System.Security.Cryptography;

namespace QuickChecksum
{
    public class SimpleChecksumComputer : IChecksumComputer
    {
        private readonly Func<HashAlgorithm> _hashAlgorithmProvider;

        public int HashSize { get; }

        public SimpleChecksumComputer(Func<HashAlgorithm> hashAlgorithmProvider)
        {
            _hashAlgorithmProvider = hashAlgorithmProvider;

            using var algorithm = hashAlgorithmProvider();
            HashSize = algorithm.HashSize;
        }

        public byte[] ComputeFile(string filePath)
        {
            using var hashAlg = _hashAlgorithmProvider();
            using var stream = File.OpenRead(filePath);
            return hashAlg.ComputeHash(stream);
        }
    }
}
