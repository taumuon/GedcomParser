using System;

namespace Taumuon.GedcomParser.Parser
{
    public static class ParserHelper
    {
        // Parses a string of format @I2@, @F3@ or simply @3@
        public static string ParseID(string idString)
        {
            int index = idString.IndexOf('@', 1);
            return idString.Substring(1, index - 1);
        }

        public static GedcomLine ParseLine(string line)
        {
            var trimmedLine = line.Trim();
            var indexOfFirstSpace = trimmedLine.IndexOf(' ');

            if (indexOfFirstSpace == -1)
            {
                throw new InvalidOperationException("Invalid GEDCOM line");
            }

            var levelString = trimmedLine.Substring(0, indexOfFirstSpace);

            int level;
            if (!int.TryParse(levelString, out level))
            {
                throw new InvalidOperationException("GEDCOM Level number missing");
            }

            return new GedcomLine(level, trimmedLine);
        }
    }
}