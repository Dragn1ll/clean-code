using Markdown.Classes;

var md = new MarkdownProcessor();

Console.WriteLine(md.ConvertToHTML("Но не наоборот — внутри _одинарного __двойное__ не_ работает."));
