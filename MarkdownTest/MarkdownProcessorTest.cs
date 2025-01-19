using Markdown;

namespace MarkdownTest
{
    public class MarkdownProcessorTest
    {
        private readonly MarkdownProcessor _markdownProcessor = new();

        [Fact]
        public void OutputItalicString()
        {
            const string input = "_курсив_";
            const string expectString = "<em>курсив</em>";
            var output = _markdownProcessor.ConvertToHtml(input);

            Assert.Equal(expectString, output);
        }
        
        [Fact]
        public void OutputBoldString()
        {
            const string input = "__полужирный__";
            const string expectString = "<strong>полужирный</strong>";
            var output = _markdownProcessor.ConvertToHtml(input);

            Assert.Equal(expectString, output);
        }
        
        [Fact]
        public void OutputTitleFirstLevel()
        {
            const string input = "# заголовок";
            const string expectString = "<h1>заголовок</h1>";
            var output = _markdownProcessor.ConvertToHtml(input);

            Assert.Equal(expectString, output);
        }
        
        [Fact]
        public void OutputTitleSecondLevel()
        {
            const string input = "## заголовок";
            const string expectString = "<h2>заголовок</h2>";
            var output = _markdownProcessor.ConvertToHtml(input);

            Assert.Equal(expectString, output);
        }
        
        [Fact]
        public void OutputTitleThirdLevel()
        {
            const string input = "### заголовок";
            const string expectString = "<h3>заголовок</h3>";
            var output = _markdownProcessor.ConvertToHtml(input);

            Assert.Equal(expectString, output);
        }
        
        [Fact]
        public void OutputTitleFourthLevel()
        {
            const string input = "#### заголовок";
            const string expectString = "<h4>заголовок</h4>";
            var output = _markdownProcessor.ConvertToHtml(input);

            Assert.Equal(expectString, output);
        }
        
        [Fact]
        public void OutputTitleFifthLevel()
        {
            const string input = "##### заголовок";
            const string expectString = "<h5>заголовок</h5>";
            var output = _markdownProcessor.ConvertToHtml(input);

            Assert.Equal(expectString, output);
        }
        
        [Fact]
        public void OutputTitleSixthLevel()
        {
            const string input = "###### заголовок";
            const string expectString = "<h6>заголовок</h6>";
            var output = _markdownProcessor.ConvertToHtml(input);

            Assert.Equal(expectString, output);
        }
        
        [Fact]
        public void OutputTitleWithLatticeInEnd()
        {
            const string input = "# заголовок #";
            const string expectString = "<h1>заголовок</h1>";
            var output = _markdownProcessor.ConvertToHtml(input);

            Assert.Equal(expectString, output);
        }
        
        [Fact]
        public void OutputItalicInsideBold()
        {
            const string input = "Внутри __двойного выделения _одинарное_ тоже__ работает.";
            const string expectString = "Внутри <strong>двойного выделения <em>одинарное</em> тоже</strong> работает.";
            var output = _markdownProcessor.ConvertToHtml(input);

            Assert.Equal(expectString, output);
        }
        
        [Fact]
        public void OutputBoldInsideItalic()
        {
            const string input = "Но не наоборот — внутри _одинарного __двойное__ не_ работает.";
            const string expectString = "Но не наоборот — внутри <em>одинарного __двойное__ не</em> работает.";
            var output = _markdownProcessor.ConvertToHtml(input);

            Assert.Equal(expectString, output);
        }
        
        [Fact]
        public void OutputTitleWithStyleInDigits()
        {
            const string input = "_12_3";
            const string expectString = "_12_3";
            var output = _markdownProcessor.ConvertToHtml(input);

            Assert.Equal(expectString, output);
        }
        
        [Fact]
        public void OutputStyleInStart()
        {
            const string input = "_нач_але";
            const string expectString = "<em>нач</em>але";
            var output = _markdownProcessor.ConvertToHtml(input);

            Assert.Equal(expectString, output);
        }
        
        [Fact]
        public void OutputStyleInMiddle()
        {
            const string input = "сер_еди_не";
            const string expectString = "сер<em>еди</em>не";
            var output = _markdownProcessor.ConvertToHtml(input);

            Assert.Equal(expectString, output);
        }
        
        [Fact]
        public void OutputStyleInEnd()
        {
            const string input = "кон_це_";
            const string expectString = "кон<em>це</em>";
            var output = _markdownProcessor.ConvertToHtml(input);

            Assert.Equal(expectString, output);
        }
        
        [Fact]
        public void OutputTitleWithLatticeInMiddle()
        {
            const string input = "в ра_зных сл_овах не работает";
            const string expectString = "в ра_зных сл_овах не работает";
            var output = _markdownProcessor.ConvertToHtml(input);

            Assert.Equal(expectString, output);
        }
        
        [Fact]
        public void OutputWithUnpairedSymbols()
        {
            const string input = "__Непарные_";
            const string expectString = "__Непарные_";
            var output = _markdownProcessor.ConvertToHtml(input);

            Assert.Equal(expectString, output);
        }
        
        [Fact]
        public void OutputWithSymbolsInEndDifferentWords()
        {
            const string input = "эти_ подчерки_";
            const string expectString = "эти_ подчерки_";
            var output = _markdownProcessor.ConvertToHtml(input);

            Assert.Equal(expectString, output);
        }
        
        [Fact]
        public void OutputWithSymbolsInStartDifferentWords()
        {
            const string input = "_подчерки _не считаются";
            const string expectString = "_подчерки _не считаются";
            var output = _markdownProcessor.ConvertToHtml(input);

            Assert.Equal(expectString, output);
        }
        
        [Fact]
        public void OutputTrueUseSymbolsInDifferentWords()
        {
            const string input = "<em>не считаются</em>";
            const string expectString = "<em>не считаются</em>";
            var output = _markdownProcessor.ConvertToHtml(input);

            Assert.Equal(expectString, output);
        }
        
        [Fact]
        public void OutputIntersectionDifferentStyles()
        {
            const string input = "__пересечения _двойных__ и одинарных_";
            const string expectString = "__пересечения _двойных__ и одинарных_";
            var output = _markdownProcessor.ConvertToHtml(input);

            Assert.Equal(expectString, output);
        }
        
        [Fact]
        public void OutputSymbolsWithoutWords()
        {
            const string input = "____";
            const string expectString = "____";
            var output = _markdownProcessor.ConvertToHtml(input);

            Assert.Equal(expectString, output);
        }
    }
}