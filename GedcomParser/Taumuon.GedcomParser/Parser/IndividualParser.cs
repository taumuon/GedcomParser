using System;

namespace Taumuon.GedcomParser.Parser
{
    public class IndividualParser
    {
        public static ParseResult<Individual> Parse(GedcomLine first, ILineProvider lineProvider)
        {
            var individual = new Individual();

            individual.ID = ParserHelper.ParseID(first.GetTagOrRef());

            GedcomLine line = default;
            string currentRawLine;
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

                switch (line.GetTagOrRef())
                {
                    case "NAME":
                        if (individual.LastName != null || individual.FirstNames != null)
                        {
                            break;
                        }

                        ParseNames(line, individual);
                        break;
                    case "SEX":
                        individual.Gender = (line.GetLineContent() == "M") ? Gender.Male : Gender.Female;
                        break;
                    case "FAMS":
                        individual.FamilyIDSpouse = ParserHelper.ParseID(line.GetLineContent());
                        break;
                    case "FAMC":
                        individual.FamilyIDChild = ParserHelper.ParseID(line.GetLineContent());
                        break;
                    case "OBJE":
                        var objeContent = line.GetLineContent();
                        if (!string.IsNullOrEmpty(objeContent)) // TODO: test for this - currently don't support Images directly in Individual
                                                  // should fix this
                        {
                            individual.ImageID = ParserHelper.ParseID(objeContent);
                        }
                        break;
                    case "NOTE":
                        string noteText = line.GetLineContent() ?? string.Empty;
                        // My be an empty NOTE tag, but will be concatenated onto

                        if (noteText.StartsWith("@"))
                        {
                            string noteId = ParserHelper.ParseID(noteText);
                            individual.NoteID = noteId;
                        }
                        else
                        {
                            individual.Note = noteText;
                        }
                        break;
                    case "CONT":
                        string contText = line.GetLineContent() ?? string.Empty;
                        individual.Note += Environment.NewLine + contText;
                        break;
                    case "CONC":
                        // TODO: is GenesReunited maintaining the trailing space?
                        // If so, is this correct?
                        var concText = line.GetLineContent();
                        individual.Note += concText;
                        break;
                    case "BIRT":
                        var birthParseResult = EventParser.Parse(line, lineProvider);
                        if (individual.Birth == null)
                        {
                            individual.Birth = birthParseResult.Result;
                        }
                        line = birthParseResult.Line;
                        newLine = true;
                        break;
                    case "DEAT":
                        var deathParseResult = EventParser.Parse(line, lineProvider);
                        if (individual.Death == null)
                        {
                            individual.Death = deathParseResult.Result;
                        }
                        line = deathParseResult.Line;
                        newLine = true;
                        break;
                    case "BAPM":
                        var baptismParseResult = EventParser.Parse(line, lineProvider);
                        if (individual.Baptism == null)
                        {
                            individual.Baptism = baptismParseResult.Result;
                        }
                        line = baptismParseResult.Line;
                        newLine = true;
                        break;
                    case "RESI":
                        var residenceParseResult = EventParser.Parse(line, lineProvider);
                        individual.Residences.Add(residenceParseResult.Result);
                        line = residenceParseResult.Line;
                        newLine = true;
                        break;
                    case "CENS":
                        var censusParseResult = EventParser.Parse(line, lineProvider);
                        individual.Census.Add(censusParseResult.Result);
                        line = censusParseResult.Line;
                        newLine = true;
                        break;
                    case "MARR":
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
                        break;
                    //case "ENGA":
                    //    // TODO:
                    //    // individual.Engagement = EventParser.Parse(parserLinesEnga);
                    //    break;
                }

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

            var line = gedcomLine.GetLineContent();

            // If there are no slashes to indicate surname, assume that the last entry is the surname
            int lastNameStart = (line == null) ? -1 : line.IndexOf("/", StringComparison.Ordinal);
            if (lastNameStart != -1)
            {
                string preLine = line.Substring(0, lastNameStart).Trim();

                int trailingSlashStart = line.IndexOf('/', lastNameStart + 1);
                string lastName;
                if (trailingSlashStart != -1)
                {
                    lastName = line.Substring(lastNameStart + 1, trailingSlashStart - lastNameStart - 1);
                }
                else
                {
                    lastName = line.Substring(lastNameStart + 1, line.Length - lastNameStart - 1);
                }

                individual.LastName = lastName;

                individual.FirstNames = preLine;
                var indexOfSpace = preLine.IndexOf(' ');
                individual.FirstName = (indexOfSpace == -1) ? preLine : preLine.Substring(0, indexOfSpace);
            }
            else
            {
                var c = gedcomLine.GetLineContent();
                var indexOfLast = (c == null) ? -1 : c.LastIndexOf(' ');
                if (indexOfLast != -1)
                {
                    individual.LastName = c.Substring(indexOfLast + 1, c.Length - indexOfLast - 1);

                    string firstNames = c.Substring(0, indexOfLast);
                    individual.FirstNames = firstNames;
                    var indexOfSpace = firstNames.IndexOf(' ');
                    individual.FirstName = (indexOfSpace == -1) ? firstNames : firstNames.Substring(0, indexOfSpace);
                }
                else
                {
                    individual.LastName = c;
                }
            }
        }
    }
}