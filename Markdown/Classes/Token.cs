using Markdown.Enums;

namespace Markdown.Classes;

public class Token
{
    public int StartIndex { get; set; }
    public int EndIndex { get; set; }
    public Style Style { get; set; }

    public Token(int startIndex, int endIndex, Style style)
    {
        StartIndex = startIndex;
        EndIndex = endIndex;
        Style = style;
    }

    public bool Equal(Token token)
    {
        return StartIndex == token.StartIndex && Style == token.Style;
    }
}
