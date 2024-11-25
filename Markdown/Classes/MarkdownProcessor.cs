using Markdown.Interfaces;

namespace Markdown.Classes;

public class MarkdownProcessor
{
    private IParser parser;
    private IRenderer renderer;

    public MarkdownProcessor()
    {
        parser = new Parser();
        renderer = new Renderer();
    }

    public string ConvertToHTML(string text)
    {
        var outputline = renderer.Render(parser.TryParse(ref text), text);

        return outputline;
    }
}
