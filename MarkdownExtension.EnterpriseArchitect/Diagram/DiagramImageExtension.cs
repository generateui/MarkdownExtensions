using Markdig;
using Markdig.Renderers;
using MarkdownExtension.EnterpriseArchitect.EaProvider;
using MarkdownExtensions;

namespace MarkdownExtension.EnterpriseArchitect.Diagram
{
	public partial class DiagramImageExtension : IExtension
	{
		private readonly FormatSettings formatSettings;

		public DiagramImageExtension(IEaProvider provider, FormatSettings formatSettings)
		{
			Parser = new DiagramSyntax();
			Renderer = new DiagramRenderer(provider, formatSettings);
		}

		public IParser Parser { get; }
		public IRenderer Renderer { get; }
		public IValidator Validator => null;
		public ITransformer Transformer => null;
		public static ExtensionName NAME => "EA diagram";
		public ExtensionName Name => NAME;

		public void Setup(MarkdownPipelineBuilder pipeline) =>
			pipeline.BlockParsers.Insert(0, new DiagramBlockParser());

		public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer) =>
			renderer.ObjectRenderers.Insert(0, Renderer);
	}
}
