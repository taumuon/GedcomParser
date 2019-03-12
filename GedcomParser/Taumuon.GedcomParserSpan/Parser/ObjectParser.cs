using System;

namespace Taumuon.GedcomParserSpan.Parser
{
    public class ObjectParser
    {
        public static ParseResult<Image> Parse(GedcomLine first, ILineProvider lineProvider)
        {
            var image = new Image();

            image.ID = ParserHelper.ParseID(first.GetFirstItem()).ToString();

            var initialLevel = first.Level;

            GedcomLine line = default;
            ReadOnlySpan<char> currentRawLine;
            while ((currentRawLine = lineProvider.ReadLine()) != null)
            {
                line = ParserHelper.ParseLine(currentRawLine);

                if (line.Level == first.Level)
                {
                    break;
                }

                var splitLine = line.GetFirstItem();
                if (ParserHelper.Equals(splitLine, "TITL"))
                {
                    var title = line.GetLineContent(4);
                    image.Title = title.ToString();
                }
                else if (ParserHelper.Equals(splitLine, "FILE"))
                {
                    var filePath = line.GetLineContent(4);
                    image.FilePath = filePath.ToString();
                }
            }

            return ParseResult.Create(image, line);
        }
    }
}