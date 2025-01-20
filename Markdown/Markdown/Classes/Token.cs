using Markdown.Enums;

namespace Markdown.Classes;

public class Token(int startIndex, int endIndex, Style style)
{
    // Это свойство -1 только в том случае, если у нас экранирование либо сноска
    public int StartIndex { get; } = startIndex;
    // Это свойство -1, когда есть открывающийся тег, но нет закрывающегося
    public int EndIndex { get; set; } = endIndex;
    public Style Style { get; } = style;

    public override bool Equals(object? obj)
    {
        return obj is Token token && StartIndex == token.StartIndex && Style == token.Style;
    }
}
