using System;

namespace Taumuon.GedcomParserSpan.Parser
{
    public class FamilyParser
    {
        public static ParseResult<Family> Parse(GedcomLine first, ILineProvider lineProvider)
        {
            var family = new Family();

            family.ID = ParserHelper.ParseID(first.GetFirstItem()).ToString();

            bool inMarriage = false;

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

                var tag = line.GetFirstItem();

                if (ParserHelper.Equals(tag, "MARR"))
                {
                    inMarriage = true;
                }
                else if (ParserHelper.Equals(tag, "DATE"))
                {
                    if (inMarriage) // TODO: should have MARR parser
                    {
                        var date = ParserHelper.GetLineContent(line.LineContent);
                        if (family.Marriage == null)
                        {
                            family.Marriage = new GedcomEvent();
                        }
                        family.Marriage.Date = date.ToString();
                    }
                }
                else if (ParserHelper.Equals(tag, "PLAC"))
                {
                    if (inMarriage) // Assume level + 1 is MARR
                    {
                        var place = ParserHelper.GetLineContent(line.LineContent);
                        if (family.Marriage == null)
                        {
                            family.Marriage = new GedcomEvent();
                        }
                        family.Marriage.Location = place.ToString();
                    }
                }
                else if (ParserHelper.Equals(tag, "HUSB"))
                {
                    // Ignore any husband and wife information in the middle of a marriage tag.
                    // Present for torture test files - and info redundant?
                    // can have e.g. "2 HUSB", with no additional info
                    var husbContent = ParserHelper.GetLineContent(line.LineContent);
                    if (husbContent.Length > 0)
                    {
                        family.HusbandID = ParserHelper.ParseID(husbContent).ToString();
                    }
                }
                else if (ParserHelper.Equals(tag, "WIFE"))
                {
                    var wifeContent = ParserHelper.GetLineContent(line.LineContent);
                    // Ignore any husband and wife information in the middle of a marriage tag.
                    // Present for torture test files - and info redundant?
                    // can have e.g. "2 HUSB", with no additional info
                    if (wifeContent.Length > 0)
                    {
                        family.WifeID = ParserHelper.ParseID(wifeContent).ToString();
                    }
                }
                else if (ParserHelper.Equals(tag, "CHIL"))
                {
                    var childContent = ParserHelper.GetLineContent(line.LineContent);
                    family.ChildIDs.Add(ParserHelper.ParseID(childContent).ToString());
                }
                else
                {
                    inMarriage = false;
                }
            }

            return ParseResult.Create(family, line);
        }
    }
}