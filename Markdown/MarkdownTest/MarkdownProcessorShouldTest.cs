using Markdown;

namespace MarkdownTest;

public class MarkdownProcessorShouldTest
{
    private MarkdownProcessor _markdownProcessor;
    
    [SetUp]
    public void Setup()
    {
        _markdownProcessor = new MarkdownProcessor();
    }

    [Test]
    public async Task ConvertToHtml_ShouldOutputItalicString()
    {
        const string input = "_курсив_";
        const string expected = "<em>курсив</em>";
        var output = await _markdownProcessor.ConvertToHtml(input);

        Assert.That(output, Is.EqualTo(expected));
    }

    [Test]
    public async Task ConvertToHtml_ShouldOutputBoldString()
    {
        const string input = "__полужирный__";
        const string expected = "<strong>полужирный</strong>";
        var output = await _markdownProcessor.ConvertToHtml(input);

        Assert.That(output, Is.EqualTo(expected));
    }
    
    [TestCase("\\_экранирование_", "_экранирование_")]
    [TestCase("\\__экранирование__", "__экранирование__")]
    public async Task ConvertToHtml_ShouldOutputTagsWithoutStyle(string input, string expected)
    {
        var output = await _markdownProcessor.ConvertToHtml(input);

        Assert.That(output, Is.EqualTo(expected));
    }
    
    [TestCase("[ссылка]()", """<a href="#">ссылка</a>""")]
    [TestCase("[ссылка](www.google.com)", """<a href="www.google.com">ссылка</a>""")]
    public async Task ConvertToHtml_ShouldOutputUrl(string input, string expected)
    {
        var output = await _markdownProcessor.ConvertToHtml(input);

        Assert.That(output, Is.EqualTo(expected));
    }

    [TestCase("# заголовок", "<h1>заголовок</h1>")]
    [TestCase("## заголовок", "<h2>заголовок</h2>")]
    [TestCase("### заголовок", "<h3>заголовок</h3>")]
    [TestCase("#### заголовок", "<h4>заголовок</h4>")]
    [TestCase("##### заголовок", "<h5>заголовок</h5>")]
    [TestCase("###### заголовок", "<h6>заголовок</h6>")]
    [TestCase("# заголовок #", "<h1>заголовок</h1>")]
    public async Task ConvertToHtml_ShouldOutputTitles(string input, string expected)
    {
        var output = await _markdownProcessor.ConvertToHtml(input);

        Assert.That(output, Is.EqualTo(expected));
    }

    [Test]
    public async Task ConvertToHtml_ShouldOutputItalicInsideBold()
    {
        const string input = "Внутри __двойного выделения _одинарное_ тоже__ работает.";
        const string expected = "Внутри <strong>двойного выделения <em>одинарное</em> тоже</strong> работает.";
        var output = await _markdownProcessor.ConvertToHtml(input);

        Assert.That(output, Is.EqualTo(expected));
    }

    [Test]
    public async Task ConvertToHtml_ShouldOutputBoldInsideItalic()
    {
        const string input = "Но не наоборот — внутри _одинарного __двойное__ не_ работает.";
        const string expected = "Но не наоборот — внутри <em>одинарного __двойное__ не</em> работает.";
        var output = await _markdownProcessor.ConvertToHtml(input);

        Assert.That(output, Is.EqualTo(expected));
    }

    [TestCase("_12_3", "_12_3")]
    [TestCase("_нач_але", "<em>нач</em>але")]
    [TestCase("сер_еди_не", "сер<em>еди</em>не")]
    [TestCase("кон_це_", "кон<em>це</em>")]
    [TestCase("в ра_зных сл_овах не работает", "в ра_зных сл_овах не работает")]
    [TestCase("__Непарные_", "__Непарные_")]
    [TestCase("эти_ подчерки_", "эти_ подчерки_")]
    [TestCase("_подчерки _не считаются", "_подчерки _не считаются")]
    [TestCase("<em>не считаются</em>", "<em>не считаются</em>")]
    [TestCase("__пересечения _двойных__ и одинарных_", "__пересечения _двойных__ и одинарных_")]
    [TestCase("__ __", "__ __")]
    public async Task ConvertToHtml_ShouldHandleEdgeCases(string input, string expected)
    {
        var output = await _markdownProcessor.ConvertToHtml(input);

        Assert.That(output, Is.EqualTo(expected));
    }
}