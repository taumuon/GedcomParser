using System;

namespace Taumuon.GedcomParserSpan.Parser
{
    public static class ParserHelper
    {
        // Parses a string of format @I2@, @F3@ or simply @3@
        public static ReadOnlySpan<char> ParseID(ReadOnlySpan<char> idString)
        {
            return idString.Slice(1, idString.Length - 2);
        }

        public static ReadOnlySpan<char> GetLineContent(ReadOnlySpan<char> line)
        {
            int indexOfSpace = line.IndexOf(' ');
            if (indexOfSpace == -1)
            {
                return new ReadOnlySpan<char>();
            }

            return line.Slice(indexOfSpace + 1, line.Length - indexOfSpace - 1);
        }

        public static GedcomLine ParseLine(ReadOnlySpan<char> line)
        {
            var trimmedLine = line.Trim();
            int indexOfSpace = trimmedLine.IndexOf(' ');
            if (indexOfSpace == -1)
            {
                throw new InvalidOperationException("GEDCOM Level number missing");
            }

            var levelContent = trimmedLine.Slice(0, indexOfSpace);
            int level;
            if (!int.TryParse(levelContent, out level))
            {
                throw new InvalidOperationException("GEDCOM Level number missing");
            }

            ReadOnlySpan<char> lineContent = trimmedLine.Slice(indexOfSpace + 1, trimmedLine.Length - indexOfSpace - 1);

            return new GedcomLine(level, lineContent);
        }

        public static bool Equals(ReadOnlySpan<char> span, string str)
        {
            return span.Equals(str.AsSpan(), StringComparison.Ordinal);
        }
    }
}