namespace MarkdownExtensions.Extensions.NestedBlock
{
    public class NestedBlockExample : IMarkdownExtension
    {
        private class Example
        {
            public string Markdown { get; set; }
        }
        private class SyntaxImpl : ISyntax
        {
            public IParseResult Parse(string text)
            {
                return new ParseSuccess(new Example { Markdown = text });
            }
        }
        private class FormatterImpl : IFormatter
        {
            public ICodeByName GetCss() => null;
            public ICodeByName GetJs() => null;

            public FormatResult Format(object root, IFormatState state)
            {
                var example = root as Example;
                return FormatResult.FromMd(example.Markdown);
            }
        }

        public string Prefix => "nested-block-md";
        public MarkdownExtensionName Name => "Nested markdown example";
        public Output Output => Output.Markdown;
        public IElementType Type => ElementType.Block;
        public ISyntax Syntax { get; } = new SyntaxImpl();
        public IValidator Validator => null;
        public IFormatter Formatter { get; } = new FormatterImpl();
    }
}
