namespace Taumuon.GedcomParser.Parser
{
    public struct ParseResult<T> where T : class
    {
        public T Result { get; }
        public GedcomLine Line { get; }

        public ParseResult(T result, GedcomLine line)
        {
            Result = result;
            Line = line;
        }
    }

    public static class ParseResult
    {
        public static ParseResult<T> Create<T>(T result, GedcomLine line) where T : class
        {
            return new ParseResult<T>(result, line);
        }
    }
}
