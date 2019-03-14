using BenchmarkDotNet.Attributes;
using Taumuon.GedcomParserSpan.Parser;

namespace Taumuon.GedcomParser.Benchmark
{
    [MemoryDiagnoser]
    public class BenchmarkGedcomParse
    {
        public BenchmarkGedcomParse()
        {
        }

        [Params(false, true)]
        public bool File { get; set; }

        [Benchmark]
        public int FileParserSpan()
        {
            int itemCount = 0;

            Taumuon.GedcomParserSpan.GedcomStreamingParser gedcomStreamingParserSpan = new Taumuon.GedcomParserSpan.GedcomStreamingParser(
                null,
                individual => itemCount++,
                family => itemCount++,
                image => itemCount++,
                note => itemCount++);
            using (var fs = BenchmarkHelper.GetStreamReader(File))
            {
                gedcomStreamingParserSpan.Parse(new LineProvider(fs));
            }

            return itemCount;
        }

        [Benchmark(Baseline = true)]
        public int FileParser()
        {
            int itemCount = 0;

            Taumuon.GedcomParser.GedcomStreamingParser gedcomStreamingParser = new Taumuon.GedcomParser.GedcomStreamingParser(
                null,
                individual => itemCount++,
                family => itemCount++,
                image => itemCount++,
                note => itemCount++);
            using (var fs = BenchmarkHelper.GetStreamReader(File))
            {
                gedcomStreamingParser.Parse(fs.BaseStream);
            }

            return itemCount;
        }
    }
}
