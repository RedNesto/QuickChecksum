using System;
using System.Linq;
using System.Text;

namespace QuickChecksum
{
    public static class HexConvert
    {
        public static string ToString(byte[] hex)
        {
            var stringBuilder = new StringBuilder(hex.Length * 2);
            foreach (var computedByte in hex)
            {
                stringBuilder.Append(computedByte.ToString("x2"));
            }

            return stringBuilder.ToString();
        }

        public static byte[] FromString(string hex)
        {
            try
            {
                return Enumerable.Range(0, hex.Length)
                    .Where(x => x % 2 == 0)
                    .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                    .ToArray();
            }
            catch (FormatException e)
            {
                throw new ArgumentException("Given string is not valid hexadecimal", nameof(hex), e);
            }
        }
    }
}
