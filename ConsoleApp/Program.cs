using System;

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
                    var subArray = new string[args.Length - 1];
                    Array.Copy(args, 1, subArray, 0, subArray.Length);
                    Console.WriteLine(CommandLineHelper.ProcessCommand(subArray).message);
                    break;
                default:
                    Console.WriteLine("Unrecognized mode.");
                    break;
            }
        }
    }
}
