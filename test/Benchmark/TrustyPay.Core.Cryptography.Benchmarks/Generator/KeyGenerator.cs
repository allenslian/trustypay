

using System.Security.Cryptography;
using BenchmarkDotNet.Attributes;

namespace TrustyPay.Core.Cryptography.Benchmarks.Generator
{
    public class KeyGenerator
    {
        private const string AlphabetAndNumbers = "AaBbC5cDdEe0FfGgH6hIiJj1KkLlM7mNnOo2PpQqR8rSsTt3UuVvW9wXxYy4Zz";
        private const string Numbers = "0123456789";

        private const string Charsets = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz0123456789";

        private static char[] CharsetArray = new char[]{
            'A','B','C','D','E','F','G','H','J','K','L','M','N','P','Q','R','S','T','U','V','W','X','Y','Z',
            'a','b','c','d','e','f','g','h','j','k','l','m','n','p','q','r','s','t','u','v','w','x','y','z',
            '0','1','2','3','4','5','6','7','8','9'
        };

        private const int bufferSize = 32;

        //[Benchmark]
        public string GenerateRandomText1()
        {
            // return SecretKeyGenerator.GenerateRandomAlphabetAndNumbers(16);
            // bad
            var buffer = new byte[bufferSize];
            RandomNumberGenerator.Fill(buffer);
            var numbers = new char[buffer.Length];
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
        public string GenerateRandomText2()
        {
            var buffer = new byte[bufferSize];
            RandomNumberGenerator.Fill(buffer);
            var charsets = new char[buffer.Length];
            for (var i = 0; i < buffer.Length; i++)
            {
                var m = buffer[i] % Charsets.Length;
                charsets[i] = Charsets[m];
            }
            return new string(charsets);
        }

        [Benchmark]
        public string GenerateRandomTextFromDict1()
        {
            var buffer = new byte[bufferSize];
            RandomNumberGenerator.Fill(buffer);
            var text = new System.Text.StringBuilder(buffer.Length);
            var length = Charsets.Length;
            foreach (var b in buffer)
            {
                var position = b % length;
                text.Append(Charsets[position]);
            }
            return text.ToString();
        }

        [Benchmark]
        public string GenerateRandomTextFromDict2()
        {
            var buffer = new byte[bufferSize];
            RandomNumberGenerator.Fill(buffer);
            var text = new char[buffer.Length];
            var length = Charsets.Length;
            for (var i = 0; i < buffer.Length; i++)
            {
                var position = buffer[i] % length;
                text[i] = Charsets[position];
            }
            return new string(text);
        }

        [Benchmark]
        public string GenerateRandomTextFromDict3()
        {
            var buffer = new byte[bufferSize];
            RandomNumberGenerator.Fill(buffer);
            var text = new char[buffer.Length];
            var length = CharsetArray.Length;
            for (var i = 0; i < buffer.Length; i++)
            {
                var position = buffer[i] % length;
                text[i] = CharsetArray[position];
            }
            return new string(text);
        }

        [Benchmark]
        public string GenerateRandomNumbers1()
        {
            var buffer = new byte[bufferSize];
            RandomNumberGenerator.Fill(buffer);
            var numbers = new char[buffer.Length];
            for (var i = 0; i < buffer.Length; i++)
            {
                numbers[i] = (char)(buffer[i] % 10 + '0'); // 10 means 0-9 chars.
            }
            return new string(numbers);
        }

        [Benchmark]
        public string GenerateRandomNumbers2()
        {
            var buffer = new byte[bufferSize];
            RandomNumberGenerator.Fill(buffer);
            var numbers = new char[buffer.Length];
            for (var i = 0; i < buffer.Length; i++)
            {
                var m = buffer[i] % 10;
                numbers[i] = Charsets[24 + 24 + m];
            }
            return new string(numbers);
        }

        //[Benchmark]
        public string GenerateRandomAlphabets1()
        {
            // bad
            var buffer = new byte[bufferSize];
            RandomNumberGenerator.Fill(buffer);
            var chars = new char[buffer.Length];
            for (var i = 0; i < buffer.Length; i++)
            {
                var m = buffer[i] % 52;
                chars[i] = m >= 26 ? (char)(m - 26 + 'a') : (char)(m + 'A'); // 52 means a-z + A-Z
            }
            return new string(chars);
        }

        [Benchmark]
        public string GenerateRandomAlphabets2()
        {
            var buffer = new byte[bufferSize];
            RandomNumberGenerator.Fill(buffer);
            var chars = new char[buffer.Length];
            for (var i = 0; i < buffer.Length; i++)
            {
                var m = buffer[i] % 48;
                chars[i] = Charsets[m];
            }
            return new string(chars);
        }
    }
}