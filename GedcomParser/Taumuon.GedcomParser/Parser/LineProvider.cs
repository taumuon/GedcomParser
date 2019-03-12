using System.IO;

namespace Taumuon.GedcomParser.Parser
{
    public class LineProvider : ILineProvider
    {
        private readonly StreamReader _streamReader;

        public LineProvider(StreamReader streamReader)
        {
            _streamReader = streamReader;
        }

        public string ReadLine()
        {
            return _streamReader.ReadLine();
        }
    }
}
