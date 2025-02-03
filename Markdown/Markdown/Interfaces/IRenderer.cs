using Markdown.Classes;

namespace Markdown.Interfaces;

public interface IRenderer
{
    /// <summary>
    /// Преобразовать строку с токенами в HTML-код
    /// </summary>
    /// <param name="tokens">Список токенов для составления HTML-кода</param>
    /// <param name="inputLine">Текст без тегов</param>
    /// <returns>Строку с HTML-кодами</returns>
    Task<string> Render(List<Token?> tokens, string inputLine);
}
