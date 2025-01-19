using Markdown.Enums;
using Markdown.Interfaces;
using System.Text;

namespace Markdown.Classes;

public class Renderer : IRenderer
{
    private static readonly Dictionary<Style, string[]> StyleToHtml = new(){
        { Style.Italic, ["<em>", "</em>"] }, { Style.Bold, ["<strong>", "</strong>"] }, 
        { Style.Header1, ["<h1>", "</h1>"] }, { Style.Header2, ["<h2>", "</h2>"] },
        { Style.Header3, ["<h3>", "</h3>"] }, { Style.Header4, ["<h4>", "</h4>"] },
        { Style.Header5, ["<h5>", "</h5>"] }, { Style.Header6, ["<h6>", "</h6>"] },
        { Style.Shielding, [""] }
    };

    private static readonly Dictionary<Style, string> StyleToMd = new(){
        { Style.Italic, "_" }, { Style.Bold, "__" },{ Style.Header1, "#" }, { Style.Header2, "##" },
        { Style.Header3, "###" }, { Style.Header4, "####" },{ Style.Header5, "#####" }, 
        { Style.Header6, "######" }, { Style.Shielding, "\\"} };

    public string Render(List<Token?> tokens, string inputLine)
    {
        if (tokens.Count == 0)
            return inputLine;
        
        var tokensStartIndexSort = tokens.OrderBy(x => x!.StartIndex)
                                                    .Where(x => x!.StartIndex != -1)
                                                    .ToList();
        var tokensEndIndexSort = tokens.OrderBy(x => x!.EndIndex)
                                                    .ToList();

        var lastEndTagIndex = tokensEndIndexSort.Count - 1;
        var lastStartTagIndex = tokensStartIndexSort.Count - 1;

        var listInputTokens = new List<Token?>();

        var stringBuilder = new StringBuilder(inputLine);

        for (var index = inputLine.Length - 1; index >= 0; index--)
        {
            if (lastStartTagIndex > -1 && listInputTokens.Count > 0)
            {
                var tokenStartIndex = tokensStartIndexSort[lastStartTagIndex];
                
                if (tokenStartIndex!.StartIndex == index)
                {
                    if (!ThereAreDigits(inputLine, tokenStartIndex.StartIndex, tokenStartIndex.EndIndex) 
                        && listInputTokens.Contains(tokenStartIndex))
                    {
                        stringBuilder.Replace(StyleToMd[tokenStartIndex.Style],
                            StyleToHtml[tokenStartIndex.Style][0], index,
                            StyleToMd[tokenStartIndex.Style].Length);

                        listInputTokens.Remove(tokenStartIndex);
                    }

                    lastStartTagIndex--;
                }
            }

            if (lastEndTagIndex <= -1) continue;
            
            var tokenEndIndex = tokensEndIndexSort[lastEndTagIndex];
            
            if (tokenEndIndex!.EndIndex == index && tokenEndIndex.Style == Style.Shielding)
            {
                stringBuilder.Replace(StyleToMd[tokenEndIndex.Style],
                    StyleToHtml[tokenEndIndex.Style][0], index,
                    StyleToMd[tokenEndIndex.Style].Length);
                
                lastEndTagIndex--;
            }
            
            else if (tokenEndIndex!.StartIndex > -1 
                     && ThereAreDigits(inputLine, tokenEndIndex.StartIndex, tokenEndIndex.EndIndex))
                lastEndTagIndex--;

            else if (listInputTokens.Count > 0
                     && tokenEndIndex.Style != listInputTokens[^1]!.Style
                     && listInputTokens[^1] is { Style: Style.Italic }
                     && tokenEndIndex.StartIndex > listInputTokens[^1]!.StartIndex)
            {
                lastEndTagIndex--;
            }
            
            else if (tokenEndIndex.EndIndex == index )
            {
                stringBuilder.Replace(StyleToMd[tokenEndIndex.Style],
                    StyleToHtml[tokenEndIndex.Style][1], index,
                    StyleToMd[tokenEndIndex.Style].Length);

                listInputTokens.Add(tokenEndIndex);
                lastEndTagIndex--;
            }
        }

        return stringBuilder.ToString();
    }

    private static bool ThereAreDigits(string line, int start, int end)
    {
        for (var index = start; index < end + 1; index++)
        {
            if (char.IsDigit(line[index]))
                return true;
        }

        return false;
    }
}
