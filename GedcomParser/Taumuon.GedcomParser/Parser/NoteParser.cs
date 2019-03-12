using System;

namespace Taumuon.GedcomParser.Parser
{
    public class NoteParser
    {
        public static ParseResult<Note> Parse(GedcomLine first, ILineProvider lineProvider)
        {
            string id = ParserHelper.ParseID(first.GetTagOrRef());
            string text = null;

            var initialLevel = first.Level;

            GedcomLine line = default;
            string currentRawLine;
            while ((currentRawLine = lineProvider.ReadLine()) != null)
            {
                line = ParserHelper.ParseLine(currentRawLine);

                if (line.Level == first.Level)
                {
                    break;
                }

                switch (line.GetTagOrRef())
                {
                    case "CONT":
                        string contText = line.GetLineContent();
                        text += Environment.NewLine + contText;
                        break;
                    case "CONC":
                        // TODO: is GenesReunited maintaining the trailing space?
                        // If so, is this correct?
                        string concText = line.GetLineContent();
                        text += concText;
                        break;
                }
            }

            return ParseResult.Create(new Note(id, text), line);
        }
    }
}