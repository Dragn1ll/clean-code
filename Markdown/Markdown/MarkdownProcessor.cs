using Markdown.Classes;
using Markdown.Interfaces;

namespace Markdown;

public class MarkdownProcessor
{
    private readonly IParser _parser = new Parser();
    private readonly IRenderer _renderer = new Renderer();

    public string ConvertToHtml(string text)
    {
        return _renderer.Render(_parser.ParseToTokens(ref text), text);
    }
}
