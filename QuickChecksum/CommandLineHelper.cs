using System;
using System.IO;

namespace QuickChecksum
{
    public static class CommandLineHelper
    {
        public static (bool success, string message) ProcessCommand(string[] args)
        {
            if (args.Length < 2) 
                return (false, "Arguments are missing: <algorithm> <file path> [expected hash]");
            
            var algorithmArg = args[0];
            if (!Enum.TryParse(algorithmArg, true, out ChecksumAlgorithm algorithm))
            {
                return (false, $"{algorithmArg} is not a supported algorithm.");
            }

            var fileToCheck = args[1];
            if (!File.Exists(fileToCheck))
            {
                return (false, $"File '{fileToCheck}' does not exist.");
            }

            if (args.Length > 2)
            {
                var expectedHash = args[2];
                var (hashMatches, _) = ChecksumChecker.CheckFile(fileToCheck, expectedHash, algorithm);
                return (hashMatches, hashMatches ? "File hash matches expected hash." : "File hash does not matches expected hash.");
            }

            var computedHash = ChecksumChecker.ComputeFileHash(fileToCheck, algorithm);
            if (computedHash == null)
            {
                return (false, "Could not compute file hash.");
            }

            var hashString = HexConvert.ToString(computedHash);
            return (true, $"Hash of file '{fileToCheck}' is '{hashString}'.");

        }
    }
}
