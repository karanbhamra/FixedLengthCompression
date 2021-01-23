using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FixedLengthCompression
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputFile = "data.txt";
            var input = File.ReadAllText(inputFile);
            var mapping = new Dictionary<char, string>()
            {
                ['A'] = "00",
                ['C'] = "01",
                ['G'] = "10",
                ['T'] = "11"
            };

            Console.WriteLine("Writing...");
            Console.WriteLine($"Input: {input}");
            var binary = LettersToBinary(input, mapping);
            Console.WriteLine($"Binary: {binary}");
            var bytes = BinaryToBytes(binary);

            string outputFile = "output.txt";
            File.WriteAllBytes(outputFile, bytes.ToArray());

            Console.WriteLine("Reading...");
            var readfile = File.ReadAllBytes(outputFile).ToList();
            var readBinary = BytesToBinary(readfile);
            Console.WriteLine($"Binary: {readBinary}");
            var reverseMapping = mapping.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
            var readOutput = BinaryToLetters(readBinary, reverseMapping);
            Console.WriteLine($"Output: {readOutput}");
        }

        private static object BinaryToLetters(string readBinary, Dictionary<string, char> reverseMapping)
        {
            StringBuilder sb = new();

            for (int i = 0; i < readBinary.Length; i += 2)
            {
                string bin = readBinary.Substring(i, 2);
                sb.Append(reverseMapping[bin]);
            }

            return sb.ToString();
        }

        private static string BytesToBinary(List<byte> bytes)
        {
            if (bytes == null) throw new ArgumentNullException(nameof(bytes));
            if (bytes.Count == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(bytes));
            StringBuilder sb = new();

            const int bits = 8;
            var lastByte = bytes[^1];

            for (int i = 0; i < bytes.Count - 1; i++)
            {
                StringBuilder bin = new(Convert.ToString(bytes[i], 2));

                // expand each bit pattern for byte to 8 bits, otherwise if its second to last then to the size specified
                // by the last byte
                int size = i < bytes.Count - 2 ? bits : lastByte;
                while (bin.Length != size)
                {
                    bin.Insert(0, '0');
                }

                sb.Append(bin);
            }

            return sb.ToString();
        }

        private static List<byte> BinaryToBytes(string binary)
        {
            if (binary == null) throw new ArgumentNullException(nameof(binary));
            if (binary.Length == 0) throw new ArgumentException("Binary cannot be an empty string.", nameof(binary));

            const int bits = 8;
            List<byte> bytes = new();
            int excessLength = binary.Length % bits;

            for (int i = 0; i < binary.Length - excessLength; i += bits)
            {
                string chunk = binary.Substring(i, bits);
                byte newByte = Convert.ToByte(chunk, 2);
                bytes.Add(newByte);
            }

            // if leftover bytes
            if (excessLength != 0)
            {
                bytes.Add(Convert.ToByte(binary.Substring(binary.Length - excessLength), 2));
            }

            // last byte specifies how many bits long the second to last byte was
            int lastByte = excessLength == 0 ? bits : excessLength;
            bytes.Add(Convert.ToByte(lastByte));

            return bytes;
        }

        private static string LettersToBinary(string input, Dictionary<char, string> mapping)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (input.Length == 0) throw new ArgumentException("Input cannot be an empty string.", nameof(input));

            if (mapping == null) throw new ArgumentNullException(nameof(mapping));
            if (mapping.Count == 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(mapping));

            StringBuilder sb = new();

            foreach (var letter in input)
            {
                sb.Append(mapping[letter]);
            }

            return sb.ToString();
        }
    }
}