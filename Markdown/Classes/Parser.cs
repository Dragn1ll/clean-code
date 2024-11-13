using Markdown.Interfaces;
using Markdown.Enums;
using System.Text;

namespace Markdown.Classes;

public class Parser : IParser
{
    public bool TryParse(List<Token> tokensBeParsed, ref string line)
    {   
        if (tokensBeParsed.Count != 0)
            tokensBeParsed.Clear();

        CheckHeader(tokensBeParsed, ref line);
        
        var words = line.Split(' ');
        var stack = new Stack<Token>();
        int lengthVerifiedWords = 0;

        foreach (var word in words)
        {
            var tmpStack = new Stack<Token>();

            for (int i = 0; i < word.Length; i++)
            {
                if (word[i] == '_')
                {
                    Style style = word[i + 1] == '_' ? Style.Bold : Style.Italic;

                    if (i == 0)
                    {
                        Token tmpToken = new Token(lengthVerifiedWords, -1, style);

                        tokensBeParsed.Add(tmpToken);
                        stack.Push(tmpToken);
                        tmpStack.Push(tmpToken);
                    }
                    else if (i == word.Length - 1)
                    {
                        if (!TryFindTokenEnd(tmpStack, style, i, lengthVerifiedWords) 
                            && !TryFindTokenEnd(stack, style, i, lengthVerifiedWords))
                        {
                            var tmpToken = new Token(i + lengthVerifiedWords, -1, style);

                            tokensBeParsed.Add(tmpToken);
                            stack.Push(tmpToken);
                        }
                    }
                    else
                    {

                    }

                }
            }

            lengthVerifiedWords += word.Length + 1;
        }

        return true;
    }

    private void CheckHeader(List<Token> tokensBeParsed, ref string line)
    {
        var stringBuilder = new StringBuilder(line);

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

                            tokensBeParsed.Add(new Token(0, stringBuilder.Length - 1, isAllSpace ?
                                                                        Style.LineBreak : (Style)firstTextIndex));
                        }
                    }
                    else
                        tokensBeParsed.Add(new Token(0, stringBuilder.Length - 1, (Style)firstTextIndex));
                }
            }
        }

        line = stringBuilder.ToString();
    }

    private bool TryFindTokenEnd(Stack<Token> stackTokens, Style style, int index, int lengthVerifiedWords)
    {
        if (stackTokens.Count > 2)
        {
            if (stackTokens.Peek().Style == style)
            {
                var tmpToken = stackTokens.Pop();

                tmpToken.EndIndex = index + lengthVerifiedWords;
                return true;
            }
            else
            {
                while (stackTokens.Count > 1)
                    stackTokens.Pop();

                var tmpToken = stackTokens.Pop();

                tmpToken.EndIndex = index + lengthVerifiedWords;
                tmpToken.Style = Style.Italic;
                return true;
            }
        }
        else if (stackTokens.Peek().Style == style)
        {
            var tmpToken = stackTokens.Pop();

            tmpToken.EndIndex = index + lengthVerifiedWords;
            return true;
        }

        return false;
    }
}