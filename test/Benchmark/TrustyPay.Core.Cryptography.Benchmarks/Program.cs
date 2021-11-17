using BenchmarkDotNet.Running;

namespace TrustyPay.Core.Cryptography.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<HexBenchmark>();
        }
    }
}
