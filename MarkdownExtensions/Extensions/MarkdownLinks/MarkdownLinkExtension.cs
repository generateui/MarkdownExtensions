using Markdig;
using Markdig.Renderers;
using MarkdownExtensions.Extensions.MarkdownLinks;

namespace MarkdownExtensions.Extensions.MarkdownLink
{
	public class MarkdownLinkExtension : IExtension
	{
		public static readonly ExtensionName NAME = "Markdown link";

		public ExtensionName Name => NAME;
		public IParser Parser => null;
		public IValidator Validator => null;
		public IRenderer Renderer => null;
		public ITransformer Transformer => null;

		public void ParseSettings(dynamic toml) { }
		public void Setup(MarkdownPipelineBuilder pipeline) { }

		public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
		{
			renderer.ObjectRenderers.Insert(0, new MarkdownLinkRenderer());
		}
	}
}
