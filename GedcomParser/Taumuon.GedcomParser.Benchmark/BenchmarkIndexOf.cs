using BenchmarkDotNet.Attributes;
using System;

namespace Taumuon.GedcomParser.Benchmark
{
    public class BenchmarkIndexOf
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
