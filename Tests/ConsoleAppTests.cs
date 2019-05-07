using System;
using System.IO;
using NUnit.Framework;
using QuickChecksum.ConsoleApp;

namespace QuickChecksum.Tests
{
    [TestFixture]
    public class ConsoleAppTests
    {
        private static readonly string TestDataDir = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData");
        private static readonly string TestFile = Path.Combine(TestDataDir, "TestFile1.txt");

        private static void AssertConsoleOutput(string expectedConsoleOutput, Action action)
        {
            using var consoleOut = new StringWriter();
            Console.SetOut(consoleOut);

            action();

            Assert.AreEqual(expectedConsoleOutput, consoleOut.ToString());
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Directory.CreateDirectory(TestDataDir);
            File.WriteAllText(TestFile, "Text file used for tests");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            File.Delete(TestFile);
            Directory.Delete(TestDataDir);
        }

        [SetUp]
        public void SetUp()
        {
            var stdOut = new StreamWriter(Console.OpenStandardOutput())
            {
                AutoFlush = true
            };

            Console.SetOut(stdOut);
        }

        [Test]
        public void TestNoArgs()
        {
            AssertConsoleOutput("Mode is missing. Available modes: /CONSOLE." + Environment.NewLine,
                () => Program.ProcessCommandLine(new string[0]));
        }

        [Test]
        public void TestConsoleWithoutArgs()
        {
            AssertConsoleOutput("Arguments are missing: <algorithm> <file path> [expected hash]" + Environment.NewLine,
                () => Program.ProcessCommandLine(new[] {"/CONSOLE"}));
        }

        [Test]
        public void TestFileChecksumComputation()
        {
            AssertConsoleOutput("Hash of file '" + TestFile + "' is 'bfa378f54f80bf63165366acda8dbd19'." + Environment.NewLine,
                () => Program.ProcessCommandLine(new[] {"/CONSOLE", "MD5", TestFile}));
        }

        [Test]
        public void TestNonExistentFileChecksumComputation()
        {
            var testFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestData", "File_That_Does_Not_Exist.txt");
            AssertConsoleOutput("File '" + testFilePath + "' does not exist." + Environment.NewLine,
                () => Program.ProcessCommandLine(new[] {"/CONSOLE", "MD5", testFilePath}));
        }

        [Test]
        public void TestInvalidAlgorithm()
        {
            const string invalidAlgorithm = "NOT_MD5";
            AssertConsoleOutput(invalidAlgorithm + " is not a supported algorithm." + Environment.NewLine,
                () => Program.ProcessCommandLine(new[] {"/CONSOLE", invalidAlgorithm, TestFile}));
        }

        [Test]
        public void TestFileCheckMatches()
        {
            AssertConsoleOutput("File hash matches expected hash." + Environment.NewLine,
                () => Program.ProcessCommandLine(new[] {"/CONSOLE", "MD5", TestFile, "bfa378f54f80bf63165366acda8dbd19"}));
        }

        [Test]
        public void TestFileCheckNotMatches()
        {
            AssertConsoleOutput("File hash does not matches expected hash." + Environment.NewLine,
                () => Program.ProcessCommandLine(new[] {"/CONSOLE", "MD5", TestFile, "0fa378f54f80bf63165366acda8dbd19"}));
        }
    }
}
