using System;

namespace Taumuon.GedcomParser.Parser
{
    public class ObjectParser
    {
        public static ParseResult<Image> Parse(GedcomLine first, ILineProvider lineProvider)
        {
            var image = new Image();

            image.ID = ParserHelper.ParseID(first.GetTagOrRef());

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
                    case "TITL":
                        var title = line.GetLineContent();
                        image.Title = title;
                        break;
                    case "FILE":
                        var filePath = line.GetLineContent();
                        image.FilePath = filePath;
                        break;
                }
            }

            return ParseResult.Create(image, line);
        }
    }
}