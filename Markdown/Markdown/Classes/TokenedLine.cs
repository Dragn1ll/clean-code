namespace Markdown.Classes;

public class TokenedLine(string line, List<Token> tokens)
{
    public string Line { get; set; } = line;
    public List<Token> Tokens { get; set; } = tokens;
}