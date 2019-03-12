namespace Taumuon.GedcomParser.Parser
{
    public class FamilyParser
    {
        public static ParseResult<Family> Parse(GedcomLine first, ILineProvider lineProvider)
        {
            var family = new Family();

            family.ID = ParserHelper.ParseID(first.GetTagOrRef());

            bool inMarriage = false;

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
                    case "MARR":
                        {
                            inMarriage = true;
                            break;
                        }
                    case "DATE":
                        if (inMarriage) // TODO: should have MARR parser
                        {
                            var date = line.GetLineContent();
                            if (family.Marriage == null)
                            {
                                family.Marriage = new GedcomEvent();
                            }
                            family.Marriage.Date = date;
                        }
                        break;
                    case "PLAC":
                        if (inMarriage) // Assume level + 1 is MARR
                        {
                            var place = line.GetLineContent();
                            if (family.Marriage == null)
                            {
                                family.Marriage = new GedcomEvent();
                            }
                            family.Marriage.Location = place;
                        }
                        break;
                    case "HUSB":
                        // Ignore any husband and wife information in the middle of a marriage tag.
                        // Present for torture test files - and info redundant?
                        // can have e.g. "2 HUSB", with no additional info
                        var contentHusb = line.GetLineContent();
                        if (!string.IsNullOrEmpty(contentHusb))
                        {
                            family.HusbandID = ParserHelper.ParseID(contentHusb);
                        }
                        break;
                    case "WIFE":
                        // Ignore any husband and wife information in the middle of a marriage tag.
                        // Present for torture test files - and info redundant?
                        // can have e.g. "2 HUSB", with no additional info
                        var contentWife = line.GetLineContent();
                        if (!string.IsNullOrEmpty(contentWife))
                        {
                            family.WifeID = ParserHelper.ParseID(contentWife);
                        }
                        break;
                    case "CHIL":
                        family.ChildIDs.Add(ParserHelper.ParseID(line.GetLineContent()));
                        break;
                    default:
                        inMarriage = false;
                        break;
                }
            }

            return ParseResult.Create(family, line);
        }
    }
}