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

    public string Render(List<Token> tokens, string inputLine)
    {
        var tokensStartIndexSort = tokens.OrderBy(x => x.StartIndex).ToList();
        var tokensEndIndexSort = tokens.OrderBy(x => x.EndIndex).ToList();

        int lastEndTagIndex = tokens.Count - 1;
        int lastStartTagIndex = tokens.Count - 1;

        var stackInputedTokens = new List<Token>();

        var stringBuilder = new StringBuilder(inputLine);

        for (int i = inputLine.Length - 1; i >= 0; i--)
        {
            if (lastStartTagIndex > -1 && stackInputedTokens.Count > 0)
            {
                if (tokensStartIndexSort[lastStartTagIndex].StartIndex == i)
                {
                    if (stackInputedTokens.Contains(tokensStartIndexSort[lastStartTagIndex]) &&
                        !ThereAreDigits(inputLine, tokensStartIndexSort[lastStartTagIndex].StartIndex,
                        tokensStartIndexSort[lastStartTagIndex].EndIndex))
                    {
                        stringBuilder.Replace(_styleToMD[tokensStartIndexSort[lastStartTagIndex].Style],
                            _styleToHtml[tokensStartIndexSort[lastStartTagIndex].Style][0], i,
                            _styleToMD[tokensStartIndexSort[lastStartTagIndex].Style].Length);

                        stackInputedTokens.Remove(tokensStartIndexSort[lastStartTagIndex]);
                    }

                    lastStartTagIndex--;
                }
            }

            if (lastEndTagIndex > -1)
            {
                if (tokensEndIndexSort[lastEndTagIndex].EndIndex == -1 ||
                    ThereAreDigits(inputLine, tokensEndIndexSort[lastEndTagIndex].StartIndex,
                        tokensEndIndexSort[lastEndTagIndex].EndIndex))
                    lastEndTagIndex--;

                else if (stackInputedTokens.Count > 0 && 
                    tokensEndIndexSort[lastEndTagIndex].Style != stackInputedTokens[stackInputedTokens.Count - 1].Style &&
                    stackInputedTokens[stackInputedTokens.Count - 1].Style == Style.Italic && 
                    tokensEndIndexSort[lastEndTagIndex].StartIndex > stackInputedTokens[stackInputedTokens.Count - 1].StartIndex)
                    
                    lastEndTagIndex--;
                else if (tokensEndIndexSort[lastEndTagIndex].EndIndex == i)
                {
                    stringBuilder.Replace(_styleToMD[tokensEndIndexSort[lastEndTagIndex].Style],
                        _styleToHtml[tokensEndIndexSort[lastEndTagIndex].Style][1], i,
                        _styleToMD[tokensEndIndexSort[lastEndTagIndex].Style].Length);

                    stackInputedTokens.Add(tokensEndIndexSort[lastEndTagIndex]);
                    lastEndTagIndex--;
                }
            }
        }

        return stringBuilder.ToString();
    }

    private bool ThereAreDigits(string line, int start, int end)
    {
        for (int i = start; i < end + 1; i++)
        {
            if (char.IsDigit(line[i]))
                return true;
        }

        return false;
    }
}
