namespace Taumuon.GedcomParser.Parser
{
    public class HeaderParser
    {
        public static ParseResult<GedcomHeader> Parse(GedcomLine first, ILineProvider lineProvider)
        {
            CurrentLevel currentLevel = CurrentLevel.None;

            var header = new GedcomHeader();

            GedcomLine line = default;
            string currentRawLine;
            while ((currentRawLine = lineProvider.ReadLine()) != null)
            {
                line = ParserHelper.ParseLine(currentRawLine);

                if (line.Level == 0)
                {
                    break;
                }

                if (line.Level == 1)
                {
                    switch (line.GetTagOrRef())
                    {
                        case "SOUR":
                            currentLevel = CurrentLevel.Sour;
                            break;
                        case "GEDC":
                            currentLevel = CurrentLevel.Gedc;
                            break;
                        case "CHAR":
                            header.GedcomCharacterSet = line.GetLineContent();
                            break;
                    }
                }
                else if (line.Level == 2)
                {
                    if (currentLevel == CurrentLevel.Sour)
                    {
                        switch (line.GetTagOrRef())
                        {
                            case "NAME":
                                header.SourceName = line.GetLineContent();
                                break;
                            case "VERS":
                                header.SourceVers = line.GetLineContent();
                                break;
                            case "CORP":
                                header.SourceCorp = line.GetLineContent();
                                break;
                        }
                    }
                    else if (currentLevel == CurrentLevel.Gedc)
                    {
                        if (line.GetTagOrRef() == "VERS")
                        {
                            header.GedcomVers = line.GetLineContent();
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