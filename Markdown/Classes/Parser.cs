using Markdown.Interfaces;
using Markdown.Enums;
using System.Text;

namespace Markdown.Classes;

public class Parser : IParser
{
    public List<Token> TryParse(ref string line)
    {   
        var tokensBeParsed = new List<Token>();

        CheckHeader(tokensBeParsed, ref line);
        
        var words = line.Split(' ');
        var startSymbolStack = new Stack<Token>();
        var endSymbolStack = new Stack<Token>();
        int lengthVerifiedWords = 0;

        foreach (var word in words)
        {
            var tmpStack = new Stack<Token>();

            for (int i = 0; i < word.Length; i++)
            {
                if (word[i] == '_')
                {
                    Style style = i != word.Length - 1 && word[i + 1] == '_' ? Style.Bold : Style.Italic;

                    if (i == 0)
                    {
                        Token tmpToken = new Token(lengthVerifiedWords, -1, style);

                        tokensBeParsed.Add(tmpToken);
                        startSymbolStack.Push(tmpToken);
                        tmpStack.Push(tmpToken);
                    }
                    else if (i == word.Length - (style == Style.Italic ? 1 : 2))
                    {

                        if (tmpStack.TryPeek(out Token resultTmpStack) && TryFindTokenEnd(tmpStack, tokensBeParsed,
                            style, i, lengthVerifiedWords))
                        {
                            if (startSymbolStack.TryPeek(out Token resultStack) && resultStack.Equal(resultTmpStack))
                                startSymbolStack.Pop();
                        }
                        else if (!TryFindTokenEnd(startSymbolStack, tokensBeParsed, style, i, lengthVerifiedWords))
                        {
                            var tmpToken = new Token(i + lengthVerifiedWords, -1, style);

                            tokensBeParsed.Add(tmpToken);
                            endSymbolStack.Push(tmpToken);
                        }
                    }
                    else
                    {
                        if (!TryFindTokenEnd(tmpStack, tokensBeParsed, style, i, lengthVerifiedWords))
                        {
                            var tmpToken = new Token(i + lengthVerifiedWords, -1, style);

                            tokensBeParsed.Add(tmpToken);
                            tmpStack.Push(tmpToken);
                        }
                    }

                    i += style == Style.Italic ? 0 : 1;
                }
            }

            lengthVerifiedWords += word.Length + 1;
        }

        return tokensBeParsed;
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

    private bool TryFindTokenEnd(Stack<Token> stackTokens, List<Token> tokens, Style style, int index,
        int lengthVerifiedWords)
    {
        if (stackTokens.Count == 0)
            return false;

        if (stackTokens.Count == 3)
        {
            if (stackTokens.Peek().Style == style)
            {
                var tmpToken = stackTokens.Pop();

                tmpToken.EndIndex = index + lengthVerifiedWords;
                return true;
            }
            else
            {
                while (stackTokens.Count > 0)
                    tokens.Remove(stackTokens.Pop());
            }
        }
        else if (stackTokens.Peek().Style == style && 
            stackTokens.Peek().StartIndex < (index + lengthVerifiedWords - 2))
        {
            var tmpToken = stackTokens.Pop();

            tmpToken.EndIndex = index + lengthVerifiedWords;
            return true;
        }

        return false;
    }

    private bool TryFindTokenEnd(Stack<Token> startStackTokens, Stack<Token> endStackTokens, List<Token> tokens, Style style, int index,
        int lengthVerifiedWords)
    {
        if (startStackTokens.Count == 0)
            return false;

        if (startStackTokens.Count == 2 && endStackTokens.Count == 1)
        {
            if (startStackTokens.Peek().Style == style)
            {
                var tmpToken = startStackTokens.Pop();

                tmpToken.EndIndex = index + lengthVerifiedWords;
                return true;
            }
            else
            {
                var startStackFirstCount = startStackTokens.Count;

                while (startStackTokens.Count > startStackFirstCount - 2)
                    tokens.Remove(startStackTokens.Pop());

                tokens.Remove(endStackTokens.Pop());
            }
        }
        else if (startStackTokens.Peek().Style == style &&
            startStackTokens.Peek().StartIndex < (index + lengthVerifiedWords - 2))
        {
            var tmpToken = startStackTokens.Pop();

            tmpToken.EndIndex = index + lengthVerifiedWords;
            return true;
        }

        return false;
    }
}