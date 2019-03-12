using System;

namespace Taumuon.GedcomParserSpan.Parser
{
    public class IndividualParser
    {
        public static ParseResult<Individual> Parse(GedcomLine first, ILineProvider lineProvider)
        {
            var individual = new Individual();

            individual.ID = ParserHelper.ParseID(first.GetFirstItem()).ToString();

            GedcomLine line = default;
            ReadOnlySpan<char> currentRawLine;
            var newLine = false;
            while (true)
            {
                // TODO: this loop is really messy, as some of the parsing
                //  works on lines as retrieved from the parser, where some
                //  other iterations call onto a sub-parser which return a final
                //  line. Should split this logic
                if (!newLine)
                {
                    currentRawLine = lineProvider.ReadLine();
                    if (currentRawLine == null)
                    {
                        break;
                    }
                    line = ParserHelper.ParseLine(currentRawLine);
                }
                newLine = false;

                if (line.Level == 0)
                {
                    break;
                }

                var lineOriginalContent = line.LineContent;

                var tag = line.GetFirstItem();
                if (ParserHelper.Equals(tag, "NAME")
                    && individual.LastName == null && individual.FirstNames == null)
                {
                    ParseNames(line, individual);
                }
                else if (ParserHelper.Equals(tag, "SEX"))
                {
                    individual.Gender = ParserHelper.Equals(ParserHelper.GetLineContent(line.LineContent), "M") ? Gender.Male : Gender.Female;
                }
                else if (ParserHelper.Equals(tag, "FAMS"))
                {
                    individual.FamilyIDSpouse = ParserHelper.ParseID(ParserHelper.GetLineContent(line.LineContent)).ToString();
                }
                else if (ParserHelper.Equals(tag, "FAMC"))
                {
                    individual.FamilyIDChild = ParserHelper.ParseID(ParserHelper.GetLineContent(line.LineContent)).ToString();
                }

                else if (ParserHelper.Equals(tag, "OBJE"))
                {
                    var content = ParserHelper.GetLineContent(line.LineContent);
                    if (content.Length > 0)
                    {
                        individual.ImageID = ParserHelper.ParseID(content).ToString();
                    }
                }
                else if (ParserHelper.Equals(tag, "NOTE"))
                {
                    var noteContent = ParserHelper.GetLineContent(lineOriginalContent);
                    if (noteContent.Length > 0)
                    {
                        if (noteContent[0] == '@')
                        {
                            individual.NoteID = ParserHelper.ParseID(noteContent).ToString();
                        }
                        else
                        {
                            individual.Note = noteContent.ToString();
                        }
                    }
                }
                else if (ParserHelper.Equals(tag, "CONT"))
                {
                    var contContent = ParserHelper.GetLineContent(lineOriginalContent);
                    individual.Note += Environment.NewLine + contContent.ToString();
                }
                else if (ParserHelper.Equals(tag, "CONC"))
                {
                    // TODO: is GenesReunited maintaining the trailing space?
                    // If so, is this correct?
                    var concContent = ParserHelper.GetLineContent(lineOriginalContent);
                    if (concContent.Length > 0)
                    {
                        individual.Note += concContent.ToString();
                    }
                }
                else if (ParserHelper.Equals(tag, "BIRT"))
                {
                    var birthParseResult = EventParser.Parse(line, lineProvider);
                    if (individual.Birth == null)
                    {
                        individual.Birth = birthParseResult.Result;
                    }
                    line = birthParseResult.Line;
                    newLine = true;
                }
                else if (ParserHelper.Equals(tag, "DEAT"))
                {
                    var deathParseResult = EventParser.Parse(line, lineProvider);
                    if (individual.Death == null)
                    {
                        individual.Death = deathParseResult.Result;
                    }
                    line = deathParseResult.Line;
                    newLine = true;
                }
                else if (ParserHelper.Equals(tag, "BAPM"))
                {
                    var baptismParseResult = EventParser.Parse(line, lineProvider);
                    if (individual.Baptism == null)
                    {
                        individual.Baptism = baptismParseResult.Result;
                    }
                    line = baptismParseResult.Line;
                    newLine = true;
                }
                else if (ParserHelper.Equals(tag, "RESI"))
                {
                    var residenceParseResult = EventParser.Parse(line, lineProvider);
                    individual.Residences.Add(residenceParseResult.Result);
                    line = residenceParseResult.Line;
                    newLine = true;
                }
                else if (ParserHelper.Equals(tag, "CENS"))
                {
                    var censusParseResult = EventParser.Parse(line, lineProvider);
                    individual.Census.Add(censusParseResult.Result);
                    line = censusParseResult.Line;
                    newLine = true;
                }
                else if (ParserHelper.Equals(tag, "MARR"))
                {
                    var marriageParseResult = EventParser.Parse(line, lineProvider);
                    if (marriageParseResult.Result != null)
                    {
                        individual.Marriages.Add(marriageParseResult.Result);
                    }
                    line = marriageParseResult.Line;
                    newLine = true;
                    // TODO: cope with multiple marriage records
                    // It seems that ancestry only outputs multiple marriage records in an individual
                    //  if there's conflicting information, not for multiple marriages
                    // Multiple marriages for an individual are put into the FAMs tag
                }
                //case "ENGA":
                //    // TODO:
                //    // individual.Engagement = EventParser.Parse(parserLinesEnga);
                //    break;

                if (line.Level == 0)
                {
                    break;
                }
            }

            if (individual.LastName == null)
            {
                individual.LastName = string.Empty;
            }

            if (individual.FirstName == null)
            {
                individual.FirstName = string.Empty;
            }

            if (individual.FirstNames == null)
            {
                individual.FirstNames = string.Empty;
            }

            return ParseResult.Create(individual, line);
        }

        private static void ParseNames(GedcomLine gedcomLine, Individual individual)
        {
            individual.FirstName = string.Empty;
            individual.FirstNames = string.Empty;

            var line = gedcomLine.LineContent;
            var content = ParserHelper.GetLineContent(line);

            // If there are no slashes to indicate surname, assume that the last entry is the surname
            int lastNameStart = content.IndexOf("/", StringComparison.Ordinal);
            if (lastNameStart != -1)
            {
                var preLine = content.Slice(0, lastNameStart).Trim();

                var remainingLine = content.Slice(lastNameStart + 1, content.Length - lastNameStart - 1);
                int trailingSlashStart = remainingLine.IndexOf("/", StringComparison.Ordinal);
                string lastName;
                if (trailingSlashStart != -1)
                {
                    lastName = remainingLine.Slice(0, trailingSlashStart).ToString();
                }
                else
                {
                    lastName = remainingLine.ToString();
                }
                individual.LastName = lastName;

                var firstNames = preLine.ToString();
                individual.FirstNames = firstNames;
                var indexOfSpace = preLine.IndexOf(' ');
                individual.FirstName = (indexOfSpace == -1) ? firstNames : preLine.Slice(0, indexOfSpace).ToString();
            }
            else
            {
                var indexOfLast = content.LastIndexOf(' ');
                if (indexOfLast != -1)
                {
                    individual.LastName = content.Slice(indexOfLast + 1, content.Length - indexOfLast - 1).ToString();

                    var firstNamesSlice = content.Slice(0, indexOfLast);
                    var firstNames = firstNamesSlice.ToString();
                    individual.FirstNames = firstNames;
                    var indexOfSpace = firstNamesSlice.IndexOf(' ');
                    individual.FirstName = (indexOfSpace == -1)
                        ? firstNames
                        : firstNamesSlice.Slice(0, indexOfSpace).ToString();
                }
                else
                {
                    individual.LastName = content.ToString();
                }
            }
        }
    }
}