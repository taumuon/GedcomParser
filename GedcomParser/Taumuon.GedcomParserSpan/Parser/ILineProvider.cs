using System;

namespace Taumuon.GedcomParserSpan.Parser
{
    public interface ILineProvider
    {
        ReadOnlySpan<char> ReadLine();
    }
}
