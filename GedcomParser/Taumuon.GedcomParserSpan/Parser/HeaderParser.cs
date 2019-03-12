using System;

namespace Taumuon.GedcomParserSpan.Parser
{
    public class HeaderParser
    {
        public static ParseResult<GedcomHeader> Parse(GedcomLine first, ILineProvider lineProvider)
        {
            CurrentLevel currentLevel = CurrentLevel.None;

            var header = new GedcomHeader();

            GedcomLine line = default;
            ReadOnlySpan<char> currentRawLine;
            while ((currentRawLine = lineProvider.ReadLine()).Length > 0)
            {
                line = ParserHelper.ParseLine(currentRawLine);

                if (line.Level == 0)
                {
                    break;
                }

                if (line.Level == 1)
                {
                    var tag = line.GetFirstItem();
                    if (ParserHelper.Equals(tag, "SOUR"))
                    {
                        currentLevel = CurrentLevel.Sour;
                    }
                    else if (ParserHelper.Equals(tag, "GEDC"))
                    {
                        currentLevel = CurrentLevel.Gedc;
                    }
                    else if (ParserHelper.Equals(tag, "CHAR"))
                    {
                        header.GedcomCharacterSet = ParserHelper.GetLineContent(line.LineContent).ToString();
                    }
                }
                else if (line.Level == 2)
                {
                    if (currentLevel == CurrentLevel.Sour)
                    {
                        var tag = line.GetFirstItem();
                        if (ParserHelper.Equals(tag, "NAME"))
                        {
                            header.SourceName = ParserHelper.GetLineContent(line.LineContent).ToString();
                        }
                        else if (ParserHelper.Equals(tag, "VERS"))
                        {
                            header.SourceVers = ParserHelper.GetLineContent(line.LineContent).ToString();
                        }
                        else if (ParserHelper.Equals(tag, "CORP"))
                        {
                            header.SourceCorp = ParserHelper.GetLineContent(line.LineContent).ToString();
                        }
                    }
                    else if (currentLevel == CurrentLevel.Gedc)
                    {
                        if (ParserHelper.Equals(line.GetFirstItem(), "VERS"))
                        {
                            header.GedcomVers = ParserHelper.GetLineContent(line.LineContent).ToString();
                        }
                    }
                }
            }

            return ParseResult.Create(header, line);
        }
    }

    public enum CurrentLevel
    {
        None,
        Sour,
        Gedc
    }
}