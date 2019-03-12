using System;

namespace Taumuon.GedcomParserSpan.Parser
{
    public class NoteParser
    {
        public static ParseResult<Note> Parse(GedcomLine first, ILineProvider lineProvider)
        {
            string id = ParserHelper.ParseID(first.GetFirstItem()).ToString();
            string text = null;

            var initialLevel = first.Level;

            GedcomLine line = default;
            ReadOnlySpan<char> currentRawLine;
            while ((currentRawLine = lineProvider.ReadLine()).Length > 0)
            {
                line = ParserHelper.ParseLine(currentRawLine);

                if (line.Level == first.Level)
                {
                    break;
                }

                ReadOnlySpan<char> tag = line.GetFirstItem();

                if (ParserHelper.Equals(tag, "CONT"))
                {
                    string contText = line.GetLineContent(4).ToString();
                    text += Environment.NewLine + contText;
                }
                else if (ParserHelper.Equals(tag, "CONC"))
                {
                    // TODO: is GenesReunited maintaining the trailing space?
                    // If so, is this correct?
                    string concText = line.GetLineContent(4).ToString();
                    text += concText;
                }
            }

            return ParseResult.Create(new Note(id, text), line);
        }
    }
}