using Markdown.Classes;
using Markdown.Interfaces;

namespace Markdown;

public class MarkdownProcessor
{
    private readonly IParser _parser = new Parser();
    private readonly IRenderer _renderer = new Renderer();

    public async Task<string> ConvertToHtml(string text)
    {
        var tokenedLine = await _parser.ParseToTokens(text);
        return await _renderer.Render(tokenedLine.Tokens!, tokenedLine.Line);
    }
}
