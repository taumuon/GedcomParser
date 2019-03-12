using System;
using System.IO;
using Taumuon.GedcomParserSpan.Parser;

namespace Taumuon.GedcomParserSpan
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

        public void Parse(ILineProvider lineProvider)
        {
            GedcomLine lastLine = default;

            var firstRawLine = lineProvider.ReadLine();

            if (firstRawLine == null)
            {
                throw new InvalidOperationException("File empty");
            }

            var firstLine = ParserHelper.ParseLine(firstRawLine);
            if (firstLine.Level != 0 && !ParserHelper.Equals(firstLine.GetFirstItem(), "HEAD"))
            {
                throw new InvalidOperationException("GEDCOM Header Not Found");
            }

            var gedcomHeaderParse = HeaderParser.Parse(firstLine, lineProvider);
            _headerCallback?.Invoke(gedcomHeaderParse.Result);

            var newLine = gedcomHeaderParse.Line;
            lastLine = newLine;

            while (newLine.LineContent.Length > 0)
            {
                var content = ParserHelper.GetLineContent(newLine.LineContent);
                if (content.Length == 0)
                {
                    var unrecognisedRawLine = lineProvider.ReadLine();
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

                int indexOfSpace =  newLine.LineContent.IndexOf(' ');
                var refContent = indexOfSpace == -1
                    ? new ReadOnlySpan<char>()
                    : newLine.LineContent.Slice(indexOfSpace + 1, newLine.LineContent.Length - indexOfSpace - 1);
                var unknown = false;

                if (ParserHelper.Equals(refContent, "INDI"))
                {
                    var individualParseResult = IndividualParser.Parse(newLine, lineProvider);
                    _individualCallback?.Invoke(individualParseResult.Result);
                    newLine = individualParseResult.Line;
                    lastLine = newLine;
                }
                else if (ParserHelper.Equals(refContent, "FAM"))
                {
                    var familyParseResult = FamilyParser.Parse(newLine, lineProvider);
                    _familyCallback?.Invoke(familyParseResult.Result);
                    newLine = familyParseResult.Line;
                    lastLine = newLine;
                }
                else if (ParserHelper.Equals(refContent, "NOTE"))
                {
                    var noteParseResult = NoteParser.Parse(newLine, lineProvider);
                    _noteCallback?.Invoke(noteParseResult.Result);
                    newLine = noteParseResult.Line;
                    lastLine = newLine;
                }
                else if (ParserHelper.Equals(refContent, "OBJE"))
                {
                    var objParserResult = ObjectParser.Parse(newLine, lineProvider);
                    _imageCallback?.Invoke(objParserResult.Result);
                    newLine = objParserResult.Line;
                    lastLine = newLine;
                }
                else
                {
                    var unrecognisedRawLine = lineProvider.ReadLine();
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
                }

                if (unknown)
                {
                    continue;
                }
            }

            if (lastLine.LineContent.Length == 0)
            {
                throw new InvalidOperationException("file contains no content");
            }

            if (lastLine.Level != 0 || !ParserHelper.Equals(lastLine.GetFirstItem(), "TRLR"))
            {
                throw new InvalidOperationException("GEDCOM TRLR not found");
            }
        }
    }
}
