using Markdown.Enums;

namespace Markdown.Classes;

public class Token
{
    public uint StartIndex { get; set; }
    public uint EndIndex { get; set; }
    public Style Style { get; set; }

    public Token(uint startIndex, uint endIndex, Style style)
    {
        StartIndex = startIndex;
        EndIndex = endIndex;
        Style = style;
    }
}
