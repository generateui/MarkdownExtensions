using Markdig;
using Markdig.Renderers;

namespace MarkdownExtensions.Extensions.NestedInline
{
	public partial class NestedInlineExample : IExtension
    {
        class SyntaxImpl : IParser
        {
            public IParseResult Parse(string text)
            {
                return new ParseSuccess(new NestedInlineInline { Markdown = text });
            }
        }
        //class FormatterImpl : IRenderer
        //{
        //    public ICodeByName GetCss() => null;
        //    public ICodeByName GetJs() => null;

        //    public FormatResult Format(object root, IFormatState state)
        //    {
        //        var example = root as NestedInlineInline;
        //        return FormatResult.FromMarkdown("[link to HackerNews](https://news.ycombinator.com/)");
        //    }
        //}

        public NestedInlineExample()
        {
            //Renderer = new FormatterImpl();
            Parser = new SyntaxImpl();
        }

        public string Prefix => "nested-inline-md";
        public ExtensionName Name => "Nested markdown example";
        public IParser Parser { get; }
        public IValidator Validator => null;
        public IRenderer Renderer { get; }
		public ITransformer Transformer => null;

		public void Setup(MarkdownPipelineBuilder pipeline)
		{
			throw new System.NotImplementedException();
		}

		public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
		{
			throw new System.NotImplementedException();
		}
	}
}
