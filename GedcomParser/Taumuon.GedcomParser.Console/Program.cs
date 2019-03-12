using System.IO;
using System.Linq;

namespace Taumuon.GedcomParser.Console
{
    class Program
    {
        private const string File = @"D:\git\taumuon\programming\FamilyTree\royal92ModLarge.ged";

        public static StreamReader GetStream()
        {
            var fs = new StreamReader(File);
            return fs;
        }

        public static Taumuon.GedcomParserSpan.GedcomInfo FileImportSpan()
        {
            System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
            Taumuon.GedcomParserSpan.GedcomImporter import = new Taumuon.GedcomParserSpan.GedcomImporter();
            using (var fs = GetStream())
            {
                var result = import.Import(new Taumuon.GedcomParserSpan.Parser.LineProvider(fs));
                watch.Stop();
                System.Console.WriteLine($"Imported in {watch.ElapsedMilliseconds} ms");
                return result;
            }
        }

        public static Taumuon.GedcomParser.GedcomInfo FileImport()
        {
            System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
            Taumuon.GedcomParser.GedcomImporter import = new Taumuon.GedcomParser.GedcomImporter();
            using (var fs = GetStream())
            {
                var result = import.Import(fs.BaseStream);
                watch.Stop();
                System.Console.WriteLine($"Imported norm in {watch.ElapsedMilliseconds} ms");
                return result;
            }
        }


        static void Main(string[] args)
        {
            System.Console.OutputEncoding = System.Text.Encoding.UTF8;

            var spanResult = FileImportSpan();
            var nonSpanResult = FileImport();

            System.Console.WriteLine("Individual count span:{0} non-span:{1}", spanResult.Individuals.Count, nonSpanResult.Individuals.Count);
            System.Console.WriteLine("Family count span:{0} non-span:{1}", spanResult.Families.Count, nonSpanResult.Families.Count);

            var spanNameCount = spanResult.Individuals.GroupBy(x => x.LastName)
                .ToDictionary(x => x.Key, x => x.Count());
            var nonSpanNameCount = nonSpanResult.Individuals.GroupBy(x => x.LastName)
                .ToDictionary(x => x.Key, x => x.Count());

            foreach(var individual in spanNameCount)
            {
                if (nonSpanNameCount[individual.Key] != individual.Value)
                {
                    System.Console.WriteLine($"Mismatch for {individual.Key} {individual.Value} {nonSpanNameCount.Values}");
                }
            }

            foreach (var individual in spanNameCount.OrderByDescending(x => x.Value).Take(20))
            {
                System.Console.WriteLine($"{individual.Key} {individual.Value}");
            }
        }
    }
}
