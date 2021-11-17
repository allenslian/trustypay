using System;
using System.Linq;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace TrustyPay.Core.Cryptography.Benchmarks.Hex
{
    public class Encode
    {
        private byte[] _buffer;

        public Encode()
        {
            _buffer = Encoding.ASCII.GetBytes("hello world!!!");
        }

        //[Benchmark]
        public string BitConvert()
        {
            return BitConverter.ToString(_buffer).Replace("-", "");
        }

        //[Benchmark]
        public string FormatString()
        {
            return _buffer.Aggregate(
                new StringBuilder(_buffer.Length * 2),
                (acc, value) =>
                {
                    acc.AppendFormat("{0:X2}", value);
                    return acc;
                }).ToString();
        }

        //[Benchmark]
        public string ToHexByteString()
        {
            return _buffer.ToHexString();
        }
    }
}