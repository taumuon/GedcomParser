using System;
using System.Diagnostics;

namespace Taumuon.GedcomParserSpan.Parser
{
    [DebuggerDisplay("{Level} {OriginalLine}")]
    public readonly ref struct GedcomLine
    {
        private readonly ReadOnlySpan<char> _lineContent;

        public GedcomLine(int level, ReadOnlySpan<char> lineContent)
        {
            Level = level;
            _lineContent = lineContent;
        }

        // This is either a token, or a ref
        // e.g. 
        // 1 NAME Bob/Cox
        // is NAME
        // 0 @I1@ INDI
        // is @I1@
        public ReadOnlySpan<char> GetFirstItem()
        {
            int indexOfItem = _lineContent.IndexOf(' ');

            if (indexOfItem == -1)
            {
                return _lineContent;
            }

            return _lineContent.Slice(0, indexOfItem);
        }

        public ReadOnlySpan<char> GetLineContent()
        {
            int indexOfSpace = _lineContent.IndexOf(' ');
            if (indexOfSpace == -1)
            {
                return new ReadOnlySpan<char>();
            }
            return _lineContent.Slice(indexOfSpace + 1, _lineContent.Length - indexOfSpace - 1);
        }

        public ReadOnlySpan<char> GetLineContent(int indexOfSpace)
        {
            if (indexOfSpace >= _lineContent.Length)
            {
                return new ReadOnlySpan<char>();
            }
            return _lineContent.Slice(indexOfSpace + 1, _lineContent.Length - indexOfSpace - 1);
        }

        public int Level { get; }
        public ReadOnlySpan<char> LineContent => _lineContent;
    }
}
