using Application.Interfaces.Services;
using Application.Utils;
using Markdown;

namespace Application.Services;

public class MdService : IMdService
{
    private readonly MarkdownProcessor _markdownProcessor = new();

    public async Task<Result<string>> ConvertToHtml(string mdText)
    {
        var convertedText = await Task.Run(() => 
            Result<string>.Success(_markdownProcessor.ConvertToHtml(mdText)));
        
        return convertedText;
    }
}