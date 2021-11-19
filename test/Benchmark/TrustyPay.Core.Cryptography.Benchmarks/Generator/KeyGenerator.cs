

using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;

namespace TrustyPay.Core.Cryptography.Benchmarks.Generator
{
    public class KeyGenerator
    {
        private const string AlphabetAndNumbers = "AaBbC5cDdEe0FfGgH6hIiJj1KkLlM7mNnOo2PpQqR8rSsTt3UuVvW9wXxYy4Zz";
        private const string Numbers = "0123456789";

        [Benchmark]
        public string GenerateRandomText()
        {
            // return SecretKeyGenerator.GenerateRandomAlphabetAndNumbers(16);
            
            var buffer = new byte[16];
            RandomNumberGenerator.Fill(buffer);
            var numbers = new char[16];
            for (var i = 0; i < buffer.Length; i++)
            {
                var m = buffer[i] % 62; // 62 means 0-9 + a-z + A-Z
                if (m >= 36)
                {
                    numbers[i] = (char)(m - 36 + 'a');
                }
                else if (m >= 10)
                {
                    numbers[i] = (char)(m - 10 + 'A');
                }
                else
                {
                    numbers[i] = (char)(m + '0');
                }
            }
            return new string(numbers);
        }

        [Benchmark]
        public string GenerateRandomTextFromDict()
        {
            var buffer = new byte[16];
            RandomNumberGenerator.Fill(buffer);
            var text = new System.Text.StringBuilder(16);
            var length = AlphabetAndNumbers.Length;
            foreach (var b in buffer)
            {
                var position = b % length;
                text.Append(AlphabetAndNumbers[position]);
            }
            return text.ToString();
        }

        [Benchmark]
        public string GenerateRandomNumbers()
        {
            var buffer = new byte[16];
            RandomNumberGenerator.Fill(buffer);
            var numbers = new char[16];
            for (var i = 0; i < buffer.Length; i++)
            {
                numbers[i] = (char)(buffer[i] % 10 + '0'); // 10 means 0-9 chars.
            }
            return new string(numbers);
        }

        [Benchmark]
        public string GenerateRandomAlphabets()
        {
            var buffer = new byte[16];
            RandomNumberGenerator.Fill(buffer);
            var chars = new char[16];
            for (var i = 0; i < buffer.Length; i++)
            {
                var m = buffer[i] % 52;
                chars[i] = m >= 26 ? (char)(m - 26 + 'a') : (char)(m + 'A'); // 52 means a-z + A-Z
            }
            return new string(chars);
        }
    }
}