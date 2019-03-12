using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.IO;
using System.Text;
using Taumuon.GedcomParserSpan.Parser;

namespace Taumuon.GedcomParser.Benchmark
{
    public static class BenchmarkHelper
    {
        private const string File = @"D:\git\taumuon\programming\FamilyTree\royal92Mod.ged";

        public static StreamReader GetStreamReader(bool file)
        {
            if (file)
            {
                var fs = new StreamReader(File);
                return fs;
            }

            var stream = new MemoryStream();
            SetStreamBytesFromString(stream, gedcom);
            return new StreamReader(stream);
        }

        private static void SetStreamBytesFromString(Stream stream, string input)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            stream.Write(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
        }

        private const string gedcom =
@"0 HEAD
1 SOUR GRUExport
2 VERS 2.1.41
2 NAME Genes Reunited
2 CORP Genes Reunited Ltd
1 DATE 2 Jun 2010
2 TIME 21:02:28
1 FILE C:\DOCUME~1\ADMINI~1\LOCALS~1\Temp\Evans family tree.ged
1 GEDC
2 VERS 5.5
2 FORM LINEAGE-LINKED
1 CHAR ANSI
0 @I169@ INDI
1 NAME William Henry /DAY/
1 SEX M
1 BIRT 
2 DATE 28th Decem
1 DEAT 
2 DATE 1978
1 RFN 273414810
1 FAMS @F65@
1 FAMS @F295@
0 @I303@ INDI
1 NAME James  /Shearsby/
1 SEX M
1 BIRT 
2 DATE ABT 1846
2 PLAC Warwickshire
1 DEAT 
2 DATE 1870
1 RFN 273414833
1 FAMS @F104@
1 FAMC @36@
0 @I380@ INDI
1 NAME Henry  /Adkins/
1 SEX M
1 BIRT 
2 DATE ABT 1827
2 PLAC Stoneleigh, Warwickshire
1 DEAT 
2 DATE 1899
1 BAPM 
2 DATE 11.11.1827
1 NOTE Living at 78, Cooksey Rd, Aston    Occupation  Bricklayer
2 CONT Buried at Yardliegh Cemetery , Birmingham
1 SOUR David Adkins
1 RFN 273414851
1 FAMS @F128@
1 FAMC @69@
0 @F131@ FAM
1 HUSB @I386@
1 WIFE @I392@
0 @38@ FAM
1 HUSB @460603221@
1 WIFE @460603220@
1 CHIL @460602798@
1 MARR 
2 DATE 26 OCT 1786
2 PLAC Barford, Warwickshire, England
0 @36@ FAM
1 HUSB @460629014@
1 WIFE @460629012@
1 CHIL @I303@
1 CHIL @462044838@
1 CHIL @462044855@
0 TRLR
";
    }

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

            Taumuon.GedcomParserSpan.GedcomStreamingParser gedcomStreamingParser = new Taumuon.GedcomParserSpan.GedcomStreamingParser(
                null,
                individual => itemCount++,
                family => itemCount++,
                image => itemCount++,
                note => itemCount++);
            using (var fs = BenchmarkHelper.GetStreamReader(File))
            {
                gedcomStreamingParser.Parse(new LineProvider(fs));
            }

            return itemCount;
        }

        [Benchmark(Baseline = true)]
        public int FileParser()
        {
            int itemCount = 0;

            GedcomStreamingParser gedcomStreamingParser = new GedcomStreamingParser(
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

    class Program
    {
        static void Main(string[] args)
        {
            var parserSummary = BenchmarkRunner.Run<BenchmarkGedcomParse>();
            // var parserSummary = BenchmarkRunner.Run<BenchmarkGedcomParse2>();
        }
    }

    public class BenchmarkGedcomParse2
    {
        private const string SmallString = "NAME JAMES BROWN";
        private const string MedString = "SOMELONGISHTEXTPRIORTOASPACE TEXTAFTERTHESPACE";
        private const string LongerString = "SOMELONGERTEXTPRIORTOASPACESOMELONGERTEXTPRIORTOASPACESOMELONGERTEXTPRIORTOASPACESOMELONGERTEXTPRIORTOASPACESOMELONGERTEXTPRIORTOASPACESOMELONGERTEXTPRIORTOASPACESOMELONGERTEXTPRIORTOASPACESOMELONGERTEXTPRIORTOASPACESOMELONGERTEXTPRIORTOASPACESOMELONGERTEXTPRIORTOASPACESOMELONGERTEXTPRIORTOASPACESOMELONGERTEXTPRIORTOASPACESOMELONGERTEXTPRIORTOASPACESOMELONGERTEXTPRIORTOASPACESOMELONGERTEXTPRIORTOASPACESOMELONGERTEXTPRIORTOASPACESOMELONGERTEXTPRIORTOASPACESOMELONGERTEXTPRIORTOASPACESOMELONGERTEXTPRIORTOASPACE TEXTAFTERTHESPACE";

        [Benchmark]
        public int IndexOfStrSmall()
        {
            // NOTE: artificially take the span and return its length just because
            //  want to ensure that the only difference between the two benchmarks
            //  is using IndexOf on the string, or the span
            var span = SmallString.AsSpan();
            var indexOf = SmallString.IndexOf(' ');
            return span.Length + indexOf;
        }

        [Benchmark]
        public int IndexOfSpanSmall()
        {
            var span = SmallString.AsSpan();
            var indexOf = span.IndexOf(' ');
            return span.Length + indexOf;
        }

        [Benchmark]
        public int IndexOfStrMed()
        {
            // NOTE: artificially take the span and return its length just because
            //  want to ensure that the only difference between the two benchmarks
            //  is using IndexOf on the string, or the span
            var span = MedString.AsSpan();
            var indexOf = MedString.IndexOf(' ');
            return span.Length + indexOf;
        }

        [Benchmark]
        public int IndexOfSpanMed()
        {
            var span = MedString.AsSpan();
            var indexOf = span.IndexOf(' ');
            return span.Length + indexOf;
        }

        [Benchmark]
        public int IndexOfStrLarge()
        {
            // NOTE: artificially take the span and return its length just because
            //  want to ensure that the only difference between the two benchmarks
            //  is using IndexOf on the string, or the span
            var span = LongerString.AsSpan();
            var indexOf = LongerString.IndexOf(' ');
            return span.Length + indexOf;
        }

        [Benchmark]
        public int IndexOfSpanLarge()
        {
            var span = LongerString.AsSpan();
            var indexOf = span.IndexOf(' ');
            return span.Length + indexOf;
        }
    }
}
