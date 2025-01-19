using Markdown.Classes;

namespace Markdown.Interfaces;

public interface IParser
{
    /// <summary>
    /// Метод для парса строки из MD файла в список токенов
    /// </summary>
    /// <param name="line">Текст файла или путь к нему</param>
    /// <returns>Полученный список токенов</returns>
    List<Token?> ParseToTokens(ref string line);
}
