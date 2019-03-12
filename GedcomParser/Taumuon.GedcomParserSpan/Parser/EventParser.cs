using System;

namespace Taumuon.GedcomParserSpan.Parser
{
    public class EventParser
    {
        public static ParseResult<GedcomEvent> Parse(GedcomLine first, ILineProvider lineProvider)
        {
            GedcomEvent gedcomEvent = new GedcomEvent();

            var initialLevel = first.Level;

            GedcomLine line = default;
            ReadOnlySpan<char> currentRawLine;
            while ((currentRawLine = lineProvider.ReadLine()).Length > 0)
            {
                line = ParserHelper.ParseLine(currentRawLine);

                if (line.Level <= first.Level)
                {
                    break;
                }

                ReadOnlySpan<char> tag = line.GetFirstItem();
                if (ParserHelper.Equals(tag, "DATE"))
                {
                    // If checks we're parsing actual date and not
                    // CREA or CHAN tags
                    // TODO: should actually put CREA and CHAN into different parser
                    if (line.Level == initialLevel + 1)
                    {
                        var dateContent = line.GetLineContent(4);
                        gedcomEvent.Date = dateContent.Length == 0 ? null : dateContent.ToString();
                    }
                }
                else if (ParserHelper.Equals(tag, "PLAC"))
                {
                    // If checks we're parsing actual date and not
                    // CREA or CHAN tags
                    // TODO: should actually put CREA and CHAN into different parser
                    if (line.Level == initialLevel + 1)
                    {
                        var placContent = line.GetLineContent(4);
                        gedcomEvent.Location = placContent.Length == 0 ? null : placContent.ToString();
                    }
                }
            }

            return ParseResult.Create(gedcomEvent, line);
        }
    }
}