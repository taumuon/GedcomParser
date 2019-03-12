using System;
using System.Diagnostics;

namespace Taumuon.GedcomParser.Parser
{
    [DebuggerDisplay("{Level} {OriginalLine}")]
    public struct GedcomLine : IEquatable<GedcomLine>
    {
        public GedcomLine(int level, string trimmedLine)
        {
            Level = level;

            _trimmedLine = trimmedLine;
        }

        public int Level { get; }

        private readonly string _trimmedLine;

        // Gets the second item in a line, e.g.
        // 2 BIRT
        // returns birth
        // 1 @I1@ INDI
        // returns @I1@
        public string GetTagOrRef()
        {
            var indexOfFirstSpace = _trimmedLine.IndexOf(' '); // Guaranteed to always have else will never get in here
            var indexOfSpace = _trimmedLine.IndexOf(' ', indexOfFirstSpace + 1);

            if (indexOfSpace == -1)
            {
                return _trimmedLine.Substring(indexOfFirstSpace + 1, _trimmedLine.Length - indexOfFirstSpace - 1);
            }
            return _trimmedLine.Substring(indexOfFirstSpace + 1, indexOfSpace - indexOfFirstSpace - 1);
        }

        // Gets the content after the tag or identifier.
        public string GetLineContent()
        {
            var indexOfFirstSpace = _trimmedLine.IndexOf(' ');
            var indexOfSpace = _trimmedLine.IndexOf(' ', indexOfFirstSpace + 1);

            if (indexOfSpace == -1) return null;
            return _trimmedLine.Substring(indexOfSpace + 1, _trimmedLine.Length - indexOfSpace - 1);
        }

        public bool Equals(GedcomLine other)
        {
            return Level == other.Level
                && _trimmedLine == other._trimmedLine;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is GedcomLine)) return false;

            return this.Equals((GedcomLine)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Level.GetHashCode()
                    + (39 * _trimmedLine?.GetHashCode() ?? 1);
            }
        }
    }
}
