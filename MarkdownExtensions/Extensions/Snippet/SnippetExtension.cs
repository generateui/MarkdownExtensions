using Markdig;
using Markdig.Renderers;

namespace MarkdownExtensions.Extensions.Snippet
{
	public class SnippetExtension : IExtension
	{
		public SnippetExtension()
		{
			Parser = new SnippetSyntax();
		}

		public IParser Parser { get; }
		public IRenderer Renderer => null;
		public IValidator Validator => null;
		public static ExtensionName NAME => "Markdown snippets";
		public ExtensionName Name => NAME;

		public ITransformer Transformer => new SnippetTransformer(); // TODO: inject deps

		public void Setup(MarkdownPipelineBuilder pipeline) =>
			pipeline.BlockParsers.Insert(0, new SnippetParser());

		public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer) { }
	}
}
