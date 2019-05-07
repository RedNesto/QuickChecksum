using System;
using System.IO;

namespace QuickChecksum.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ProcessCommandLine(args);
        }

        public static void ProcessCommandLine(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Mode is missing. Available modes: /CONSOLE.");
                return;
            }

            switch (args[0])
            {
                case "/CONSOLE":
                    if (args.Length > 2)
                    {
                        var algorithmArg = args[1];
                        if (!Enum.TryParse(algorithmArg, true, out ChecksumAlgorithm algorithm))
                        {
                            Console.WriteLine("{0} is not a supported algorithm.", algorithmArg);
                            return;
                        }

                        var fileToCheck = args[2];
                        if (!File.Exists(fileToCheck))
                        {
                            Console.WriteLine("File '{0}' does not exist.", fileToCheck);
                            return ;
                        }

                        if (args.Length > 3)
                        {
                            var expectedHash = args[3];
                            var (hashMatches, _) = ChecksumChecker.CheckFile(fileToCheck, expectedHash, algorithm);
                            if (hashMatches)
                            {
                                Console.WriteLine("File hash matches expected hash.");
                            }
                            else
                            {
                                Console.WriteLine("File hash does not matches expected hash.");
                            }
                        }
                        else
                        {
                            var computedHash = ChecksumChecker.ComputeFileHash(fileToCheck, algorithm);
                            if (computedHash == null)
                            {
                                Console.WriteLine("Could compute file hash.");
                            }
                            else
                            {
                                var hashString = HexConvert.ToString(computedHash);
                                Console.WriteLine("Hash of file '{0}' is '{1}'.", fileToCheck, hashString);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Arguments are missing: <algorithm> <file path> [expected hash]");
                    }

                    return;
                default:
                    Console.WriteLine("Unrecognized mode.");
                    return;
            }
        }
    }
}
