using System;
using BenchmarkDotNet.Attributes;

namespace TrustyPay.Core.Cryptography.Benchmarks.Hex
{
    public class Decode
    {
        private readonly string _hexString;

        public Decode()
        {
            _hexString = "68656c6c6f7720776f726c64";
        }

        [Benchmark]
        public byte[] FromHexString()
        {
            return _hexString.FromHexString();
        }

        // [Benchmark]
        public byte[] HexStringToByte()
        {
            var buffer = new byte[_hexString.Length / 2];
            var index = 0;
            for (var i = 0; i < _hexString.Length; i += 2)
            {
                var a = new string(new char[] { _hexString[i], _hexString[i + 1] });
                buffer[index++] = Convert.ToByte(a, 16);
            }
            return buffer;
        }

        [Benchmark]
        public byte[] HexString()
        {
            var buffer = new byte[_hexString.Length / 2];
            var index = 0;
            for (var i = 0; i < _hexString.Length; i += 2)
            {
                var a = GetHexValue(_hexString[i]) * 16 + GetHexValue(_hexString[i + 1]);
                buffer[index++] = (byte)a;
            }
            return buffer;
        }

        private int GetHexValue(char c)
        {
            if (c >= 'a')
            {
                return (c - 'a') + 10;
            }
            else if (c >= 'A')
            {
                return (c - 'A') + 10;
            }
            else
            {
                return c - '0';
            }
        }
    }
}