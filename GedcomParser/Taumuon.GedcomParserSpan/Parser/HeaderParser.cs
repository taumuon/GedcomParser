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
                        header.GedcomCharacterSet = line.GetLineContent(4).ToString();
                    }
                }
                else if (line.Level == 2)
                {
                    if (currentLevel == CurrentLevel.Sour)
                    {
                        var tag = line.GetFirstItem();
                        if (ParserHelper.Equals(tag, "NAME"))
                        {
                            header.SourceName = line.GetLineContent(4).ToString();
                        }
                        else if (ParserHelper.Equals(tag, "VERS"))
                        {
                            header.SourceVers = line.GetLineContent(4).ToString();
                        }
                        else if (ParserHelper.Equals(tag, "CORP"))
                        {
                            header.SourceCorp = line.GetLineContent(4).ToString();
                        }
                    }
                    else if (currentLevel == CurrentLevel.Gedc)
                    {
                        if (ParserHelper.Equals(line.GetFirstItem(), "VERS"))
                        {
                            header.GedcomVers = line.GetLineContent(4).ToString();
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