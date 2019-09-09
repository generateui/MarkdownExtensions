using Markdig;
using Markdig.Renderers;
using MarkdownExtension.EnterpriseArchitect.EaProvider;
using MarkdownExtensions;

namespace MarkdownExtension.EnterpriseArchitect.DatamodelApi
{
	public class DatamodelApiExtension : IExtension
	{
		public DatamodelApiExtension(IEaProvider provider, RenderSettings renderSettings)
		{
			Parser = new DatamodelApiSyntax();
			Renderer = new DatamodelApiRenderer(provider, renderSettings);
		}

		public IParser Parser { get; }
		public IRenderer Renderer { get; }
		public IValidator Validator => null;
		public ITransformer Transformer => null;
		public static ExtensionName NAME => "EA datamodel api";
		public ExtensionName Name => NAME;

		public void Setup(MarkdownPipelineBuilder pipeline) =>
			pipeline.BlockParsers.Insert(0, new DatamodelApiParser());

		public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer) =>
			renderer.ObjectRenderers.Insert(0, Renderer);
	}
}
