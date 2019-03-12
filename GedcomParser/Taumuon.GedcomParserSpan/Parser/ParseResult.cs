namespace Taumuon.GedcomParserSpan.Parser
{
    public ref struct ParseResult<T> where T : class
    {
        private GedcomLine _line;

        public T Result { get; }
        public GedcomLine Line => _line;

        public ParseResult(T result, GedcomLine line)
        {
            Result = result;
            _line = line;
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
