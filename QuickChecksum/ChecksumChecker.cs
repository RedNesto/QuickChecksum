using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace QuickChecksum
{
    public static class ChecksumChecker
    {
        private static readonly IDictionary<ChecksumAlgorithm, IChecksumComputer> ChecksumComputers = new Dictionary<ChecksumAlgorithm, IChecksumComputer>();

        static ChecksumChecker()
        {
            ChecksumComputers.Add(ChecksumAlgorithm.MD5, new SimpleChecksumComputer(MD5.Create));
            ChecksumComputers.Add(ChecksumAlgorithm.SHA1, new SimpleChecksumComputer(SHA1.Create));
            ChecksumComputers.Add(ChecksumAlgorithm.SHA256, new SimpleChecksumComputer(SHA256.Create));
            ChecksumComputers.Add(ChecksumAlgorithm.SHA512, new SimpleChecksumComputer(SHA512.Create));
        }

        public static (bool hashMatches, byte[]? computedHash) CheckFile(string filePath, string expectedHashString, ChecksumAlgorithm algorithm)
        {
            return CheckFile(filePath, HexConvert.FromString(expectedHashString), algorithm);
        }

        public static (bool hashMatches, byte[]? computedHash) CheckFile(string filePath, byte[] expectedHash, ChecksumAlgorithm algorithm)
        {
            var computedHash = ComputeFileHash(filePath, algorithm);
            return (computedHash.SequenceEqual(expectedHash), computedHash);
        }

        public static byte[]? ComputeFileHash(string filePath, ChecksumAlgorithm algorithm)
        {
            var checksumComputer = GetComputer(algorithm);
            return checksumComputer?.ComputeFile(filePath);
        }

        public static IChecksumComputer? GetComputer(ChecksumAlgorithm algorithm)
        {
            ChecksumComputers.TryGetValue(algorithm, out var computer);
            return computer;
        }

        public static int GetHashSize(ChecksumAlgorithm algorithm)
        {
            var checksumComputer = GetComputer(algorithm);
            if (checksumComputer != null)
                return checksumComputer.HashSize;

            return -1;
        }

        public static int GetHashStringSize(ChecksumAlgorithm algorithm)
        {
            var checksumComputer = GetComputer(algorithm);
            if (checksumComputer != null)
                return checksumComputer.HashSize / 4;

            return -1;
        }
    }
}
