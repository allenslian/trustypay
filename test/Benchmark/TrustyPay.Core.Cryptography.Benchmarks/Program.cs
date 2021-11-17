using System.Reflection;
using BenchmarkDotNet.Running;
using TrustyPay.Core.Cryptography.Benchmarks.Hex;

namespace TrustyPay.Core.Cryptography.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run(Assembly.GetExecutingAssembly());
        }
    }
}
