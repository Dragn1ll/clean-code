using Markdown.Enums;
using Markdown.Interfaces;
using System.Text;

namespace Markdown.Classes;

public class Renderer : IRenderer
{
    private static Dictionary<Style, string[]> _styleToHtml = new Dictionary<Style, string[]> {
        { Style.Italic, new string[]{ "<em>", "</em>" } }, { Style.Bold, new  string[] { "<strong>", "</strong>" } }, 
        { Style.Header1, new  string[]{ "<h1>", "</h1>" } }, { Style.Header2, new string[] { "<h2>", "</h2>" } },
        { Style.Header3, new string[] { "<h3>", "</h3>" } }, { Style.Header4, new string[] { "<h4>", "</h4>" } },
        { Style.Header5, new string[] { "<h5>", "</h5>" } }, { Style.Header6, new string[] { "<h6>", "</h6>" } } };

    private static Dictionary<Style, string> _styleToMD = new Dictionary<Style, string> {
        { Style.Italic, "_" }, { Style.Bold, "__" },{ Style.Header1, "#" }, { Style.Header2, "##" },
        { Style.Header3, "###" }, { Style.Header4, "####" },{ Style.Header5, "#####" }, 
        { Style.Header6, "######" } };

    public string TryRender(List<Token> tokens, string inputLine)
    {
        var tokensStartIndexSort = tokens.OrderBy(x => x.StartIndex).ToList();
        var tokensEndIndexSort = tokens.OrderBy(x => x.EndIndex).ToList();

        int lastEndTagIndex = tokens.Count - 1;
        int lastStartTagIndex = tokens.Count - 1;

        var stringBuilder = new StringBuilder(inputLine);

        for (int i = inputLine.Length - 1; i >= 0; i--)
        {
            if (lastStartTagIndex >= -1)
            {
                if (tokensStartIndexSort[lastStartTagIndex].StartIndex == i)
                {
                    if (tokensStartIndexSort[lastStartTagIndex].EndIndex != -1)
                        stringBuilder.Replace(_styleToMD[tokensStartIndexSort[lastStartTagIndex].Style], 
                            _styleToHtml[tokensStartIndexSort[lastStartTagIndex].Style][0], i,
                            _styleToMD[tokensStartIndexSort[lastStartTagIndex].Style].Length);

                    lastStartTagIndex--;
                }
            }

            if (lastEndTagIndex > -1)
            {
                if (tokensEndIndexSort[lastEndTagIndex].EndIndex == -1)
                    lastEndTagIndex--;
                else if (tokensEndIndexSort[lastEndTagIndex].EndIndex == i)
                {
                    stringBuilder.Replace(_styleToMD[tokensStartIndexSort[lastStartTagIndex].Style],
                        _styleToHtml[tokensEndIndexSort[lastEndTagIndex].Style][1], i,
                        _styleToMD[tokensStartIndexSort[lastStartTagIndex].Style].Length);

                    lastEndTagIndex--;
                }
            }
        }

        return stringBuilder.ToString();
    }
}
