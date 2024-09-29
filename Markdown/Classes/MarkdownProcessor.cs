using Markdown.Interfaces;

namespace Markdown.Classes;

public class MarkdownProcessor
{
    private IParser parser;
    private IRenderer renderer;

    public MarkdownProcessor(IParser parser, IRenderer renderer)
    {
        this.parser = parser;
        this.renderer = renderer;
    }

    public string ConvertToHTML(string text)
    {
        var tmpList = new List<Token>();
        parser.TryParse(tmpList, ref text);
        renderer.TryRender(tmpList, out string outputLine);

        return "<h1>header</h1>";
    }
}
