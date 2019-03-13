using System;
using System.IO;
using System.Text;

namespace Taumuon.GedcomParserSpan.Parser
{
    public class LineProvider : ILineProvider
    {
        private readonly StreamReader _streamReader;

        public LineProvider(StreamReader streamReader)
        {
            _streamReader = streamReader;
        }

        char[] buffer = new char[2048];
        int bufferPos;
        int bufferEndPos;
        StringBuilder sb = null;

        public ReadOnlySpan<char> ReadLine()
        {
            var bufferSpan = buffer.AsSpan();

            for (; ;)
            {
                if (bufferPos == bufferEndPos)
                {
                    int charsRead = _streamReader.Read(bufferSpan);

                    if (charsRead == 0)
                    {
                        if (sb != null)
                        {
                            string str = sb.ToString();
                            sb = null;
                            return str.AsSpan();
                        }
                        return new ReadOnlySpan<char>();
                    }
                    bufferPos = 0;
                    bufferEndPos = charsRead;
                }

                if (bufferPos == 0 && buffer[0] == '\n')
                {
                    // Must be a lingering '\n' from a '\r\n' pair
                    bufferPos = 1;
                }

                ReadOnlySpan<char> readChars = bufferSpan.Slice(bufferPos, bufferEndPos - bufferPos);

                int indexOfNewLine = readChars.IndexOf('\r');

                if (indexOfNewLine != -1)
                {
                    bool isRN = readChars.Length > (indexOfNewLine + 1) && readChars[indexOfNewLine + 1] == '\n';
                    bufferPos += isRN ? indexOfNewLine + 2 : indexOfNewLine + 1;
                    return GetLine(readChars.Slice(0, indexOfNewLine));
                }
                else if ((indexOfNewLine = readChars.IndexOf('\n')) != -1)
                {
                    bufferPos += indexOfNewLine + 1;
                    return GetLine(readChars.Slice(0, indexOfNewLine));
                }
                else
                {
                    if (sb == null) { sb = new StringBuilder(); }
                    sb.Append(readChars);
                    bufferPos = bufferEndPos;
                }
            }
        }

        private ReadOnlySpan<char> GetLine(in ReadOnlySpan<char> input)
        {
            if (sb != null)
            {
                var result = sb.Append(input).ToString().AsSpan();
                sb = null;
                return result;
            }
            return input;
        }
    }
}
