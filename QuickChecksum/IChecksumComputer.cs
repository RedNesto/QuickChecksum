namespace QuickChecksum
{
    public interface IChecksumComputer
    {
        int HashSize { get; }

        byte[] ComputeFile(string filePath);
    }
}
