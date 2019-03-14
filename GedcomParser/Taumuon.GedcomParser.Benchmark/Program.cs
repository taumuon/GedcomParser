using BenchmarkDotNet.Running;

namespace Taumuon.GedcomParser.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var parserSummary = BenchmarkRunner.Run<BenchmarkGedcomParse>();
            // var parserSummary = BenchmarkRunner.Run<BenchmarkIndexOf>();
        }
    }
}
