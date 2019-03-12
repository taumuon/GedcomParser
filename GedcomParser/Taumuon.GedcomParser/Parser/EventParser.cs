namespace Taumuon.GedcomParser.Parser
{
    public class EventParser
    {
        public static ParseResult<GedcomEvent> Parse(GedcomLine first, ILineProvider lineProvider)
        {
            GedcomEvent gedcomEvent = new GedcomEvent();

            var initialLevel = first.Level;

            GedcomLine line = default;
            string currentRawLine;
            while ((currentRawLine = lineProvider.ReadLine()) != null)
            {
                line = ParserHelper.ParseLine(currentRawLine);

                if (line.Level <= first.Level)
                {
                    break;
                }

                switch (line.GetTagOrRef())
                {
                    case "DATE":
                        // If checks we're parsing actual date and not
                        // CREA or CHAN tags
                        // TODO: should actually put CREA and CHAN into different parser
                        if (line.Level == initialLevel + 1)
                        {
                            gedcomEvent.Date = line.GetLineContent();
                        }
                        break;
                    case "PLAC":
                        // If checks we're parsing actual date and not
                        // CREA or CHAN tags
                        // TODO: should actually put CREA and CHAN into different parser
                        if (line.Level == initialLevel + 1)
                        {
                            gedcomEvent.Location = line.GetLineContent();
                        }
                        break;
                }
            }

            return ParseResult.Create(gedcomEvent, line);
        }
    }
}