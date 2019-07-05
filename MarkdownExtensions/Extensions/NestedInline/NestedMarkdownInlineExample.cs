namespace MarkdownExtensions.Extensions.NestedInline
{
    public class NestedInlineExample : IMarkdownExtension
    {
        class Example
        {
            public string Markdown { get; set; }
        }
        class SyntaxImpl : ISyntax
        {
            public IParseResult Parse(string text)
            {
                return new ParseSuccess(new Example { Markdown = text });
            }
        }
        class FormatterImpl : IFormatter
        {
            public ICodeByName GetCss() => null;
            public ICodeByName GetJs() => null;

            public FormatResult Format(object root, IFormatState state)
            {
                var example = root as Example;
                return FormatResult.FromMarkdown("[link to HackerNews](https://news.ycombinator.com/)");
            }
        }

        public NestedInlineExample()
        {
            Formatter = new FormatterImpl();
            Syntax = new SyntaxImpl();
        }

        public string Prefix => "nested-inline-md";
        public MarkdownExtensionName Name => "Nested markdown example";
        public Output Output => Output.Markdown;
        public IElementType Type => ElementType.Inline;
        public ISyntax Syntax { get; }
        public IValidator Validator => null;
        public IFormatter Formatter { get; }
    }
}
