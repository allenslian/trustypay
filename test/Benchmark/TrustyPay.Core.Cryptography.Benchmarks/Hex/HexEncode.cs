using System;
using System.Linq;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace TrustyPay.Core.Cryptography.Benchmarks
{
    public class HexBenchmark
    {
        private byte[] _testBuffer;

        public HexBenchmark()
        {
            _testBuffer = Encoding.ASCII.GetBytes(
                "hello world!!!");
        }

        // [Benchmark]
        // public string BitConvert()
        // {
        //     return BitConverter.ToString(_testBuffer).Replace("-", "");
        // }

        // [Benchmark]
        // public string FormatString()
        // {
        //     return _testBuffer.Aggregate(
        //         new StringBuilder(_testBuffer.Length * 2),
        //         (acc, value) =>
        //         {
        //             acc.AppendFormat("{0:X2}", value);
        //             return acc;
        //         }).ToString();
        // }

        // [Benchmark]
        // public string ToHexByteString()
        // {
        //     return _testBuffer.ToHexString();
        // }

        [Benchmark]
        public byte[] FromHexString()
        {
            return "68656c6c6f7720776f726c64".FromHexString();
        }

        [Benchmark]
        public byte[] FromHexString2()
        {
            var source = "68656c6c6f7720776f726c64";
            var buffer = new byte[source.Length / 2];
            var index = 0;
            for (var i = 0; i < source.Length; i += 2)
            {
                var a = new string(new char[] { source[i], source[i + 1] });
                buffer[index++] = Convert.ToByte(a, 16);
            }
            return buffer;
        }

        [Benchmark]
        public byte[] FromHexString3()
        {
            var source = "68656c6c6f7720776f726c64";
            var buffer = new byte[source.Length / 2];
            var index = 0;
            for (var i = 0; i < source.Length; i += 2)
            {
                var a = GetHexValue(source[i]) * 16 + GetHexValue(source[i + 1]);
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