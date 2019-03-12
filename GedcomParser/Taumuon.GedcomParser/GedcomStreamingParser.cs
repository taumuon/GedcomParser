using System;
using System.IO;
using Taumuon.GedcomParser.Parser;

namespace Taumuon.GedcomParser
{
    public class GedcomStreamingParser
    {
        // Could have a single func, and a tagged union of the returned type
        // Or instead of callbacks, have a cursor based parser.
        private readonly Action<GedcomHeader> _headerCallback;
        private readonly Action<Individual> _individualCallback;
        private readonly Action<Family> _familyCallback;
        private readonly Action<Image> _imageCallback;
        private readonly Action<Note> _noteCallback;

        public GedcomStreamingParser(Action<GedcomHeader> headerCallback,
                                     Action<Individual> individualCallback,
                                     Action<Family> familyCallback,
                                     Action<Image> imageCallback,
                                     Action<Note> noteCallback)
        {
            _headerCallback = headerCallback;
            _individualCallback = individualCallback;
            _familyCallback = familyCallback;
            _imageCallback = imageCallback;
            _noteCallback = noteCallback;
        }

        public void Parse(Stream stream)
        {
            if (stream == null)
            {
                throw new InvalidOperationException("Stream cannot be null");
            }

            GedcomLine lastLine = default;
            using (var reader = new StreamReader(stream))
            {
                var firstRawLine = reader.ReadLine();

                if (firstRawLine == null)
                {
                    throw new InvalidOperationException("File empty");
                }

                var firstLine = ParserHelper.ParseLine(firstRawLine);
                if (firstLine.Level != 0 && firstLine.GetTagOrRef() != "HEAD")
                {
                    throw new InvalidOperationException("GEDCOM Header Not Found");
                }

                LineProvider lineProvider = new LineProvider(reader);

                var gedcomHeaderParse = HeaderParser.Parse(firstLine, lineProvider);
                _headerCallback?.Invoke(gedcomHeaderParse.Result);

                var newLine = gedcomHeaderParse.Line;
                lastLine = newLine;

                while (!newLine.Equals(default))
                {
                    if (string.IsNullOrEmpty(newLine.GetLineContent()))
                    {
                        var unrecognisedRawLine = reader.ReadLine();
                        if (unrecognisedRawLine != null)
                        {
                            newLine = ParserHelper.ParseLine(unrecognisedRawLine);
                            lastLine = newLine;
                        }
                        else
                        {
                            newLine = default;
                        }
                        continue;
                    }

                    var firstContent = newLine.GetLineContent();
                    var unknown = false;
                    switch (firstContent)
                    {
                        case "INDI":
                            var individualParseResult = IndividualParser.Parse(newLine, lineProvider);
                            _individualCallback?.Invoke(individualParseResult.Result);
                            newLine = individualParseResult.Line;
                            lastLine = newLine;
                            break;
                        case "FAM":
                            var familyParseResult = FamilyParser.Parse(newLine, lineProvider);
                            _familyCallback?.Invoke(familyParseResult.Result);
                            newLine = familyParseResult.Line;
                            lastLine = newLine;
                            break;
                        case "NOTE":
                            var noteParseResult = NoteParser.Parse(newLine, lineProvider);
                            _noteCallback?.Invoke(noteParseResult.Result);
                            newLine = noteParseResult.Line;
                            lastLine = newLine;
                            break;
                        case "OBJE":
                            var objParserResult = ObjectParser.Parse(newLine, lineProvider);
                            _imageCallback?.Invoke(objParserResult.Result);
                            newLine = objParserResult.Line;
                            lastLine = newLine;
                            break;
                        default:
                            var unrecognisedRawLine = reader.ReadLine();
                            if (unrecognisedRawLine != null)
                            {
                                newLine = ParserHelper.ParseLine(unrecognisedRawLine);
                                lastLine = newLine;
                            }
                            else
                            {
                                newLine = default;
                            }
                            unknown = true;
                            break;
                    }

                    if (unknown)
                    {
                        continue;
                    }
                }
            }

            if (lastLine.Equals(default))
            {
                throw new InvalidOperationException("file contains no content");
            }

            if (lastLine.Level != 0 || lastLine.GetTagOrRef() != "TRLR")
            {
                throw new InvalidOperationException("GEDCOM TRLR not found");
            }
        }
    }
}
