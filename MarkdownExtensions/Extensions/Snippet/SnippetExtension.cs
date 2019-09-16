using Markdig;
using Markdig.Renderers;

namespace MarkdownExtensions.Extensions.Snippet
{
	public class SnippetExtension : IExtension
	{
		public SnippetExtension(RenderSettings renderSettings)
		{
			Parser = new SnippetSyntax();
			Transformer = new SnippetTransformer(renderSettings);
		}

		public IParser Parser { get; }
		public IRenderer Renderer => null;
		public IValidator Validator => null;
		public ITransformer Transformer { get; }

		public static ExtensionName NAME => "Markdown snippets";
		public ExtensionName Name => NAME;
		public bool IsSummary => false;

		public void Setup(MarkdownPipelineBuilder pipeline) =>
			pipeline.BlockParsers.Insert(0, new SnippetParser());

		public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer) { }
	}
}
