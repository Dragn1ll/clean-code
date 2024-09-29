using Markdown.Interfaces;
using Markdown.Enums;
using System.Text;

namespace Markdown.Classes;

public class Parser : IParser
{
    private static readonly Dictionary<string, Style> _tegsToStyle = new Dictionary<string, Style>() {
        { "_", Style.Italic }, { "__", Style.Bold } };

    public bool TryParse(List<Token> tokensBeParsed, ref string line)
    {
        StringBuilder stringBuilder = new StringBuilder(line);
        
        if (tokensBeParsed.Count != 0)
            tokensBeParsed.Clear();

        CheckHeader(stringBuilder, tokensBeParsed);
        

        
        line = stringBuilder.ToString();
        return true;
    }

    private void CheckHeader(StringBuilder stringBuilder, List<Token> tokensBeParsed)
    {
        if (stringBuilder[0] == '#')
        {
            int firstTextIndex = 1;

            // Считаем количество решёток, чтобы понять какой уровень у загаловка
            while (stringBuilder[firstTextIndex] == '#' && firstTextIndex < stringBuilder.Length)
                firstTextIndex++;

            // В данном случае firstTextIndex подразумевается как количество решёток в начале
            if (firstTextIndex <= 6)
            {
                if (firstTextIndex == stringBuilder.Length)
                    tokensBeParsed.Add(new Token(0, 0, Style.LineBreak));
                else if (stringBuilder[firstTextIndex] == ' ')
                {
                    stringBuilder.Remove(0, firstTextIndex);

                    if (stringBuilder[stringBuilder.Length - 1] == '#')
                    {
                        int lastTextIndex = stringBuilder.Length - 2;

                        while (stringBuilder[lastTextIndex] == '#')
                            lastTextIndex++;

                        if (stringBuilder[lastTextIndex] == ' ')
                        {
                            bool isAllSpace = true;
                            for (int i = firstTextIndex; i <= lastTextIndex; i++)
                                isAllSpace &= stringBuilder[i] == ' ';

                            stringBuilder.Remove(lastTextIndex, stringBuilder.Length - lastTextIndex);

                            tokensBeParsed.Add(new Token(0, (uint)stringBuilder.Length - 1, isAllSpace ?
                                                                        Style.LineBreak : (Style)firstTextIndex));
                        }
                    }
                    else
                        tokensBeParsed.Add(new Token(0, (uint)stringBuilder.Length - 1, (Style)firstTextIndex));
                }
            }
        }
    }
}